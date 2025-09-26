using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface ITaskLogRepository
{
    Task<List<TaskLogEntity>> GetLogsByTaskId(string taskId);
    Task<TaskLogEntity> AddTaskLog(TaskLogEntity taskLog);
    Task<List<TaskLogEntity>> GetLogs();
}
