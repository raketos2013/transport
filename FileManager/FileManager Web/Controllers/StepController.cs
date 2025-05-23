using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]
	public class StepController(ILogger<StepController> logger, 
                                //UserLogging userLogging, 
                                IStepService stepService) 
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
            //ViewBag.TaskId = idTask;
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
                    return RedirectToAction("Steps" );
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
            return PartialView("_StepDetails", taskStep);
        }

        [HttpGet]
        public IActionResult EditStep(string taskId, string stepNumber)
        {
            ViewBag.MaxNumber = stepService.GetAllStepsByTaskId(taskId).Count + 1;
            //ViewBag.TaskId = idTask;
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
}
