using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class OperationController(IOperationService operationService,
                                    IStepService stepService,
                                    IAddresseeService addresseeService,
                                    ILockService lockService)
            : Controller
{
    [HttpGet]
    public async Task<IActionResult> EditOperation(string taskId, string stepNumber)
    {
        await lockService.Lock(taskId);
        ViewBag.AddresseeGroups = await addresseeService.GetAllAddresseeGroups();
        var step = await stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        if (step == null) 
        {
            return RedirectToAction("Steps", "Step");
        }
        switch (step.OperationName)
        {
            case OperationName.None:
                break;
            case OperationName.Copy:
                var copy = await operationService.GetCopyByStepId(step.StepId) ?? new OperationCopyEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationCopyEdit", copy);
            case OperationName.Move:
                var move = await operationService.GetMoveByStepId(step.StepId) ?? new OperationMoveEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationMoveEdit", move);
            case OperationName.Read:
                var read = await operationService.GetReadByStepId(step.StepId) ?? new OperationReadEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = "",
                    FindString = ""
                };
                return PartialView("_OperationReadEdit", read);
            case OperationName.Exist:
                var exist = await operationService.GetExistByStepId(step.StepId) ?? new OperationExistEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationExistEdit", exist);
            case OperationName.Rename:
                var rename = await operationService.GetRenameByStepId(step.StepId) ?? new OperationRenameEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = "",
                    OldPattern = "",
                    NewPattern = ""
                };
                return PartialView("_OperationRenameEdit", rename);
            case OperationName.Delete:
                var delete = await operationService.GetDeleteByStepId(step.StepId) ?? new OperationDeleteEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationDeleteEdit", delete);
            case OperationName.Clrbuf:
                var clrbuf = await operationService.GetClrbufByStepId(step.StepId) ?? new OperationClrbufEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationClrbufEdit", clrbuf);
            default:
                break;
        }
        return RedirectToAction("Steps", "Step");
    }

    [HttpGet]
    public async Task<IActionResult> Operations(string taskId, string stepNumber, string operationName)
    {
        TaskOperation? taskOperation;
        var step = await stepService.GetStepByTaskId(taskId, int.Parse(stepNumber))
                                ?? throw new DomainException("Шаг не найден");
        switch (operationName)
        {
            case "Copy":
                taskOperation = await operationService.GetCopyByStepId(step.StepId);
                return PartialView("_OperationCopyInfo", taskOperation);
            case "Exist":
                taskOperation = await operationService.GetExistByStepId(step.StepId);
                return PartialView("_OperationExistInfo", taskOperation);
            case "Move":
                taskOperation = await operationService.GetMoveByStepId(step.StepId);
                return PartialView("_OperationMoveInfo", taskOperation);
            case "Read":
                taskOperation = await operationService.GetReadByStepId(step.StepId);
                return PartialView("_OperationReadInfo", taskOperation);
            case "Rename":
                taskOperation = await operationService.GetRenameByStepId(step.StepId);
                return PartialView("_OperationRenameInfo", taskOperation);
            case "Delete":
                taskOperation = await operationService.GetDeleteByStepId(step.StepId);
                return PartialView("_OperationDeleteInfo", taskOperation);
            case "Clrbuf":
                taskOperation = await operationService.GetClrbufByStepId(step.StepId);
                return PartialView("_OperationClrbufInfo", taskOperation);
            default:
                break;
        }
        return RedirectToAction("Tasks", "Task");
    }

    [HttpPost]
    public async Task<IActionResult> EditOperationCopy(OperationCopyEntity operation)
    {
        if (operation.OperationId == 0)
        {
            await operationService.CreateCopy(operation);
        }
        else
        {
            var step = await stepService.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
            await operationService.UpdateCopy(operation);
            await lockService.Unlock(step.TaskId);
        }
        return RedirectToAction("Steps", "Step");
    }

    public async Task<IActionResult> EditOperationMove(OperationMoveEntity operation)
    {
        if (operation.OperationId == 0)
        {
            await operationService.CreateMove(operation);
        }
        else
        {
            var step = await stepService.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
            await operationService.UpdateMove(operation);
            await lockService.Unlock(step.TaskId);
        }
        return RedirectToAction("Steps", "Step");
    }

    public async Task<IActionResult> EditOperationDelete(OperationDeleteEntity operation)
    {
        if (operation.OperationId == 0)
        {
            await operationService.CreateDelete(operation);
        }
        else
        {
            var step = await stepService.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
            await operationService.UpdateDelete(operation);
            await lockService.Unlock(step.TaskId);
        }
        return RedirectToAction("Steps", "Step");
    }

    public async Task<IActionResult> EditOperationRead(OperationReadEntity operation)
    {
        operation.FindString ??= string.Empty;
        if (operation.OperationId == 0)
        {
            await operationService.CreateRead(operation);
        }
        else
        {
            var step = await stepService.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
            await operationService.UpdateRead(operation);
            await lockService.Unlock(step.TaskId);
        }
        return RedirectToAction("Steps", "Step");
    }

    public async Task<IActionResult> EditOperationExist(OperationExistEntity operation)
    {
        if (operation.OperationId == 0)
        {
            await operationService.CreateExist(operation);
        }
        else
        {
            var step = await stepService.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
            await operationService.UpdateExist(operation);
            await lockService.Unlock(step.TaskId);
        }
        return RedirectToAction("Steps", "Step");
    }

    public async Task<IActionResult> EditOperationRename(OperationRenameEntity operation)
    {
        operation.OldPattern ??= string.Empty;
        operation.NewPattern ??= string.Empty;
        if (operation.OperationId == 0)
        {
            await operationService.CreateRename(operation);
        }
        else
        {
            var step = await stepService.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
            await operationService.UpdateRename(operation);
            await lockService.Unlock(step.TaskId);
        }
        return RedirectToAction("Steps", "Step");
    }

    public async Task<IActionResult> EditOperationClrbuf(OperationClrbufEntity operation)
    {
        if (operation.OperationId == 0)
        {
            await operationService.CreateClrbuf(operation);
        }
        else
        {
            var step = await stepService.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
            await operationService.UpdateClrbuf(operation);
            await lockService.Unlock(step.TaskId);
        }
        return RedirectToAction("Steps", "Step");
    }
}
