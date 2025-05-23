﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using FileManager_Web.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class AddresseeGroupController(ILogger<AddresseeGroupController> logger, 
                                            //UserLogging userLogging, 
                                            IAddresseeService addresseeService,
                                            AppDbContext context) 
                : Controller
    {
        public IActionResult Addressees()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddresseeList(int groupId)
        {
            List<AddresseeEntity> addressees = context.Addressee.Where(x => x.AddresseeGroupId == groupId).ToList();

            return PartialView("_AddresseeList", addressees);
        }

        public IActionResult CreateGroup(string number, string name)
        {
            AddresseeGroupEntity group = new()
            {
                Id = int.Parse(number),
                Name = name
            };
            context.AddresseeGroup.Add(group);
            context.SaveChanges();

            //_userLogging.Logging(HttpContext.User.Identity.Name, $"Создание группы рассылки: {group.Id}", JsonSerializer.Serialize(group));

            return RedirectToAction("Tasks", "Task");
        }

        [HttpGet]
        public IActionResult CreateAddressee()
        {
            List<AddresseeGroupEntity> addresseeGroups = context.AddresseeGroup.ToList();
            ViewBag.AddresseeGroups = addresseeGroups;
            AddresseeEntity newAddressee = new();
            return PartialView("_CreateAddressee", newAddressee);
        }

        [HttpPost]
        public IActionResult CreateAddressee(AddresseeEntity addressee, IFormCollection collection)
        {
            try
            {
                List<AddresseeGroupEntity> addresseeGroups = context.AddresseeGroup.ToList();
                ViewBag.AddresseeGroups = addresseeGroups;

                if (ModelState.IsValid)
                {
                    AddresseeEntity entity = new()
                    {
                        PersonalNumber = addressee.PersonalNumber,// collection["PersonalNumber"],
                        EMail = addressee.EMail,// collection["EMail"],
                        Fio = addressee.Fio,// collection["Fio"],
                        StructuralUnit = addressee.PersonalNumber,// collection["StructuralUnit"],
                        AddresseeGroupId = addressee.AddresseeGroupId //int.Parse(collection["AddresseeGroupId"])
                    };
                    context.Addressee.Add(entity);

                    context.SaveChanges();

                    return RedirectToAction(nameof(Addressees));
                }
                return PartialView("_CreateAddressee", addressee);
            }
            catch (Exception)
            {
                return PartialView("_CreateAddressee", addressee);
            }
        }

        [HttpPost]
        public IActionResult ActivatedAddressee(string id)
        {
            AddresseeEntity addressee = context.Addressee.FirstOrDefault(x => x.PersonalNumber == id);

            addressee.IsActive = !addressee.IsActive;

            context.Addressee.Update(addressee);
            context.SaveChanges();
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
            AddresseeGroupEntity deletedGroup = context.AddresseeGroup.FirstOrDefault(x => x.Id == id);
            context.AddresseeGroup.Remove(deletedGroup);
            List<AddresseeEntity> deletedAddresses = context.Addressee.Where(x => x.AddresseeGroupId == id).ToList();
            context.Addressee.RemoveRange(deletedAddresses);
            List<TaskEntity> tasks = context.Task.Where(x => x.AddresseeGroupId == id).ToList();
            foreach (TaskEntity task in tasks)
            {
                task.AddresseeGroupId = null;
            }
            //_appDbContext.Task.UpdateRange(tasks);
            context.SaveChanges();

            //userLogging.Logging(HttpContext.User.Identity.Name, $"Удаление группы рассылки номер: {deletedGroup.Id}", JsonSerializer.Serialize(deletedGroup));

            return RedirectToAction("Tasks", "Task");
        }

        public IActionResult DeleteAddressee(string number, int idGroup)
        {
            AddresseeEntity addressee = context.Addressee.FirstOrDefault(x => x.PersonalNumber == number && x.AddresseeGroupId == idGroup);
            if (addressee != null)
            {
                context.Addressee.Remove(addressee);
                context.SaveChanges();
               /* userLogging.Logging(HttpContext.User.Identity.Name,
                                     $"Удаление адресата {number} из группы номер {idGroup}", 
                                     JsonSerializer.Serialize(addressee));*/
            }
            return RedirectToAction("Addressees");
        }
    }
}
