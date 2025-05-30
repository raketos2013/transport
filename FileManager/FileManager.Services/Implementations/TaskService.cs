using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.ViewModels.Step;
using FileManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileManager.Services.Implementations;

public class TaskService(ITaskRepository taskRepository,
                            IStepRepository stepRepository,
                            IOperationRepository operationRepository,
                            IUserLogService userLogService,
                            IHttpContextAccessor httpContextAccessor
                            )
            : ITaskService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    public TaskEntity CreateTask(TaskEntity task)
    {
        TaskEntity taskEntity = taskRepository.CreateTask(task);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, "Создание задачи", JsonSerializer.Serialize(taskEntity, _options));
        return taskEntity;
    }

    public bool DeleteTask(string idTask)
    {
        return taskRepository.DeleteTask(idTask);
    }

    public List<TaskGroupEntity> GetAllGroups()
    {
        return taskRepository.GetAllGroups();
    }

    public List<TaskEntity> GetAllTasks()
    {
        return taskRepository.GetAllTasks();
    }

    public TaskEntity GetTaskById(string idTask)
    {
        return taskRepository.GetTaskById(idTask);
    }

    public List<TaskEntity> GetTasksByGroup(string nameGroup)
    {
        List<TaskEntity> tasks = [];
        TaskGroupEntity taskGroup = taskRepository.GetTaskGroupByName(nameGroup);
        if (nameGroup == "Все")
        {
            tasks = taskRepository.GetAllTasks()
                                    .OrderByDescending(x => x.IsActive)
                                    .ThenBy(x => x.TaskId)
                                    .ToList();
        }
        else
        {
            tasks = taskRepository.GetTasksByGroup(taskGroup.Id)
                                    .OrderByDescending(x => x.IsActive)
                                    .ThenBy(x => x.TaskId)
                                    .ToList();
        }
        return tasks;
    }

    public bool EditTask(TaskEntity task)
    {
        bool edited = taskRepository.EditTask(task);
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, "Создание задачи", JsonSerializer.Serialize(task, _options));
        return edited;
    }

    public bool UpdateLastModifiedTask(string idTask)
    {
        return taskRepository.UpdateLastModifiedTask(idTask);
    }

    public TaskGroupEntity? CreateTaskGroup(string name)
    {
        return taskRepository.CreateTaskGroup(name);
    }

    public bool DeleteTaskGroup(int idGroup)
    {
        return taskRepository.DeleteTaskGroup(idGroup);
    }

    public bool ActivatedTask(string idTask)
    {
        var result = taskRepository.ActivatedTask(idTask);
        var task = taskRepository.GetTaskById(idTask);
        var text = task.IsActive ? "Включение" : "Выключение";
        userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"{text} задачи", JsonSerializer.Serialize(task, _options));
        return result;
    }

    public bool CopyTask(string idTask, string newIdTask, string isCopySteps, List<CopyStepViewModel> copyStep)
    {
        TaskEntity copiedTask = GetTaskById(idTask);
        if (copiedTask == null)
        {
            return false;
        }
        TaskEntity newTask = new()
        {
            TaskId = newIdTask,
            Name = copiedTask.Name,
            TimeBegin = copiedTask.TimeBegin,
            TimeEnd = copiedTask.TimeEnd,
            DayActive = copiedTask.DayActive,
            AddresseeGroupId = copiedTask.AddresseeGroupId,
            IsActive = copiedTask.IsActive,
            LastModified = DateTime.Now,
            TaskGroupId = copiedTask.TaskGroupId,
            ExecutionLeft = copiedTask.ExecutionLimit,
            ExecutionLimit = copiedTask.ExecutionLimit,
            IsProgress = copiedTask.IsProgress
        };
        CreateTask(newTask);

        if (isCopySteps == "True")
        {
            List<TaskStepEntity> steps = stepRepository.GetAllStepsByTaskId(idTask)
                                                        .OrderBy(x => x.StepNumber)
                                                        .ToList();
            int i = 1;
            foreach (var item in copyStep)
            {
                if (item.IsCopy)
                {
                    TaskStepEntity? oldStep = steps.FirstOrDefault(x => x.TaskId == idTask &&
                                                                        x.StepNumber == item.StepNumber);
                    if (oldStep != null)
                    {
                        TaskStepEntity newStep = new()
                        {
                            TaskId = newIdTask,
                            StepNumber = i,
                            OperationName = oldStep.OperationName,
                            Description = oldStep.Description,
                            FileMask = oldStep.FileMask,
                            Source = oldStep.Source,
                            Destination = oldStep.Destination,
                            IsBreak = oldStep.IsBreak,
                            IsActive = oldStep.IsActive
                        };
                        stepRepository.CreateStep(newStep);
                        i++;
                        if (item.IsCopyOperation)
                        {
                            switch (newStep.OperationName)
                            {
                                case OperationName.Copy:
                                    OperationCopyEntity newCopy = new();
                                    OperationCopyEntity? oldCopy = operationRepository.GetCopyByStepId(oldStep.StepId);
                                    if (oldCopy != null)
                                    {
                                        newCopy.StepId = newStep.StepId;
                                        newCopy.InformSuccess = oldCopy.InformSuccess;
                                        newCopy.AddresseeGroupId = oldCopy.AddresseeGroupId;
                                        newCopy.AdditionalText = oldCopy.AdditionalText;
                                        newCopy.FileInSource = oldCopy.FileInSource;
                                        newCopy.FileInDestination = oldCopy.FileInDestination;
                                        newCopy.FileInLog = oldCopy.FileInLog;
                                        newCopy.Sort = oldCopy.Sort;
                                        newCopy.FileAttribute = oldCopy.FileAttribute;
                                        operationRepository.CreateCopy(newCopy);
                                    }
                                    break;
                                case OperationName.Move:
                                    OperationMoveEntity newMove = new();
                                    OperationMoveEntity? oldMove = operationRepository.GetMoveByStepId(oldStep.StepId);
                                    if (oldMove != null)
                                    {
                                        newMove.StepId = newStep.StepId;
                                        newMove.InformSuccess = oldMove.InformSuccess;
                                        newMove.AddresseeGroupId = oldMove.AddresseeGroupId;
                                        newMove.AdditionalText = oldMove.AdditionalText;
                                        newMove.FileInDestination = oldMove.FileInDestination;
                                        newMove.FileInLog = oldMove.FileInLog;
                                        newMove.Sort = oldMove.Sort;
                                        newMove.FileAttribute = oldMove.FileAttribute;
                                        operationRepository.CreateMove(newMove);
                                    }
                                    break;
                                case OperationName.Read:
                                    OperationReadEntity newRead = new();
                                    OperationReadEntity? oldRead = operationRepository.GetReadByStepId(oldStep.StepId);
                                    if (oldRead != null)
                                    {
                                        newRead.StepId = newStep.StepId;
                                        newRead.InformSuccess = oldRead.InformSuccess;
                                        newRead.AddresseeGroupId = oldRead.AddresseeGroupId;
                                        newRead.AdditionalText = oldRead.AdditionalText;
                                        newRead.FileInSource = oldRead.FileInSource;
                                        newRead.Encode = oldRead.Encode;
                                        newRead.SearchRegex = oldRead.SearchRegex;
                                        newRead.FindString = oldRead.FindString;
                                        newRead.ExpectedResult = oldRead.ExpectedResult;
                                        newRead.BreakTaskAfterError = oldRead.BreakTaskAfterError;
                                        operationRepository.CreateRead(newRead);
                                    }
                                    break;
                                case OperationName.Exist:
                                    OperationExistEntity newExist = new();
                                    OperationExistEntity? oldExist = operationRepository.GetExistByStepId(oldStep.StepId);
                                    if (oldExist != null)
                                    {
                                        newExist.StepId = newStep.StepId;
                                        newExist.InformSuccess = oldExist.InformSuccess;
                                        newExist.AddresseeGroupId = oldExist.AddresseeGroupId;
                                        newExist.AdditionalText = oldExist.AdditionalText;
                                        newExist.ExpectedResult = oldExist.ExpectedResult;
                                        newExist.BreakTaskAfterError = oldExist.BreakTaskAfterError;
                                        operationRepository.CreateExist(newExist);
                                    }
                                    break;
                                case OperationName.Rename:
                                    OperationRenameEntity newRename = new();
                                    OperationRenameEntity? oldRename = operationRepository.GetRenameByStepId(oldStep.StepId);
                                    if (oldRename != null)
                                    {
                                        newRename.StepId = newStep.StepId;
                                        newRename.InformSuccess = oldRename.InformSuccess;
                                        newRename.AddresseeGroupId = oldRename.AddresseeGroupId;
                                        newRename.AdditionalText = oldRename.AdditionalText;
                                        newRename.OldPattern = oldRename.OldPattern;
                                        newRename.NewPattern = oldRename.NewPattern;
                                        operationRepository.CreateRename(newRename);
                                    }
                                    break;
                                case OperationName.Delete:
                                    OperationDeleteEntity newDelete = new();
                                    OperationDeleteEntity? oldDelete = operationRepository.GetDeleteByStepId(oldStep.StepId);
                                    if (oldDelete != null)
                                    {
                                        newDelete.StepId = newStep.StepId;
                                        newDelete.InformSuccess = oldDelete.InformSuccess;
                                        newDelete.AddresseeGroupId = oldDelete.AddresseeGroupId;
                                        newDelete.AdditionalText = oldDelete.AdditionalText;
                                        operationRepository.CreateDelete(newDelete);
                                    }
                                    break;
                                case OperationName.Clrbuf:
                                    OperationClrbufEntity newClrbuf = new();
                                    OperationClrbufEntity? oldClrbuf = operationRepository.GetClrbufByStepId(oldStep.StepId);
                                    if (oldClrbuf != null)
                                    {
                                        newClrbuf.StepId = newStep.StepId;
                                        newClrbuf.InformSuccess = oldClrbuf.InformSuccess;
                                        newClrbuf.AddresseeGroupId = oldClrbuf.AddresseeGroupId;
                                        newClrbuf.AdditionalText = oldClrbuf.AdditionalText;
                                        operationRepository.CreateClrbuf(newClrbuf);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
        return true;
    }

    public bool CreateTaskStatuse(string idTask)
    {
        return taskRepository.CreateTaskStatuse(idTask);
    }
}
