using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.ViewModels;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskLogger
{
    Task TaskLog(string TaskId, string text, ResultOperation? resultOperation = null);
    Task StepLog(TaskStepEntity step, string text, string filename = "", ResultOperation resultOperation = ResultOperation.I);
    Task LogFiles(TaskStepEntity step, List<FileLog> files);
    Task OperationLog(TaskStepEntity step);
}
