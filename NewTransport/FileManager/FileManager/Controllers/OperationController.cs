using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers;

[Authorize(Roles = "o.br.ДИТ")]
public class OperationController(IOperationService operationService,
                                    IStepService stepService,
                                    IAddresseeService addresseeService)
            : Controller
{
    [HttpGet]
    public IActionResult EditOperation(string taskId, string stepNumber)
    {
        ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
        var step = stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        if (step == null) 
        {
            return RedirectToAction("Steps", "Step");
        }
        switch (step.OperationName)
        {
            case OperationName.None:
                break;
            case OperationName.Copy:
                var copy = operationService.GetCopyByStepId(step.StepId) ?? new OperationCopyEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationCopyEdit", copy);
            case OperationName.Move:
                var move = operationService.GetMoveByStepId(step.StepId) ?? new OperationMoveEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationMoveEdit", move);
            case OperationName.Read:
                var read = operationService.GetReadByStepId(step.StepId) ?? new OperationReadEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = "",
                    FindString = ""
                };
                return PartialView("_OperationReadEdit", read);
            case OperationName.Exist:
                var exist = operationService.GetExistByStepId(step.StepId) ?? new OperationExistEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationExistEdit", exist);
            case OperationName.Rename:
                var rename = operationService.GetRenameByStepId(step.StepId) ?? new OperationRenameEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = "",
                    OldPattern = "",
                    NewPattern = ""
                };
                return PartialView("_OperationRenameEdit", rename);
            case OperationName.Delete:
                var delete = operationService.GetDeleteByStepId(step.StepId) ?? new OperationDeleteEntity
                {
                    StepId = step.StepId,
                    AddresseeGroupId = 0,
                    AdditionalText = ""
                };
                return PartialView("_OperationDeleteEdit", delete);
            case OperationName.Clrbuf:
                var clrbuf = operationService.GetClrbufByStepId(step.StepId) ?? new OperationClrbufEntity
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
    public IActionResult Operations(string taskId, string stepNumber, string operationName)
    {
        TaskOperation? taskOperation;
        var step = stepService.GetStepByTaskId(taskId, int.Parse(stepNumber));
        switch (operationName)
        {
            case "Copy":
                taskOperation = operationService.GetCopyByStepId(step.StepId);
                return PartialView("_OperationCopyInfo", taskOperation);
            case "Exist":
                taskOperation = operationService.GetExistByStepId(step.StepId);
                return PartialView("_OperationExistInfo", taskOperation);
            case "Move":
                taskOperation = operationService.GetMoveByStepId(step.StepId);
                return PartialView("_OperationMoveInfo", taskOperation);
            case "Read":
                taskOperation = operationService.GetReadByStepId(step.StepId);
                return PartialView("_OperationReadInfo", taskOperation);
            case "Rename":
                taskOperation = operationService.GetRenameByStepId(step.StepId);
                return PartialView("_OperationRenameInfo", taskOperation);
            case "Delete":
                taskOperation = operationService.GetDeleteByStepId(step.StepId);
                return PartialView("_OperationDeleteInfo", taskOperation);
            case "Clrbuf":
                taskOperation = operationService.GetClrbufByStepId(step.StepId);
                return PartialView("_OperationClrbufInfo", taskOperation);
            default:
                break;
        }
        return RedirectToAction("Tasks", "Task");
    }

    [HttpPost]
    public IActionResult EditOperationCopy(OperationCopyEntity operation)
    {
        if (operation.OperationId == 0)
        {
            operationService.CreateCopy(operation);
        }
        else
        {
            operationService.UpdateCopy(operation);
        }
        return RedirectToAction("Steps", "Step");
    }

    public IActionResult EditOperationMove(OperationMoveEntity operation)
    {
        if (operation.OperationId == 0)
        {
            operationService.CreateMove(operation);
        }
        else
        {
            operationService.UpdateMove(operation);
        }
        return RedirectToAction("Steps", "Step");
    }

    public IActionResult EditOperationDelete(OperationDeleteEntity operation)
    {
        if (operation.OperationId == 0)
        {
            operationService.CreateDelete(operation);
        }
        else
        {
            operationService.UpdateDelete(operation);
        }
        return RedirectToAction("Steps", "Step");
    }

    public IActionResult EditOperationRead(OperationReadEntity operation)
    {
        operation.FindString ??= string.Empty;
        if (operation.OperationId == 0)
        {
            operationService.CreateRead(operation);
        }
        else
        {
            operationService.UpdateRead(operation);
        }
        return RedirectToAction("Steps", "Step");
    }

    public IActionResult EditOperationExist(OperationExistEntity operation)
    {
        if (operation.OperationId == 0)
        {
            operationService.CreateExist(operation);
        }
        else
        {
            operationService.UpdateExist(operation);
        }
        return RedirectToAction("Steps", "Step");
    }

    public IActionResult EditOperationRename(OperationRenameEntity operation)
    {
        operation.OldPattern ??= string.Empty;
        operation.NewPattern ??= string.Empty;
        if (operation.OperationId == 0)
        {
            operationService.CreateRename(operation);
        }
        else
        {
            operationService.UpdateRename(operation);
        }
        return RedirectToAction("Steps", "Step");
    }

    public IActionResult EditOperationClrbuf(OperationClrbufEntity operation)
    {
        if (operation.OperationId == 0)
        {
            operationService.CreateClrbuf(operation);
        }
        else
        {
            operationService.UpdateClrbuf(operation);
        }
        return RedirectToAction("Steps", "Step");
    }
}
