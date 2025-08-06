using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using FileManager.Extensions;
using FileManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class TaskController(ITaskService taskService,
                            IAddresseeService addresseeService,
                            IStepService stepService,
                            ITaskLogService taskLogService)
            : Controller
{
    public IActionResult Tasks()
    {
        return View();
    }

    [HttpPost]
    public IActionResult TasksList(/*string taskGroup*/)
    {
        //List<TaskEntity> tasks = taskService.GetTasksByGroup(taskGroup);
        var tasks = taskService.GetAllTasks();
        List<TaskStatusEntity> statuses = taskService.GetTaskStatuses();
        foreach (var item in tasks)
        {
            item.TaskStatus = statuses.FirstOrDefault(x => x.TaskId == item.TaskId);
        }
        return PartialView("_TasksList", tasks);
    }

    [HttpGet]
    public IActionResult CreateTask()
    {
        ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
        //ViewBag.TaskGroups = taskService.GetAllGroups();
        TaskEntity task = new();
        return PartialView("_CreateTask", task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateTask(TaskEntity task)
    {
        ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
        //ViewBag.TaskGroups = taskService.GetAllGroups();
        try
        {
            if (ModelState.IsValid)
            {
                taskService.CreateTask(task);
                return RedirectToAction(nameof(Tasks));
            }
            return PartialView("_CreateTask", task);
        }
        catch (Exception)
        {
            return PartialView("_CreateTask", task);
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
        //ViewBag.TaskGroups = taskService.GetAllGroups();
        return PartialView("_TaskDetails", taskDetails);
    }

    [HttpPost]
    public IActionResult DeleteTask(string taskId)
    {
        taskService.DeleteTask(taskId);
        return RedirectToAction("Tasks");
    }

    public IActionResult TaskLog(TaskLogViewModel model, string? taskId, int? page)
    {
        if (taskId != null)
        {
            @ViewBag.TaskId = taskId;
        }

        DateTime date = model.DateFrom == DateTime.MinValue ? DateTime.Today : model.DateFrom;
        DateTime date2 = model.DateTo == DateTime.MinValue ? DateTime.Today : model.DateTo;
        DateTime timeFrom = model.TimeFrom == DateTime.MinValue ? DateTime.Today : model.TimeFrom;
        DateTime timeTo = model.TimeTo == DateTime.MinValue ? DateTime.Today : model.TimeTo;

        int pageNumber = page ?? 1;


        if (model.PageSize == 0)
        {
            model.PageSize = 10;
        }

        TaskLogViewModel sessionModel = HttpContext?.Session.Get<TaskLogViewModel>("LogFilters");
        if (sessionModel != null)
        {
            date = sessionModel.DateFrom;
            date2 = sessionModel.DateTo;
            timeFrom = sessionModel.TimeFrom;
            timeTo = sessionModel.TimeTo;
            IEnumerable<TaskLogEntity> taskLogs = [];
            if (sessionModel.TaskId != null)
            {
                if (sessionModel.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && sessionModel.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
                {
                    taskLogs = taskLogService.GetLogsByTaskId(sessionModel.TaskId)
                                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                                    x.DateTimeLog.Date <= date2)
                                                        .ToList();
                }
                else
                {
                    taskLogs = taskLogService.GetLogsByTaskId(sessionModel.TaskId)
                                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                                    x.DateTimeLog.Date <= date2 &&
                                                                    x.DateTimeLog.TimeOfDay >= timeFrom.TimeOfDay &&
                                                                    x.DateTimeLog.TimeOfDay <= timeTo.TimeOfDay)
                                                        .ToList();
                }
            }
            else
            {
                if (sessionModel.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && sessionModel.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
                {
                    taskLogs = taskLogService.GetLogs()
                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                        x.DateTimeLog.Date <= date2)
                                            .ToList();
                }
                else
                {
                    taskLogs = taskLogService.GetLogs()
                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                        x.DateTimeLog.Date <= date2 &&
                                                        x.DateTimeLog.TimeOfDay >= timeFrom.TimeOfDay &&
                                                        x.DateTimeLog.TimeOfDay <= timeTo.TimeOfDay)
                                            .ToList();
                }
            }

            if (sessionModel.PageSize == 0)
            {
                sessionModel.PageSize = 10;
            }
            if (taskLogs != null)
            {
                if (sessionModel.OperationName != OperationName.None)
                {
                    taskLogs = taskLogs.Where(x => x.OperationName == sessionModel.OperationName.ToString()).ToList();
                }
                if (sessionModel.StepNumber != 0)
                {
                    taskLogs = taskLogs.Where(x => x.StepNumber == sessionModel.StepNumber).ToList();
                }
                if (sessionModel.ResultOperation != ResultOperation.N)
                {
                    taskLogs = taskLogs.Where(x => x.ResultOperation == sessionModel.ResultOperation).ToList();
                }
                if (!string.IsNullOrEmpty(sessionModel.FileName))
                {
                    taskLogs = taskLogs.Where(x => x.FileName == sessionModel.FileName).ToList();
                }
                if (!string.IsNullOrEmpty(sessionModel.Text))
                {
                    taskLogs = taskLogs.Where(x => x.ResultText == sessionModel.Text).ToList();
                }

                switch (sessionModel.FieldSortLogs)
                {
                    case FieldSortLogs.Date:
                        if (sessionModel.SortLogs == SortLogs.Ascending)
                        {
                            taskLogs = taskLogs.OrderBy(x => x.DateTimeLog).ToList();
                        }
                        else
                        {
                            taskLogs = taskLogs.OrderByDescending(x => x.DateTimeLog).ToList();
                        }
                        break;
                    case FieldSortLogs.Task:
                        if (sessionModel.SortLogs == SortLogs.Ascending)
                        {
                            taskLogs = taskLogs.OrderBy(x => x.TaskId).ToList();
                        }
                        else
                        {
                            taskLogs = taskLogs.OrderByDescending(x => x.TaskId).ToList();
                        }
                        break;
                    case FieldSortLogs.Operation:
                        if (sessionModel.SortLogs == SortLogs.Ascending)
                        {
                            taskLogs = taskLogs.OrderBy(x => x.OperationName).ToList();
                        }
                        else
                        {
                            taskLogs = taskLogs.OrderByDescending(x => x.OperationName).ToList();
                        }
                        break;
                    case FieldSortLogs.Result:
                        if (sessionModel.SortLogs == SortLogs.Ascending)
                        {
                            taskLogs = taskLogs.OrderBy(x => x.ResultOperation).ToList();
                        }
                        else
                        {
                            taskLogs = taskLogs.OrderByDescending(x => x.ResultOperation).ToList();
                        }
                        break;
                    case FieldSortLogs.FileName:
                        if (sessionModel.SortLogs == SortLogs.Ascending)
                        {
                            taskLogs = taskLogs.OrderBy(x => x.FileName).ToList();
                        }
                        else
                        {
                            taskLogs = taskLogs.OrderByDescending(x => x.FileName).ToList();
                        }
                        break;
                    default:
                        break;
                }

                TaskLogViewModel viewModel = new()
                {
                    PageSize = sessionModel.PageSize,
                    TaskId = sessionModel.TaskId,
                    DateFrom = date,
                    DateTo = date2,
                    TimeFrom = sessionModel.TimeFrom,
                    TimeTo = sessionModel.TimeTo,
                    Logs = taskLogs.ToPagedList(pageNumber, sessionModel.PageSize),
                    StepNumber = sessionModel.StepNumber,
                    OperationName = sessionModel.OperationName,
                    ResultOperation = sessionModel.ResultOperation,
                    FileName = sessionModel.FileName,
                    Text = sessionModel.Text
                };

                return View(viewModel);
            }

        }

        List<TaskLogEntity> taskLogs2 = [];

        if (string.IsNullOrEmpty(taskId))
        {
            taskLogs2 = taskLogService.GetLogs()
                                        .OrderBy(x => x.DateTimeLog)
                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                    x.DateTimeLog.Date <= date2)
                                        .ToList();
        }
        else
        {
            taskLogs2 = taskLogService.GetLogsByTaskId(taskId)
                                        .OrderBy(x => x.DateTimeLog)
                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                    x.DateTimeLog.Date <= date2)
                                        .ToList();
        }


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

        IEnumerable<TaskLogEntity> taskLogs = [];

        if (string.IsNullOrEmpty(model.TaskId))
        {
            if (model.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && model.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
            {
                taskLogs = taskLogService.GetLogs()
                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                        x.DateTimeLog.Date <= date2)
                                            .ToList();
            }
            else
            {
                taskLogs = taskLogService.GetLogs()
                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                        x.DateTimeLog.Date <= date2 &&
                                                        x.DateTimeLog.TimeOfDay >= model.TimeFrom.TimeOfDay &&
                                                        x.DateTimeLog.TimeOfDay <= model.TimeTo.TimeOfDay)
                                            .ToList();
            }
        }
        else
        {
            if (model.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && model.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
            {
                taskLogs = taskLogService.GetLogsByTaskId(model.TaskId)
                                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                                    x.DateTimeLog.Date <= date2)
                                                        .ToList();
            }
            else
            {
                taskLogs = taskLogService.GetLogsByTaskId(model.TaskId)
                                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                                    x.DateTimeLog.Date <= date2 &&
                                                                    x.DateTimeLog.TimeOfDay >= model.TimeFrom.TimeOfDay &&
                                                                    x.DateTimeLog.TimeOfDay <= model.TimeTo.TimeOfDay)
                                                        .ToList();
            }
        }
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
            switch (model.FieldSortLogs)
            {
                case FieldSortLogs.Date:
                    if (model.SortLogs == SortLogs.Ascending)
                    {
                        taskLogs = taskLogs.OrderBy(x => x.DateTimeLog).ToList();
                    }
                    else
                    {
                        taskLogs = taskLogs.OrderByDescending(x => x.DateTimeLog).ToList();
                    }
                    break;
                case FieldSortLogs.Task:
                    if (model.SortLogs == SortLogs.Ascending)
                    {
                        taskLogs = taskLogs.OrderBy(x => x.TaskId).ToList();
                    }
                    else
                    {
                        taskLogs = taskLogs.OrderByDescending(x => x.TaskId).ToList();
                    }
                    break;
                case FieldSortLogs.Operation:
                    if (model.SortLogs == SortLogs.Ascending)
                    {
                        taskLogs = taskLogs.OrderBy(x => x.OperationName).ToList();
                    }
                    else
                    {
                        taskLogs = taskLogs.OrderByDescending(x => x.OperationName).ToList();
                    }
                    break;
                case FieldSortLogs.Result:
                    if (model.SortLogs == SortLogs.Ascending)
                    {
                        taskLogs = taskLogs.OrderBy(x => x.ResultOperation).ToList();
                    }
                    else
                    {
                        taskLogs = taskLogs.OrderByDescending(x => x.ResultOperation).ToList();
                    }
                    break;
                case FieldSortLogs.FileName:
                    if (model.SortLogs == SortLogs.Ascending)
                    {
                        taskLogs = taskLogs.OrderBy(x => x.FileName).ToList();
                    }
                    else
                    {
                        taskLogs = taskLogs.OrderByDescending(x => x.FileName).ToList();
                    }
                    break;
                default:
                    break;
            }

        }

        model.Logs = null;
        HttpContext?.Session.Set<TaskLogViewModel>("LogFilters", model);

        if (model.PageSize == 0)
        {
            model.PageSize = 10;
        }
        int pageNumber = 1;

        TaskLogViewModel viewModel = new()
        {
            PageSize = model.PageSize,
            TaskId = model.TaskId,
            DateFrom = date,
            DateTo = date2,
            TimeFrom = model.TimeFrom,
            TimeTo = model.TimeTo,
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
        taskService.CreateTaskGroup(nameGroup);
        return RedirectToAction("Tasks");
    }

    public IActionResult DeleteTaskGroup(int idDeleteGroup)
    {
        if (idDeleteGroup == 0)
        {
            return RedirectToAction("Tasks");
        }
        taskService.DeleteTaskGroup(idDeleteGroup);
        return RedirectToAction("Tasks");
    }

    [HttpPost]
    public IActionResult ActivatedTask(string id)
    {
        taskService.ActivatedTask(id);
        return RedirectToAction("Tasks");
    }

    public IActionResult EditTask(string taskId)
    {
        var task = taskService.GetTaskById(taskId);
        ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
        ViewBag.TaskGroups = taskService.GetAllGroups();
        return PartialView("_EditTask", task);
    }

    [HttpPost]
    public IActionResult EditTask(TaskEntity task, string taskId)
    {
        taskService.EditTask(task);
        return RedirectToAction("Tasks", "Task", new { taskId });
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
        task.CopySteps = copySteps;
        task.IsCopySteps = false;
        return PartialView("_StepsForCopy", task);
    }

    [HttpPost]
    public IActionResult CopyTask(CopyTaskViewModel task)
    {
        taskService.CopyTask(task.TaskId, task.NewTaskId, task.IsCopySteps.ToString(), task.CopySteps);
        return RedirectToAction("Tasks");
    }

    [HttpPost]
    public IActionResult LimitTask(string taskId, int limit)
    {
        var task = taskService.GetTaskById(taskId);
        if (task == null)
        {
            return RedirectToAction("Tasks");
        }
        var executing = task.ExecutionLimit - task.ExecutionLeft;
        task.ExecutionLimit = limit;
        if (task.ExecutionLimit <= executing)
        {
            task.ExecutionLeft = 0;
        }
        else
        {
            task.ExecutionLeft = task.ExecutionLimit - executing;
        }
        taskService.EditTask(task);
        return RedirectToAction("Tasks");
    }
}