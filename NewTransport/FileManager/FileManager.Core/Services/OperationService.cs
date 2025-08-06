using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileManager.Core.Services;

public class OperationService(IOperationRepository operationRepository,
                                IStepRepository stepRepository,
                                IUserLogService userLogService,
                                IHttpContextAccessor httpContextAccessor)
            : IOperationService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    public bool CreateClrbuf(OperationClrbufEntity operation)
    {
        var result = operationRepository.CreateClrbuf(operation);
        var step = stepRepository.GetStepByStepId(operation.StepId);
        step.OperationId = operation.OperationId;
        stepRepository.EditStep(step);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Clrbuf для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public bool CreateCopy(OperationCopyEntity operation)
    {
        var result = operationRepository.CreateCopy(operation);
        var step = stepRepository.GetStepByStepId(operation.StepId);
        step.OperationId = operation.OperationId;
        stepRepository.EditStep(step);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Copy для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public bool CreateDelete(OperationDeleteEntity operation)
    {
        var result = operationRepository.CreateDelete(operation);
        var step = stepRepository.GetStepByStepId(operation.StepId);
        step.OperationId = operation.OperationId;
        stepRepository.EditStep(step);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Delete для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public bool CreateExist(OperationExistEntity operation)
    {
        var result = operationRepository.CreateExist(operation);
        var step = stepRepository.GetStepByStepId(operation.StepId);
        step.OperationId = operation.OperationId;
        stepRepository.EditStep(step);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Exist для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public bool CreateMove(OperationMoveEntity operation)
    {
        var result = operationRepository.CreateMove(operation);
        var step = stepRepository.GetStepByStepId(operation.StepId);
        step.OperationId = operation.OperationId;
        stepRepository.EditStep(step);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Move для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public bool CreateRead(OperationReadEntity operation)
    {
        var result = operationRepository.CreateRead(operation);
        var step = stepRepository.GetStepByStepId(operation.StepId);
        step.OperationId = operation.OperationId;
        stepRepository.EditStep(step);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Read для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public bool CreateRename(OperationRenameEntity operation)
    {
        var result = operationRepository.CreateRename(operation);
        var step = stepRepository.GetStepByStepId(operation.StepId);
        step.OperationId = operation.OperationId;
        stepRepository.EditStep(step);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Rename для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public bool DeleteClrbuf(OperationClrbufEntity operation)
    {
        return operationRepository.DeleteClrbuf(operation);
    }

    public bool DeleteCopy(OperationCopyEntity operation)
    {
        return operationRepository.DeleteCopy(operation);
    }

    public bool DeleteDelete(OperationDeleteEntity operation)
    {
        return operationRepository.DeleteDelete(operation);
    }

    public bool DeleteExist(OperationExistEntity operation)
    {
        return operationRepository.DeleteExist(operation);
    }

    public bool DeleteMove(OperationMoveEntity operation)
    {
        return operationRepository.DeleteMove(operation);
    }

    public bool DeleteRead(OperationReadEntity operation)
    {
        return operationRepository.DeleteRead(operation);
    }

    public bool DeleteRename(OperationRenameEntity operation)
    {
        return operationRepository.DeleteRename(operation);
    }

    public OperationClrbufEntity? GetClrbufByStepId(int stepId)
    {
        return operationRepository.GetClrbufByStepId(stepId);
    }

    public OperationCopyEntity? GetCopyByStepId(int stepId)
    {
        return operationRepository.GetCopyByStepId(stepId);
    }

    public OperationDeleteEntity? GetDeleteByStepId(int stepId)
    {
        return operationRepository.GetDeleteByStepId(stepId);
    }

    public OperationExistEntity? GetExistByStepId(int stepId)
    {
        return operationRepository.GetExistByStepId(stepId);
    }

    public OperationMoveEntity? GetMoveByStepId(int stepId)
    {
        return operationRepository.GetMoveByStepId(stepId);
    }

    public OperationReadEntity? GetReadByStepId(int stepId)
    {
        return operationRepository.GetReadByStepId(stepId);
    }

    public OperationRenameEntity? GetRenameByStepId(int stepId)
    {
        return operationRepository.GetRenameByStepId(stepId);
    }

    public bool UpdateClrbuf(OperationClrbufEntity operation)
    {
        return operationRepository.UpdateClrbuf(operation);
    }

    public bool UpdateCopy(OperationCopyEntity operation)
    {
        return operationRepository.UpdateCopy(operation);
    }

    public bool UpdateDelete(OperationDeleteEntity operation)
    {
        return operationRepository.UpdateDelete(operation);
    }

    public bool UpdateExist(OperationExistEntity operation)
    {
        return operationRepository.UpdateExist(operation);
    }

    public bool UpdateMove(OperationMoveEntity operation)
    {
        return operationRepository.UpdateMove(operation);
    }

    public bool UpdateRead(OperationReadEntity operation)
    {
        return operationRepository.UpdateRead(operation);
    }

    public bool UpdateRename(OperationRenameEntity operation)
    {
        return operationRepository.UpdateRename(operation);
    }
}
