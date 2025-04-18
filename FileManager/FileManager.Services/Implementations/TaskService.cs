using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels.Step;
using FileManager.Services.Interfaces;


namespace FileManager.Services.Implementations
{
    public class TaskService : ITaskService
	{
        private readonly ITaskRepository _taskRepository;
		private readonly IStepRepository _stepRepository;
		private readonly IOperationRepository _operationRepository;
		public TaskService(ITaskRepository taskRepository, IStepRepository stepRepository, IOperationRepository operationRepository)
		{
			_taskRepository = taskRepository;
			_stepRepository = stepRepository;
			_operationRepository = operationRepository;
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
			TaskEntity newTask = new TaskEntity();
            newTask.TaskId = newIdTask;
			newTask.Name = copiedTask.Name;
			newTask.TimeBegin = copiedTask.TimeBegin;
			newTask.TimeEnd = copiedTask.TimeEnd;
			newTask.DayActive = copiedTask.DayActive;
			newTask.AddresseeGroupId = copiedTask.AddresseeGroupId;
			newTask.IsActive = copiedTask.IsActive;
			newTask.LastModified = DateTime.Now;
			newTask.TaskGroupId = copiedTask.TaskGroupId;
			newTask.ExecutionLeft = copiedTask.ExecutionLimit;
			newTask.ExecutionLimit = copiedTask.ExecutionLimit;
			newTask.IsProgress = copiedTask.IsProgress;
			CreateTask(newTask);
            
            if (isCopySteps == "on")
            {
                List<TaskStepEntity> steps = _stepRepository.GetAllStepsByTaskId(idTask).OrderBy(x => x.StepNumber).ToList();
				int i = 1;
                foreach (var item in copyStep)
                {
                    if (item.IsCopy)
                    {
                        TaskStepEntity? oldStep = steps.FirstOrDefault(x => x.TaskId == idTask &&
                                                                            x.StepNumber == item.StepNumber);
                        if (oldStep != null)
                        {
                            TaskStepEntity newStep = new TaskStepEntity();
                            newStep.TaskId = newIdTask;
                            newStep.StepNumber = i;
                            newStep.OperationName = oldStep.OperationName;
                            newStep.Description = oldStep.Description;
                            newStep.FileMask = oldStep.FileMask;
                            newStep.Source = oldStep.Source;
                            newStep.Destination = oldStep.Destination;
                            newStep.IsBreak = oldStep.IsBreak;
                            newStep.IsActive = oldStep.IsActive;
							_stepRepository.CreateStep(newStep);
							i++;
							if (item.IsCopyOperation)
							{
                                switch (newStep.OperationName)
                                {
                                    case OperationName.Copy:
										OperationCopyEntity newCopy = new OperationCopyEntity();
										OperationCopyEntity? oldCopy = _operationRepository.GetCopyByStepId(oldStep.StepId);
										if (oldCopy != null)
										{
											newCopy.StepId = newStep.StepId;
											newCopy.InformSuccess = oldCopy.InformSuccess;
											newCopy.AddresseeGroupId = oldCopy.AddresseeGroupId;
											newCopy.AdditionalText = oldCopy.AdditionalText;
											newCopy.FileInSource = oldCopy.FileInSource;
											newCopy.FileInDestination = oldCopy.FileInDestination;
											newCopy.FileInLog = oldCopy.FileInLog;
											newCopy.Sort = oldCopy.Sort;
											newCopy.FileAttribute = oldCopy.FileAttribute;
											_operationRepository.CreateCopy(newCopy);
										}
                                        break;
                                    case OperationName.Move:
                                        OperationMoveEntity newMove = new OperationMoveEntity();
                                        OperationMoveEntity? oldMove = _operationRepository.GetMoveByStepId(oldStep.StepId);
                                        if (oldMove != null)
                                        {
                                            newMove.StepId = newStep.StepId;
                                            newMove.InformSuccess = oldMove.InformSuccess;
                                            newMove.AddresseeGroupId = oldMove.AddresseeGroupId;
                                            newMove.AdditionalText = oldMove.AdditionalText;
                                            newMove.FileInDestination = oldMove.FileInDestination;
                                            newMove.FileInLog = oldMove.FileInLog;
                                            newMove.Sort = oldMove.Sort;
                                            newMove.FileAttribute = oldMove.FileAttribute;
                                            _operationRepository.CreateMove(newMove);
                                        }
                                        break;
                                    case OperationName.Read:
                                        OperationReadEntity newRead = new OperationReadEntity();
                                        OperationReadEntity? oldRead = _operationRepository.GetReadByStepId(oldStep.StepId);
                                        if (oldRead != null)
                                        {
                                            newRead.StepId = newStep.StepId;
                                            newRead.InformSuccess = oldRead.InformSuccess;
                                            newRead.AddresseeGroupId = oldRead.AddresseeGroupId;
                                            newRead.AdditionalText = oldRead.AdditionalText;
                                            newRead.FileInSource = oldRead.FileInSource;
                                            newRead.Encode = oldRead.Encode;
                                            newRead.SearchRegex = oldRead.SearchRegex;
                                            newRead.FindString = oldRead.FindString;
                                            newRead.ExpectedResult = oldRead.ExpectedResult;
                                            newRead.BreakTaskAfterError = oldRead.BreakTaskAfterError;
                                            _operationRepository.CreateRead(newRead);
                                        }
                                        break;
                                    case OperationName.Exist:
                                        OperationExistEntity newExist = new OperationExistEntity();
                                        OperationExistEntity? oldExist = _operationRepository.GetExistByStepId(oldStep.StepId);
                                        if (oldExist != null)
                                        {
                                            newExist.StepId = newStep.StepId;
                                            newExist.InformSuccess = oldExist.InformSuccess;
                                            newExist.AddresseeGroupId = oldExist.AddresseeGroupId;
                                            newExist.AdditionalText = oldExist.AdditionalText;
                                            newExist.ExpectedResult = oldExist.ExpectedResult;
                                            newExist.BreakTaskAfterError = oldExist.BreakTaskAfterError;
                                            _operationRepository.CreateExist(newExist);
                                        }
                                        break;
                                    case OperationName.Rename:
                                        OperationRenameEntity newRename = new OperationRenameEntity();
                                        OperationRenameEntity? oldRename = _operationRepository.GetRenameByStepId(oldStep.StepId);
                                        if (oldRename != null)
                                        {
                                            newRename.StepId = newStep.StepId;
                                            newRename.InformSuccess = oldRename.InformSuccess;
                                            newRename.AddresseeGroupId = oldRename.AddresseeGroupId;
                                            newRename.AdditionalText = oldRename.AdditionalText;
                                            newRename.OldPattern = oldRename.OldPattern;
                                            newRename.NewPattern = oldRename.NewPattern;
                                            _operationRepository.CreateRename(newRename);
                                        }
                                        break;
                                    case OperationName.Delete:
                                        OperationDeleteEntity newDelete = new OperationDeleteEntity();
                                        OperationDeleteEntity? oldDelete = _operationRepository.GetDeleteByStepId(oldStep.StepId);
                                        if (oldDelete != null)
                                        {
                                            newDelete.StepId = newStep.StepId;
                                            newDelete.InformSuccess = oldDelete.InformSuccess;
                                            newDelete.AddresseeGroupId = oldDelete.AddresseeGroupId;
                                            newDelete.AdditionalText = oldDelete.AdditionalText;
                                            _operationRepository.CreateDelete(newDelete);
                                        }
                                        break;
                                    case OperationName.Clrbuf:
                                        OperationClrbufEntity newClrbuf = new OperationClrbufEntity();
                                        OperationClrbufEntity? oldClrbuf = _operationRepository.GetClrbufByStepId(oldStep.StepId);
                                        if (oldClrbuf != null)
                                        {
                                            newClrbuf.StepId = newStep.StepId;
                                            newClrbuf.InformSuccess = oldClrbuf.InformSuccess;
                                            newClrbuf.AddresseeGroupId = oldClrbuf.AddresseeGroupId;
                                            newClrbuf.AdditionalText = oldClrbuf.AdditionalText;
                                            _operationRepository.CreateClrbuf(newClrbuf);
                                        }
                                        break;
                                    default:
										break;
                                }
                            }
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
