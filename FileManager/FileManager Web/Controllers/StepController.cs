using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]
	public class StepController : Controller
    {
        private readonly ILogger<StepController> _logger;
        private readonly UserLogging _userLogging;
        private readonly IStepService _stepService;
        private readonly IAddresseeService _addresseeService;
        private readonly IOperationService _operationService;

        public StepController(ILogger<StepController> logger, UserLogging userLogging, IStepService stepService,
                                IAddresseeService addresseeService, IOperationService operationService)
		{
			_logger = logger;
            _userLogging = userLogging;
            _stepService = stepService;
            _operationService = operationService;
            _addresseeService = addresseeService;
		}

        [HttpGet]
        public IActionResult CreateStep(string idTask)
        {
            ViewBag.MaxNumber = _stepService.GetAllStepsByTaskId(idTask).Count + 1;
            ViewBag.TaskId = idTask;
            TaskStepEntity step = new TaskStepEntity();
            step.TaskId = idTask;
            return View(step);
        }
        [HttpPost]
        public IActionResult CreateStep(TaskStepEntity modelStep, string taskId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _stepService.CreateStep(modelStep);
                    return RedirectToAction("TaskDetails", new { taskId = taskId });
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
            _stepService.ReplaceSteps(taskId, numberStep, operation);
            return RedirectToAction("TaskDetails", "Task",  new { taskId = taskId });
        }

        [HttpPost]
        public IActionResult ActivatedStep(string taskId, int stepId)
        {
            _stepService.ActivatedStep(stepId);
            return RedirectToAction("TaskDetails", "Task", new { taskId = taskId });
        }

        public IActionResult StepDetails(string taskId, string stepNumber)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
            return View(taskStep);
        }

        [HttpPost]
        public IActionResult EditStep(TaskStepEntity stepModel) 
        {
            _stepService.EditStep(stepModel);
            return RedirectToAction("StepDetails", new { taskId = stepModel.TaskId, stepNumber = stepModel.StepNumber });
        }

        

    }
}
