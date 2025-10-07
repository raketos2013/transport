using FileManager.Core.Constants;
using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;

namespace FileManager.Core.Services;

public class TaskService(IUnitOfWork unitOfWork)
            : ITaskService
{
    public async Task<TaskEntity> CreateTask(TaskEntity task)
    {
        task.ExecutionLeft = 1;
        task.ExecutionLimit = 1;
        var createdTask = await unitOfWork.TaskRepository.CreateTask(task);
        return await unitOfWork.SaveAsync() > 0 ? createdTask
                        : throw new DomainException("Ошибка создания задачи");
    }

    public async Task<bool> DeleteTask(string idTask)
    {
        var deletedSteps = await unitOfWork.StepRepository.DeleteStepsByTaskId(idTask);
        var deletedTask = await unitOfWork.TaskRepository.DeleteTask(idTask);
        return await unitOfWork.SaveAsync() > 0 &&
                    deletedSteps &&
                    deletedTask;
    }

    public async Task<List<TaskEntity>> GetAllTasks()
    {
        return await unitOfWork.TaskRepository.GetAllTasks();
    }

    public async Task<TaskEntity?> GetTaskById(string idTask)
    {
        return await unitOfWork.TaskRepository.GetTaskById(idTask);
    }

    public async Task<TaskEntity> EditTask(TaskEntity task)
    {
        var editedTask = await unitOfWork.TaskRepository.EditTask(task);
        return await unitOfWork.SaveAsync() > 0 ? editedTask
                        : throw new DomainException("Ошибка обновления задачи");
    }

    public async Task<TaskEntity> ActivatedTask(string idTask)
    {
        var task = await unitOfWork.TaskRepository.GetTaskById(idTask)
                                ?? throw new DomainException("Задача с таким Id не найдена");
        task.IsActive = !task.IsActive;
        var editedTask = await EditTask(task);
        return editedTask;
    }

    public async Task<TaskEntity?> CopyTask(string idTask, string newIdTask, string isCopySteps, List<CopyStepViewModel> copyStep)
    {
        var copiedTask = await GetTaskById(idTask);
        if (copiedTask == null)
        {
            return null;
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
            var stepsAsync = await unitOfWork.StepRepository.GetAllStepsByTaskId(idTask);
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
                        await unitOfWork.StepRepository.CreateStep(newStep);
                        i++;
                        if (item.IsCopyOperation)
                        {
                            switch (newStep.OperationName)
                            {
                                case OperationName.Copy:
                                    OperationCopyEntity newCopy = new();
                                    OperationCopyEntity? oldCopy = await unitOfWork.OperationRepository.GetCopyByStepId(oldStep.StepId);
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
                                        await unitOfWork.OperationRepository.CreateCopy(newCopy);
                                    }
                                    break;
                                case OperationName.Move:
                                    OperationMoveEntity newMove = new();
                                    OperationMoveEntity? oldMove = await unitOfWork.OperationRepository.GetMoveByStepId(oldStep.StepId);
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
                                        await unitOfWork.OperationRepository.CreateMove(newMove);
                                    }
                                    break;
                                case OperationName.Read:
                                    OperationReadEntity newRead = new();
                                    OperationReadEntity? oldRead = await unitOfWork.OperationRepository.GetReadByStepId(oldStep.StepId);
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
                                        await unitOfWork.OperationRepository.CreateRead(newRead);
                                    }
                                    break;
                                case OperationName.Exist:
                                    OperationExistEntity newExist = new();
                                    OperationExistEntity? oldExist = await unitOfWork.OperationRepository.GetExistByStepId(oldStep.StepId);
                                    if (oldExist != null)
                                    {
                                        newExist.StepId = newStep.StepId;
                                        newExist.InformSuccess = oldExist.InformSuccess;
                                        newExist.AddresseeGroupId = oldExist.AddresseeGroupId;
                                        newExist.AdditionalText = oldExist.AdditionalText;
                                        newExist.ExpectedResult = oldExist.ExpectedResult;
                                        newExist.BreakTaskAfterError = oldExist.BreakTaskAfterError;
                                        await unitOfWork.OperationRepository.CreateExist(newExist);
                                    }
                                    break;
                                case OperationName.Rename:
                                    OperationRenameEntity newRename = new();
                                    OperationRenameEntity? oldRename = await unitOfWork.OperationRepository.GetRenameByStepId(oldStep.StepId);
                                    if (oldRename != null)
                                    {
                                        newRename.StepId = newStep.StepId;
                                        newRename.InformSuccess = oldRename.InformSuccess;
                                        newRename.AddresseeGroupId = oldRename.AddresseeGroupId;
                                        newRename.AdditionalText = oldRename.AdditionalText;
                                        newRename.OldPattern = oldRename.OldPattern;
                                        newRename.NewPattern = oldRename.NewPattern;
                                        await unitOfWork.OperationRepository.CreateRename(newRename);
                                    }
                                    break;
                                case OperationName.Delete:
                                    OperationDeleteEntity newDelete = new();
                                    OperationDeleteEntity? oldDelete = await unitOfWork.OperationRepository.GetDeleteByStepId(oldStep.StepId);
                                    if (oldDelete != null)
                                    {
                                        newDelete.StepId = newStep.StepId;
                                        newDelete.InformSuccess = oldDelete.InformSuccess;
                                        newDelete.AddresseeGroupId = oldDelete.AddresseeGroupId;
                                        newDelete.AdditionalText = oldDelete.AdditionalText;
                                        await unitOfWork.OperationRepository.CreateDelete(newDelete);
                                    }
                                    break;
                                case OperationName.Clrbuf:
                                    OperationClrbufEntity newClrbuf = new();
                                    OperationClrbufEntity? oldClrbuf = await unitOfWork.OperationRepository.GetClrbufByStepId(oldStep.StepId);
                                    if (oldClrbuf != null)
                                    {
                                        newClrbuf.StepId = newStep.StepId;
                                        newClrbuf.InformSuccess = oldClrbuf.InformSuccess;
                                        newClrbuf.AddresseeGroupId = oldClrbuf.AddresseeGroupId;
                                        newClrbuf.AdditionalText = oldClrbuf.AdditionalText;
                                        await unitOfWork.OperationRepository.CreateClrbuf(newClrbuf);
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                }
            }
            await unitOfWork.SaveAsync();
        }
        return newTask;
    }

    public async Task<TaskStatusEntity> CreateTaskStatus(string idTask)
    {
        var createdStatus = await unitOfWork.TaskRepository.CreateTaskStatus(idTask);
        return await unitOfWork.SaveAsync() > 0 ? createdStatus
                            : throw new DomainException("Ошибка создания статуса задачи");
    }

    public async Task<List<TaskStatusEntity>> GetTaskStatuses()
    {
        return await unitOfWork.TaskRepository.GetTaskStatuses();
    }

    public async Task<TaskStatusEntity> UpdateTaskStatus(TaskStatusEntity taskStatus)
    {
        var editedStatus = unitOfWork.TaskRepository.UpdateTaskStatus(taskStatus);
        return await unitOfWork.SaveAsync() > 0 ? editedStatus
                            : throw new DomainException("Ошибка обновления статуса задачи");
    }

}