using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Implementations
{
    public class TaskLogService : ITaskLogService
    {
        private readonly ITaskLogRepository _taskLogRepository;

		public TaskLogService(ITaskLogRepository taskLogRepository)
		{
			_taskLogRepository = taskLogRepository;
		}

		public bool AddTaskLog(TaskLogEntity taskLog)
		{
			return _taskLogRepository.AddTaskLog(taskLog);
		}

		public List<TaskLogEntity> GetLogsByTaskId(string taskId)
		{
			return _taskLogRepository.GetLogsByTaskId(taskId);
		}
	}
}
