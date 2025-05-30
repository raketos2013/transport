using FileManager.Domain.Entity;

namespace FileManager.Services.Interfaces;

public interface ITaskLogService
{
    List<TaskLogEntity> GetLogsByTaskId(string taskId);
    bool AddTaskLog(TaskLogEntity taskLog);
}
