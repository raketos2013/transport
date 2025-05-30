using FileManager.Domain.Entity;

namespace FileManager.DAL.Repositories.Interfaces
{
    public interface ITaskLogRepository
    {
        List<TaskLogEntity> GetLogsByTaskId(string taskId);
        bool AddTaskLog(TaskLogEntity taskLog);
    }
}
