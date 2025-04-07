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
		private readonly IAddresseeService _addresseeService;
		private readonly IStepService _stepService;
        public TaskController(ILogger<TaskController> logger, 
								UserLogging userLogging, 
								AppDbContext appDbContext, 
								ITaskService taskService,
								IAddresseeService addresseeService,
								IStepService stepService)
        {
            _logger = logger;
            _userLogging = userLogging;
            _appDbContext = appDbContext;
			_taskService = taskService;
			_addresseeService = addresseeService;
			_stepService = stepService;
        }

        public IActionResult Tasks()
        {
			List<TaskGroupEntity> tasksGroups = _taskService.GetAllGroups().OrderBy(x => x.Id).ToList();
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
            ViewBag.AddresseeGroups = _addresseeService.GetAllAddresseeGroups();
            ViewBag.TaskGroups = _taskService.GetAllGroups();
            TaskEntity task = new TaskEntity();
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateTask(TaskEntity task)
        {
            try
            {
                if (ModelState.IsValid)
                {
					_taskService.CreateTask(task);
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
            TaskEntity task = _taskService.GetTaskById(taskId);
            List<TaskStepEntity> steps = _stepService.GetAllStepsByTaskId(taskId).OrderBy(x => x.StepNumber).ToList();
            TaskDetailsViewModel taskDetails = new TaskDetailsViewModel(task, steps);
            ViewBag.AddresseeGroups = _addresseeService.GetAllAddresseeGroups();
            ViewBag.TaskGroups = _taskService.GetAllGroups();
            return View(taskDetails);
        }

        [HttpGet]
        public IActionResult CreateStep(string idTask)
        {
			ViewBag.MaxNumber = _stepService.GetAllStepsByTaskId(idTask).Count + 1;
            ViewBag.TaskId = idTask;
			TaskStepEntity step = new TaskStepEntity();
			step.TaskId = idTask;
			return View(step);
        }

		[HttpPost]
		public IActionResult CreateStep(TaskStepEntity modelStep, string taskId)
		{
			try
			{
				if (ModelState.IsValid)
				{
                    _stepService.CreateStep(modelStep);
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
			_stepService.ReplaceSteps(taskId, numberStep, operation);
			return RedirectToAction("TaskDetails", new { taskId = taskId });
		}

        public IActionResult TaskLog(string dateFrom, string dateTo, string taskId)
        {
			DateTime date = dateFrom == null ? DateTime.Today : DateTime.Parse(dateFrom);
			DateTime date2 = dateTo == null ? DateTime.Today : DateTime.Parse(dateTo);

			List<TaskLogEntity> taskLogs = _appDbContext.TaskLog.Where(x => x.TaskId == taskId && 
                                                                            x.DateTimeLog.Date >= date &&
																			x.DateTimeLog.Date <= date2).OrderBy(x => x.DateTimeLog).ToList();
			ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
			ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
			ViewBag.TaskId = taskId;
			return View(taskLogs);
        }

		[HttpPost]
		public IActionResult CreateTaskGroup(string nameGroup)
		{
			_taskService.CreateTaskGroup(nameGroup);
			return RedirectToAction("Tasks");
		}

		public IActionResult DeleteTaskGroup(int idDeleteGroup)
		{
			if (idDeleteGroup == 0)
			{
				return RedirectToAction("Tasks");
			}
			_taskService.DeleteTaskGroup(idDeleteGroup);
			//_userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление группы задач: {taskGroup.Id}", JsonSerializer.Serialize(taskGroup));
			return RedirectToAction("Tasks");
		}

		[HttpPost]
		public IActionResult ActivatedTask(string id)
		{
			_taskService.ActivatedTask(id);
			return RedirectToAction("Tasks");
		}

		[HttpPost]
		public IActionResult EditTask(TaskEntity task, string taskId)
		{
			try
			{
				if (ModelState.IsValid)
				{
					_taskService.EditTask(task);
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
		public IActionResult ActivatedStep(string taskId, int stepId)
		{
			_stepService.ActivatedStep(stepId);
			return RedirectToAction("TaskDetails", new { taskId = taskId });
		}
		
		public IActionResult StepDetails(string taskId, string stepNumber)
		{
			TaskStepEntity? taskStep = _stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
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
						case OperationName.Clrbuf:
							OperationClrbufEntity clrbuf = _appDbContext.OperationClrbuf.FirstOrDefault(x => x.StepId == step.StepId);
							if (clrbuf != null)
							{
								_appDbContext.OperationClrbuf.Remove(clrbuf);
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