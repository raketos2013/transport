using FileManager.Domain.Entity;

namespace FileManager_Server.Loggers;

public interface ITaskLogger
{
    void TaskLog(string TaskId, string text);
    void StepLog(TaskStepEntity step, string text, string filename = "");
    void OperationLog(TaskStepEntity step);
}
