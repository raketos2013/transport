using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using System.Text.Json;
using X.PagedList;

namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;

        public HomeController(ILogger<HomeController> logger, UserLogging userLogging, AppDbContext appDbContext)
        {
            _logger = logger;
            _userLogging = userLogging;
            _appDbContext = appDbContext;
        }

        
        public IActionResult Index()
        {
            /*List<TaskStatusEntity> listStatuses = _appDbContext.TaskStatuses.ToList();
            ViewBag.Today = DateTime.Today;*/
            List<TaskGroupEntity> tasksGroups = _appDbContext.TaskGroups.ToList();
            List<TaskEntity> tasks = _appDbContext.Tasks.ToList();
            return View(tasksGroups);
        }

        public IActionResult Details(string id, string dateFrom, string dateTo, int? page, string? searchId, string? searchFileName, string? searchResult, string? searchText)
        {
            _userLogging.Logging(HttpContext.User.Identity.Name, $"Просмотр статуса задачи: {id}", "");
            DateTime date = dateFrom == null ? DateTime.Today : DateTime.Parse(dateFrom);
            DateTime date2 = dateTo == null ? DateTime.Today : DateTime.Parse(dateTo);

			TaskStatusEntity taskStatus = _appDbContext.TaskStatuses.Where(x => x.TaskId == id).First();
            List<TransportTaskLogEntity> transportLogs = _appDbContext.TransportTaskLogs
                                                                      .Where(x => x.TaskId == id &&
                                                                                  x.DateTimeLog.Date >= date.Date && 
                                                                                  x.DateTimeLog.Date <= date2.Date)
                                                                      .OrderByDescending(x => x.DateTimeLog)
                                                                      .ToList();
			ViewBag.SearchId = searchId;
			ViewBag.SearchFileName = searchFileName;
			ViewBag.SearchResult = searchResult;
			ViewBag.SearchText = searchText;
			if (searchId != null)
            {
                transportLogs = transportLogs.Where(x => x.OperationId.Contains(searchId) || 
                                                         x.OperationId == "0").ToList();
                
            }
            if (searchFileName != null)
            {
                transportLogs = transportLogs.Where(x => x.FileName.Contains(searchFileName)).ToList();
                
            }
            if (searchResult != "6" && searchResult != null)
            {
				ResultOperation res = (ResultOperation)Enum.GetValues(typeof(ResultOperation)).GetValue(Int32.Parse(searchResult));
				transportLogs = transportLogs.Where(x => x.ResultOperation.ToString() == res.ToString()).ToList();
                
            }
            if (searchText != null)
            {
                transportLogs = transportLogs.Where(x => x.ResultText.Contains(searchText)).ToList();
                
            }

            ViewBag.TaskStatus = taskStatus;
			ViewBag.FilterDateFrom = date.ToString("yyyy-MM-dd");
			ViewBag.FilterDateTo = date2.ToString("yyyy-MM-dd");
			

			int pageSize = 15;
            int pageNumber = (page ?? 1);

            return View(transportLogs.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public IActionResult MonitoringTasks(string taskGroup)
        {
            TaskGroupEntity taskGroupEntity = _appDbContext.TaskGroups.FirstOrDefault(x => x.Name == taskGroup);
            List<TaskStatusEntity> listStatuses = _appDbContext.TaskStatuses.Where(x => x.Task.TaskGroup == taskGroupEntity.Id)
                                                                            .OrderByDescending(x => x.Task.IsActive).ThenBy(x => x.TaskId)
                                                                            .ToList();
        
            foreach (TaskStatusEntity taskStatusEntity in listStatuses)
            {
                taskStatusEntity.Task = _appDbContext.Tasks.First(x => x.TaskId == taskStatusEntity.TaskId);
            }
            ViewBag.Today = DateTime.Today;
            return PartialView(listStatuses);
        }

        [HttpPost]
        public IActionResult CreateTaskGroup(string nameGroup)
        {
            TaskGroupEntity taskGroups = _appDbContext.TaskGroups.FirstOrDefault(x => x.Name == nameGroup);
            if (taskGroups == null)
            {
                TaskGroupEntity taskGroupEntity = new TaskGroupEntity();
                taskGroupEntity.Name = nameGroup;
                _appDbContext.TaskGroups.Add(taskGroupEntity);
                _appDbContext.SaveChanges();

                _userLogging.Logging(HttpContext.User.Identity.Name, $"Создание группы задач: {taskGroupEntity.Id}", JsonSerializer.Serialize(taskGroupEntity));
            }
            

            return RedirectToAction("Index");
        }

        
        public IActionResult DeleteGroup(int id)
        {
            if (id == 1)
            {
				return RedirectToAction("Index");
			}
            List<TaskEntity> tasks = _appDbContext.Tasks.Where(x => x.TaskGroup == id).ToList();
            foreach (var task in tasks)
            {
                task.TaskGroup = 1;
            }
            _appDbContext.SaveChanges();

            TaskGroupEntity taskGroup = _appDbContext.TaskGroups.FirstOrDefault(x => x.Id == id);
            _appDbContext.TaskGroups.Remove(taskGroup);
            _appDbContext.SaveChanges();

            _userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление группы задач: {taskGroup.Id}", JsonSerializer.Serialize(taskGroup));

            return RedirectToAction("Index");
        }

    /*    [HttpGet]
        public IActionResult CopyTaskForm(string idTask)
        {
            List<TaskOperationEntity> operations = _appDbContext.TaskOperations.Where(x => x.TaskId == idTask).ToList();
            List<CopyTaskOperationsViewModel> copiedOperations = new List<CopyTaskOperationsViewModel>();
            foreach (var operation in operations)
            {
                CopyTaskOperationsViewModel copyOperation = new CopyTaskOperationsViewModel();
                copyOperation.OperationId = operation.OperationId;
                copiedOperations.Add(copyOperation);
            }

            return PartialView(copiedOperations);
        }

        [HttpPost]
        public IActionResult Index(CopyTaskOperationsViewModel[] operation)
        {
            foreach (var item in operation)
            {

            }

            return RedirectToAction("Index");
		}*/


    }
}
