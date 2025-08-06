using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Entities;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class AddresseeGroupController(IAddresseeService addresseeService,
                                        IUserLogService userLogService,
                                        IHttpContextAccessor httpContextAccessor
                                        //AppDbContext context
                                        )
            : Controller
{
    private static readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    public IActionResult Addressees()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddresseeList(int groupId)
    {
        List<AddresseeEntity> addressees = addresseeService.GetAllAddressees().Where(x => x.AddresseeGroupId == groupId).ToList();
        //context.Addressee.Where(x => x.AddresseeGroupId == groupId).ToList();

        return PartialView("_AddresseeList", addressees);
    }

    public IActionResult CreateGroup(string number, string name)
    {
        AddresseeGroupEntity group = new()
        {
            Id = int.Parse(number),
            Name = name
        };
        addresseeService.CreateAddresseeGroup(group);
        /*context.AddresseeGroup.Add(group);
        context.SaveChanges();*/
        return RedirectToAction("Addressees");
    }

    [HttpGet]
    public IActionResult CreateAddressee()
    {
        List<AddresseeGroupEntity> addresseeGroups = addresseeService.GetAllAddresseeGroups();
            //context.AddresseeGroup.ToList();
        ViewBag.AddresseeGroups = addresseeGroups;
        AddresseeEntity newAddressee = new();
        return PartialView("_CreateAddressee", newAddressee);
    }

    [HttpPost]
    public IActionResult CreateAddressee(AddresseeEntity addressee)
    {
        try
        {
            List<AddresseeGroupEntity> addresseeGroups = addresseeService.GetAllAddresseeGroups();
                //context.AddresseeGroup.ToList();
            ViewBag.AddresseeGroups = addresseeGroups;

            if (ModelState.IsValid)
            {
                AddresseeEntity entity = new()
                {
                    PersonalNumber = addressee.PersonalNumber,
                    EMail = addressee.EMail,
                    Fio = addressee.Fio,
                    StructuralUnit = addressee.PersonalNumber,
                    AddresseeGroupId = addressee.AddresseeGroupId
                };
                addresseeService.CreateAddressee(entity);
                /*context.Addressee.Add(entity);
                context.SaveChanges();*/

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
        AddresseeEntity addressee = addresseeService.GetAllAddressees().FirstOrDefault(x => x.PersonalNumber == id);
            //context.Addressee.FirstOrDefault(x => x.PersonalNumber == id);
        if (addressee != null)
        {
            addressee.IsActive = !addressee.IsActive;
            addresseeService.EditAddressee(addressee);
            /*context.Addressee.Update(addressee);
            context.SaveChanges();*/
        }
        return RedirectToAction("Addressees");
    }

    public IActionResult DeleteAddresseeGroup(int id)
    {
        addresseeService.DeleteAddresseeGroup(id);
        return RedirectToAction("Tasks", "Task");
    }

    public IActionResult DeleteAddressee(string number, int idGroup)
    {
        var addr = addresseeService.GetAllAddressees()
                                    .FirstOrDefault(x => x.PersonalNumber == number && 
                                                        x.AddresseeGroupId == idGroup);
        var deletedAddressee = new AddresseeEntity()
        {
            PersonalNumber = number,
            AddresseeGroupId = idGroup,
            EMail = addr.EMail,
            StructuralUnit = addr.StructuralUnit,
            IsActive = addr.IsActive,
            Fio = addr.Fio
        };
        addresseeService.DeleteAddressee(number, idGroup);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                        $"Удаление адресата {number} из группы номер {idGroup}",
                                        JsonSerializer.Serialize(deletedAddressee, _options));
        return RedirectToAction("Addressees");
    }
}
