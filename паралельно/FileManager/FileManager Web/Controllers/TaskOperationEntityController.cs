using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]

    public class TaskOperationEntityController : Controller
    {
        private readonly ILogger<TaskOperationEntityController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;



        public TaskOperationEntityController(ILogger<TaskOperationEntityController> logger, UserLogging userLogging, AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _userLogging = userLogging;
        }

        
        // GET: TaskOperationEntityController
        public ActionResult Index(string id, string contr, string act)
        {
            List<TaskOperationEntity> entities = _appDbContext.TaskOperations.Where(x => x.TaskId == id).ToList();
            ViewBag.TaskId = id;

            ViewBag.Controller = contr;
            ViewBag.Action = act;

            return View(entities);
        }

        // GET: TaskOperationEntityController/Details/5
        public ActionResult Details(string id, string operationid, string contr, string act)
        {
            TaskOperationEntity entity = _appDbContext.TaskOperations.FirstOrDefault(x => x.TaskId == id && x.OperationId == operationid);
            _userLogging.Logging(HttpContext.User.Identity.Name, $"Просмотр назначения: {entity.OperationId}", JsonSerializer.Serialize(entity));

            ViewBag.TaskId = id;

            ViewBag.Controller = contr;
            ViewBag.Action = act;

            return View(entity);
        }

        // GET: TaskOperationEntityController/Create
        public ActionResult Create(string id, string contr, string act)
        {
            List<MailGroups> groups = _appDbContext.MailGroups.ToList();
            ViewBag.MailGroups = groups;
            ViewBag.TaskId = id;

            ViewBag.Controller = contr;
            ViewBag.Action = act;
            return View();
        }

        // POST: TaskOperationEntityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TaskOperationEntity entity = new TaskOperationEntity();
                    entity.TaskId = collection["TaskId"].ToString();
                    entity.OperationId = collection["OperationId"].ToString();
                    entity.Description = collection["Description"].ToString();
                    entity.DestinationDirectory = collection["DestinationDirectory"].ToString();
                    entity.TemplateFileName = collection["TemplateFileName"].ToString();
                    entity.IsActive = Convert.ToBoolean(collection["IsActive"].ToString().Split(',')[0]);
                    entity.IsRename = Convert.ToBoolean(collection["IsRename"].ToString().Split(',')[0]);
                    entity.TemplateFileName = collection["TemplateFileName"].ToString();
                    entity.NewTemplateFileName = collection["NewTemplateFileName"].ToString();
                    entity.DublDest = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["DublDest"]);
                   // entity.Prior = (TypeSort)Enum.Parse(typeof(TypeSort), collection["Prior"]);
                    //entity.ScanAttr = (AttributForScaning)Enum.Parse(typeof(AttributForScaning), collection["ScanAttr"]);
                    //entity.IsComplit = Convert.ToBoolean(collection["IsComplit"].ToString().Split(',')[0]);
                    entity.AdditionalText = collection["AdditionalText"].ToString();
                    entity.Group = int.Parse(collection["Group"]);
                    _appDbContext.TaskOperations.Add(entity);

                    TaskEntity task = _appDbContext.Tasks.FirstOrDefault(x => x.TaskId == entity.TaskId);
                    task.LastModified = DateTime.Now;
                    _appDbContext.Tasks.Update(task);

                    _appDbContext.SaveChanges();

                    JsonSerializerOptions options = new()
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        WriteIndented = true
                    };
                    _userLogging.Logging(HttpContext.User.Identity.Name, $"Создание назначения: {entity.OperationId}", JsonSerializer.Serialize(entity, options));

                    return RedirectToAction(nameof(Index), new { id = entity.TaskId });

                }


                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskOperationEntityController/Edit/5
        public ActionResult Edit(string id, string operationid, string contr, string act)
        {
            TaskOperationEntity entity = _appDbContext.TaskOperations.First(x => x.TaskId == id && x.OperationId == operationid);
			ViewBag.TaskId = id;
            List<MailGroups> groups = _appDbContext.MailGroups.ToList();
            ViewBag.MailGroups = groups;

            ViewBag.Controller = contr;
			ViewBag.Action = act;
			return View(entity);
        }

        // POST: TaskOperationEntityController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid) 
                {
                    TaskOperationEntity entity = _appDbContext.TaskOperations.First(x => x.TaskId == id && x.OperationId == collection["OperationId"].ToString());
                    string oldentity = JsonSerializer.Serialize(entity);

                    entity.Description = collection["Description"].ToString();
                    entity.DestinationDirectory = collection["DestinationDirectory"].ToString();
                    //entity.NewMask = collection["NewMask"].ToString();
                    entity.IsActive = Convert.ToBoolean(collection["IsActive"].ToString().Split(',')[0]);
                    entity.IsRename = Convert.ToBoolean(collection["IsRename"].ToString().Split(',')[0]);

                    entity.TemplateFileName = collection["TemplateFileName"].ToString();
                    entity.NewTemplateFileName = collection["NewTemplateFileName"].ToString();
                    entity.DublDest = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["DublDest"]);
                    //entity.Prior = (TypeSort)Enum.Parse(typeof(TypeSort), collection["Prior"]);
                    //entity.ScanAttr = (AttributForScaning)Enum.Parse(typeof(AttributForScaning), collection["ScanAttr"]);
                    //entity.IsComplit = Convert.ToBoolean(collection["IsComplit"].ToString().Split(',')[0]);
                    entity.AdditionalText = collection["AdditionalText"].ToString();
                    entity.Group = int.Parse(collection["Group"]);
                    _appDbContext.TaskOperations.Update(entity);

                    TaskEntity task = _appDbContext.Tasks.FirstOrDefault(x => x.TaskId == entity.TaskId);
                    task.LastModified = DateTime.Now;
                    _appDbContext.Tasks.Update(task);

                    _appDbContext.SaveChanges();

                    JsonSerializerOptions options = new()
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        WriteIndented = true
                    };
                    _userLogging.Logging(HttpContext.User.Identity.Name, $"Редактирование назначения: было  - {entity.OperationId}", oldentity);
                    _userLogging.Logging(HttpContext.User.Identity.Name, $"Редактирование назначения: стало - {entity.OperationId}", JsonSerializer.Serialize(entity, options));


                    return RedirectToAction(nameof(Index), new { id = entity.TaskId });

                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskOperationEntityController/Delete/5
        public ActionResult Delete(string id, string operationid)
        {
            try
            {
                TaskOperationEntity entity = _appDbContext.TaskOperations.First(x => x.TaskId == id && x.OperationId == operationid);
                return View(entity);
            }
            catch 
            {
                return RedirectToAction(nameof(Index), new { id = id});
            }

        }

        // POST: TaskOperationEntityController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, string operationid, IFormCollection collection)
        {
            try
            {
                TaskOperationEntity entity = _appDbContext.TaskOperations.First(x => x.TaskId == id && x.OperationId == operationid);
                _appDbContext.TaskOperations.Remove(entity);

                TaskEntity task = _appDbContext.Tasks.FirstOrDefault(x => x.TaskId == entity.TaskId);
                task.LastModified = DateTime.Now;
                _appDbContext.Tasks.Update(task);

                _appDbContext.SaveChanges();

                _userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление назначения: {entity.OperationId}", JsonSerializer.Serialize(entity));

                return RedirectToAction(nameof(Index), new { id = id});
            }
            catch
            {
                return RedirectToAction(nameof(Index), new { id = id });
            }
        }

		[HttpPost]
		public IActionResult ActivatedOperation(string id)
		{
			TaskOperationEntity operation = _appDbContext.TaskOperations.FirstOrDefault(x => x.OperationId == id);
			operation.IsActive = !operation.IsActive;
			//task.LastModified = DateTime.Now;
			_appDbContext.TaskOperations.Update(operation);
			_appDbContext.SaveChanges();
			if (operation.IsActive)
			{
				_userLogging.Logging(HttpContext.User.Identity.Name, $"Включение назначения : {operation.OperationId}", JsonSerializer.Serialize(operation));
			}
			else
			{
				_userLogging.Logging(HttpContext.User.Identity.Name, $"Выключение назначения: {operation.OperationId}", JsonSerializer.Serialize(operation));
			}

			List<TaskOperationEntity> entities = _appDbContext.TaskOperations.ToList();
			return RedirectToAction("Index");
		}
	}
}
