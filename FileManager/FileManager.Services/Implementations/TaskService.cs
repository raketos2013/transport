using FileManager.DAL;
using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.ViewModels.Step;
using FileManager.Services.Interfaces;


namespace FileManager.Services.Implementations
{
    public class TaskService : ITaskService
	{
        private readonly ITaskRepository _taskRepository;
		private readonly IStepRepository _stepRepository;
		public TaskService(ITaskRepository taskRepository, IStepRepository stepRepository)
		{
			_taskRepository = taskRepository;
			_stepRepository = stepRepository;
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

		public bool CreateTaskGroup(string name)
		{
			return _taskRepository.CreateTaskGroup(name);
		}

		public bool DeleteTaskGroup(int idGroup)
		{
			return _taskRepository.DeleteTaskGroup(idGroup);
		}

		public bool ActivatedTask(string idTask)
		{
			return _taskRepository.ActivatedTask(idTask);
		}

        public bool CopyTask(string idTask, string newIdTask, string isCopySteps, CopyStepViewModel[] copyStep)
        {
            TaskEntity copiedTask = GetTaskById(idTask);
            if (copiedTask == null)
            {
                return false;
            }
            copiedTask.TaskId = newIdTask;
            EditTask(copiedTask);
			CreateTaskStatuse(newIdTask);
            
            if (isCopySteps == "on")
            {
                List<TaskStepEntity> steps = _stepRepository.GetAllStepsByTaskId(idTask);
                foreach (var item in copyStep)
                {
                    if (item.IsCopy)
                    {
                        TaskStepEntity oldStep = steps.FirstOrDefault(x => x.TaskId == idTask &&
                                                                            x.StepNumber == item.StepNumber);
                        TaskStepEntity newStep = new TaskStepEntity();
                        if (oldStep != null)
                        {
                            newStep.TaskId = newIdTask;
                            newStep.StepNumber = oldStep.StepNumber;
                            newStep.OperationName = oldStep.OperationName;
                            newStep.Description = oldStep.Description;
                            newStep.FileMask = oldStep.FileMask;
                            newStep.Source = oldStep.Source;
                            newStep.Destination = oldStep.Destination;
                            newStep.IsBreak = oldStep.IsBreak;
                            newStep.IsActive = oldStep.IsActive;
                        }
                    }
                }
            }
			return true;
        }

        public bool CreateTaskStatuse(string idTask)
        {
            return _taskRepository.CreateTaskStatuse(idTask);
        }
    }
}
