using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager_Web.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class StepController(IStepService stepService)
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
        ViewBag.MaxNumber = stepService.GetAllStepsByTaskId(taskId).Count + 1;
        TaskStepEntity step = stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        return PartialView("_EditStep", step);
    }

    [HttpPost]
    public IActionResult EditStep(TaskStepEntity stepModel)
    {
        stepService.EditStep(stepModel);
        return RedirectToAction("Steps");
    }
}
