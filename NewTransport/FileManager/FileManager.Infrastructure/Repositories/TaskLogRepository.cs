using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class TaskLogRepository(AppDbContext appDbContext) : ITaskLogRepository
{
    public async Task<bool> AddTaskLog(TaskLogEntity taskLog)
    {
        try
        {

            appDbContext.TaskLog.Add(taskLog);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<TaskLogEntity>> GetLogs()
    {
        return await appDbContext.TaskLog.ToListAsync();
    }

    public async Task<List<TaskLogEntity>> GetLogsByTaskId(string taskId)
    {
        return await appDbContext.TaskLog.Where(x => x.TaskId == taskId).ToListAsync();
    }
}
