using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class TaskLogService(ITaskLogRepository taskLogRepository)
            : ITaskLogService
{
    public bool AddTaskLog(TaskLogEntity taskLog)
    {
        return taskLogRepository.AddTaskLog(taskLog);
    }

    public List<TaskLogEntity> GetLogs()
    {
        return taskLogRepository.GetLogs();
    }

    public List<TaskLogEntity> GetLogsByTaskId(string taskId)
    {
        return taskLogRepository.GetLogsByTaskId(taskId);
    }
}
