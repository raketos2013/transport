using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;


namespace FileManager.Services.Implementations
{
    public class TaskLogService(ITaskLogRepository taskLogRepository) 
				: ITaskLogService
    {
        public bool AddTaskLog(TaskLogEntity taskLog)
		{
			return taskLogRepository.AddTaskLog(taskLog);
		}

		public List<TaskLogEntity> GetLogsByTaskId(string taskId)
		{
			return taskLogRepository.GetLogsByTaskId(taskId);
		}
	}
}
