using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FileManager.Core.Operations;

public class Exist(TaskStepEntity step,
                    TaskOperation? operation,
                    //ITaskLogger taskLogger,
                    //IMailSender mailSender,
                    //IOptions<AuthTokenConfiguration> authTokenConfigurations,
                    //IOperationService operationService,
                    //IAddresseeService addresseeService,
                    //ITaskLogService taskLogService,
                    //IHttpClientFactory httpClientFactory
                    IServiceScopeFactory scopeFactory)
            : StepOperation(step, operation, 
                            //taskLogger, mailSender, authTokenConfigurations, 
                            //operationService, addresseeService, taskLogService, httpClientFactory
                            scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"ПРОВЕРКА НАЛИЧИЯ: {TaskStep.Source} => {TaskStep.Destination}");
        await _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileName;
        OperationExistEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
        if (files.Length == 0 && TaskStep.IsBreak)
        {
            await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
            throw new Exception("Операция Exist: найдено 0 файлов");
        }
        await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Length}");

        operation = await _operationService.GetExistByStepId(TaskStep.StepId);
        //_appDbContext.OperationExist.FirstOrDefault(x => x.StepId == TaskStep.StepId);
        if (operation != null)
        {

            if (operation.InformSuccess)
            {
                var addressesAsync = await _addresseeService.GetAllAddressees();
                addresses = addressesAsync
                                                    .Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                                x.IsActive == true).ToList();
                /*_appDbContext.Addressee.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                            x.IsActive == true).ToList();*/
            }
            bool isBreakTask = false;
            //_taskLogger.StepLog(TaskStep, $"Ожидаемый результат - {operation.ExpectedResult.GetDescription()}");
            switch (operation.ExpectedResult)
            {
                case ExpectedResult.Success:
                    if (files.Length > 0)
                    {
                        if (operation.BreakTaskAfterError)
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
                        if (operation.BreakTaskAfterError)
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
                    if (files.Length == 0)
                    {
                        if (operation.BreakTaskAfterError)
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
                        if (operation.BreakTaskAfterError)
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
                    if (operation.BreakTaskAfterError)
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
            if (isBreakTask)
            {
                await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: несоответствие ожидаемому результату", "", ResultOperation.W);
                throw new Exception("Ошибка при операции Exist: несоответствие ожидаемому результату");
            }
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            foreach (var file in files)
            {
                successFiles.Add(file);
            }
            await _mailSender.Send(TaskStep, addresses, successFiles);
        }

        _nextStep?.Execute(bufferFiles);
    }
}
