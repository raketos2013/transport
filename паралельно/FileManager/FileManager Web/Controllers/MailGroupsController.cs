using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]
	public class MailGroupsController : Controller
	{
		private readonly ILogger<MailGroupsController> _logger;
		private readonly UserLogging _userLogging;

		private readonly AppDbContext _appDbContext;

		public MailGroupsController(ILogger<MailGroupsController> logger, UserLogging userLogging, AppDbContext context)
		{
			_logger = logger;
			_userLogging = userLogging;
			_appDbContext = context;
		}

		// GET: MailGroupsController

		public ActionResult Index()
		{
			IQueryable<MailGroups> groups = _appDbContext.MailGroups;
			return View(groups);
		}

		// GET: MailGroupsController/Details/5
		public ActionResult Details(int id)
		{
			MailGroups group = _appDbContext.MailGroups.Where(x => x.Id == id).ToList().First();
			return View(group);
		}

		// GET: MailGroupsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: MailGroupsController/Create
		[HttpPost]
		public IActionResult Create(string numberGroup, string nameGroup)
		{
			MailGroups group = new MailGroups();
			group.Id = int.Parse(numberGroup);
			group.Name = nameGroup;
			_appDbContext.Add(group);
			_appDbContext.SaveChanges();

			_userLogging.Logging(HttpContext.User.Identity.Name, $"Создание группы рассылки: {group.Id}", JsonSerializer.Serialize(group));

			return RedirectToAction("Index");
		}

		// GET: MailGroupsController/Edit/5
		public ActionResult Edit(int id)
		{
			MailGroups group = _appDbContext.MailGroups.Where(x => x.Id == id).ToList().First();

			return View(group);
		}

		// POST: MailGroupsController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				if (ModelState.IsValid)
				{
					MailGroups group = _appDbContext.MailGroups.Where(x => x.Id == id).ToList().First();
					string oldgroup = JsonSerializer.Serialize(group);
					group.Name = collection["name"];
					_appDbContext.Update(group);
					_appDbContext.SaveChanges();

					_userLogging.Logging(HttpContext.User.Identity.Name, $"Редактирование группы рассылки: было - {group.Id}", oldgroup);
					_userLogging.Logging(HttpContext.User.Identity.Name, $"Редактирование группы рассылки: стало - {group.Id}", JsonSerializer.Serialize(group));

					return RedirectToAction(nameof(Index));
				}
				return View();
			}
			catch
			{
				return View();
			}
		}

		// GET: MailGroupsController/Delete/5
		public ActionResult Delete(int id)
		{
			try
			{
				MailGroups group = _appDbContext.MailGroups.Where(x => x.Id == id).First();
				return View(group);
			}
			catch
			{
				return RedirectToAction(nameof(Index));
			}

		}

		// POST: MailGroupsController/Delete/5
		[HttpPost]
		public ActionResult DeleteGroup(int id)
		{

			MailGroups group = _appDbContext.MailGroups.FirstOrDefault(x => x.Id == id);
			_appDbContext.MailGroups.Remove(group);
			_appDbContext.SaveChanges();

			_userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление группы рассылки: {group.Id}", JsonSerializer.Serialize(group));

			return RedirectToAction("Index");

		}

		[HttpPost]
		public IActionResult MailList(int id)
		{
			List<MailList> mailList = _appDbContext.MailLists.Where(x => x.MailGroupsId == id).ToList();
			ViewBag.MailGroupId = id;
			return PartialView(mailList);
		}

		[HttpPost]
		public IActionResult CreateEmail(string groupId, string email)
		{
			MailList mail = new MailList();
			mail.MailGroupsId = int.Parse(groupId);
			mail.EMail = email;
			_appDbContext.MailLists.Add(mail);
			_appDbContext.SaveChanges();

			_userLogging.Logging(HttpContext.User.Identity.Name, $"Добалвение адресата {mail.EMail} в группу {mail.MailGroupsId}", JsonSerializer.Serialize(mail));

			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult DeleteEmail(int id, string email)
		{
			MailList mailList = _appDbContext.MailLists.FirstOrDefault(x => x.MailGroupsId == id && x.EMail == email);
			_appDbContext.Remove(mailList);
			_appDbContext.SaveChanges();

			_userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление адресата: {mailList.EMail}", JsonSerializer.Serialize(mailList));

			return RedirectToAction("Index");
		}
	}
}
