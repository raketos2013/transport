﻿using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels.Step;
using FileManager.Domain.ViewModels.Task;
using FileManager.Services.Interfaces;
using FileManager_Web.Session;
using FileManager_Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;


namespace FileManager_Web.Controllers;

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
    public IActionResult TasksList(string taskGroup)
    {
        List<TaskEntity> tasks = taskService.GetTasksByGroup(taskGroup);
        List<TaskStatusEntity> statuses = taskService.GetTaskStatuses();
        foreach(var item in tasks)
        {
            item.TaskStatus = statuses.FirstOrDefault(x => x.TaskId ==item.TaskId);
        }
        return PartialView("_TasksList", tasks);
    }

    [HttpGet]
    public IActionResult CreateTask()
    {
        ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
        ViewBag.TaskGroups = taskService.GetAllGroups();
        TaskEntity task = new();
        return PartialView("_CreateTask", task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateTask(TaskEntity task)
    {
        ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
        ViewBag.TaskGroups = taskService.GetAllGroups();
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
        ViewBag.TaskGroups = taskService.GetAllGroups();
        return PartialView("_TaskDetails", taskDetails);
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
            model.PageSize = 10;
        }



        TaskLogViewModel sessionModel = HttpContext?.Session.Get<TaskLogViewModel>("LogFilters");
        if (sessionModel != null)
        {
            if (sessionModel.TaskId != null)
            {
                date = sessionModel.DateFrom;
                date2 = sessionModel.DateTo;
                var taskLogs = taskLogService.GetLogsByTaskId(sessionModel.TaskId)
                                                        .OrderByDescending(x => x.DateTimeLog)
                                                        .Where(x => x.DateTimeLog.Date >= date &&
                                                                    x.DateTimeLog.Date <= date2);
                if (sessionModel.PageSize == 0)
                {
                    sessionModel.PageSize = 10;
                }
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
                                                        .OrderByDescending(x => x.DateTimeLog)
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
            model.PageSize = 10;
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
}