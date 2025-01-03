using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace FileManager_Web.Controllers
{
	[Authorize(Roles = "o.br.ДИТ")]

    public class MailListController : Controller
    {
        private readonly ILogger<MailListController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;


        public MailListController(ILogger<MailListController> logger, UserLogging userLogging, AppDbContext context) 
        {
            _appDbContext = context;
            _logger = logger;   
            _userLogging = userLogging;
        }



        // GET: MailListController
        public ActionResult Index(int id)
        {
            List<MailList> maillist = _appDbContext.MailLists.Where(x => x.MailGroupsId == id).ToList();
            ViewBag.MailGroupId = id;
            return View(maillist);
        }

        // GET: MailListController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MailListController/Create
        public ActionResult Create(int MailGroupId)
        {
            ViewBag.MailGroupId = MailGroupId;
            return View();
        }

        // POST: MailListController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MailList mailList = new MailList();
                    mailList.MailGroupsId = int.Parse(collection["MailGroupsid"]);
                    mailList.EMail = collection["Email"];
                    _appDbContext.MailLists.Add(mailList);
                    _appDbContext.SaveChanges();

                    _userLogging.Logging(HttpContext.User.Identity.Name, $"Создание адресата: {mailList.EMail}", JsonSerializer.Serialize(mailList));

                    return RedirectToAction(nameof(Index), new { Id = mailList.MailGroupsId });
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: MailListController/Edit/5
/*        public ActionResult Edit(int id, string email)
        {
            MailList mailList = _appDbContext.MailLists.Where(x => x.MailGroupsId == id && x.EMail == email).First();
            return View(mailList);
        }

        // POST: MailListController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
*/
        // GET: MailListController/Delete/5
        public ActionResult Delete(int id, string email)
        {
            try
            {
                MailList mailList = _appDbContext.MailLists.Where(x => x.MailGroupsId == id && x.EMail == email).First();
                _appDbContext.Remove(mailList);
                _appDbContext.SaveChanges();

                _userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление адресата: {mailList.EMail}", JsonSerializer.Serialize(mailList));

                return RedirectToAction(nameof(Index), new {id = id});
            }
            catch  
            {
                return RedirectToAction(nameof(Index), new { id = id });
            }
        }

        /*        // POST: MailListController/Delete/5
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult Delete(int id, IFormCollection collection)
                {
                    try
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    catch
                    {
                        return View();
                    }
                }
        */



    
    
    
    }




}
