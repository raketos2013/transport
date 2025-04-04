using FileManager.DAL;
using FileManager.DAL.Repositories.Implementations;
using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Implementations
{
    public class TaskService : ITaskService
	{
        private readonly ITaskRepository _taskRepository;
		private readonly IServiceProvider _serviceProvider;
		private readonly IStepService _stepRepository;
		public TaskService(ITaskRepository taskRepository)
		{
			_taskRepository = taskRepository;
		}

		public bool CreateTask(TaskEntity task)
		{
			return _taskRepository.CreateTask(task);
		}

		public bool DeleteTask(string idTask)
		{
			return _taskRepository.DeleteTask(idTask);	
		}

		public List<TaskGroupEntity> GetAllGroups()
		{
			return _taskRepository.GetAllGroups();
		}

		public List<TaskEntity> GetAllTasks()
		{
			return _taskRepository.GetAllTasks();
		}

		public TaskEntity GetTaskById(string idTask)
		{
			return _taskRepository.GetTaskById(idTask);
		}

		public List<TaskEntity> GetTasksByGroup(string nameGroup)
		{
			List<TaskEntity> tasks = new List<TaskEntity>();
			TaskGroupEntity taskGroup = _taskRepository.GetTaskGroupByName(nameGroup);
			if (nameGroup == "Все")
			{
				tasks = _taskRepository.GetAllTasks().OrderByDescending(x => x.IsActive). ThenBy(x => x.TaskId).ToList();
			}
			else
			{
				tasks = _taskRepository.GetTasksByGroup(taskGroup.Id).OrderByDescending(x => x.IsActive)
																		.ThenBy(x => x.TaskId)
																		.ToList();
			}
			return tasks;
		}

		public bool UpdateTask(TaskEntity task)
		{
			return _taskRepository.UpdateTask(task);
		}
	}
}
