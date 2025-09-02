using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class StepController(IStepService stepService,
                            ILockService lockService,
                            IHttpContextAccessor httpContextAccessor)
            : Controller
{
    public IActionResult Steps()
    {
        return View();
    }

    public IActionResult StepList(string taskId)
    {
        List<TaskStepEntity> steps = stepService.GetAllStepsByTaskId(taskId).OrderBy(x => x.StepNumber).ToList();
        return PartialView("_StepList", steps);
    }

    [HttpGet]
    public IActionResult CreateStep(string taskId)
    {
        lockService.Lock(taskId, httpContextAccessor.HttpContext.User.Identity.Name);
        ViewBag.MaxNumber = stepService.GetAllStepsByTaskId(taskId).Count + 1;
        TaskStepEntity step = new()
        {
            TaskId = taskId
        };
        return PartialView("_CreateStep", step);
    }
    [HttpPost]
    public IActionResult CreateStep(TaskStepEntity modelStep)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (modelStep.OperationName == OperationName.Delete || modelStep.OperationName == OperationName.Clrbuf ||
                    modelStep.OperationName == OperationName.Read || modelStep.OperationName == OperationName.Exist)
                {
                    modelStep.Destination = "";
                }
                stepService.CreateStep(modelStep);
                return RedirectToAction("Steps");
            }
            return PartialView("_CreateStep", modelStep);
        }
        catch (Exception)
        {
            return PartialView("_CreateStep", modelStep);
        }
    }

    [HttpPost]
    public IActionResult ReplaceStep(string taskId, string numberStep, string operation)
    {
        stepService.ReplaceSteps(taskId, numberStep, operation);
        return RedirectToAction("Steps");
    }

    [HttpPost]
    public IActionResult ActivatedStep(string taskId, int stepNumber)
    {
        var step = stepService.GetStepByTaskId(taskId, stepNumber);
        stepService.ActivatedStep(step.StepId);
        return RedirectToAction("Steps");
    }

    public IActionResult StepDetails(string taskId, string stepNumber)
    {
        TaskStepEntity? taskStep = stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        return PartialView("_StepDetails", taskStep);
    }

    [HttpGet]
    public IActionResult EditStep(string taskId, string stepNumber)
    {
        lockService.Lock(taskId, httpContextAccessor.HttpContext.User.Identity.Name);
        ViewBag.MaxNumber = stepService.GetAllStepsByTaskId(taskId).Count + 1;
        TaskStepEntity step = stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        return PartialView("_EditStep", step);
    }

    [HttpPost]
    public IActionResult EditStep(TaskStepEntity stepModel)
    {
        if (stepModel.OperationName == OperationName.Delete || stepModel.OperationName == OperationName.Clrbuf ||
            stepModel.OperationName == OperationName.Read || stepModel.OperationName == OperationName.Exist)
        {
            stepModel.Destination = "";
        }
        stepService.EditStep(stepModel);
        lockService.Unlock(stepModel.TaskId);
        return RedirectToAction("Steps");
    }

    [HttpPost]
    public IActionResult DeleteStep(int stepId)
    {
        stepService.DeleteStep(stepId);
        return RedirectToAction("Steps");
    }

    [HttpPost]
    public IActionResult CopyStep(string taskId, int stepNumber, int newNumber)
    {
        var step = stepService.GetStepByTaskId(taskId, stepNumber);
        stepService.CopyStep(step.StepId, newNumber);
        
        return RedirectToAction("Steps");
    }

    public IActionResult ExistFiles(string taskId)
    {
        List<int> countFiles = [];
        var steps = stepService.GetAllStepsByTaskId(taskId);
        foreach (var step in steps)
        {
            countFiles.Add(stepService.CountFiles(step.StepId));
        }
        return Ok(countFiles);
    }
}
