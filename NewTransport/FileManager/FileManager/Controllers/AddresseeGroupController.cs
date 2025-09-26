using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Entities;
using FileManager.Core.Constants;
using FileManager.Core.Exceptions;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class AddresseeGroupController(IAddresseeService addresseeService,
                                        IUserLogService userLogService)
            : Controller
{
    public IActionResult Addressees()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddresseeList(int groupId)
    {
        var addresseesAsync = await addresseeService.GetAllAddressees();
        var addressees = addresseesAsync.Where(x => x.AddresseeGroupId == groupId).ToList();
        return PartialView("_AddresseeList", addressees);
    }

    public async Task<IActionResult> CreateGroup(string number, string name)
    {
        AddresseeGroupEntity group = new()
        {
            Id = int.Parse(number),
            Name = name
        };
        var createdGroup = await addresseeService.CreateAddresseeGroup(group);
        await userLogService.AddLog($"Создание группы рассылки номер {createdGroup.Id}",
                                    JsonSerializer.Serialize(createdGroup, AppConstants.JSON_OPTIONS));
        return RedirectToAction(nameof(Addressees));
    }

    [HttpGet]
    public async Task<IActionResult> CreateAddressee()
    {
        List<AddresseeGroupEntity> addresseeGroups = await addresseeService.GetAllAddresseeGroups();
        ViewBag.AddresseeGroups = addresseeGroups;
        AddresseeEntity newAddressee = new();
        return PartialView("_CreateAddressee", newAddressee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAddressee(AddresseeEntity addressee)
    {
        List<AddresseeGroupEntity> addresseeGroups = await addresseeService.GetAllAddresseeGroups();
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
            var createdAddressee = await addresseeService.CreateAddressee(entity);
            await userLogService.AddLog($"Добавление адресата в группу рассылки номер {createdAddressee.AddresseeGroupId}",
                                        JsonSerializer.Serialize(createdAddressee, AppConstants.JSON_OPTIONS));
            return RedirectToAction(nameof(Addressees));
        }
        return PartialView("_CreateAddressee", addressee);
    }

    [HttpPost]
    public async Task<IActionResult> ActivatedAddressee(string id)
    {
        var addresseeAsync = await addresseeService.GetAllAddressees();
        var addressee = addresseeAsync.FirstOrDefault(x => x.PersonalNumber == id);
        if (addressee != null)
        {
            addressee.IsActive = !addressee.IsActive;
            var editedAddressee = await addresseeService.EditAddressee(addressee);
            string activated = "";
            if (editedAddressee.IsActive)
            {
                activated = "Включение";
            }
            else {
                activated = "Выключение";
            }
            await userLogService.AddLog($"{activated} адресата в группе рассылки номер {editedAddressee.AddresseeGroupId}",
                                        JsonSerializer.Serialize(editedAddressee, AppConstants.JSON_OPTIONS));
        }
        return RedirectToAction(nameof(Addressees));
    }

    public async Task<IActionResult> DeleteAddresseeGroup(int id)
    {
        var resultDelete = await addresseeService.DeleteAddresseeGroup(id);
        if (resultDelete)
        {
            await userLogService.AddLog($"Удаление группы рассылки номер {id}", "");
        }
        return RedirectToAction("Tasks", "Task");
    }

    public async Task<IActionResult> DeleteAddressee(string number, int idGroup)
    {
        var addrAsync = await addresseeService.GetAllAddressees();
        var addr = addrAsync.FirstOrDefault(x => x.PersonalNumber == number &&
                                                 x.AddresseeGroupId == idGroup)
                                ?? throw new DomainException("Адресат не найден"); 
        var deletedAddressee = new AddresseeEntity()
        {
            PersonalNumber = number,
            AddresseeGroupId = idGroup,
            EMail = addr.EMail,
            StructuralUnit = addr.StructuralUnit,
            IsActive = addr.IsActive,
            Fio = addr.Fio
        };
        var resultDelete = await addresseeService.DeleteAddressee(number, idGroup);
        if (resultDelete)
        {
            await userLogService.AddLog($"Удаление адресата {number} из группы рассылки номер {idGroup}",
                                        JsonSerializer.Serialize(deletedAddressee, AppConstants.JSON_OPTIONS));
        }
        return RedirectToAction(nameof(Addressees));
    }
}
