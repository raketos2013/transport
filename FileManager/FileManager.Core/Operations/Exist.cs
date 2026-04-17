using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace FileManager.Core.Operations;

public class Exist(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScope scopeFactory)
            : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles, CancellationToken cancellationToken)
    {
        await _taskLogger.StepLog(TaskStep, $"ПРОВЕРКА НАЛИЧИЯ: {TaskStep.Source}");
        await _taskLogger.OperationLog(TaskStep);

        if (!Directory.Exists(TaskStep.Source))
        {
            await _taskLogger.StepLog(TaskStep, "Каталог источника не найден", "", ResultOperation.E);
            throw new DomainException("Каталог источника не найден");
        }

        string[] files = [];
        OperationExistEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
        if (files.Length == 0 && TaskStep.IsBreak)
        {
            await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
            _nextStep = null;
            return;
        }
        else
        {
            await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Length}");

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            operation = await _operationService.GetExistByStepId(TaskStep.StepId);
            if (operation != null)
            {
                if (operation.InformSuccess)
                {
                    var addressesAsync = await _addresseeService.GetAllAddressees();
                    addresses = addressesAsync.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                          x.IsActive == true).ToList();
                }
                bool isBreakTask = false;
                isBreakTask = CheckExpectedResult(operation.ExpectedResult, operation.BreakTaskAfterError, files.Length);
                if (isBreakTask)
                {
                    await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: несоответствие ожидаемому результату", "", ResultOperation.W); 
                    _nextStep = null;
                }
            }

            if (addresses.Count > 0 && files.Length > 0)
            {
                foreach (var file in files)
                {
                    successFiles.Add(file);
                }
                await _mailSender.Send(TaskStep, addresses, successFiles);
            }
        }
        

        if (_nextStep != null)
        {
            await _nextStep.Execute(bufferFiles, cancellationToken);
        }
    }

    public bool CheckExpectedResult(ExpectedResult expectedResult, bool breakTaskAfterError, int countFiles)
    {
        bool isBreakTask = false;
        switch (expectedResult)
        {
            case ExpectedResult.Success:
                if (countFiles > 0)
                {
                    if (breakTaskAfterError)
                    {
                        isBreakTask = true;
                    }
                    else
                    {
                        isBreakTask = false;
                    }
                }
                else
                {
                    if (breakTaskAfterError)
                    {
                        isBreakTask = false;
                    }
                    else
                    {
                        isBreakTask = true;
                    }
                }
                break;
            case ExpectedResult.Error:
                if (countFiles == 0)
                {
                    if (breakTaskAfterError)
                    {
                        isBreakTask = true;
                    }
                    else
                    {
                        isBreakTask = false;
                    }
                }
                else
                {
                    if (breakTaskAfterError)
                    {
                        isBreakTask = false;
                    }
                    else
                    {
                        isBreakTask = true;
                    }
                }
                break;
            case ExpectedResult.Any:
                if (breakTaskAfterError)
                {
                    isBreakTask = true;
                }
                else
                {
                    isBreakTask = false;
                }
                break;
            default:
                break;
        }
        return isBreakTask;
    }
}
