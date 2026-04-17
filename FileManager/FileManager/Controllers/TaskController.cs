using FileManager.Core.Constants;
using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.Extensions;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Queries;
using FileManager.Core.ViewModels;
using FileManager.Extensions;
using FileManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using System.Text.Json;
using System.Threading.Tasks;
//using X.PagedList.Extensions;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class TaskController(ITaskService taskService,
                            IAddresseeService addresseeService,
                            IStepService stepService,
                            ITaskLogService taskLogService,
                            ILockService lockService,
                            IUserLogService userLogService,
                            ISchedulerFactory schedulerFactory)
            : Controller
{
    public async Task<IActionResult> Tasks()
    {
        HttpContext?.Session.Set<TaskFilterViewModel>("LogFilters", null);
        var groups = await addresseeService.GetAllAddresseeGroups();
        List<AddresseeGroupViewModel> list = [];
        var allGroup = new AddresseeGroupViewModel()
        {
            Id = 0,
            Name = ""
        };
        list.Add(allGroup);
        foreach (var item in groups)
        {
            var newGroup = new AddresseeGroupViewModel()
            {
                Id = item.Id,
                Name = item.Id + " " + item.Name
            };
            list.Add(newGroup);
        }
        ViewBag.AddresseeGroups = list;
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
        var groups = await addresseeService.GetAllAddresseeGroups();
        List<AddresseeGroupViewModel> list = [];
        foreach (var item in groups)
        {
            var newGroup = new AddresseeGroupViewModel()
            {
                Id = item.Id,
                Name = item.Id + " " + item.Name
            };
            list.Add(newGroup);
        }
        ViewBag.AddresseeGroups = list;
        TaskEntity task = new();
        return PartialView("_CreateTask", task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTask(TaskEntity task)
    {
        if (ModelState.IsValid)
        {
            var tasks = await taskService.GetAllTasks();
            var oldTask = tasks.FirstOrDefault(x => x.TaskId == task.TaskId);
            if (oldTask != null)
            {
                return RedirectToAction(nameof(Tasks));
            }

            var createdTask = await taskService.CreateTask(task);
            await userLogService.AddLog($"Создание задачи {createdTask.TaskId}",
                                        JsonSerializer.Serialize(createdTask, AppConstants.JSON_OPTIONS));
            return RedirectToAction(nameof(Tasks));
        }
        var groups = await addresseeService.GetAllAddresseeGroups();
        List<AddresseeGroupViewModel> list = [];
        foreach (var item in groups)
        {
            var newGroup = new AddresseeGroupViewModel()
            {
                Id = item.Id,
                Name = item.Id + " " + item.Name
            };
            list.Add(newGroup);
        }
        ViewBag.AddresseeGroups = list;
        return RedirectToAction(nameof(Tasks));
    }

    public async Task<IActionResult> TaskDetails(string taskId)
    {
        var task = await taskService.GetTaskById(taskId)
                                ?? throw new DomainException("Задача не найдена");

        var statuses = await taskService.GetTaskStatuses();
        var taskStatus = statuses.FirstOrDefault(x => x.TaskId == task.TaskId);
        if (taskStatus.DateLastExecute.Date != DateTime.Now.Date && taskStatus.Status != StatusTask.Process)
        {
            task.ExecutionCount = 0;
            await taskService.EditTask(task);
        }

        var stepsAsync = await stepService.GetAllStepsByTaskId(taskId);
        var steps = stepsAsync.OrderBy(x => x.StepNumber)
                                 .ToList();
        TaskDetailsViewModel taskDetails = new(task, steps);
        var groups = await addresseeService.GetAllAddresseeGroups();
        ViewBag.AddresseeGroups = groups;
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
        DateTime date = model.DateFrom == DateTime.MinValue ? DateTime.Today : model.DateFrom;
        DateTime date2 = model.DateTo == DateTime.MinValue ? DateTime.Today : model.DateTo;
        DateTime timeFrom = model.TimeFrom == DateTime.MinValue ? DateTime.Today : model.TimeFrom;
        DateTime timeTo = model.TimeTo == DateTime.MinValue ? DateTime.Today : model.TimeTo;

        TaskLogViewModel sessionModel = HttpContext?.Session.Get<TaskLogViewModel>("LogFilters");
        if (sessionModel != null)
        {
            if (sessionModel.PageSize == 0)
            {
                sessionModel.PageSize = 10;
            }
            date = sessionModel.DateFrom;
            date2 = sessionModel.DateTo;
            timeFrom = sessionModel.TimeFrom;
            timeTo = sessionModel.TimeTo;
            Query query2 = new()
            {
                Skip = page ?? 1,
                Take = sessionModel.PageSize,
                TaskId = sessionModel.TaskId,
                TaskIdOption = sessionModel.TaskIdOption,
                DateFrom = date,
                DateTo = date2,
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                StepNumber = sessionModel.StepNumber,
                StepNumberOption = sessionModel.StepNumberOption,
                OperationName = sessionModel.OperationName,
                OperationNameOption = sessionModel.OperationNameOption,
                ResultOperation = sessionModel.ResultOperation,
                ResultOperationOption = sessionModel.ResultOperationOption,
                FileName = sessionModel.FileName,
                FileNameOption = sessionModel.FileNameOption,
                Text = sessionModel.Text,
                TextOption = sessionModel.TextOption,
                FieldSortLogs = sessionModel.FieldSortLogs,
                SortLogs = sessionModel.SortLogs
            };
            var pagedLogs2 = await taskLogService.GetLogs(query2);

            TaskLogViewModel viewModel = new()
            {
                TaskId = taskId,
                DateFrom = date,
                DateTo = date2,
                TimeFrom = timeFrom,
                TimeTo = timeTo,
                StepNumber = sessionModel.StepNumber,
                StepNumberOption = sessionModel.StepNumberOption,
                OperationName = sessionModel.OperationName,
                OperationNameOption = sessionModel.OperationNameOption,
                ResultOperation = sessionModel.ResultOperation,
                ResultOperationOption = sessionModel.ResultOperationOption,
                FileName = sessionModel.FileName,
                FileNameOption = sessionModel.FileNameOption,
                Text = sessionModel.Text,
                TextOption = sessionModel.TextOption,
                FieldSortLogs = sessionModel.FieldSortLogs,
                SortLogs = sessionModel.SortLogs,
                PageSize = sessionModel.PageSize,
                Logs = new PagedList<TaskLogEntity>(pagedLogs2, pagedLogs2.TotalCount, pagedLogs2.CurrentPage, pagedLogs2.PageSize)
            };

            return View(viewModel);
        }

        Query query = new()
        {
            Skip = page ?? 1,
            Take = 20,
            TaskId = taskId,
            DateFrom = date,
            DateTo = date2,
            TimeFrom = timeFrom,
            TimeTo = timeTo,
            FieldSortLogs =FieldSortLogs.DateTimeLog,
            SortLogs = SortLogs.Ascending
        };
        var pagedLogs = await taskLogService.GetLogs(query);

        TaskLogViewModel viewModel2 = new()
        {
            TaskId = taskId,
            DateFrom = date,
            DateTo = date2,
            TimeFrom = timeFrom,
            TimeTo = timeTo,
            PageSize = 20,
            Logs = new PagedList<TaskLogEntity>(pagedLogs, pagedLogs.TotalCount, pagedLogs.CurrentPage, pagedLogs.PageSize)
        };

        return View(viewModel2);
    }

    [HttpPost]
    public async Task<IActionResult> TaskLog(TaskLogViewModel model)
    {
        DateTime date = model.DateFrom == DateTime.MinValue ? DateTime.Today : model.DateFrom;
        DateTime date2 = model.DateTo == DateTime.MinValue ? DateTime.Today : model.DateTo;

        if (model.PageSize == 0)
        {
            model.PageSize = 20;
        }
        Query query2 = new()
        {
            Skip = 1,
            Take = model.PageSize,
            TaskId = model.TaskId,
            TaskIdOption = model.TaskIdOption,
            DateFrom = date,
            DateTo = date2,
            TimeFrom = model.TimeFrom,
            TimeTo = model.TimeTo,
            StepNumber = model.StepNumber,
            StepNumberOption = model.StepNumberOption,
            OperationName = model.OperationName,
            OperationNameOption = model.OperationNameOption,
            ResultOperation = model.ResultOperation,
            ResultOperationOption = model.ResultOperationOption,
            FileName = model.FileName,
            FileNameOption = model.FileNameOption,
            Text = model.Text,
            TextOption = model.TextOption,
            FieldSortLogs = model.FieldSortLogs,
            SortLogs = model.SortLogs
        };
        var pagedLogs = await taskLogService.GetLogs(query2);

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
            StepNumber = model.StepNumber,
            StepNumberOption = model.StepNumberOption,
            OperationName = model.OperationName,
            OperationNameOption = model.OperationNameOption,
            ResultOperation = model.ResultOperation,
            ResultOperationOption = model.ResultOperationOption,
            FileName = model.FileName,
            FileNameOption = model.FileNameOption,
            Text = model.Text,
            TextOption = model.TextOption,
            FieldSortLogs = model.FieldSortLogs,
            SortLogs = model.SortLogs,
            Logs = new PagedList<TaskLogEntity>(pagedLogs, pagedLogs.TotalCount, pagedLogs.CurrentPage, pagedLogs.PageSize)
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ActivatedTask(string id)
    {
        await taskService.ActivatedTask(id);
        var task = await taskService.GetTaskById(id);
        if (task != null)
        {
            var statusAsync = await taskService.GetTaskStatuses();
            var status = statusAsync.First(x => x.TaskId == id);
            if (status != null)
            {
                //status.IsProgress = false;
                //status.IsError = false;
                status.Status = StatusTask.Wait;
                await taskService.UpdateTaskStatus(status);
            }

            var stringResult = "";
            if (task.IsActive)
            {
                stringResult = "Включение";
            }
            else
            {
                stringResult = "Выключение";
                IScheduler scheduler = await schedulerFactory.GetScheduler();
                JobKey jobKey = new JobKey(id, "FManager");
                bool interrupted = await scheduler.Interrupt(jobKey);
                if (interrupted)
                    Console.WriteLine($"---stop task {id}");
                else
                    Console.WriteLine($"---task {id} not found");
            }
            await userLogService.AddLog($"{stringResult} задачи {task.TaskId}",
                                            JsonSerializer.Serialize(task, AppConstants.JSON_OPTIONS));
        }
        return RedirectToAction(nameof(Tasks));
    }

    [HttpPost]
    public async Task<IActionResult> UnlockTask(string taskId)
    {
        await lockService.Unlock(taskId);
        await userLogService.AddLog($"Разблокировка задачи {taskId}", JsonSerializer.Serialize(""));
        return NoContent();
    }
    [HttpGet]
    public async Task<IActionResult> LockTask(string taskId)
    {
        await lockService.Lock(taskId);
        return NoContent();
    }

    public async Task<IActionResult> EditTask(string taskId)
    {
        await lockService.Lock(taskId);
        var task = await taskService.GetTaskById(taskId);
        var groups = await addresseeService.GetAllAddresseeGroups();
        List<AddresseeGroupViewModel> list = [];
        foreach (var item in groups)
        {
            var newGroup = new AddresseeGroupViewModel()
            {
                Id = item.Id,
                Name = item.Id + " " + item.Name
            };
            list.Add(newGroup);
        }
        ViewBag.AddresseeGroups = list;
        return PartialView("_EditTask", task);
    }

    [HttpPost]
    public async Task<IActionResult> EditTask(TaskEntity task, string taskId)
    {
        await taskService.EditTask(task);
        await lockService.Unlock(taskId);
        await userLogService.AddLog($"Изменение задачи {task.TaskId}",
                                            JsonSerializer.Serialize(task, AppConstants.JSON_OPTIONS));
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
        await lockService.Lock(taskId);
        return PartialView("_StepsForCopy", task);
    }

    [HttpPost]
    public async Task<IActionResult> CopyTask(CopyTaskViewModel task)
    {
        await taskService.CopyTask(task.TaskId, task.NewTaskId, task.IsCopySteps.ToString(), task.CopySteps, task.IsActivate);
        await lockService.Unlock(task.TaskId);
        var copiedTask = await taskService.GetTaskById(task.TaskId);
        await userLogService.AddLog($"Копирование задачи {task.TaskId}",
                                            JsonSerializer.Serialize(copiedTask, AppConstants.JSON_OPTIONS));
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
        _ = limit > 9999 ? task.ExecutionLimit = 9999 : task.ExecutionLimit = limit;
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

    [HttpGet]
    public async Task<ActionResult<TaskStatusEntity>> TaskStatuses()
    {
        var statuses = await taskLogService.GetLastLogTasks(); 
        return View(statuses);
    }

    [HttpGet]
    public async Task<ActionResult<TaskStatusEntity>> LockedTasks()
    {
        ViewBag.Date = DateTime.Now;
        var tasks = await lockService.GetLockedTasks();
        return View(tasks);
    }

    [HttpGet]
    public async Task<IActionResult> FilterTask()
    {
        var groups = await addresseeService.GetAllAddresseeGroups();
        List<AddresseeGroupViewModel> list = [];
        var allGroup = new AddresseeGroupViewModel()
        {
            Id = 0,
            Name = ""
        };
        list.Add(allGroup);
        foreach (var item in groups)
        {
            var newGroup = new AddresseeGroupViewModel()
            {
                Id = item.Id,
                Name = item.Id + " " + item.Name
            };
            list.Add(newGroup);
        }
        ViewBag.AddresseeGroups = list;
        TaskFilterViewModel filterViewModel = new();

        TaskFilterViewModel sessionModel = HttpContext?.Session.Get<TaskFilterViewModel>("TaskFilters");

        return PartialView("_TaskFilter", sessionModel);
    }

    [HttpPost]
    public async Task<IActionResult> FilterTask([FromBody] TaskFilterViewModel model)
    {
        var tasks = await taskService.GetAllTasks();
        List<TaskStatusEntity> statuses = await taskService.GetTaskStatuses();
        foreach (var item in tasks)
        {
            item.TaskStatus = statuses.FirstOrDefault(x => x.TaskId == item.TaskId);
        }

        tasks = tasks.Where(x => x.TimeBegin >= model.TimeBegin &&
                                    x.TimeEnd <= model.TimeEnd).ToList();
        if (!string.IsNullOrEmpty(model.TaskId))
        {
            tasks = tasks.Where(x => x.TaskId == model.TaskId).ToList();
        }
        if (!string.IsNullOrEmpty(model.Name))
        {
            tasks = tasks.Where(x => x.Name == model.Name).ToList();
        }
        if (model.Status != StatusTaskViewModel.NOFILTER)
        {
            tasks = tasks.Where(x => (int)x.TaskStatus.Status == (int)model.Status).ToList();
        }
        if (model.DayActive != DayActiveViewModel.NOFILTER)
        {
            tasks = tasks.Where(x => (int)x.DayActive == (int)model.DayActive).ToList();
        }
        if (model.AddresseeGroupId != 0)
        {
            tasks = tasks.Where(x => x.AddresseeGroupId == model.AddresseeGroupId).ToList();
        }

        return PartialView("_TasksList", tasks);
    }
}