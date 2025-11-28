using FileManager.Core.Entities;
using FileManager.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace FileManager.Core.Operations;

public class Exist(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScope scopeFactory)
            : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"ПРОВЕРКА НАЛИЧИЯ: {TaskStep.Source} => {TaskStep.Destination}");
        await _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        OperationExistEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
        if (files.Length == 0 && TaskStep.IsBreak)
        {
            await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
            _nextStep = null;
        }
        else
        {
            await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Length}");

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
                CheckExpectedResult(operation.ExpectedResult, operation.BreakTaskAfterError, files.Length);
                if (isBreakTask)
                {
                    await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: несоответствие ожидаемому результату", "", ResultOperation.W);
                    throw new Exception("Ошибка при операции Exist: несоответствие ожидаемому результату");
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
            await _nextStep.Execute(bufferFiles);
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
                        isBreakTask = false;
                    }
                    else
                    {
                        isBreakTask = true;
                    }
                }
                else
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
