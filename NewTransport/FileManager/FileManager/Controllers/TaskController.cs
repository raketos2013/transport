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
                            ITaskLogService taskLogService,
                            ILockService lockService,
                            IHttpContextAccessor httpContextAccessor)
            : Controller
{
    public IActionResult Tasks()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> TasksList()
    {
        var tasks = await taskService.GetAllTasks();
        List<TaskStatusEntity> statuses = await taskService.GetTaskStatuses();
        foreach (var item in tasks)
        {
            item.TaskStatus = statuses.FirstOrDefault(x => x.TaskId == item.TaskId);
        }
        return PartialView("_TasksList", tasks);
    }

    [HttpGet]
    public async Task<IActionResult> CreateTask()
    {
        ViewBag.AddresseeGroups = await addresseeService.GetAllAddresseeGroups();
        TaskEntity task = new();
        return PartialView("_CreateTask", task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTask(TaskEntity task)
    {
        ViewBag.AddresseeGroups = await addresseeService.GetAllAddresseeGroups();
        try
        {
            if (ModelState.IsValid)
            {
                await taskService.CreateTask(task);
                return RedirectToAction(nameof(Tasks));
            }
            return PartialView("_CreateTask", task);
        }
        catch (Exception)
        {
            return PartialView("_CreateTask", task);
        }
    }

    public async Task<IActionResult> TaskDetails(string taskId)
    {
        TaskEntity task = await taskService.GetTaskById(taskId);
        var stepsAsync = await stepService.GetAllStepsByTaskId(taskId);
        var steps = stepsAsync.OrderBy(x => x.StepNumber)
                                 .ToList();
        TaskDetailsViewModel taskDetails = new(task, steps);
        ViewBag.AddresseeGroups = await addresseeService.GetAllAddresseeGroups();
        return PartialView("_TaskDetails", taskDetails);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTask(string taskId)
    {
        await taskService.DeleteTask(taskId);
        return RedirectToAction(nameof(Tasks));
    }

    public async Task<IActionResult> TaskLog(TaskLogViewModel model, string? taskId, int? page)
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
            model.PageSize = 20;
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
                var taskLogsAsync = await taskLogService.GetLogsByTaskId(sessionModel.TaskId);
                if (sessionModel.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && sessionModel.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
                {
                    taskLogs = taskLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
                                                        x.DateTimeLog.Date <= date2)
                                            .ToList();
                }
                else
                {
                    taskLogs = taskLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
                                                        x.DateTimeLog.Date <= date2 &&
                                                        x.DateTimeLog.TimeOfDay >= timeFrom.TimeOfDay &&
                                                        x.DateTimeLog.TimeOfDay <= timeTo.TimeOfDay)
                                            .ToList();
                }
            }
            else
            {
                var taskLogsAsync = await taskLogService.GetLogs();
                if (sessionModel.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && sessionModel.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
                {
                    taskLogs = taskLogsAsync
                                            .Where(x => x.DateTimeLog.Date >= date &&
                                                        x.DateTimeLog.Date <= date2)
                                            .ToList();
                }
                else
                {
                    taskLogs = taskLogsAsync
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
            var taskLogsAsync = await taskLogService.GetLogs();
            taskLogs2 = taskLogsAsync
                                        .OrderBy(x => x.DateTimeLog)
                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                    x.DateTimeLog.Date <= date2)
                                        .ToList();
        }
        else
        {
            var taskLogsAsync = await taskLogService.GetLogsByTaskId(taskId);
            taskLogs2 = taskLogsAsync.OrderBy(x => x.DateTimeLog)
                                     .Where(x => x.DateTimeLog.Date >= date &&
                                                 x.DateTimeLog.Date <= date2)
                                     .ToList();
        }


        TaskLogViewModel viewModel2 = new()
        {
            TaskId = taskId,
            DateFrom = date,
            DateTo = date2,
            PageSize = model.PageSize,
            Logs = taskLogs2.ToPagedList(pageNumber, model.PageSize)
        };

        return View(viewModel2);
    }

    [HttpPost]
    public async Task<IActionResult> TaskLog(TaskLogViewModel model)
    {
        DateTime date = model.DateFrom == DateTime.MinValue ? DateTime.Today : model.DateFrom;
        DateTime date2 = model.DateTo == DateTime.MinValue ? DateTime.Today : model.DateTo;

        IEnumerable<TaskLogEntity> taskLogs = [];

        if (string.IsNullOrEmpty(model.TaskId))
        {
            var taskLogsAsync = await taskLogService.GetLogs();
            if (model.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && model.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
            {
                taskLogs = taskLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
                                                    x.DateTimeLog.Date <= date2)
                                        .ToList();
            }
            else
            {   
                taskLogs = taskLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
                                                    x.DateTimeLog.Date <= date2 &&
                                                    x.DateTimeLog.TimeOfDay >= model.TimeFrom.TimeOfDay &&
                                                    x.DateTimeLog.TimeOfDay <= model.TimeTo.TimeOfDay)
                                        .ToList();
            }
        }
        else
        {
            var taskLogsAsync = await taskLogService.GetLogsByTaskId(model.TaskId);
            if (model.TimeFrom.TimeOfDay == DateTime.MinValue.TimeOfDay && model.TimeTo.TimeOfDay == DateTime.MinValue.TimeOfDay)
            {
                
                taskLogs = taskLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
                                                    x.DateTimeLog.Date <= date2)
                                        .ToList();
            }
            else
            {
                
                taskLogs = taskLogsAsync.Where(x => x.DateTimeLog.Date >= date &&
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
            model.PageSize = 20;
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
    public async Task<IActionResult> CreateTaskGroup(string nameGroup)
    {
        await taskService.CreateTaskGroup(nameGroup);
        return RedirectToAction(nameof(Tasks));
    }

    public async Task<IActionResult> DeleteTaskGroup(int idDeleteGroup)
    {
        if (idDeleteGroup == 0)
        {
            return RedirectToAction(nameof(Tasks));
        }
        await taskService.DeleteTaskGroup(idDeleteGroup);
        return RedirectToAction(nameof(Tasks));
    }

    [HttpPost]
    public async Task<IActionResult> ActivatedTask(string id)
    {
        await taskService.ActivatedTask(id);
        return RedirectToAction(nameof(Tasks));
    }

    [HttpPost]
    public async Task<IActionResult> UnlockTask(string taskId)
    {
        await lockService.Unlock(taskId);
        return NoContent();
    }
    [HttpGet]
    public async Task<IActionResult> LockTask(string taskId)
    {
        await lockService.Lock(taskId, httpContextAccessor.HttpContext.User.Identity.Name);
        return NoContent();
    }

    public async Task<IActionResult> EditTask(string taskId)
    {
        await lockService.Lock(taskId, httpContextAccessor.HttpContext.User.Identity.Name);
        var task = await taskService.GetTaskById(taskId);
        ViewBag.AddresseeGroups = await addresseeService.GetAllAddresseeGroups();
        ViewBag.TaskGroups = await taskService.GetAllGroups();
        return PartialView("_EditTask", task);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask(TaskEntity task, string taskId)
    {
        await taskService.EditTask(task);
        await lockService.Unlock(taskId);
        return RedirectToAction(nameof(Tasks), "Task", new { taskId });
    }

    [HttpPost]
    public async Task<IActionResult> StepsForCopy(string taskId)
    {
        var stepsAsync = await stepService.GetAllStepsByTaskId(taskId);
        var steps = stepsAsync.OrderBy(x => x.StepNumber)
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
        await lockService.Lock(taskId, httpContextAccessor.HttpContext.User.Identity.Name);
        return PartialView("_StepsForCopy", task);
    }

    [HttpPost]
    public async Task<IActionResult> CopyTask(CopyTaskViewModel task)
    {
        await taskService.CopyTask(task.TaskId, task.NewTaskId, task.IsCopySteps.ToString(), task.CopySteps);
        await lockService.Unlock(task.TaskId);
        return RedirectToAction(nameof(Tasks));
    }

    [HttpPost]
    public async Task<IActionResult> LimitTask(string taskId, int limit)
    {
        var task = await taskService.GetTaskById(taskId);
        if (task == null)
        {
            return RedirectToAction(nameof(Tasks));
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
        await taskService.EditTask(task);
        await lockService.Unlock(taskId);
        return RedirectToAction(nameof(Tasks));
    }

    [HttpGet]
    public async Task<ActionResult<LockedTaskViewModel>> IsLockedTask(string taskId)
    {
        var lockedTask = await lockService.IsLocked(taskId);
        LockedTaskViewModel result = new();
        if (lockedTask != null)
        {
            result.IsLocked = true;
            result.UserId = lockedTask.UserId;
            return Ok(result);
        }
        else
        {
            result.IsLocked = false;
            return Ok(result);
        }
    }

}