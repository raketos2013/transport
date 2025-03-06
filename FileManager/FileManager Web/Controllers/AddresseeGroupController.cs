using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class AddresseeGroupController : Controller
    {
        private readonly ILogger<AddresseeGroupController> _logger;
        private readonly UserLogging _userLogging;
        private readonly AppDbContext _appDbContext;

        public AddresseeGroupController(ILogger<AddresseeGroupController> logger, UserLogging userLogging, AppDbContext context)
        {
            _logger = logger;
            _userLogging = userLogging;
            _appDbContext = context;
        }

        public IActionResult Addressees()
        {
            List<AddresseeGroupEntity> groups = _appDbContext.AddresseeGroup.ToList();
            return View(groups);
        }

        [HttpPost]
        public IActionResult AddresseeList(int groupId)
        {
            List<AddresseeEntity> addressees = _appDbContext.Addressee.Where(x => x.AddresseeGroupId == groupId).ToList();

            return PartialView(addressees);
        }

        public IActionResult CreateGroup(string number, string name)
        {
            AddresseeGroupEntity group = new AddresseeGroupEntity();
            group.Id = int.Parse(number);
            group.Name = name;
            _appDbContext.AddresseeGroup.Add(group);
            _appDbContext.SaveChanges();

            //_userLogging.Logging(HttpContext.User.Identity.Name, $"Создание группы рассылки: {group.Id}", JsonSerializer.Serialize(group));

            return RedirectToAction("Addressees");
        }

        [HttpGet]
        public IActionResult CreateAddressee()
        {
            List<AddresseeGroupEntity> addresseeGroups = _appDbContext.AddresseeGroup.ToList();
            ViewBag.AddresseeGroups = addresseeGroups;
            return View();
        }

        [HttpPost]
        public IActionResult CreateAddressee(IFormCollection collection)
        {
            try
            {
                List<AddresseeGroupEntity> addresseeGroups = _appDbContext.AddresseeGroup.ToList();
                ViewBag.AddresseeGroups = addresseeGroups;

                if (ModelState.IsValid)
                {
                    AddresseeEntity entity = new AddresseeEntity();
                    entity.PersonalNumber = collection["PersonalNumber"];
                    entity.EMail = collection["EMail"];
                    entity.Fio = collection["Fio"];
                    entity.StructuralUnit = collection["StructuralUnit"];
                    entity.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                    _appDbContext.Addressee.Add(entity);

                    _appDbContext.SaveChanges();

                    return RedirectToAction(nameof(Addressees));
                }
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult ActivatedAddressee(string id)
        {
            AddresseeEntity addressee = _appDbContext.Addressee.FirstOrDefault(x => x.PersonalNumber == id);

            addressee.IsActive = !addressee.IsActive;

            _appDbContext.Addressee.Update(addressee);
            _appDbContext.SaveChanges();
            /*if (addressee.IsActive)
            {
                _userLogging.Logging(HttpContext.User.Identity.Name, $"Включение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
            }
            else
            {
                _userLogging.Logging(HttpContext.User.Identity.Name, $"Выключение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
            }*/

            return RedirectToAction("Addressees");
        }

        public IActionResult DeleteAddresseeGroup(int id)
        {
            AddresseeGroupEntity deletedGroup = _appDbContext.AddresseeGroup.FirstOrDefault(x => x.Id == id);
            _appDbContext.AddresseeGroup.Remove(deletedGroup);
            List<AddresseeEntity> deletedAddresses = _appDbContext.Addressee.Where(x => x.AddresseeGroupId == id).ToList();
            _appDbContext.Addressee.RemoveRange(deletedAddresses);
            List<TaskEntity> tasks = _appDbContext.Task.Where(x => x.AddresseeGroupId == id).ToList();
            foreach (TaskEntity task in tasks)
            {
                task.AddresseeGroupId = null;
            }
            //_appDbContext.Task.UpdateRange(tasks);
            _appDbContext.SaveChanges();

            _userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление группы рассылки номер: {deletedGroup.Id}", JsonSerializer.Serialize(deletedGroup));

            return RedirectToAction("Addressees");
        }

        public IActionResult DeleteAddressee(string number, int idGroup)
        {
            AddresseeEntity addressee = _appDbContext.Addressee.FirstOrDefault(x => x.PersonalNumber == number && x.AddresseeGroupId == idGroup);
            if (addressee != null)
            {
                _appDbContext.Addressee.Remove(addressee);
                _appDbContext.SaveChanges();
                _userLogging.Logging(HttpContext.User.Identity.Name,
                                     $"Удаление адресата {number} из группы номер {idGroup}", 
                                     JsonSerializer.Serialize(addressee));
            }
            return RedirectToAction("Addressees");
        }
    }
}
