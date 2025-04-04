using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Implementations
{
    public class TaskLogRepository : ITaskLogRepository
    {
        private readonly AppDbContext _appDbContext;

		public TaskLogRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public bool AddTaskLog(TaskLogEntity taskLog)
		{
			try
			{
				_appDbContext.TaskLog.Add(taskLog);
				_appDbContext.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public List<TaskLogEntity> GetLogsByTaskId(string taskId)
		{
			return _appDbContext.TaskLog.Where(x => x.TaskId == taskId).ToList();	
		}
	}
}
