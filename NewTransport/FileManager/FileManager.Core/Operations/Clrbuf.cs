using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileManager.Core.Operations;

public class Clrbuf(TaskStepEntity step,
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
                            //operationService, addresseeService, taskLogService, httpClientFactory,
                            scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"ОЧИСТКА БУФЕРА: {TaskStep.Source}");
        int countFiles = 0;
        if (bufferFiles != null)
        {
            if (TaskStep.FileMask == "{BUFFER}")
            {
                countFiles = bufferFiles.Count;
                bufferFiles = null;
            }
            else
            {
                Regex regex = new(TaskStep.FileMask);
                bufferFiles = bufferFiles.Where(x => !regex.IsMatch(x)).ToList();
            }
            
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                throw new Exception("Операция Clrbuf: найдено 0 файлов");
            }
        }
        await _taskLogger.StepLog(TaskStep, $"Удалено файлов из буфера: {countFiles}");

        _nextStep?.Execute(bufferFiles);
    }
}
