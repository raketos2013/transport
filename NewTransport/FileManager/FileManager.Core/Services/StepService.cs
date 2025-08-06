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

    public bool ActivatedStep(int stepId)
    {
        var result = stepRepository.ActivatedStep(stepId);
        var step = stepRepository.GetStepByStepId(stepId);
        var text = step.IsActive ? "Включение" : "Выключение";
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"{text} шага номер {step.StepNumber} задачи {step.TaskId}",
                                JsonSerializer.Serialize(step, _options));
        return result;
    }

    public bool CreateStep(TaskStepEntity taskStep)
    {
        //stepRepository.CreateStep(taskStep);
        taskService.UpdateLastModifiedTask(taskStep.TaskId);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"Добавление шага для задачи {taskStep.TaskId}",
                                JsonSerializer.Serialize(taskStep, _options));
        return stepRepository.CreateStep(taskStep);
    }

    public bool DeleteStep(int stepId)
    {
        return stepRepository.DeleteStep(stepId);
    }

    public bool EditStep(TaskStepEntity taskStep)
    {
        var step = stepRepository.GetStepByTaskId(taskStep.TaskId, taskStep.StepNumber);
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
                    OperationCopyEntity? copy = operationRepository.GetCopyByStepId(step.StepId);
                    if (copy != null)
                    {
                        operationRepository.DeleteCopy(copy);
                    }
                    break;
                case OperationName.Move:
                    OperationMoveEntity? move = operationRepository.GetMoveByStepId(step.StepId);
                    if (move != null)
                    {
                        operationRepository.DeleteMove(move);
                    }
                    break;
                case OperationName.Read:
                    OperationReadEntity? read = operationRepository.GetReadByStepId(step.StepId);
                    if (read != null)
                    {
                        operationRepository.DeleteRead(read);
                    }
                    break;
                case OperationName.Exist:
                    OperationExistEntity? exist = operationRepository.GetExistByStepId(step.StepId);
                    if (exist != null)
                    {
                        operationRepository.DeleteExist(exist);
                    }
                    break;
                case OperationName.Rename:
                    OperationRenameEntity? rename = operationRepository.GetRenameByStepId(step.StepId);
                    if (rename != null)
                    {
                        operationRepository.DeleteRename(rename);
                    }
                    break;
                case OperationName.Delete:
                    OperationDeleteEntity? delete = operationRepository.GetDeleteByStepId(step.StepId);
                    if (delete != null)
                    {
                        operationRepository.DeleteDelete(delete);
                    }
                    break;
                case OperationName.Clrbuf:
                    OperationClrbufEntity? clrbuf = operationRepository.GetClrbufByStepId(step.StepId);
                    if (clrbuf != null)
                    {
                        operationRepository.DeleteClrbuf(clrbuf);
                    }
                    break;
                default:
                    break;
            }
        }
        return stepRepository.EditStep(step);
    }

    public List<TaskStepEntity> GetAllSteps()
    {
        return stepRepository.GetAllSteps();
    }

    public List<TaskStepEntity> GetAllStepsByTaskId(string taskId)
    {
        return stepRepository.GetAllStepsByTaskId(taskId);
    }

    public TaskStepEntity? GetStepByStepId(int stepId)
    {
        return stepRepository.GetStepByStepId(stepId);
    }

    public TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber)
    {
        return stepRepository.GetStepByTaskId(taskId, stepNumber);
    }

    public bool ReplaceSteps(string taskId, string numberStep, string operation)
    {
        try
        {
            List<TaskStepEntity> steps = stepRepository.GetAllStepsByTaskId(taskId)
                                                        .OrderBy(x => x.StepNumber)
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

            return stepRepository.UpdateRangeSteps(steps);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CopyStep(int stepId, int newNumber)
    {
        var step = GetStepByStepId(stepId);
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
        CreateStep(newStep);
        if (step.OperationId != 0)
        {
            int newOperationId = 0;
            int qwe = newStep.StepId;
            switch (step.OperationName)
            {
                case OperationName.Copy:
                    OperationCopyEntity? oldCopy = operationService.GetCopyByStepId(step.StepId);
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
                        operationService.CreateCopy(newCopy);
                        newOperationId = newCopy.OperationId;
                    }
                    break;
                case OperationName.Move:
                    OperationMoveEntity? oldMove = operationRepository.GetMoveByStepId(step.StepId);
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
                        operationRepository.CreateMove(newMove);
                        newOperationId = newMove.OperationId;
                    }
                    break;
                case OperationName.Read:
                    OperationReadEntity? oldRead = operationRepository.GetReadByStepId(step.StepId);
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
                        operationRepository.CreateRead(newRead);
                        newOperationId = newRead.OperationId;
                    }
                    break;
                case OperationName.Exist:
                    OperationExistEntity? oldExist = operationRepository.GetExistByStepId(step.StepId);
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
                        operationRepository.CreateExist(newExist);
                        newOperationId = newExist.OperationId;
                    }
                    break;
                case OperationName.Rename:
                    OperationRenameEntity? oldRename = operationRepository.GetRenameByStepId(step.StepId);
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
                        operationRepository.CreateRename(newRename);
                        newOperationId = newRename.OperationId;
                    }
                    break;
                case OperationName.Delete:
                    OperationDeleteEntity? oldDelete = operationRepository.GetDeleteByStepId(step.StepId);
                    if (oldDelete != null)
                    {
                        OperationDeleteEntity newDelete = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldDelete.InformSuccess,
                            AddresseeGroupId = oldDelete.AddresseeGroupId,
                            AdditionalText = oldDelete.AdditionalText
                        };
                        operationRepository.CreateDelete(newDelete);
                        newOperationId = newDelete.OperationId;
                    }
                    break;
                case OperationName.Clrbuf:
                    OperationClrbufEntity? oldClrbuf = operationRepository.GetClrbufByStepId(step.StepId);
                    if (oldClrbuf != null)
                    {
                        OperationClrbufEntity newClrbuf = new()
                        {
                            StepId = newStep.StepId,
                            InformSuccess = oldClrbuf.InformSuccess,
                            AddresseeGroupId = oldClrbuf.AddresseeGroupId,
                            AdditionalText = oldClrbuf.AdditionalText
                        };
                        operationRepository.CreateClrbuf(newClrbuf);
                        newOperationId = newClrbuf.OperationId;
                    }
                    break;
            }
            newStep.OperationId = newOperationId;
            EditStep(newStep);
        }
        return true;
    }

    
}
