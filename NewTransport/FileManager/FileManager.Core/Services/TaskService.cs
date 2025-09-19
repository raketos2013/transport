using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileManager.Core.Services;

public class TaskService(ITaskRepository taskRepository,
                            IStepRepository stepRepository,
                            IOperationRepository operationRepository,
                            IUserLogService userLogService,
                            IHttpContextAccessor httpContextAccessor,
                            IUnitOfWork unitOfWork
                            )
            : ITaskService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    public async Task<TaskEntity> CreateTask(TaskEntity task)
    {
        task.ExecutionLeft = 1;
        task.ExecutionLimit = 1;
        var taskEntity = await unitOfWork.TaskRepository.CreateTask(task);
        await unitOfWork.SaveAsync();
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, 
                                    "Создание задачи", JsonSerializer.Serialize(taskEntity, _options));
        return taskEntity;
    }

    public async Task<bool> DeleteTask(string idTask)
    {
        var deletedSteps = await stepRepository.DeleteStepsByTaskId(idTask);
        var deletedTask = await taskRepository.DeleteTask(idTask);
        return deletedSteps && deletedTask;
    }

    public async Task<List<TaskGroupEntity>> GetAllGroups()
    {
        return await taskRepository.GetAllGroups();
    }

    public async Task<List<TaskEntity>> GetAllTasks()
    {
        return await taskRepository.GetAllTasks();
    }

    public async Task<TaskEntity> GetTaskById(string idTask)
    {
        return await taskRepository.GetTaskById(idTask);
    }

    public async Task<List<TaskEntity>> GetTasksByGroup(string nameGroup)
    {
        //List<TaskEntity> tasks = [];
        //TaskGroupEntity taskGroup = await taskRepository.GetTaskGroupByName(nameGroup);
        //if (nameGroup == "Все")
        //{
        //    tasks = await taskRepository.GetAllTasks()
        //                            .OrderByDescending(x => x.IsActive)
        //                            .ThenBy(x => x.TaskId)
        //                            .ToList();
        //}
        //else
        //{
        //    tasks = taskRepository.GetTasksByGroup(taskGroup.Id)
        //                            .OrderByDescending(x => x.IsActive)
        //                            .ThenBy(x => x.TaskId)
        //                            .ToList();
        //}
        //return tasks;
        throw new NotImplementedException();
    }

    public async Task<bool> EditTask(TaskEntity task)
    {
        bool edited = await taskRepository.EditTask(task);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"Изменение задачи {task.TaskId}", JsonSerializer.Serialize(task, _options));
        return edited;
    }

    public async Task<bool> UpdateLastModifiedTask(string idTask)
    { 
        return await taskRepository.UpdateLastModifiedTask(idTask);
    }

    public async Task<TaskGroupEntity?> CreateTaskGroup(string name)
    {
        var taskGroup = await taskRepository.CreateTaskGroup(name);
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"Создание группы задач {name}", JsonSerializer.Serialize(taskGroup, _options));
        return taskGroup;
    }

    public async Task<bool> DeleteTaskGroup(int idGroup)
    {
        return await taskRepository.DeleteTaskGroup(idGroup);
    }

    public async Task<bool> ActivatedTask(string idTask)
    {
        var result = await taskRepository.ActivatedTask(idTask);
        var task = await taskRepository.GetTaskById(idTask);
        var text = task.IsActive ? "Включение" : "Выключение";
        await userLogService.AddLog(httpContextAccessor.HttpContext.User.Identity.Name, $"{text} задачи {idTask}", JsonSerializer.Serialize(task, _options));
        return result;
    }

    public async Task<bool> CopyTask(string idTask, string newIdTask, string isCopySteps, List<CopyStepViewModel> copyStep)
    {
        TaskEntity copiedTask = await GetTaskById(idTask);
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
        await CreateTask(newTask);

        if (isCopySteps == "True")
        {
            var stepsAsync = await stepRepository.GetAllStepsByTaskId(idTask);
            var steps = stepsAsync.OrderBy(x => x.StepNumber)
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
                        await stepRepository.CreateStep(newStep);
                        i++;
                        if (item.IsCopyOperation)
                        {
                            switch (newStep.OperationName)
                            {
                                case OperationName.Copy:
                                    OperationCopyEntity newCopy = new();
                                    OperationCopyEntity? oldCopy = await operationRepository.GetCopyByStepId(oldStep.StepId);
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
                                        await operationRepository.CreateCopy(newCopy);
                                    }
                                    break;
                                case OperationName.Move:
                                    OperationMoveEntity newMove = new();
                                    OperationMoveEntity? oldMove = await operationRepository.GetMoveByStepId(oldStep.StepId);
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
                                        await operationRepository.CreateMove(newMove);
                                    }
                                    break;
                                case OperationName.Read:
                                    OperationReadEntity newRead = new();
                                    OperationReadEntity? oldRead = await operationRepository.GetReadByStepId(oldStep.StepId);
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
                                        await operationRepository.CreateRead(newRead);
                                    }
                                    break;
                                case OperationName.Exist:
                                    OperationExistEntity newExist = new();
                                    OperationExistEntity? oldExist = await operationRepository.GetExistByStepId(oldStep.StepId);
                                    if (oldExist != null)
                                    {
                                        newExist.StepId = newStep.StepId;
                                        newExist.InformSuccess = oldExist.InformSuccess;
                                        newExist.AddresseeGroupId = oldExist.AddresseeGroupId;
                                        newExist.AdditionalText = oldExist.AdditionalText;
                                        newExist.ExpectedResult = oldExist.ExpectedResult;
                                        newExist.BreakTaskAfterError = oldExist.BreakTaskAfterError;
                                        await operationRepository.CreateExist(newExist);
                                    }
                                    break;
                                case OperationName.Rename:
                                    OperationRenameEntity newRename = new();
                                    OperationRenameEntity? oldRename = await operationRepository.GetRenameByStepId(oldStep.StepId);
                                    if (oldRename != null)
                                    {
                                        newRename.StepId = newStep.StepId;
                                        newRename.InformSuccess = oldRename.InformSuccess;
                                        newRename.AddresseeGroupId = oldRename.AddresseeGroupId;
                                        newRename.AdditionalText = oldRename.AdditionalText;
                                        newRename.OldPattern = oldRename.OldPattern;
                                        newRename.NewPattern = oldRename.NewPattern;
                                        await operationRepository.CreateRename(newRename);
                                    }
                                    break;
                                case OperationName.Delete:
                                    OperationDeleteEntity newDelete = new();
                                    OperationDeleteEntity? oldDelete = await operationRepository.GetDeleteByStepId(oldStep.StepId);
                                    if (oldDelete != null)
                                    {
                                        newDelete.StepId = newStep.StepId;
                                        newDelete.InformSuccess = oldDelete.InformSuccess;
                                        newDelete.AddresseeGroupId = oldDelete.AddresseeGroupId;
                                        newDelete.AdditionalText = oldDelete.AdditionalText;
                                        await operationRepository.CreateDelete(newDelete);
                                    }
                                    break;
                                case OperationName.Clrbuf:
                                    OperationClrbufEntity newClrbuf = new();
                                    OperationClrbufEntity? oldClrbuf = await operationRepository.GetClrbufByStepId(oldStep.StepId);
                                    if (oldClrbuf != null)
                                    {
                                        newClrbuf.StepId = newStep.StepId;
                                        newClrbuf.InformSuccess = oldClrbuf.InformSuccess;
                                        newClrbuf.AddresseeGroupId = oldClrbuf.AddresseeGroupId;
                                        newClrbuf.AdditionalText = oldClrbuf.AdditionalText;
                                        await operationRepository.CreateClrbuf(newClrbuf);
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

    public async Task<bool> CreateTaskStatuse(string idTask)
    {
        return await taskRepository.CreateTaskStatuse(idTask);
    }

    public async Task<List<TaskStatusEntity>> GetTaskStatuses()
    {
        return await taskRepository.GetTaskStatuses();
    }

    public async Task<bool> UpdateTaskStatus(TaskStatusEntity taskStatus)
    {
        return await taskRepository.UpdateTaskStatus(taskStatus);
    }

}
