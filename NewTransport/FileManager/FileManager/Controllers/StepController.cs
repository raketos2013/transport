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

    public async Task<IActionResult> StepList(string taskId)
    {
        var stepsAsync = await stepService.GetAllStepsByTaskId(taskId);
        var steps = stepsAsync.OrderBy(x => x.StepNumber).ToList();
        return PartialView("_StepList", steps);
    }

    [HttpGet]
    public async Task<IActionResult> CreateStep(string taskId)
    {
        await lockService.Lock(taskId, httpContextAccessor.HttpContext.User.Identity.Name);
        var steps = await stepService.GetAllStepsByTaskId(taskId);
        ViewBag.MaxNumber = steps.Count + 1;
        TaskStepEntity step = new()
        {
            TaskId = taskId
        };
        return PartialView("_CreateStep", step);
    }
    [HttpPost]
    public async Task<IActionResult> CreateStep(TaskStepEntity modelStep)
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
                await stepService.CreateStep(modelStep);
                await lockService.Unlock(modelStep.TaskId);
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
    public async Task<IActionResult> ReplaceStep(string taskId, string numberStep, string operation)
    {
        await stepService.ReplaceSteps(taskId, numberStep, operation);
        return RedirectToAction(nameof(Steps));
    }

    [HttpPost]
    public async Task<IActionResult> ActivatedStep(string taskId, int stepNumber)
    {
        var step = await stepService.GetStepByTaskId(taskId, stepNumber);
        await stepService.ActivatedStep(step.StepId);
        return RedirectToAction(nameof(Steps));
    }

    public async Task<IActionResult> StepDetails(string taskId, string stepNumber)
    {
        TaskStepEntity? taskStep = await stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        return PartialView("_StepDetails", taskStep);
    }

    [HttpGet]
    public async Task<IActionResult> EditStep(string taskId, string stepNumber)
    {
        await lockService.Lock(taskId, httpContextAccessor.HttpContext.User.Identity.Name);
        var steps = await stepService.GetAllStepsByTaskId(taskId);
        ViewBag.MaxNumber = steps.Count + 1;
        TaskStepEntity step = await stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        return PartialView("_EditStep", step);
    }

    [HttpPost]
    public async Task<IActionResult> EditStep(TaskStepEntity stepModel)
    {
        if (stepModel.OperationName == OperationName.Delete || stepModel.OperationName == OperationName.Clrbuf ||
            stepModel.OperationName == OperationName.Read || stepModel.OperationName == OperationName.Exist)
        {
            stepModel.Destination = "";
        }
        await stepService.EditStep(stepModel);
        await lockService.Unlock(stepModel.TaskId);
        return RedirectToAction(nameof(Steps));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteStep(int stepId)
    {
        await stepService.DeleteStep(stepId);
        return RedirectToAction(nameof(Steps));
    }

    [HttpPost]
    public async Task<IActionResult> CopyStep(string taskId, int stepNumber, int newNumber)
    {
        var step = await stepService.GetStepByTaskId(taskId, stepNumber);
        await stepService.CopyStep(step.StepId, newNumber);
        await lockService.Unlock(taskId);
        return RedirectToAction(nameof(Steps));
    }

    public async Task<IActionResult> ExistFiles(string taskId)
    {
        List<int> countFiles = [];
        var steps = await stepService.GetAllStepsByTaskId(taskId);
        foreach (var step in steps)
        {
            var count = await stepService.CountFiles(step.StepId);
            countFiles.Add(count);
        }
        return Ok(countFiles);
    }
}
