using FileManager.Core.Entities;
using FileManager.Core.Extensions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Queries;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class TaskLogRepository(AppDbContext appDbContext) : ITaskLogRepository
{
    public async Task<List<TaskLogEntity>> AddTaskLogRange(List<TaskLogEntity> taskLogs)
    {
        await appDbContext.TaskLog.AddRangeAsync(taskLogs);
        return taskLogs;
    }
    public async Task<TaskLogEntity> AddTaskLog(TaskLogEntity taskLog)
    {
        await appDbContext.TaskLog.AddAsync(taskLog);
        return taskLog;
    }

    public async Task<PagedList<TaskLogEntity>> GetLogs(Query query)
    {
        var entity = appDbContext.TaskLog.AsNoTracking().AsQueryable();
        
        return await PagedList<TaskLogEntity>.ToPagedList(entity, query.Skip, query.Take);
    }

    public IQueryable<TaskLogEntity> GetLogs()
    {
        return appDbContext.TaskLog.AsNoTracking();
    }

    public IQueryable<TaskLogEntity> GetLogsByTaskId(string taskId)
    {
        return appDbContext.TaskLog.Where(x => x.TaskId == taskId).AsNoTracking();
    }

    public async Task<List<TaskLogEntity>> GetLastLogs()
    {
        return await appDbContext.TaskLog
            .FromSqlRaw(@"
                SELECT DISTINCT ON (""TaskId"") *
                FROM ""TaskLog""
                ORDER BY ""TaskId"", ""DateTimeLog"" DESC")
            .ToListAsync();
    }
}
