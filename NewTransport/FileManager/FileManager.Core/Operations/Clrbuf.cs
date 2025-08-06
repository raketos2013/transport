using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using System.Text.RegularExpressions;

namespace FileManager.Core.Operations;

public class Clrbuf(TaskStepEntity step,
                    TaskOperation? operation,
                    ITaskLogger taskLogger,
                    IMailSender mailSender, 
                    IOperationService operationService,
                    IAddresseeService addresseeService,
                    ITaskLogService taskLogService)
            : StepOperation(step, operation, taskLogger, mailSender, operationService, addresseeService, taskLogService)
{
    public override void Execute(List<string>? bufferFiles)
    {
        _taskLogger.StepLog(TaskStep, $"ОЧИСТКА БУФЕРА: {TaskStep.Source}");
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
                _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                throw new Exception("Операция Clrbuf: найдено 0 файлов");
            }
        }
        _taskLogger.StepLog(TaskStep, $"Удалено файлов из буфера: {countFiles}");

        _nextStep?.Execute(bufferFiles);
    }
}
