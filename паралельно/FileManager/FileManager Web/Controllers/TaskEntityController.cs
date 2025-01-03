using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class TaskEntityController : Controller
    {

        private readonly ILogger<TaskEntityController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;

        public TaskEntityController(ILogger<TaskEntityController> logger, UserLogging userLogging, AppDbContext appDbContext)
        {
            _logger = logger;
            _userLogging = userLogging;
            _appDbContext = appDbContext;
        }

        // GET: TaskEntityController        
        public ActionResult Index()
        {
            List<TaskEntity> entities = _appDbContext.Tasks.OrderByDescending(x => x.IsActive).ThenBy(x => x.TaskId).ToList(); 
            return View(entities);
        }

        // GET: TaskEntityController/Details/5
        public ActionResult Details(string id, string contr, string act)
        {
            TaskEntity task = _appDbContext.Tasks.Where(x => x.TaskId == id).First();
            _userLogging.Logging(HttpContext.User.Identity.Name, $"Просмотр задачи: {task.TaskId}", JsonSerializer.Serialize(task));

            ViewBag.Controller = contr;
            ViewBag.Action = act;

            return View(task);
        }

        // GET: TaskEntityController/Create
        public ActionResult Create()
        {
            List<MailGroups> groups = _appDbContext.MailGroups.ToList();
            ViewBag.MailGroups = groups;
            List<TaskGroupEntity> taskGroups = _appDbContext.TaskGroups.ToList();
            ViewBag.TaskGroups = taskGroups;

            return View();
        }

        // POST: TaskEntityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                List<MailGroups> groups = _appDbContext.MailGroups.ToList();
                ViewBag.MailGroups = groups;
                List<TaskGroupEntity> taskGroups = _appDbContext.TaskGroups.ToList();
                ViewBag.TaskGroups = taskGroups;

                if (ModelState.IsValid)
                {
                    TaskEntity entity = new TaskEntity();
                    entity.TaskId = collection["Taskid"];
                    entity.Name = collection["Name"];
                    entity.TimeBegin = TimeOnly.Parse(collection["TimeBegin"]);
                    entity.TimeEnd = TimeOnly.Parse(collection["TimeEnd"]);
                    entity.DayActive = (DayActive)Enum.Parse(typeof(DayActive),collection["DayActive"]);
                    entity.Group = int.Parse(collection["Group"]);
                    //entity.IsActive = Convert.ToBoolean(collection["IsActive"].ToString().Split(',')[0]);
                    //entity.IsProgress = Convert.ToBoolean(collection["IsProgress"].ToString().Split(',')[0]);
                    entity.SourceCatalog = collection["SourceCatalog"];
                    entity.FileMask = collection["FileMask"];
                    entity.Delay = TimeOnly.Parse(collection["Delay"]);
                    entity.ArchiveCatalog = collection["ArchiveCatalog"];
                    entity.BadArchiveCatalog = collection["BadArchiveCatalog"];
                    entity.IsDeleteSource = Convert.ToBoolean(collection["IsDeleteSource"].ToString().Split(',')[0]);
                    entity.MaxAmountFiles = int.Parse(collection["MaxAmountFiles"]);
                    entity.DublNameJr = Convert.ToBoolean(collection["DublNameJr"].ToString().Split(',')[0]);
                    entity.LastModified = DateTime.Now;
                    entity.SplitFiles = Convert.ToBoolean(collection["SplitFiles"].ToString().Split(',')[0]);
                    entity.IsRegex = Convert.ToBoolean(collection["IsRegex"].ToString().Split(',')[0]);
                    entity.ValueParameterOfSplit = collection["ValueParameterOfSplit"];
					entity.MoveToTmp = Convert.ToBoolean(collection["MoveToTmp"].ToString().Split(',')[0]);
                    entity.TmpCatalog = collection["TmpCatalog"];
                    entity.DelayBeforeExecuting = TimeOnly.Parse(collection["DelayBeforeExecuting"]);
                    entity.SubMask = collection["SubMask"];
                    //entity.SubArchiveCatalog = collection["SubArchiveCatalog"];
                    
                    entity.TaskGroup = int.Parse(collection["TaskGroup"]);

                    _appDbContext.Tasks.Add(entity);


                    TaskStatusEntity taskStatusEntity = new TaskStatusEntity();
                    taskStatusEntity.TaskId = collection["Taskid"];
                    /*taskStatusEntity.CountExecute = 0;
                    taskStatusEntity.CountProcessedFiles = 0;
                    taskStatusEntity.DateLastExecute = DateTime.MinValue;
                    taskStatusEntity.IsError = false;
                    taskStatusEntity.IsProgress = false;*/
                    //taskStatusEntity.Task = entity;

                    
                    _appDbContext.TaskStatuses.Add(taskStatusEntity);

                    
                    _appDbContext.SaveChanges();

                    JsonSerializerOptions options = new()
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        WriteIndented = true
                    };
                    _userLogging.Logging(HttpContext.User.Identity.Name, $"Создание задачи: {entity.TaskId}", JsonSerializer.Serialize(entity, options));

                    return RedirectToAction(nameof(Index));
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskEntityController/Edit/5
        public ActionResult Edit(string id, string contr, string act)
        {
            TaskEntity entity = _appDbContext.Tasks.Where(x => x.TaskId == id).First();
            List<MailGroups> groups = _appDbContext.MailGroups.ToList();
            ViewBag.MailGroups = groups;
			List<TaskGroupEntity> taskGroups = _appDbContext.TaskGroups.ToList();

			/*List<SelectListItem> groupsTask = new List<SelectListItem>();
            foreach (var item in taskGroups)
            {
                groupsTask.Add(new SelectListItem { Value = item.Id.ToString(), Text = item.Name });
            }*/

			ViewBag.TaskGroups = taskGroups;

			ViewBag.Controller = contr;
			ViewBag.Action = act;

			return View(entity);
        }

        // POST: TaskEntityController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, IFormCollection collection)
        {
            try
            {
                TaskEntity entity = _appDbContext.Tasks.Where(x => x.TaskId == id).First();
                string oldentity = JsonSerializer.Serialize(entity);
                if (ModelState.IsValid && entity != null) 
                {
                    entity.TaskId = collection["Taskid"];
                    entity.Name = collection["Name"];
                    entity.TimeBegin = TimeOnly.Parse(collection["TimeBegin"]);
                    entity.TimeEnd = TimeOnly.Parse(collection["TimeEnd"]);
                    entity.DayActive = (DayActive)Enum.Parse(typeof(DayActive), collection["DayActive"]);
                    entity.Group = int.Parse(collection["Group"]);
                    //entity.IsActive = Convert.ToBoolean(collection["IsActive"].ToString().Split(',')[0]);
                    entity.SourceCatalog = collection["SourceCatalog"];
                    entity.FileMask = collection["FileMask"];
                    entity.Delay = TimeOnly.Parse(collection["Delay"]);
                    entity.ArchiveCatalog = collection["ArchiveCatalog"];
                    entity.BadArchiveCatalog = collection["BadArchiveCatalog"];
                    entity.IsDeleteSource = Convert.ToBoolean(collection["IsDeleteSource"].ToString().Split(',')[0]);
                    entity.MaxAmountFiles = int.Parse(collection["MaxAmountFiles"]);
                    entity.DublNameJr = Convert.ToBoolean(collection["DublNameJr"].ToString().Split(',')[0]);
                    entity.LastModified = DateTime.Now;
                    entity.SplitFiles = Convert.ToBoolean(collection["SplitFiles"].ToString().Split(',')[0]);
                    entity.IsRegex = Convert.ToBoolean(collection["IsRegex"].ToString().Split(',')[0]);
                    entity.ValueParameterOfSplit = collection["ValueParameterOfSplit"];
                    entity.MoveToTmp = Convert.ToBoolean(collection["MoveToTmp"].ToString().Split(',')[0]);
                    entity.TmpCatalog = collection["TmpCatalog"];
                    entity.DelayBeforeExecuting = TimeOnly.Parse(collection["DelayBeforeExecuting"]);
                    entity.SubMask = collection["SubMask"];
                    //entity.SubArchiveCatalog = collection["SubArchiveCatalog"];
                    entity.TaskGroup = int.Parse(collection["TaskGroup"]);

                    _appDbContext.Tasks.Update(entity);
                    _appDbContext.SaveChanges();

                    _userLogging.Logging(HttpContext.User.Identity.Name, $"Редактирование задачи: было  - {entity.TaskId}", oldentity);
                    _userLogging.Logging(HttpContext.User.Identity.Name, $"Редактирование задачи: стало - {entity.TaskId}", JsonSerializer.Serialize(entity));

                    return RedirectToAction(nameof(Index));

                }


                return View(id);
            }
            catch
            {
                return View(id);
            }
        }

        // GET: TaskEntityController/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                TaskEntity entity = _appDbContext.Tasks.First(x => x.TaskId == id);
                return View(entity);

            }catch 
            {
                return RedirectToAction(nameof(Index));
            }
            
        }

        // POST: TaskEntityController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                TaskEntity entity = _appDbContext.Tasks.First(x => x.TaskId == id);
                _appDbContext.Tasks.Remove(entity);
                _appDbContext.SaveChanges();

                _userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление задачи: {entity.TaskId}", JsonSerializer.Serialize(entity));
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        [HttpPost]
        public IActionResult ActivatedTask(string id)
        {
            TaskEntity task = _appDbContext.Tasks.FirstOrDefault(x => x.TaskId == id);
            task.IsActive = !task.IsActive;
            task.LastModified = DateTime.Now;
            _appDbContext.Tasks.Update(task);
            _appDbContext.SaveChanges();
            if (task.IsActive)
            {
                _userLogging.Logging(HttpContext.User.Identity.Name, $"Включение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
            }
            else
            {
                _userLogging.Logging(HttpContext.User.Identity.Name, $"Выключение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
            }

            List<TaskEntity> entities = _appDbContext.Tasks.ToList();
            return RedirectToAction("Index");
        }

        [HttpPost]
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

        public IActionResult CopyTask(string idTask, string newIdTask, string isCopy, CopyTaskOperationsViewModel[] operation)
        {
            TaskEntity task = _appDbContext.Tasks.FirstOrDefault(x => x.TaskId == idTask);
            if (task == null)
            {
				return RedirectToAction("Index");
			}
            task.TaskId = newIdTask;
            _appDbContext.Tasks.Add(task);
            
			TaskStatusEntity taskStatusEntity = new TaskStatusEntity();
			taskStatusEntity.TaskId = idTask;
			_appDbContext.TaskStatuses.Add(taskStatusEntity);

			_appDbContext.SaveChanges();
			if (isCopy == "on")
            {
				List<TaskOperationEntity> operations = _appDbContext.TaskOperations.Where(x => x.TaskId == idTask).ToList();
				foreach (var item in operation)
				{
					if (item.IsCopied)
					{
                        TaskOperationEntity newOperation = operations.FirstOrDefault(x => x.OperationId == item.OperationId);
                        newOperation.OperationId = item.NewOperationId;
                        newOperation.TaskId = newIdTask;
                        _appDbContext.TaskOperations.Add(newOperation);
                        _appDbContext.SaveChanges();
					}
				}
			}
           
            return RedirectToAction("Index");
        }

		

	}
}
