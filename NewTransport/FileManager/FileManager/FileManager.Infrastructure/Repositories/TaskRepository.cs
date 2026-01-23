using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class TaskRepository(AppDbContext dbContext) : ITaskRepository
{
    public async Task<TaskEntity> CreateTask(TaskEntity task)
    {
        task.LastModified = DateTime.Now;
        await dbContext.Task.AddAsync(task);
        await CreateTaskStatus(task.TaskId);
        return task;
    }

    public async Task<bool> DeleteTask(string idTask)
    {
        try
        {
            var task = await GetTaskById(idTask);
            if (task == null)
            {
                return false;
            }
            dbContext.Task.Remove(task);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<TaskEntity>> GetAllTasks()
    {
        return await dbContext.Task.ToListAsync();
    }

    public async Task<TaskEntity?> GetTaskById(string idTask)
    {
        return await dbContext.Task.FirstOrDefaultAsync(x => x.TaskId == idTask);
    }

    public async Task<TaskEntity> EditTask(TaskEntity task)
    {
        TaskEntity editedTask = await dbContext.Task.FirstOrDefaultAsync(x => x.TaskId == task.TaskId)
                                        ?? throw new DomainException("Задача с таким Id не найдена");
        editedTask.Name = task.Name;
        editedTask.TaskGroupId = task.TaskGroupId;
        editedTask.AddresseeGroupId = task.AddresseeGroupId;
        editedTask.IsActive = task.IsActive;
        editedTask.DayActive = task.DayActive;
        editedTask.ExecutionLimit = task.ExecutionLimit;
        editedTask.TimeEnd = task.TimeEnd;
        editedTask.TimeBegin = task.TimeBegin;
        editedTask.LastModified = DateTime.Now;
        dbContext.Task.Update(editedTask);
        return editedTask;
    }

    public async Task<bool> ActivatedTask(string idTask)
    {
        TaskEntity task = await dbContext.Task.FirstOrDefaultAsync(x => x.TaskId == idTask)
                                    ?? throw new DomainException("Задача с таким Id не найдена");
        task.IsActive = !task.IsActive;
        task.LastModified = DateTime.Now;
        dbContext.Task.Update(task);
        return true;
    }

    public async Task<TaskStatusEntity> CreateTaskStatus(string idTask)
    {
        TaskStatusEntity taskStatus = new()
        {
            TaskId = idTask,
            Status = StatusTask.Wait
        };
        await dbContext.TaskStatuse.AddAsync(taskStatus);
        return taskStatus;
    }

    public async Task<List<TaskStatusEntity>> GetTaskStatuses()
    {
        return await dbContext.TaskStatuse.ToListAsync();
    }

    public TaskStatusEntity UpdateTaskStatus(TaskStatusEntity taskStatus)
    {
        dbContext.TaskStatuse.Update(taskStatus);
        return taskStatus;
    }
}
