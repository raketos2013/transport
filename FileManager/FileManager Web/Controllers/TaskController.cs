using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels.Step;
using FileManager.Domain.ViewModels.Task;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FileManager_Web.ViewModels;
using FileManager_Web.Session;
using X.PagedList;
using X.PagedList.Extensions;
using System.Text.Json;



namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class TaskController(ILogger<TaskController> logger,
                                UserLogging userLogging,
                                ITaskService taskService,
                                IAddresseeService addresseeService,
                                IStepService stepService,
                                ITaskLogService taskLogService)
                : Controller
    {
        public IActionResult Tasks()
        {
            List<TaskGroupEntity> tasksGroups = taskService.GetAllGroups().OrderBy(x => x.Id).ToList();
            return View(tasksGroups);
        }

        [HttpPost]
        public IActionResult TasksList(string taskGroup)
        {
            List<TaskEntity> tasks = taskService.GetTasksByGroup(taskGroup);
            return PartialView(tasks);
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
            ViewBag.TaskGroups = taskService.GetAllGroups();
            TaskEntity task = new();
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
                    taskService.CreateTask(task);
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
            TaskEntity task = taskService.GetTaskById(taskId);
            List<TaskStepEntity> steps = stepService.GetAllStepsByTaskId(taskId)
                                                        .OrderBy(x => x.StepNumber)
                                                        .ToList();
            TaskDetailsViewModel taskDetails = new(task, steps);
            ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
            ViewBag.TaskGroups = taskService.GetAllGroups();
            return View(taskDetails);
        }

        public IActionResult TaskLog(TaskLogViewModel model, string? taskId, int? page)
        {
            if (taskId != null)
            {
                @ViewBag.TaskId = taskId;
            }

            DateTime date = model.DateFrom == DateTime.MinValue ? DateTime.Today : model.DateFrom;
            DateTime date2 = model.DateTo == DateTime.MinValue ? DateTime.Today : model.DateTo;


            int pageNumber = (page ?? 1);


            if (model.PageSize == 0)
            {
                model.PageSize = 40;
            }


            TaskLogViewModel sessionModel = HttpContext?.Session.Get<TaskLogViewModel>("LogFilters");
            if (sessionModel != null)
            {
                if (sessionModel.TaskId != null)
                {
                    date = sessionModel.DateFrom;
                    date2 = sessionModel.DateTo;
                    var taskLogs = taskLogService.GetLogsByTaskId(sessionModel.TaskId)
                                                            .OrderBy(x => x.DateTimeLog)
                                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                                        x.DateTimeLog.Date <= date2);
                    if (taskLogs != null)
                    {
                        if (sessionModel.OperationName != OperationName.None)
                        {
                            taskLogs = taskLogs.Where(x => x.OperationName == sessionModel.OperationName.ToString());
                        }
                        if (sessionModel.StepNumber != 0)
                        {
                            taskLogs = taskLogs.Where(x => x.StepNumber == sessionModel.StepNumber);
                        }
                        if (sessionModel.ResultOperation != ResultOperation.N)
                        {
                            taskLogs = taskLogs.Where(x => x.ResultOperation == sessionModel.ResultOperation);
                        }
                        if (!string.IsNullOrEmpty(sessionModel.FileName))
                        {
                            taskLogs = taskLogs.Where(x => x.FileName == sessionModel.FileName);
                        }
                        if (!string.IsNullOrEmpty(sessionModel.Text))
                        {
                            taskLogs = taskLogs.Where(x => x.ResultText == sessionModel.Text);
                        }

                        TaskLogViewModel viewModel = new()
                        {
                            PageSize = sessionModel.PageSize,
                            TaskId = sessionModel.TaskId,
                            DateFrom = date,
                            DateTo = date2,
                            Logs = taskLogs.ToPagedList(pageNumber, model.PageSize),
                            StepNumber = sessionModel.StepNumber,
                            OperationName = sessionModel.OperationName,
                            ResultOperation = sessionModel.ResultOperation,
                            FileName = sessionModel.FileName,
                            Text = sessionModel.Text
                        };

                        return View(viewModel);
                    }
                }
            }

            var taskLogs2 = taskLogService.GetLogsByTaskId(taskId)
                                                            .OrderBy(x => x.DateTimeLog)
                                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                                        x.DateTimeLog.Date <= date2);

            TaskLogViewModel viewModel2 = new()
            {
                TaskId = taskId,
                DateFrom = date,
                DateTo = date2,
                Logs = taskLogs2.ToPagedList(pageNumber, model.PageSize)
            };

            return View(viewModel2);
        }

        [HttpPost]
        public IActionResult TaskLog(TaskLogViewModel model)
        {
            DateTime date = model.DateFrom == DateTime.MinValue ? DateTime.Today : model.DateFrom;
            DateTime date2 = model.DateTo == DateTime.MinValue ? DateTime.Today : model.DateTo;

            var taskLogs = taskLogService.GetLogsByTaskId(model.TaskId)
                                                            .OrderBy(x => x.DateTimeLog)
                                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                                        x.DateTimeLog.Date <= date2);

            if (taskLogs != null)
            {
                if (model.OperationName != OperationName.None)
                {
                    taskLogs = taskLogs.Where(x => x.OperationName == model.OperationName.ToString());
                }
                if (model.StepNumber != 0)
                {
                    taskLogs = taskLogs.Where(x => x.StepNumber == model.StepNumber);
                }
                if (model.ResultOperation != ResultOperation.N)
                {
                    taskLogs = taskLogs.Where(x => x.ResultOperation == model.ResultOperation);
                }
                if (!string.IsNullOrEmpty(model.FileName))
                {
                    taskLogs = taskLogs.Where(x => x.FileName == model.FileName);
                }
                if (!string.IsNullOrEmpty(model.Text))
                {
                    taskLogs = taskLogs.Where(x => x.ResultText == model.Text);
                }

            }


            model.Logs = null;
            HttpContext?.Session.Set<TaskLogViewModel>("LogFilters", model);




            if (model.PageSize == 0)
            {
                model.PageSize = 40;
            }
            int pageNumber = 1;


            TaskLogViewModel viewModel = new()
            {
                PageSize = model.PageSize,
                TaskId = model.TaskId,
                DateFrom = date,
                DateTo = date2,
                Logs = taskLogs.ToPagedList(pageNumber, model.PageSize),
                StepNumber = model.StepNumber,
                ResultOperation = model.ResultOperation,
                OperationName = model.OperationName,
                FileName = model.FileName,
                Text = model.Text
            };

            return View(viewModel);
        }




        [HttpPost]
        public IActionResult CreateTaskGroup(string nameGroup)
        {
            TaskGroupEntity? newTaskGroup = taskService.CreateTaskGroup(nameGroup);
            if (newTaskGroup != null)
            {
                userLogging.Logging(HttpContext.User.Identity.Name, $"Создание группы задач: {newTaskGroup.Name}", JsonSerializer.Serialize(newTaskGroup));
            }
            return RedirectToAction("Tasks");
        }

        public IActionResult DeleteTaskGroup(int idDeleteGroup)
        {
            if (idDeleteGroup == 0)
            {
                return RedirectToAction("Tasks");
            }
            taskService.DeleteTaskGroup(idDeleteGroup);
            //_userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление группы задач: {taskGroup.Id}", JsonSerializer.Serialize(taskGroup));
            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public IActionResult ActivatedTask(string id)
        {
            taskService.ActivatedTask(id);
            TaskEntity task = taskService.GetTaskById(id);
            string message;
            if (task.IsActive)
            {
                message = $"Включил задачу: {id}";
            }
            else
            {
                message = $"Выключил задачу: {id}";
            }
            userLogging.Logging(HttpContext.User.Identity.Name, message, JsonSerializer.Serialize(task));
            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public IActionResult EditTask(TaskEntity task, string taskId)
        {
            taskService.EditTask(task);
            return RedirectToAction("TaskDetails", "Task", new { taskId });
        }

        [HttpPost]
        public IActionResult StepsForCopy(string taskId)
        {
            List<TaskStepEntity> steps = stepService.GetAllStepsByTaskId(taskId)
                                                        .OrderBy(x => x.StepNumber)
                                                        .ToList();
            CopyTaskViewModel task = new();
            List<CopyStepViewModel> copySteps = [];
            task.TaskId = taskId;
            foreach (var step in steps)
            {
                CopyStepViewModel stepViewModel = new()
                {
                    StepNumber = step.StepNumber,
                    Description = step.Description,
                    IsCopy = true,
                    IsCopyOperation = true
                };
                copySteps.Add(stepViewModel);
            }
            return PartialView(copySteps);
        }

        [HttpPost]
        public IActionResult CopyTask(string idTask, string newIdTask, string isCopySteps,
                                        CopyStepViewModel[] copyStep)
        {
            taskService.CopyTask(idTask, newIdTask, isCopySteps, copyStep);
            return RedirectToAction("Tasks");
        }


    }
}