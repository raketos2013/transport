using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;


namespace FileManager.Services.Implementations
{
    public class TaskService : ITaskService
	{
        private readonly ITaskRepository _taskRepository;
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

		public bool EditTask(TaskEntity task)
		{
			return _taskRepository.EditTask(task);
		}

        public bool UpdateLastModifiedTask(string idTask)
        {
            return _taskRepository.UpdateLastModifiedTask(idTask);
        }
    }
}
