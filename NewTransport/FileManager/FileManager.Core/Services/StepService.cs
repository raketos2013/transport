using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileManager.Core.Services;

public class StepService(IStepRepository stepRepository,
                            ITaskService taskService,
                            IOperationRepository operationRepository,
                            IOperationService operationService,
                            IUserLogService userLogService,
                            IHttpContextAccessor httpContextAccessor)
            : IStepService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };

    public async Task<bool> ActivatedStep(int stepId)
    {
        var result = await stepRepository.ActivatedStep(stepId);
        var step = await stepRepository.GetStepByStepId(stepId);
        var text = step.IsActive ? "Включение" : "Выключение";
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"{text} шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(step, _options));
        return result;
    }

    public async Task<bool> CreateStep(TaskStepEntity taskStep)
    {
        //stepRepository.CreateStep(taskStep);
        await taskService.UpdateLastModifiedTask(taskStep.TaskId);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"Добавление шага для задачи {taskStep.TaskId}",
                                JsonSerializer.Serialize(taskStep, _options));
        return await stepRepository.CreateStep(taskStep);
    }

    public async Task<bool> DeleteStep(int stepId)
    {
        return await stepRepository.DeleteStep(stepId);
    }

    public async Task<bool> EditStep(TaskStepEntity taskStep)
    {
        var step = await stepRepository.GetStepByTaskId(taskStep.TaskId, taskStep.StepNumber);
        if (step == null)
        {
            return false;
        }
        //step.StepNumber = taskStep.StepNumber;
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
                    OperationCopyEntity? copy = await operationRepository.GetCopyByStepId(step.StepId);
                    if (copy != null)
                    {
                        await operationRepository.DeleteCopy(copy);
                    }
                    break;
                case OperationName.Move:
                    OperationMoveEntity? move = await operationRepository.GetMoveByStepId(step.StepId);
                    if (move != null)
                    {
                        await operationRepository.DeleteMove(move);
                    }
                    break;
                case OperationName.Read:
                    OperationReadEntity? read = await operationRepository.GetReadByStepId(step.StepId);
                    if (read != null)
                    {
                        await operationRepository.DeleteRead(read);
                    }
                    break;
                case OperationName.Exist:
                    OperationExistEntity? exist = await operationRepository.GetExistByStepId(step.StepId);
                    if (exist != null)
                    {
                        await operationRepository.DeleteExist(exist);
                    }
                    break;
                case OperationName.Rename:
                    OperationRenameEntity? rename = await operationRepository.GetRenameByStepId(step.StepId);
                    if (rename != null)
                    {
                        await operationRepository.DeleteRename(rename);
                    }
                    break;
                case OperationName.Delete:
                    OperationDeleteEntity? delete = await operationRepository.GetDeleteByStepId(step.StepId);
                    if (delete != null)
                    {
                        await operationRepository.DeleteDelete(delete);
                    }
                    break;
                case OperationName.Clrbuf:
                    OperationClrbufEntity? clrbuf = await operationRepository.GetClrbufByStepId(step.StepId);
                    if (clrbuf != null)
                    {
                        await operationRepository.DeleteClrbuf(clrbuf);
                    }
                    break;
                default:
                    break;
            }
        }
        return await stepRepository.EditStep(step);
    }

    public async Task<List<TaskStepEntity>> GetAllSteps()
    {
        return await stepRepository.GetAllSteps();
    }

    public async Task<List<TaskStepEntity>> GetAllStepsByTaskId(string taskId)
    {
        return await stepRepository.GetAllStepsByTaskId(taskId);
    }

    public async Task<TaskStepEntity?> GetStepByStepId(int stepId)
    {
        return await stepRepository.GetStepByStepId(stepId);
    }

    public async Task<TaskStepEntity?> GetStepByTaskId(string taskId, int stepNumber)
    {
        return await stepRepository.GetStepByTaskId(taskId, stepNumber);
    }

    public async Task<bool> ReplaceSteps(string taskId, string numberStep, string operation)
    {
        try
        {
            var stepsAsync = await stepRepository.GetAllStepsByTaskId(taskId);
            var steps = stepsAsync.OrderBy(x => x.StepNumber)
                                    .ToList();
            TaskStepEntity step1, step2, tmpStep;
            switch (operation)
            {
                case "up":
                    if (int.Parse(numberStep) > 1)
                    {
                        step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
                        step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) - 1);
                        step1.StepNumber = int.Parse(numberStep) - 1;
                        step2.StepNumber = int.Parse(numberStep);
                    }
                    break;
                case "down":
                    if (int.Parse(numberStep) < steps.Count)
                    {
                        step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
                        step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) + 1);
                        step1.StepNumber = int.Parse(numberStep) + 1;
                        step2.StepNumber = int.Parse(numberStep);
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

            return await stepRepository.UpdateRangeSteps(steps);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CopyStep(int stepId, int newNumber)
    {
        var step = await GetStepByStepId(stepId);
        if (step == null)
        {
            return false;
        }
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
        /*        
                var steps = GetAllStepsByTaskId(step.TaskId);
                foreach (var item in steps.Where(x => x.StepNumber >= newNumber))
                {
                    item.StepNumber++;
                    EditStep(item);
                }*/
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
                            //Step = newStep,
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
                    OperationMoveEntity? oldMove = await operationRepository.GetMoveByStepId(step.StepId);
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
                        await operationRepository.CreateMove(newMove);
                        newOperationId = newMove.OperationId;
                    }
                    break;
                case OperationName.Read:
                    OperationReadEntity? oldRead = await operationRepository.GetReadByStepId(step.StepId);
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
                        await operationRepository.CreateRead(newRead);
                        newOperationId = newRead.OperationId;
                    }
                    break;
                case OperationName.Exist:
                    OperationExistEntity? oldExist = await operationRepository.GetExistByStepId(step.StepId);
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
                        await operationRepository.CreateExist(newExist);
                        newOperationId = newExist.OperationId;
                    }
                    break;
                case OperationName.Rename:
                    OperationRenameEntity? oldRename = await operationRepository.GetRenameByStepId(step.StepId);
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
                        await operationRepository.CreateRename(newRename);
                        newOperationId = newRename.OperationId;
                    }
                    break;
                case OperationName.Delete:
                    OperationDeleteEntity? oldDelete = await operationRepository.GetDeleteByStepId(step.StepId);
                    if (oldDelete != null)
                    {
                        OperationDeleteEntity newDelete = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldDelete.InformSuccess,
                            AddresseeGroupId = oldDelete.AddresseeGroupId,
                            AdditionalText = oldDelete.AdditionalText
                        };
                        await operationRepository.CreateDelete(newDelete);
                        newOperationId = newDelete.OperationId;
                    }
                    break;
                case OperationName.Clrbuf:
                    OperationClrbufEntity? oldClrbuf = await operationRepository.GetClrbufByStepId(step.StepId);
                    if (oldClrbuf != null)
                    {
                        OperationClrbufEntity newClrbuf = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldClrbuf.InformSuccess,
                            AddresseeGroupId = oldClrbuf.AddresseeGroupId,
                            AdditionalText = oldClrbuf.AdditionalText
                        };
                        await operationRepository.CreateClrbuf(newClrbuf);
                        newOperationId = newClrbuf.OperationId;
                    }
                    break;
            }
            newStep.OperationId = newOperationId;
            await EditStep(newStep);
        }
        return true;
    }


    public async Task<int> CountFiles(int stepId)
    {
        var step = await GetStepByStepId(stepId);
        string[] files = Directory.GetFiles(step.Source, step.FileMask);
        return files.Length;
    }

}
