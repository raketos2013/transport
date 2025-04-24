using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels.Step;
using FileManager.Domain.ViewModels.Task;
using FileManager.Services.Implementations;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;


//using Microsoft.Build.Framework;
using System.Text.Json;
using System.Threading.Tasks;
using FileManager_Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FileManager_Web.Session;

namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly UserLogging _userLogging;
        private readonly ITaskService _taskService;
        private readonly IAddresseeService _addresseeService;
        private readonly IStepService _stepService;
        private readonly ITaskLogService _taskLogService;
        private readonly IOperationService _operationService;
        public TaskController(ILogger<TaskController> logger,
                                UserLogging userLogging,
                                ITaskService taskService,
                                IAddresseeService addresseeService,
                                IStepService stepService,
                                ITaskLogService taskLogService,
                                IOperationService operationService)
        {
            _logger = logger;
            _userLogging = userLogging;
            _taskService = taskService;
            _addresseeService = addresseeService;
            _stepService = stepService;
            _taskLogService = taskLogService;
            _operationService = operationService;
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
            List<TaskStepEntity> steps = _stepService.GetAllStepsByTaskId(taskId)
                                                        .OrderBy(x => x.StepNumber)
                                                        .ToList();
            TaskDetailsViewModel taskDetails = new(task, steps);
            ViewBag.AddresseeGroups = _addresseeService.GetAllAddresseeGroups();
            ViewBag.TaskGroups = _taskService.GetAllGroups();
            return View(taskDetails);
        }

        /* public IActionResult TaskLog(string dateFrom, string dateTo, string taskId, int? page)
         {
             DateTime date = dateFrom == null ? DateTime.Today : DateTime.Parse(dateFrom);
             DateTime date2 = dateTo == null ? DateTime.Today : DateTime.Parse(dateTo);

             List<TaskLogEntity> taskLogs = _taskLogService.GetLogsByTaskId(taskId)
                                                             .Where(x => x.DateTimeLog.Date >= date &&
                                                                         x.DateTimeLog.Date <= date2)
                                                             .OrderBy(x => x.DateTimeLog)
                                                             .ToList();
             ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
             ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
             ViewBag.TaskId = taskId;

             int pageSize = 40;
             int pageNumber = (page ?? 1);

             return View(taskLogs.ToPagedList(pageNumber, pageSize));
         }*/


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
                    var taskLogs = _taskLogService.GetLogsByTaskId(sessionModel.TaskId)
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

                        //taskLogs = taskLogs.Skip(sessionModel.PageSize * (page ?? 1)).Take(sessionModel.PageSize);

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

            var taskLogs2 = _taskLogService.GetLogsByTaskId(taskId)
                                                            .OrderBy(x => x.DateTimeLog)
                                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                                        x.DateTimeLog.Date <= date2)
                                                            //.Skip(1)
                                                            .Take(40);

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

            var taskLogs = _taskLogService.GetLogsByTaskId(model.TaskId)
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
            _taskService.EditTask(task);
            return RedirectToAction("TaskDetails", "Task", new { taskId });
        }

        [HttpPost]
        public IActionResult StepsForCopy(string taskId)
        {
            List<TaskStepEntity> steps = _stepService.GetAllStepsByTaskId(taskId)
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
            _taskService.CopyTask(idTask, newIdTask, isCopySteps, copyStep);
            return RedirectToAction("Tasks");
        }


    }
}