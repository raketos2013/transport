using FileManager.Domain.Entity;
using FileManager.Domain.Enum;

namespace FileManager_Web.Loggers;

public interface ITaskLogger
{
    void TaskLog(string TaskId, string text, ResultOperation? resultOperation = null);
    void StepLog(TaskStepEntity step, string text, string filename = "", ResultOperation resultOperation = ResultOperation.I);
    void OperationLog(TaskStepEntity step);
}
