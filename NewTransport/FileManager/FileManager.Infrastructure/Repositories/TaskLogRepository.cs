using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;

namespace FileManager.Infrastructure.Repositories;

public class TaskLogRepository(AppDbContext appDbContext) : ITaskLogRepository
{
    public bool AddTaskLog(TaskLogEntity taskLog)
    {
        try
        {

            appDbContext.TaskLog.Add(taskLog);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<TaskLogEntity> GetLogs()
    {
        return appDbContext.TaskLog.ToList();
    }

    public List<TaskLogEntity> GetLogsByTaskId(string taskId)
    {
        return appDbContext.TaskLog.Where(x => x.TaskId == taskId).ToList();
    }
}
