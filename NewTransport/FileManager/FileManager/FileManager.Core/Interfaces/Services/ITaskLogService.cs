using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskLogService
{
    Task<List<TaskLogEntity>> GetLogsByTaskId(string taskId);
    Task<TaskLogEntity> AddTaskLog(TaskLogEntity taskLog);
    Task<List<TaskLogEntity>> GetLogs();
}
