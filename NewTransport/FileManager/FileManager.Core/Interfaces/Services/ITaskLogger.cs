using FileManager.Core.Entities;
using FileManager.Core.Enums;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskLogger
{
    Task TaskLog(string TaskId, string text, ResultOperation? resultOperation = null);
    Task StepLog(TaskStepEntity step, string text, string filename = "", ResultOperation resultOperation = ResultOperation.I);
    Task OperationLog(TaskStepEntity step);
}
