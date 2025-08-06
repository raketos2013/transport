using FileManager.Core.Entities;
using FileManager.Core.Enums;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskLogger
{
    void TaskLog(string TaskId, string text, ResultOperation? resultOperation = null);
    void StepLog(TaskStepEntity step, string text, string filename = "", ResultOperation resultOperation = ResultOperation.I);
    void OperationLog(TaskStepEntity step);
}
