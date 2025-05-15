using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]
	public class StepController(ILogger<StepController> logger, 
                                UserLogging userLogging, 
                                IStepService stepService) 
                : Controller
    {
        [HttpGet]
        public IActionResult CreateStep(string idTask)
        {
            ViewBag.MaxNumber = stepService.GetAllStepsByTaskId(idTask).Count + 1;
            ViewBag.TaskId = idTask;
            TaskStepEntity step = new()
            {
                TaskId = idTask
            };
            return View(step);
        }
        [HttpPost]
        public IActionResult CreateStep(TaskStepEntity modelStep, string taskId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    stepService.CreateStep(modelStep);
                    return RedirectToAction("TaskDetails", "Task", new { taskId });
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
            stepService.ReplaceSteps(taskId, numberStep, operation);
            return RedirectToAction("TaskDetails", "Task",  new { taskId });
        }

        [HttpPost]
        public IActionResult ActivatedStep(string taskId, int stepId)
        {
            stepService.ActivatedStep(stepId);
            return RedirectToAction("TaskDetails", "Task", new { taskId });
        }

        public IActionResult StepDetails(string taskId, string stepNumber)
        {
            TaskStepEntity? taskStep = stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
            return View(taskStep);
        }

        [HttpPost]
        public IActionResult EditStep(TaskStepEntity stepModel) 
        {
            stepService.EditStep(stepModel);
            return RedirectToAction("StepDetails", "Step", new { taskId = stepModel.TaskId, stepNumber = stepModel.StepNumber });
        }

        

    }
}
