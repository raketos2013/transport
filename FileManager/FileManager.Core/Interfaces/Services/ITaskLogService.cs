using FileManager.Core.Entities;
using FileManager.Core.Extensions;
using FileManager.Core.Queries;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskLogService
{
    IQueryable<TaskLogEntity> GetLogsByTaskId(string taskId);
    Task<TaskLogEntity> AddTaskLog(TaskLogEntity taskLog);
    Task<PagedList<TaskLogEntity>> GetLogs(Query query);
    Task<List<TaskLogEntity>> GetLogs();
    Task<List<TaskLogEntity>> GetLastLogTasks();
}
