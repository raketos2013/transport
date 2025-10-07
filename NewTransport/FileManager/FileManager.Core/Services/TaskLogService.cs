using FileManager.Core.Entities;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class TaskLogService(IUnitOfWork unitOfWork)
            : ITaskLogService
{
    public async Task<TaskLogEntity> AddTaskLog(TaskLogEntity taskLog)
    {
        var createdLog = await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
        return await unitOfWork.SaveAsync() > 0 ? createdLog
                            : throw new DomainException("Ошибка добавления лога");
    }

    public async Task<List<TaskLogEntity>> GetLogs()
    {
        return await unitOfWork.TaskLogRepository.GetLogs();
    }

    public async Task<List<TaskLogEntity>> GetLogsByTaskId(string taskId)
    {
        return await unitOfWork.TaskLogRepository.GetLogsByTaskId(taskId);
    }

    public async Task<List<TaskLogEntity>> GetLastLogTasks()
    {
        var tasks = await unitOfWork.TaskRepository.GetAllTasks();
        List<TaskLogEntity> logs = [];
        foreach (var task in tasks)
        {
            var taskLogs = await unitOfWork.TaskLogRepository.GetLogsByTaskId(task.TaskId);
            var lastLog = taskLogs.OrderBy(x => x.DateTimeLog).Last();
            logs.Add(lastLog);
        }
        return logs;
    }
}
