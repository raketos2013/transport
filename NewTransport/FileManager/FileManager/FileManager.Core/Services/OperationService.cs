using FileManager.Core.Constants;
using FileManager.Core.Entities;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace FileManager.Core.Services;

public class OperationService(IUnitOfWork unitOfWork
                                //IUserLogService userLogService,
                                //IHttpContextAccessor httpContextAccessor
                                )
            : IOperationService
{
    public async Task<OperationClrbufEntity> CreateClrbuf(OperationClrbufEntity operation)
    {
        var step = await unitOfWork.StepRepository.GetStepByStepId(operation.StepId) 
                                ?? throw new DomainException("Шаг не найден");
        await unitOfWork.OperationRepository.CreateClrbuf(operation);
        await unitOfWork.SaveAsync();
        step.OperationId = operation.OperationId;
        unitOfWork.StepRepository.EditStep(step);
        //await userLogService.AddLog($"Добавление доп. св-в операции Clrbuf для шага номер {step.StepNumber} задачи {step.TaskId}",
        //                            JsonSerializer.Serialize(operation, AppConstants.JSON_OPTIONS));
        return await unitOfWork.SaveAsync() > 0 ? operation
                : throw new DomainException($"Ошибка добавления доп. св-в операции Clrbuf для шага номер {step.StepNumber} задачи {step.TaskId}");
    }

    public async Task<OperationCopyEntity> CreateCopy(OperationCopyEntity operation)
    {
        var step = await unitOfWork.StepRepository.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
        await unitOfWork.OperationRepository.CreateCopy(operation);
        await unitOfWork.SaveAsync();
        step.OperationId = operation.OperationId;
        unitOfWork.StepRepository.EditStep(step);
        //await userLogService.AddLog($"Добавление доп. св-в операции Copy для шага номер {step.StepNumber} задачи {step.TaskId}",
        //                            JsonSerializer.Serialize(operation, AppConstants.JSON_OPTIONS));
        return await unitOfWork.SaveAsync() > 0 ? operation
                : throw new DomainException($"Ошибка добавления доп. св-в операции Copy для шага номер {step.StepNumber} задачи {step.TaskId}");
    }

    public async Task<OperationDeleteEntity> CreateDelete(OperationDeleteEntity operation)
    {
        var step = await unitOfWork.StepRepository.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
        await unitOfWork.OperationRepository.CreateDelete(operation);
        await unitOfWork.SaveAsync();
        step.OperationId = operation.OperationId;
        unitOfWork.StepRepository.EditStep(step);
        //await userLogService.AddLog($"Добавление доп. св-в операции Delete для шага номер {step.StepNumber} задачи {step.TaskId}",
        //                            JsonSerializer.Serialize(operation, AppConstants.JSON_OPTIONS));
        return await unitOfWork.SaveAsync() > 0 ? operation
                : throw new DomainException($"Ошибка добавления доп. св-в операции Delete для шага номер {step.StepNumber} задачи {step.TaskId}");
    }

    public async Task<OperationExistEntity> CreateExist(OperationExistEntity operation)
    {
        var step = await unitOfWork.StepRepository.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
        await unitOfWork.OperationRepository.CreateExist(operation);
        await unitOfWork.SaveAsync();
        step.OperationId = operation.OperationId;
        unitOfWork.StepRepository.EditStep(step);
        //await userLogService.AddLog($"Добавление доп. св-в операции Exist для шага номер {step.StepNumber} задачи {step.TaskId}",
        //                            JsonSerializer.Serialize(operation, AppConstants.JSON_OPTIONS));
        return await unitOfWork.SaveAsync() > 0 ? operation
                : throw new DomainException($"Ошибка добавления доп. св-в операции Exist для шага номер {step.StepNumber} задачи {step.TaskId}");
    }

    public async Task<OperationMoveEntity> CreateMove(OperationMoveEntity operation)
    {
        var step = await unitOfWork.StepRepository.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
        await unitOfWork.OperationRepository.CreateMove(operation);
        await unitOfWork.SaveAsync();
        step.OperationId = operation.OperationId;
        unitOfWork.StepRepository.EditStep(step);
        //await userLogService.AddLog($"Добавление доп. св-в операции Move для шага номер {step.StepNumber} задачи {step.TaskId}",
        //                            JsonSerializer.Serialize(operation, AppConstants.JSON_OPTIONS));
        return await unitOfWork.SaveAsync() > 0 ? operation
                : throw new DomainException($"Ошибка добавления доп. св-в операции Move для шага номер {step.StepNumber} задачи {step.TaskId}");
    }

    public async Task<OperationReadEntity> CreateRead(OperationReadEntity operation)
    {
        var step = await unitOfWork.StepRepository.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
        await unitOfWork.OperationRepository.CreateRead(operation);
        await unitOfWork.SaveAsync();
        step.OperationId = operation.OperationId;
        unitOfWork.StepRepository.EditStep(step);
        //await userLogService.AddLog($"Добавление доп. св-в операции Read для шага номер {step.StepNumber} задачи {step.TaskId}",
        //                            JsonSerializer.Serialize(operation, AppConstants.JSON_OPTIONS));
        return await unitOfWork.SaveAsync() > 0 ? operation
                : throw new DomainException($"Ошибка добавления доп. св-в операции Read для шага номер {step.StepNumber} задачи {step.TaskId}");
    }

    public async Task<OperationRenameEntity> CreateRename(OperationRenameEntity operation)
    {
        var step = await unitOfWork.StepRepository.GetStepByStepId(operation.StepId)
                                ?? throw new DomainException("Шаг не найден");
        await unitOfWork.OperationRepository.CreateRename(operation);
        await unitOfWork.SaveAsync();
        step.OperationId = operation.OperationId;
        unitOfWork.StepRepository.EditStep(step);
        //await userLogService.AddLog($"Добавление доп. св-в операции Rename для шага номер {step.StepNumber} задачи {step.TaskId}",
        //                            JsonSerializer.Serialize(operation, AppConstants.JSON_OPTIONS));
        return await unitOfWork.SaveAsync() > 0 ? operation
                : throw new DomainException($"Ошибка добавления доп. св-в операции Rename для шага номер {step.StepNumber} задачи {step.TaskId}");
    }

    public async Task<bool> DeleteClrbuf(OperationClrbufEntity operation)
    {
        var result = unitOfWork.OperationRepository.DeleteClrbuf(operation);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteCopy(OperationCopyEntity operation)
    {
        var result = unitOfWork.OperationRepository.DeleteCopy(operation);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteDelete(OperationDeleteEntity operation)
    {
        var result = unitOfWork.OperationRepository.DeleteDelete(operation);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteExist(OperationExistEntity operation)
    {
        var result = unitOfWork.OperationRepository.DeleteExist(operation);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteMove(OperationMoveEntity operation)
    {
        var result = unitOfWork.OperationRepository.DeleteMove(operation);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteRead(OperationReadEntity operation)
    {
        var result = unitOfWork.OperationRepository.DeleteRead(operation);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteRename(OperationRenameEntity operation)
    {
        var result = unitOfWork.OperationRepository.DeleteRename(operation);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<OperationClrbufEntity?> GetClrbufByStepId(int stepId)
    {
        return await unitOfWork.OperationRepository.GetClrbufByStepId(stepId);
    }

    public async Task<OperationCopyEntity?> GetCopyByStepId(int stepId)
    {
        return await unitOfWork.OperationRepository.GetCopyByStepId(stepId);
    }

    public async Task<OperationDeleteEntity?> GetDeleteByStepId(int stepId)
    {
        return await unitOfWork.OperationRepository.GetDeleteByStepId(stepId);
    }

    public async Task<OperationExistEntity?> GetExistByStepId(int stepId)
    {
        return await unitOfWork.OperationRepository.GetExistByStepId(stepId);
    }

    public async Task<OperationMoveEntity?> GetMoveByStepId(int stepId)
    {
        return await unitOfWork.OperationRepository.GetMoveByStepId(stepId);
    }

    public async Task<OperationReadEntity?> GetReadByStepId(int stepId)
    {
        return await unitOfWork.OperationRepository.GetReadByStepId(stepId);
    }

    public async Task<OperationRenameEntity?> GetRenameByStepId(int stepId)
    {
        return await unitOfWork.OperationRepository.GetRenameByStepId(stepId);
    }

    public async Task<OperationClrbufEntity> UpdateClrbuf(OperationClrbufEntity operation)
    {
        var edited = unitOfWork.OperationRepository.UpdateClrbuf(operation);
        return await unitOfWork.SaveAsync() > 0 ? edited
                                : throw new DomainException("Ошибка при обновлении операции Clrbuf");
    }

    public async Task<OperationCopyEntity> UpdateCopy(OperationCopyEntity operation)
    {
        var edited = unitOfWork.OperationRepository.UpdateCopy(operation);
        return await unitOfWork.SaveAsync() > 0 ? edited
                                : throw new DomainException("Ошибка при обновлении операции Copy");
    }

    public async Task<OperationDeleteEntity> UpdateDelete(OperationDeleteEntity operation)
    {
        var edited = unitOfWork.OperationRepository.UpdateDelete(operation);
        return await unitOfWork.SaveAsync() > 0 ? edited
                                : throw new DomainException("Ошибка при обновлении операции Delete");
    }

    public async Task<OperationExistEntity> UpdateExist(OperationExistEntity operation)
    {
        var edited = unitOfWork.OperationRepository.UpdateExist(operation);
        return await unitOfWork.SaveAsync() > 0 ? edited
                                : throw new DomainException("Ошибка при обновлении операции Exist");
    }

    public async Task<OperationMoveEntity> UpdateMove(OperationMoveEntity operation)
    {
        var edited = unitOfWork.OperationRepository.UpdateMove(operation);
        return await unitOfWork.SaveAsync() > 0 ? edited
                                : throw new DomainException("Ошибка при обновлении операции Move");
    }

    public async Task<OperationReadEntity> UpdateRead(OperationReadEntity operation)
    {
        var edited = unitOfWork.OperationRepository.UpdateRead(operation);
        return await unitOfWork.SaveAsync() > 0 ? edited
                                : throw new DomainException("Ошибка при обновлении операции Read");
    }

    public async Task<OperationRenameEntity> UpdateRename(OperationRenameEntity operation)
    {
        var edited = unitOfWork.OperationRepository.UpdateRename(operation);
        return await unitOfWork.SaveAsync() > 0 ? edited
                                : throw new DomainException("Ошибка при обновлении операции Rename");
    }
}