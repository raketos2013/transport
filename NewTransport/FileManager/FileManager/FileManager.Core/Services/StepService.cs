using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class StepService(IOperationService operationService,
                            IUnitOfWork unitOfWork)
            : IStepService
{
    public async Task<TaskStepEntity> ActivatedStep(int stepId)
    {
        var step = await unitOfWork.StepRepository.ActivatedStep(stepId);
        return await unitOfWork.SaveAsync() > 0 ? step
                            : throw new DomainException("Ошибка изменения шага");
    }

    public async Task<TaskStepEntity> CreateStep(TaskStepEntity taskStep)
    {
        var createdStep = await unitOfWork.StepRepository.CreateStep(taskStep);
        var task = await unitOfWork.TaskRepository.GetTaskById(taskStep.TaskId)
                            ?? throw new DomainException("Задача не найдена");
        await unitOfWork.TaskRepository.EditTask(task);
        return await unitOfWork.SaveAsync() > 0 ? createdStep
                            : throw new DomainException("Ошибка создания шага");
    }

    public async Task<bool> DeleteStep(int stepId)
    {
        var result = await unitOfWork.StepRepository.DeleteStep(stepId);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<TaskStepEntity> EditStep(TaskStepEntity taskStep)
    {
        var step = await unitOfWork.StepRepository.GetStepByTaskId(taskStep.TaskId, taskStep.StepNumber)
                            ?? throw new DomainException("Шаг не найден");
        step.Description = taskStep.Description;
        step.Source = taskStep.Source;
        step.Destination = taskStep.Destination;
        step.FileMask = taskStep.FileMask;
        step.IsActive = taskStep.IsActive;
        step.IsBreak = taskStep.IsBreak;
        if (taskStep.OperationName != step.OperationName)
        {
            step.OperationName = taskStep.OperationName;
            switch (step.OperationName)
            {
                case OperationName.Copy:
                    OperationCopyEntity? copy = await unitOfWork.OperationRepository.GetCopyByStepId(step.StepId);
                    if (copy != null)
                    {
                        unitOfWork.OperationRepository.DeleteCopy(copy);
                    }
                    break;
                case OperationName.Move:
                    OperationMoveEntity? move = await unitOfWork.OperationRepository.GetMoveByStepId(step.StepId);
                    if (move != null)
                    {
                        unitOfWork.OperationRepository.DeleteMove(move);
                    }
                    break;
                case OperationName.Read:
                    OperationReadEntity? read = await unitOfWork.OperationRepository.GetReadByStepId(step.StepId);
                    if (read != null)
                    {
                        unitOfWork.OperationRepository.DeleteRead(read);
                    }
                    break;
                case OperationName.Exist:
                    OperationExistEntity? exist = await unitOfWork.OperationRepository.GetExistByStepId(step.StepId);
                    if (exist != null)
                    {
                        unitOfWork.OperationRepository.DeleteExist(exist);
                    }
                    break;
                case OperationName.Rename:
                    OperationRenameEntity? rename = await unitOfWork.OperationRepository.GetRenameByStepId(step.StepId);
                    if (rename != null)
                    {
                        unitOfWork.OperationRepository.DeleteRename(rename);
                    }
                    break;
                case OperationName.Delete:
                    OperationDeleteEntity? delete = await unitOfWork.OperationRepository.GetDeleteByStepId(step.StepId);
                    if (delete != null)
                    {
                        unitOfWork.OperationRepository.DeleteDelete(delete);
                    }
                    break;
                case OperationName.Clrbuf:
                    OperationClrbufEntity? clrbuf = await unitOfWork.OperationRepository.GetClrbufByStepId(step.StepId);
                    if (clrbuf != null)
                    {
                        unitOfWork.OperationRepository.DeleteClrbuf(clrbuf);
                    }
                    break;
                default:
                    break;
            }
        }
        var editedStep = unitOfWork.StepRepository.EditStep(step);
        return await unitOfWork.SaveAsync() > 0 ? editedStep
                            : throw new DomainException("Ошибка изменения шага");
    }

    public async Task<List<TaskStepEntity>> GetAllSteps()
    {
        return await unitOfWork.StepRepository.GetAllSteps();
    }

    public async Task<List<TaskStepEntity>> GetAllStepsByTaskId(string taskId)
    {
        return await unitOfWork.StepRepository.GetAllStepsByTaskId(taskId);
    }

    public async Task<TaskStepEntity?> GetStepByStepId(int stepId)
    {
        return await unitOfWork.StepRepository.GetStepByStepId(stepId);
    }

    public async Task<TaskStepEntity?> GetStepByTaskId(string taskId, int stepNumber)
    {
        return await unitOfWork.StepRepository.GetStepByTaskId(taskId, stepNumber);
    }

    public async Task<bool> ReplaceSteps(string taskId, string numberStep, string operation)
    {
        var stepsAsync = await unitOfWork.StepRepository.GetAllStepsByTaskId(taskId);
        var steps = stepsAsync.OrderBy(x => x.StepNumber)
                                .ToList();
        TaskStepEntity? step1, step2, tmpStep;
        switch (operation)
        {
            case "up":
                if (int.Parse(numberStep) > 1)
                {
                    step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
                    step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) - 1);
                    if (step1 != null && step2 != null)
                    {
                        step1.StepNumber = int.Parse(numberStep) - 1;
                        step2.StepNumber = int.Parse(numberStep);
                    }
                }
                break;
            case "down":
                if (int.Parse(numberStep) < steps.Count)
                {
                    step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
                    step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) + 1);
                    if (step1 != null && step2 != null)
                    {
                        step1.StepNumber = int.Parse(numberStep) + 1;
                        step2.StepNumber = int.Parse(numberStep);
                    }
                }
                break;
            case "maxup":
                if (int.Parse(numberStep) > 1)
                {
                    for (int i = int.Parse(numberStep) - 1; i > 0; i--)
                    {
                        steps[i].StepNumber = i;
                        steps[i - 1].StepNumber = i + 1;
                        tmpStep = steps[i];
                        steps[i] = steps[i - 1];
                        steps[i - 1] = tmpStep;
                    }
                }
                break;
            case "maxdown":
                if (int.Parse(numberStep) < steps.Count)
                {
                    for (int i = int.Parse(numberStep) - 1; i < steps.Count - 1; i++)
                    {
                        steps[i].StepNumber = i + 2;
                        steps[i + 1].StepNumber = i + 1;
                        tmpStep = steps[i];
                        steps[i] = steps[i + 1];
                        steps[i + 1] = tmpStep;
                    }
                }
                break;
            default:
                break;
        }
        var result = unitOfWork.StepRepository.UpdateRangeSteps(steps);
        return await unitOfWork.SaveAsync() > 0;
    }

    public async Task<TaskStepEntity> CopyStep(int stepId, int newNumber)
    {
        var step = await GetStepByStepId(stepId) ?? throw new DomainException("Шаг не найден");
        var newStep = new TaskStepEntity()
        {
            TaskId = step.TaskId,
            StepNumber = newNumber,
            OperationName = step.OperationName,
            Description = step.Description,
            FileMask = step.FileMask,
            Source = step.Source,
            Destination = step.Destination,
            IsActive = step.IsActive,
            IsBreak = step.IsBreak
        };
        await CreateStep(newStep);

        if (step.OperationId != 0)
        {
            int newOperationId = 0;
            int qwe = newStep.StepId;
            switch (step.OperationName)
            {
                case OperationName.Copy:
                    OperationCopyEntity? oldCopy = await operationService.GetCopyByStepId(step.StepId);
                    if (oldCopy != null)
                    {
                        OperationCopyEntity newCopy = new()
                        {
                            StepId = qwe,
                            InformSuccess = oldCopy.InformSuccess,
                            AddresseeGroupId = oldCopy.AddresseeGroupId,
                            AdditionalText = oldCopy.AdditionalText,
                            FileInSource = oldCopy.FileInSource,
                            FileInDestination = oldCopy.FileInDestination,
                            FileInLog = oldCopy.FileInLog,
                            Sort = oldCopy.Sort,
                            FileAttribute = oldCopy.FileAttribute
                        };
                        await operationService.CreateCopy(newCopy);
                        newOperationId = newCopy.OperationId;
                    }
                    break;
                case OperationName.Move:
                    OperationMoveEntity? oldMove = await operationService.GetMoveByStepId(step.StepId);
                    if (oldMove != null)
                    {
                        OperationMoveEntity newMove = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldMove.InformSuccess,
                            AddresseeGroupId = oldMove.AddresseeGroupId,
                            AdditionalText = oldMove.AdditionalText,
                            FileInDestination = oldMove.FileInDestination,
                            FileInLog = oldMove.FileInLog,
                            Sort = oldMove.Sort,
                            FileAttribute = oldMove.FileAttribute
                        };
                        await operationService.CreateMove(newMove);
                        newOperationId = newMove.OperationId;
                    }
                    break;
                case OperationName.Read:
                    OperationReadEntity? oldRead = await operationService.GetReadByStepId(step.StepId);
                    if (oldRead != null)
                    {
                        OperationReadEntity newRead = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldRead.InformSuccess,
                            AddresseeGroupId = oldRead.AddresseeGroupId,
                            AdditionalText = oldRead.AdditionalText,
                            FileInSource = oldRead.FileInSource,
                            Encode = oldRead.Encode,
                            SearchRegex = oldRead.SearchRegex,
                            FindString = oldRead.FindString,
                            ExpectedResult = oldRead.ExpectedResult,
                            BreakTaskAfterError = oldRead.BreakTaskAfterError
                        };
                        await operationService.CreateRead(newRead);
                        newOperationId = newRead.OperationId;
                    }
                    break;
                case OperationName.Exist:
                    OperationExistEntity? oldExist = await operationService.GetExistByStepId(step.StepId);
                    if (oldExist != null)
                    {
                        OperationExistEntity newExist = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldExist.InformSuccess,
                            AddresseeGroupId = oldExist.AddresseeGroupId,
                            AdditionalText = oldExist.AdditionalText,
                            ExpectedResult = oldExist.ExpectedResult,
                            BreakTaskAfterError = oldExist.BreakTaskAfterError
                        };
                        await operationService.CreateExist(newExist);
                        newOperationId = newExist.OperationId;
                    }
                    break;
                case OperationName.Rename:
                    OperationRenameEntity? oldRename = await operationService.GetRenameByStepId(step.StepId);
                    if (oldRename != null)
                    {
                        OperationRenameEntity newRename = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldRename.InformSuccess,
                            AddresseeGroupId = oldRename.AddresseeGroupId,
                            AdditionalText = oldRename.AdditionalText,
                            OldPattern = oldRename.OldPattern,
                            NewPattern = oldRename.NewPattern
                        };
                        await operationService.CreateRename(newRename);
                        newOperationId = newRename.OperationId;
                    }
                    break;
                case OperationName.Delete:
                    OperationDeleteEntity? oldDelete = await operationService.GetDeleteByStepId(step.StepId);
                    if (oldDelete != null)
                    {
                        OperationDeleteEntity newDelete = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldDelete.InformSuccess,
                            AddresseeGroupId = oldDelete.AddresseeGroupId,
                            AdditionalText = oldDelete.AdditionalText
                        };
                        await operationService.CreateDelete(newDelete);
                        newOperationId = newDelete.OperationId;
                    }
                    break;
                case OperationName.Clrbuf:
                    OperationClrbufEntity? oldClrbuf = await operationService.GetClrbufByStepId(step.StepId);
                    if (oldClrbuf != null)
                    {
                        OperationClrbufEntity newClrbuf = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldClrbuf.InformSuccess,
                            AddresseeGroupId = oldClrbuf.AddresseeGroupId,
                            AdditionalText = oldClrbuf.AdditionalText
                        };
                        await operationService.CreateClrbuf(newClrbuf);
                        newOperationId = newClrbuf.OperationId;
                    }
                    break;
            }
            newStep.OperationId = newOperationId;
            await EditStep(newStep);
        }
        //return await unitOfWork.SaveAsync() > 0 ? newStep 
        //                    : throw new DomainException("Ошибка копирования шага");
        return newStep;
    }

    public async Task<int> CountFiles(int stepId)
    {
        var step = await GetStepByStepId(stepId)
                            ?? throw new DomainException("Шаг не найден");
        string[] files = Directory.GetFiles(step.Source, step.FileMask);
        return files.Length;
    }

}
