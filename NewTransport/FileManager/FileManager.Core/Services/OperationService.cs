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
    public async Task<bool> CreateClrbuf(OperationClrbufEntity operation)
    {
        var result = await operationRepository.CreateClrbuf(operation);
        var step = await stepRepository.GetStepByStepId(operation.StepId);
        if (step == null)
        {
            await operationRepository.DeleteClrbuf(operation);
            return false;
        }
        step.OperationId = operation.OperationId;
        await stepRepository.EditStep(step);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Clrbuf для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public async Task<bool> CreateCopy(OperationCopyEntity operation)
    {
        var result = await operationRepository.CreateCopy(operation);
        var step = await stepRepository.GetStepByStepId(operation.StepId);
        if (step == null)
        {
            await operationRepository.DeleteCopy(operation);
            return false;
        }
        step.OperationId = operation.OperationId;
        await stepRepository.EditStep(step);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Copy для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public async Task<bool> CreateDelete(OperationDeleteEntity operation)
    {
        var result = await operationRepository.CreateDelete(operation);
        var step = await stepRepository.GetStepByStepId(operation.StepId);
        if (step == null)
        {
            await operationRepository.DeleteDelete(operation);
            return false;
        }
        step.OperationId = operation.OperationId;
        await stepRepository.EditStep(step);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Delete для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public async Task<bool> CreateExist(OperationExistEntity operation)
    {
        var result = await operationRepository.CreateExist(operation);
        var step = await stepRepository.GetStepByStepId(operation.StepId);
        if (step == null)
        {
            await operationRepository.DeleteExist(operation);
            return false;
        }
        step.OperationId = operation.OperationId;
        await stepRepository.EditStep(step);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Exist для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public async Task<bool> CreateMove(OperationMoveEntity operation)
    {
        var result = await operationRepository.CreateMove(operation);
        var step = await stepRepository.GetStepByStepId(operation.StepId);
        if (step == null)
        {
            await operationRepository.DeleteMove(operation);
            return false;
        }
        step.OperationId = operation.OperationId;
        await stepRepository.EditStep(step);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Move для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public async Task<bool> CreateRead(OperationReadEntity operation)
    {
        var result = await operationRepository.CreateRead(operation);
        var step = await stepRepository.GetStepByStepId(operation.StepId);
        if (step == null)
        {
            await operationRepository.DeleteRead(operation);
            return false;
        }
        step.OperationId = operation.OperationId;
        await stepRepository.EditStep(step);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Read для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public async Task<bool> CreateRename(OperationRenameEntity operation)
    {
        var result = await operationRepository.CreateRename(operation);
        var step = await stepRepository.GetStepByStepId(operation.StepId);
        if (step == null)
        {
            await operationRepository.DeleteRename(operation);
            return false;
        }
        step.OperationId = operation.OperationId;
        await stepRepository.EditStep(step);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name,
                                $"Добавление доп. св-в операции Rename для шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(operation, _options));
        return result;
    }

    public async Task<bool> DeleteClrbuf(OperationClrbufEntity operation)
    {
        return await operationRepository.DeleteClrbuf(operation);
    }

    public async Task<bool> DeleteCopy(OperationCopyEntity operation)
    {
        return await operationRepository.DeleteCopy(operation);
    }

    public async Task<bool> DeleteDelete(OperationDeleteEntity operation)
    {
        return await operationRepository.DeleteDelete(operation);
    }

    public async Task<bool> DeleteExist(OperationExistEntity operation)
    {
        return await operationRepository.DeleteExist(operation);
    }

    public async Task<bool> DeleteMove(OperationMoveEntity operation)
    {
        return await operationRepository.DeleteMove(operation);
    }

    public async Task<bool> DeleteRead(OperationReadEntity operation)
    {
        return await operationRepository.DeleteRead(operation);
    }

    public async Task<bool> DeleteRename(OperationRenameEntity operation)
    {
        return await operationRepository.DeleteRename(operation);
    }

    public async Task<OperationClrbufEntity?> GetClrbufByStepId(int stepId)
    {
        return await operationRepository.GetClrbufByStepId(stepId);
    }

    public async Task<OperationCopyEntity?> GetCopyByStepId(int stepId)
    {
        return await operationRepository.GetCopyByStepId(stepId);
    }

    public async Task<OperationDeleteEntity?> GetDeleteByStepId(int stepId)
    {
        return await operationRepository.GetDeleteByStepId(stepId);
    }

    public async Task<OperationExistEntity?> GetExistByStepId(int stepId)
    {
        return await operationRepository.GetExistByStepId(stepId);
    }

    public async Task<OperationMoveEntity?> GetMoveByStepId(int stepId)
    {
        return await operationRepository.GetMoveByStepId(stepId);
    }

    public async Task<OperationReadEntity?> GetReadByStepId(int stepId)
    {
        return await operationRepository.GetReadByStepId(stepId);
    }

    public async Task<OperationRenameEntity?> GetRenameByStepId(int stepId)
    {
        return await operationRepository.GetRenameByStepId(stepId);
    }

    public async Task<bool> UpdateClrbuf(OperationClrbufEntity operation)
    {
        return await operationRepository.UpdateClrbuf(operation);
    }

    public async Task<bool> UpdateCopy(OperationCopyEntity operation)
    {
        return await operationRepository.UpdateCopy(operation);
    }

    public async Task<bool> UpdateDelete(OperationDeleteEntity operation)
    {
        return await operationRepository.UpdateDelete(operation);
    }

    public async Task<bool> UpdateExist(OperationExistEntity operation)
    {
        return await operationRepository.UpdateExist(operation);
    }

    public async Task<bool> UpdateMove(OperationMoveEntity operation)
    {
        return await operationRepository.UpdateMove(operation);
    }

    public async Task<bool> UpdateRead(OperationReadEntity operation)
    {
        return await operationRepository.UpdateRead(operation);
    }

    public async Task<bool> UpdateRename(OperationRenameEntity operation)
    {
        return await operationRepository.UpdateRename(operation);
    }
}
