using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class TaskLogService(ITaskLogRepository taskLogRepository)
            : ITaskLogService
{
    public async Task<bool> AddTaskLog(TaskLogEntity taskLog)
    {
        return await taskLogRepository.AddTaskLog(taskLog);
    }

    public async Task<List<TaskLogEntity>> GetLogs()
    {
        return await taskLogRepository.GetLogs();
    }

    public async Task<List<TaskLogEntity>> GetLogsByTaskId(string taskId)
    {
        return await taskLogRepository.GetLogsByTaskId(taskId);
    }
}
