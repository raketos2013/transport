using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface ITaskLogRepository
{
    List<TaskLogEntity> GetLogsByTaskId(string taskId);
    bool AddTaskLog(TaskLogEntity taskLog);
    List<TaskLogEntity> GetLogs();
}
