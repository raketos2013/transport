using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels.Task;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;
        public TaskController(ILogger<TaskController> logger, UserLogging userLogging, AppDbContext appDbContext)
        {
            _logger = logger;
            _userLogging = userLogging;
            _appDbContext = appDbContext;
        }
        public IActionResult Tasks()
        {
            List<TaskGroupEntity> tasksGroups = _appDbContext.TaskGroup.ToList();
            return View(tasksGroups);
        }


        [HttpPost]
        public IActionResult TasksList(string taskGroup)
        {
            List<TaskEntity> tasks = _appDbContext.Task.ToList();
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
					stepEntity.IsBreak = Convert.ToBoolean(collection["IsActive"].ToString().Split(',')[0]);
					stepEntity.OperationName = (OperationName)Enum.Parse(typeof(OperationName), collection["OperationName"]);

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
			return View(taskLogs);
        }
	}
}
