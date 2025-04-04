using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels.Task;
using FileManager.Services.Implementations;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.Build.Framework;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;
		private readonly ITaskService _taskService;
        public TaskController(ILogger<TaskController> logger, UserLogging userLogging, AppDbContext appDbContext, ITaskService taskService)
        {
            _logger = logger;
            _userLogging = userLogging;
            _appDbContext = appDbContext;
			_taskService = taskService;
        }

        public IActionResult Tasks()
        {
			List<TaskGroupEntity> tasksGroups = _taskService.GetAllGroups();
			return View(tasksGroups);
        }

        [HttpPost]
        public IActionResult TasksList(string taskGroup)
        {
			List<TaskEntity> tasks = _taskService.GetTasksByGroup(taskGroup);
            return PartialView(tasks);
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            List<AddresseeGroupEntity> addresseeGroups = _appDbContext.AddresseeGroup.ToList();
            ViewBag.AddresseeGroups = addresseeGroups;
            List<TaskGroupEntity> taskGroups = _appDbContext.TaskGroup.ToList();
            ViewBag.TaskGroups = taskGroups;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTask(IFormCollection collection)
        {
            try
            {
                List<AddresseeGroupEntity> addresseeGroups = _appDbContext.AddresseeGroup.ToList();
                ViewBag.AddresseeGroups = addresseeGroups;
                List<TaskGroupEntity> taskGroups = _appDbContext.TaskGroup.ToList();
                ViewBag.TaskGroups = taskGroups;

                if (ModelState.IsValid)
                {
                    TaskEntity entity = new TaskEntity();
                    entity.TaskId = collection["TaskId"];
                    entity.Name = collection["Name"];
                    entity.TimeBegin = TimeOnly.Parse(collection["TimeBegin"]);
                    entity.TimeEnd = TimeOnly.Parse(collection["TimeEnd"]);
                    entity.DayActive = (DayActive)Enum.Parse(typeof(DayActive), collection["DayActive"]);
                    entity.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                    entity.TaskGroupId = int.Parse(collection["TaskGroupId"]);
                    entity.LastModified = DateTime.Now;
                    _appDbContext.Task.Add(entity);


                    TaskStatusEntity taskStatus = new TaskStatusEntity();
                    taskStatus.TaskId = collection["TaskId"];
                    _appDbContext.TaskStatuse.Add(taskStatus);


                    _appDbContext.SaveChanges();

                    return RedirectToAction(nameof(Tasks));
                }
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }

        public IActionResult TaskDetails(string taskId)
        {
            TaskEntity task = _appDbContext.Task.FirstOrDefault(x => x.TaskId == taskId);
            List<TaskStepEntity> steps = _appDbContext.TaskStep.Where(x => x.TaskId == taskId).OrderBy(x => x.StepNumber).ToList();
            TaskDetailsViewModel taskDetails = new TaskDetailsViewModel(task, steps);
			List<AddresseeGroupEntity> addresseeGroups = _appDbContext.AddresseeGroup.ToList();
			ViewBag.AddresseeGroups = addresseeGroups;
			List<TaskGroupEntity> taskGroups = _appDbContext.TaskGroup.ToList();
			ViewBag.TaskGroups = taskGroups;
			return View(taskDetails);
        }

        [HttpGet]
        public IActionResult CreateStep(string idTask)
        {
			ViewBag.MaxNumber = _appDbContext.TaskStep.Where(_x => _x.TaskId == idTask).ToList().Count + 1;
            ViewBag.TaskId = idTask;
			return View();
        }

		[HttpPost]
		public IActionResult CreateStep(IFormCollection collection, string taskId)
		{
			try
			{
				List<TaskStepEntity> steps = _appDbContext.TaskStep.Where(x => x.TaskId == taskId).ToList();

				if (ModelState.IsValid)
				{
					TaskStepEntity stepEntity = new TaskStepEntity();
					stepEntity.TaskId = taskId;
					stepEntity.StepNumber = int.Parse(collection["StepNumber"]);
					stepEntity.Description = collection["Description"];
                    stepEntity.Source = collection["Source"];
                    stepEntity.Destination = collection["Destination"];
                    stepEntity.FileMask = collection["FileMask"];
					stepEntity.IsActive = Convert.ToBoolean(collection["IsActive"].ToString().Split(',')[0]);
					stepEntity.IsBreak = Convert.ToBoolean(collection["IsBreak"].ToString().Split(',')[0]);
					stepEntity.OperationName = (OperationName)Enum.Parse(typeof(OperationName), collection["OperationName"]);

					foreach (var step in steps)
					{
						if (step.StepNumber >= stepEntity.StepNumber)
						{
							step.StepNumber++;
						}
					}
					_appDbContext.TaskStep.UpdateRange(steps);

					_appDbContext.TaskStep.Add(stepEntity);
					TaskEntity task = _appDbContext.Task.FirstOrDefault(x => x.TaskId == taskId);
					task.LastModified = DateTime.Now;
					_appDbContext.Task.Update(task);

					_appDbContext.SaveChanges();

					return RedirectToAction("TaskDetails", new { taskId = taskId });
				}
				return View();
			}
			catch (Exception)
			{
				return View();
			}
		}

        [HttpPost]
        public IActionResult ReplaceStep(string taskId, string numberStep, string operation)
        {
			List<TaskStepEntity> steps = _appDbContext.TaskStep.Where(x => x.TaskId == taskId).OrderBy(x => x.StepNumber).ToList();
            TaskStepEntity step1, step2, tmpStep;
            switch (operation)
            {
                case "up":
                    if (int.Parse(numberStep) > 1)
                    {
                        step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
						step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) - 1);
						step1.StepNumber = int.Parse(numberStep) - 1;
						step2.StepNumber = int.Parse(numberStep);
					}
                    break;
                case "down":
					if (int.Parse(numberStep) < steps.Count)
					{
						step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
						step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) + 1);
						step1.StepNumber = int.Parse(numberStep) + 1;
						step2.StepNumber = int.Parse(numberStep);
					}
					break;
                case "maxup":
					if (int.Parse(numberStep) > 1)
					{
                        for (int i = int.Parse(numberStep) - 1; i > 0; i--)
                        {
                            steps[i].StepNumber = i;
                            steps[i-1].StepNumber = i + 1;
                            tmpStep = steps[i];
                            steps[i] = steps[i - 1];
                            steps[i - 1] = tmpStep;
                        }
					}
					break;
                case "maxdown":
					if (int.Parse(numberStep) < steps.Count)
					{
                        for (int i = int.Parse(numberStep) - 1; i < steps.Count - 1; i++)
                        {
							steps[i].StepNumber = i + 2;
							steps[i + 1].StepNumber = i + 1;
							tmpStep = steps[i];
							steps[i] = steps[i + 1];
							steps[i + 1] = tmpStep;
						}
					}
					break;
                default:
                    break;
            }
			_appDbContext.SaveChanges();


			return RedirectToAction("TaskDetails", new { taskId = taskId });
		}

        public IActionResult TaskLog(string dateFrom, string dateTo, string taskId)
        {
			DateTime date = dateFrom == null ? DateTime.Today : DateTime.Parse(dateFrom);
			DateTime date2 = dateTo == null ? DateTime.Today : DateTime.Parse(dateTo);

			List<TaskLogEntity> taskLogs = _appDbContext.TaskLog.Where(x => x.TaskId == taskId && 
                                                                            x.DateTimeLog.Date >= date &&
																			x.DateTimeLog.Date <= date2).OrderByDescending(x => x.DateTimeLog).ToList();
			ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
			ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
			ViewBag.TaskId = taskId;
			return View(taskLogs);
        }

		[HttpPost]
		public IActionResult CreateTaskGroup(string nameGroup)
		{
			TaskGroupEntity taskGroups = _appDbContext.TaskGroup.FirstOrDefault(x => x.Name == nameGroup);
			if (taskGroups == null)
			{
				TaskGroupEntity taskGroupEntity = new TaskGroupEntity();
				taskGroupEntity.Name = nameGroup;
				_appDbContext.TaskGroup.Add(taskGroupEntity);
				_appDbContext.SaveChanges();

				_userLogging.Logging(HttpContext.User.Identity.Name, $"Создание группы задач: {taskGroupEntity.Id}", JsonSerializer.Serialize(taskGroupEntity));
			}


			return RedirectToAction("Tasks");
		}

		public IActionResult DeleteTaskGroup(int idDeleteGroup)
		{
			if (idDeleteGroup == 0)
			{
				return RedirectToAction("Tasks");
			}
			List<TaskEntity> tasks = _appDbContext.Task.Where(x => x.TaskGroupId == idDeleteGroup).ToList();
			foreach (var task in tasks)
			{
				task.TaskGroupId = 0;
			}
			_appDbContext.SaveChanges();

			TaskGroupEntity taskGroup = _appDbContext.TaskGroup.FirstOrDefault(x => x.Id == idDeleteGroup);
			_appDbContext.TaskGroup.Remove(taskGroup);
			_appDbContext.SaveChanges();

			_userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление группы задач: {taskGroup.Id}", JsonSerializer.Serialize(taskGroup));

			return RedirectToAction("Tasks");
		}

		[HttpPost]
		public IActionResult ActivatedTask(string id)
		{
			TaskEntity task = _appDbContext.Task.FirstOrDefault(x => x.TaskId == id);
			task.IsActive = !task.IsActive;
			task.LastModified = DateTime.Now;
			_appDbContext.Task.Update(task);
			_appDbContext.SaveChanges();
			if (task.IsActive)
			{
				_userLogging.Logging(HttpContext.User.Identity.Name, $"Включение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
			}
			else
			{
				_userLogging.Logging(HttpContext.User.Identity.Name, $"Выключение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
			}

			List<TaskEntity> entities = _appDbContext.Task.ToList();
			return RedirectToAction("Tasks");
		}

		[HttpPost]
		public IActionResult EditTask(IFormCollection collection, string taskId)
		{
			try
			{
				if (ModelState.IsValid)
				{
					TaskEntity entity = _appDbContext.Task.FirstOrDefault(x => x.TaskId == taskId);
					entity.Name = collection["Task.Name"];
					entity.TimeBegin = TimeOnly.Parse(collection["Task.TimeBegin"]);
					entity.TimeEnd = TimeOnly.Parse(collection["Task.TimeEnd"]);
					entity.DayActive = (DayActive)Enum.Parse(typeof(DayActive), collection["Task.DayActive"]);
					entity.AddresseeGroupId = int.Parse(collection["Task.AddresseeGroupId"]);
					entity.TaskGroupId = int.Parse(collection["Task.TaskGroupId"]);
					entity.LastModified = DateTime.Now;
					_appDbContext.Task.Update(entity);
					_appDbContext.SaveChanges();

					return RedirectToAction("TaskDetails", new { taskId = taskId });
				}
				return RedirectToAction("TaskDetails", new { taskId = taskId });
			}
			catch (Exception)
			{
				return RedirectToAction("TaskDetails", new { taskId = taskId });
			}
			
		}

		[HttpPost]
		public IActionResult ActivatedStep(string taskId, string stepNumber)
		{
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.TaskId == taskId &&
																				 x.StepNumber == int.Parse(stepNumber));
			taskStep.IsActive = !taskStep.IsActive;
			TaskEntity task = _appDbContext.Task.FirstOrDefault(x => x.TaskId == taskId);
			task.LastModified = DateTime.Now;
			_appDbContext.Task.Update(task);
			_appDbContext.TaskStep.Update(taskStep); 
			_appDbContext.SaveChanges();
			if (taskStep.IsActive)
			{
				_userLogging.Logging(HttpContext.User.Identity.Name, 
									$"Включение шага номер {taskStep.StepNumber} задачи {task.TaskId}", 
									JsonSerializer.Serialize(task));
			}
			else
			{
				_userLogging.Logging(HttpContext.User.Identity.Name, 
									$"Выключение шага номер {taskStep.StepNumber} задачи {task.TaskId}", 
									JsonSerializer.Serialize(task));
			}
			return RedirectToAction("TaskDetails", new { taskId = taskId });
		}
		
		public IActionResult StepDetails(string taskId, string stepNumber)
		{
			TaskEntity task;
			task = _appDbContext.Task.FirstOrDefault(x => x.TaskId == taskId);
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.TaskId == taskId && 
																				 x.StepNumber == int.Parse(stepNumber));
			return View(taskStep);
		}

		[HttpPost]
		public IActionResult EditStep(IFormCollection collection, string taskId, string stepNumber)
		{
			try
			{
				if (ModelState.IsValid)
				{
					TaskStepEntity step = _appDbContext.TaskStep.FirstOrDefault(x => x.TaskId == taskId &&
																						x.StepNumber == int.Parse(stepNumber));
					step.Description = collection["Description"];
					step.Source = collection["Source"];
					step.Destination = collection["Destination"];
					step.FileMask = collection["FileMask"];
					step.IsActive = Convert.ToBoolean(collection["IsActive"].ToString().Split(',')[0]);
					step.IsBreak = Convert.ToBoolean(collection["IsBreak"].ToString().Split(',')[0]);
					
					switch (step.OperationName)
					{
						case OperationName.Copy:
							OperationCopyEntity copy = _appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == step.StepId);
							if (copy != null)
							{
								_appDbContext.OperationCopy.Remove(copy);
							}
							break;
						case OperationName.Move:
							OperationMoveEntity move = _appDbContext.OperationMove.FirstOrDefault(x => x.StepId == step.StepId);
							if (move != null)
							{
								_appDbContext.OperationMove.Remove(move);
							}
							break;
						case OperationName.Read:
							OperationReadEntity read = _appDbContext.OperationRead.FirstOrDefault(x => x.StepId == step.StepId);
							if (read != null)
							{
								_appDbContext.OperationRead.Remove(read);
							}
							break;
						case OperationName.Exist:
							OperationExistEntity exist = _appDbContext.OperationExist.FirstOrDefault(x => x.StepId == step.StepId);
							if (exist != null)
							{
								_appDbContext.OperationExist.Remove(exist);
							}
							break;
						case OperationName.Rename:
							OperationRenameEntity rename = _appDbContext.OperationRename.FirstOrDefault(x => x.StepId == step.StepId);
							if (rename != null)
							{
								_appDbContext.OperationRename.Remove(rename);
							}
							break;
						case OperationName.Delete:
							OperationDeleteEntity delete = _appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == step.StepId);
							if (delete != null)
							{
								_appDbContext.OperationDelete.Remove(delete);
							}
							break;
						default:
							break;
					}

					step.OperationName = (OperationName)Enum.Parse(typeof(OperationName), collection["OperationName"]);

					_appDbContext.TaskStep.Update(step);
					_appDbContext.SaveChanges();

					return RedirectToAction("StepDetails", new { taskId = taskId, stepNumber = stepNumber });
				}
				return RedirectToAction("StepDetails", new { taskId = taskId, stepNumber = stepNumber });
			}
			catch (Exception)
			{
				return RedirectToAction("StepDetails", new { taskId = taskId, stepNumber = stepNumber });
			}

		}

		[HttpPost]
		public IActionResult Operation(string stepId, string operationName)
		{
			TaskOperation taskOperation;
			ViewBag.OperationName = operationName;
			ViewBag.StepId = stepId;
            List<AddresseeGroupEntity> addresseeGroups = _appDbContext.AddresseeGroup.ToList();
            ViewBag.AddresseeGroups = addresseeGroups;
            switch (operationName)
			{
				case "Copy":
					taskOperation = _appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == int.Parse(stepId));
					return PartialView("OperationCopy", taskOperation);
                case "Exist":
                    taskOperation = _appDbContext.OperationExist.FirstOrDefault(x => x.StepId == int.Parse(stepId));
                    return PartialView("OperationExist", taskOperation);
                case "Move":
                    taskOperation = _appDbContext.OperationMove.FirstOrDefault(x => x.StepId == int.Parse(stepId));
                    return PartialView("OperationMove", taskOperation);
                case "Read":
                    taskOperation = _appDbContext.OperationRead.FirstOrDefault(x => x.StepId == int.Parse(stepId));
                    return PartialView("OperationRead", taskOperation);
                case "Rename":
                    taskOperation = _appDbContext.OperationRename.FirstOrDefault(x => x.StepId == int.Parse(stepId));
                    return PartialView("OperationRename", taskOperation);
                case "Delete":
                    taskOperation = _appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == int.Parse(stepId));
                    return PartialView("OperationDelete", taskOperation);
                default:
					break;
			}
			return RedirectToAction("Tasks");
        }

		public IActionResult CreateOperationCopy(IFormCollection collection, string stepId)
		{
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == int.Parse(stepId));

            if (ModelState.IsValid)
            {
                OperationCopyEntity operation = new OperationCopyEntity();
				operation.StepId = int.Parse(stepId);
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess) {
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.FileInSource = (FileInSource)Enum.Parse(typeof(FileInSource), collection["FileInSource"]);
				operation.FileInDestination = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["FileInDestination"]);
                operation.FileInLog = Convert.ToBoolean(collection["FileInLog"].ToString().Split(',')[0]);
				operation.Sort = (SortFiles)Enum.Parse(typeof(SortFiles), collection["Sort"]);
				if (collection["FilesForProcessing"] == "")
				{
					operation.FilesForProcessing = 0;
				}
				else
				{
					operation.FilesForProcessing = int.Parse(collection["FilesForProcessing"]);
				}
				operation.FileAttribute = (AttributeFile)Enum.Parse(typeof(AttributeFile), collection["FileAttribute"]);

                _appDbContext.OperationCopy.Add(operation);
                _appDbContext.SaveChanges();

				operation = _appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == int.Parse(stepId));
				taskStep.OperationId = operation.OperationId;
				_appDbContext.TaskStep.Update(taskStep);
				_appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult EditOperationCopy(IFormCollection collection, string operationId)
		{
			OperationCopyEntity operation = _appDbContext.OperationCopy.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

			if (ModelState.IsValid)
			{
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.FileInSource = (FileInSource)Enum.Parse(typeof(FileInSource), collection["FileInSource"]);
				operation.FileInDestination = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["FileInDestination"]);
				operation.FileInLog = Convert.ToBoolean(collection["FileInLog"].ToString().Split(',')[0]);
				operation.Sort = (SortFiles)Enum.Parse(typeof(SortFiles), collection["Sort"]);
				if (collection["FilesForProcessing"] == "")
				{
					operation.FilesForProcessing = 0;
				}
				else
				{
					operation.FilesForProcessing = int.Parse(collection["FilesForProcessing"]);
				}
				
				operation.FileAttribute = (AttributeFile)Enum.Parse(typeof(AttributeFile), collection["FileAttribute"]);

				_appDbContext.OperationCopy.Update(operation);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult CreateOperationMove(IFormCollection collection, string stepId)
		{
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == int.Parse(stepId));

			if (ModelState.IsValid)
			{
				OperationMoveEntity operation = new OperationMoveEntity();
				operation.StepId = int.Parse(stepId);
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.FileInDestination = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["FileInDestination"]);
				operation.FileInLog = Convert.ToBoolean(collection["FileInLog"].ToString().Split(',')[0]);
				operation.Sort = (SortFiles)Enum.Parse(typeof(SortFiles), collection["Sort"]);
				if (collection["FilesForProcessing"] == "")
				{
					operation.FilesForProcessing = 0;
				}
				else
				{
					operation.FilesForProcessing = int.Parse(collection["FilesForProcessing"]);
				}
				operation.FileAttribute = (AttributeFile)Enum.Parse(typeof(AttributeFile), collection["FileAttribute"]);

				_appDbContext.OperationMove.Add(operation);
				_appDbContext.SaveChanges();
				operation = _appDbContext.OperationMove.FirstOrDefault(x => x.StepId == int.Parse(stepId));
				taskStep.OperationId = operation.OperationId;
				_appDbContext.TaskStep.Update(taskStep);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult EditOperationMove(IFormCollection collection, string operationId)
		{
			OperationMoveEntity operation = _appDbContext.OperationMove.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

			if (ModelState.IsValid)
			{
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.FileInDestination = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["FileInDestination"]);
				operation.FileInLog = Convert.ToBoolean(collection["FileInLog"].ToString().Split(',')[0]);
				operation.Sort = (SortFiles)Enum.Parse(typeof(SortFiles), collection["Sort"]);
				if (collection["FilesForProcessing"] == "")
				{
					operation.FilesForProcessing = 0;
				}
				else
				{
					operation.FilesForProcessing = int.Parse(collection["FilesForProcessing"]);
				}

				operation.FileAttribute = (AttributeFile)Enum.Parse(typeof(AttributeFile), collection["FileAttribute"]);

				_appDbContext.OperationMove.Update(operation);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult CreateOperationDelete(IFormCollection collection, string stepId)
		{
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == int.Parse(stepId));

			if (ModelState.IsValid)
			{
				OperationDeleteEntity operation = new OperationDeleteEntity();
				operation.StepId = int.Parse(stepId);
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];

				_appDbContext.OperationDelete.Add(operation);
				_appDbContext.SaveChanges();
				operation = _appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == int.Parse(stepId));
				taskStep.OperationId = operation.OperationId;
				_appDbContext.TaskStep.Update(taskStep);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult EditOperationDelete(IFormCollection collection, string operationId)
		{
			OperationDeleteEntity operation = _appDbContext.OperationDelete.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

			if (ModelState.IsValid)
			{
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				

				_appDbContext.OperationDelete.Update(operation);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}


		public IActionResult CreateOperationRead(IFormCollection collection, string stepId)
		{
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == int.Parse(stepId));

			if (ModelState.IsValid)
			{
				OperationReadEntity operation = new OperationReadEntity();
				operation.StepId = int.Parse(stepId);
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.FileInSource = (FileInSource)Enum.Parse(typeof(FileInDestination), collection["FileInSource"]);
				operation.Encode = (Encode)Enum.Parse(typeof(Encode), collection["Encode"]);
				operation.SearchRegex = Convert.ToBoolean(collection["SearchRegex"].ToString().Split(',')[0]);
				operation.FindString = collection["FindString"];
				operation.ExpectedResult = (ExpectedResult)Enum.Parse(typeof(ExpectedResult), collection["ExpectedResult"]);
				operation.BreakTaskAfterError = Convert.ToBoolean(collection["BreakTaskAfterError"].ToString().Split(',')[0]);
				

				_appDbContext.OperationRead.Add(operation);
				_appDbContext.SaveChanges();
				operation = _appDbContext.OperationRead.FirstOrDefault(x => x.StepId == int.Parse(stepId));
				taskStep.OperationId = operation.OperationId;
				_appDbContext.TaskStep.Update(taskStep);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult EditOperationRead(IFormCollection collection, string operationId)
		{
			OperationReadEntity operation = _appDbContext.OperationRead.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

			if (ModelState.IsValid)
			{
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.FileInSource = (FileInSource)Enum.Parse(typeof(FileInDestination), collection["FileInSource"]);
				operation.Encode = (Encode)Enum.Parse(typeof(Encode), collection["Encode"]);
				operation.SearchRegex = Convert.ToBoolean(collection["SearchRegex"].ToString().Split(',')[0]);
				operation.FindString = collection["FindString"];
				operation.ExpectedResult = (ExpectedResult)Enum.Parse(typeof(ExpectedResult), collection["ExpectedResult"]);
				operation.BreakTaskAfterError = Convert.ToBoolean(collection["BreakTaskAfterError"].ToString().Split(',')[0]);

				_appDbContext.OperationRead.Update(operation);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}


		public IActionResult CreateOperationExist(IFormCollection collection, string stepId)
		{
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == int.Parse(stepId));

			if (ModelState.IsValid)
			{
				OperationExistEntity operation = new OperationExistEntity();
				operation.StepId = int.Parse(stepId);
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.ExpectedResult = (ExpectedResult)Enum.Parse(typeof(ExpectedResult), collection["ExpectedResult"]);
				operation.BreakTaskAfterError = Convert.ToBoolean(collection["BreakTaskAfterError"].ToString().Split(',')[0]);


				_appDbContext.OperationExist.Add(operation);
				_appDbContext.SaveChanges();
				operation = _appDbContext.OperationExist.FirstOrDefault(x => x.StepId == int.Parse(stepId));
				taskStep.OperationId = operation.OperationId;
				_appDbContext.TaskStep.Update(taskStep);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult EditOperationExist(IFormCollection collection, string operationId)
		{
			OperationExistEntity operation = _appDbContext.OperationExist.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

			if (ModelState.IsValid)
			{
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.ExpectedResult = (ExpectedResult)Enum.Parse(typeof(ExpectedResult), collection["ExpectedResult"]);
				operation.BreakTaskAfterError = Convert.ToBoolean(collection["BreakTaskAfterError"].ToString().Split(',')[0]);

				_appDbContext.OperationExist.Update(operation);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}


		public IActionResult CreateOperationRename(IFormCollection collection, string stepId)
		{
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == int.Parse(stepId));

			if (ModelState.IsValid)
			{
				OperationRenameEntity operation = new OperationRenameEntity();
				operation.StepId = int.Parse(stepId);
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.Pattern = collection["Pattern"];


				_appDbContext.OperationRename.Add(operation);
				_appDbContext.SaveChanges();
				operation = _appDbContext.OperationRename.FirstOrDefault(x => x.StepId == int.Parse(stepId));
				taskStep.OperationId = operation.OperationId;
				_appDbContext.TaskStep.Update(taskStep);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}

		public IActionResult EditOperationRename(IFormCollection collection, string operationId)
		{
			OperationRenameEntity operation = _appDbContext.OperationRename.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
			TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

			if (ModelState.IsValid)
			{
				operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
				if (operation.InformSuccess)
				{
					operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
				}
				operation.AdditionalText = collection["AdditionalText"];
				operation.Pattern = collection["Pattern"];

				_appDbContext.OperationRename.Update(operation);
				_appDbContext.SaveChanges();

			}


			return RedirectToAction("StepDetails", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
		}



	}
}