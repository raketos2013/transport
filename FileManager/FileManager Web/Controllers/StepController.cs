using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager_Web.Controllers
{
	//[Authorize(Roles = "o.br.ДИТ")]
	public class StepController : Controller
    {
        private readonly ILogger<StepController> _logger;
        private readonly UserLogging _userLogging;
        private readonly IStepService _stepService;

		public StepController(ILogger<StepController> logger, UserLogging userLogging, IStepService stepService)
		{
			_logger = logger;
            _userLogging = userLogging;
            _stepService = stepService;
		}

		public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateStep(string taskId)
        {
            TaskStepEntity taskStep = new TaskStepEntity();
            return View(taskStep);
        }
        [HttpPost]
        public IActionResult CreateStep(TaskStepEntity taskStep)
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReplaceStep(string taskId, string numberStep, string operation)
        {
            return View();
        }

        [HttpPost]
        public IActionResult ActivatedStep(string taskId, string stepNumber)
        {
            return View();
        }

        public IActionResult StepDetails(string taskId, string stepNumber)
        {
            return View();
        }

        [HttpPost]
        public IActionResult EditStep(string taskId, string stepNumber) 
        {
            return View();
        }

    }
}
