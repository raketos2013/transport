using FileManager.Core.Entities;
using FileManager.Core.Extensions;
using FileManager.Core.Queries;

namespace FileManager.Core.Interfaces.Repositories;

public interface ITaskLogRepository
{
    IQueryable<TaskLogEntity> GetLogsByTaskId(string taskId);
    Task<TaskLogEntity> AddTaskLog(TaskLogEntity taskLog);
    Task<List<TaskLogEntity>> AddTaskLogRange(List<TaskLogEntity> taskLogs);
    Task<PagedList<TaskLogEntity>> GetLogs(Query query);
    IQueryable<TaskLogEntity> GetLogs();
    Task<List<TaskLogEntity>> GetLastLogs();
}
