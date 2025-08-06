using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskLogService
{
    List<TaskLogEntity> GetLogsByTaskId(string taskId);
    bool AddTaskLog(TaskLogEntity taskLog);
    List<TaskLogEntity> GetLogs();
}
