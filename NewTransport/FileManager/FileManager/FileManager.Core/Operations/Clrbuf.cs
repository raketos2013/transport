using FileManager.Core.Constants;
using FileManager.Core.Entities;
using FileManager.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace FileManager.Core.Operations;

public class Clrbuf(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScope scopeFactory)
            : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"ОЧИСТКА БУФЕРА: {TaskStep.Source}");
        int countFiles = 0;
        if (bufferFiles != null)
        {
            if (TaskStep.FileMask == AppConstants.BUFFER_FILE_MASK)
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
                _nextStep = null; 
            }
        }
        await _taskLogger.StepLog(TaskStep, $"Удалено файлов из буфера: {countFiles}");

        if (_nextStep != null)
        {
            await _nextStep.Execute(bufferFiles);
        }
    }
}
