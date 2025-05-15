using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;


namespace FileManager.DAL.Repositories.Implementations
{
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

		public List<TaskLogEntity> GetLogsByTaskId(string taskId)
		{
			return appDbContext.TaskLog.Where(x => x.TaskId == taskId).ToList();	
		}
	}
}
