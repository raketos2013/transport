using FileManager.Core.Entities;
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
        await dbContext.SaveChangesAsync();
        await CreateTaskStatuse(task.TaskId);
        return task;
    }

    public async Task<bool> DeleteTask(string idTask)
    {
        try
        {
            var task = await GetTaskById(idTask);
            dbContext.Task.Remove(task);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<TaskGroupEntity>> GetAllGroups()
    {
        return await dbContext.TaskGroup.ToListAsync();
    }

    public async Task<List<TaskEntity>> GetAllTasks()
    {
        return await dbContext.Task.ToListAsync();
    }

    public async Task<TaskEntity> GetTaskById(string idTask)
    {
        return await dbContext.Task.FirstOrDefaultAsync(x => x.TaskId == idTask);
    }

    public async Task<TaskGroupEntity> GetTaskGroupByName(string groupName)
    {
        return await dbContext.TaskGroup.FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<List<TaskEntity>> GetTasksByGroup(int idGroup)
    {
        return await dbContext.Task.Where(x => x.TaskGroupId == idGroup).ToListAsync();
    }

    public async Task<bool> EditTask(TaskEntity task)
    {

        TaskEntity? oldTask = await dbContext.Task.FirstOrDefaultAsync(x => x.TaskId == task.TaskId);
        if (oldTask != null)
        {
            oldTask.Name = task.Name;
            oldTask.TaskGroupId = task.TaskGroupId;
            oldTask.AddresseeGroupId = task.AddresseeGroupId;
            oldTask.IsActive = task.IsActive;
            oldTask.DayActive = task.DayActive;
            oldTask.ExecutionLimit = task.ExecutionLimit;
            oldTask.TimeEnd = task.TimeEnd;
            oldTask.TimeBegin = task.TimeBegin;
            oldTask.LastModified = DateTime.Now;
            dbContext.Task.Update(oldTask);
            await dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateLastModifiedTask(string idTask)
    {
        try
        {
            TaskEntity? task = await dbContext.Task.FirstOrDefaultAsync(x => x.TaskId == idTask);
            if (task != null)
            {
                task.LastModified = DateTime.Now;
                dbContext.Task.Update(task);
                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;

        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<TaskGroupEntity?> CreateTaskGroup(string name)
    {
        try
        {
            TaskGroupEntity? taskGroups = await dbContext.TaskGroup.FirstOrDefaultAsync(x => x.Name == name);
            if (taskGroups == null)
            {
                TaskGroupEntity taskGroupEntity = new()
                {
                    Name = name
                };
                await dbContext.TaskGroup.AddAsync(taskGroupEntity);
                await dbContext.SaveChangesAsync();
                return taskGroupEntity;
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> DeleteTaskGroup(int idGroup)
    {
        try
        {
            List<TaskEntity> tasks = await dbContext.Task.Where(x => x.TaskGroupId == idGroup).ToListAsync();
            foreach (var task in tasks)
            {
                task.TaskGroupId = 0;
            }
            await dbContext.SaveChangesAsync();
            TaskGroupEntity? taskGroup = await dbContext.TaskGroup.FirstOrDefaultAsync(x => x.Id == idGroup);
            if (taskGroup != null)
            {
                dbContext.TaskGroup.Remove(taskGroup);
                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ActivatedTask(string idTask)
    {
        try
        {
            TaskEntity? task = await dbContext.Task.FirstOrDefaultAsync(x => x.TaskId == idTask);
            if (task != null)
            {
                task.IsActive = !task.IsActive;
                task.LastModified = DateTime.Now;
                dbContext.Task.Update(task);
                await dbContext.SaveChangesAsync();
            }
            //if (task.IsActive)
            //{
            //	_userLogging.Logging(HttpContext.User.Identity.Name, $"Включение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
            //}
            //else
            //{
            //	_userLogging.Logging(HttpContext.User.Identity.Name, $"Выключение задачи: {task.TaskId}", JsonSerializer.Serialize(task));
            //}
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CreateTaskStatuse(string idTask)
    {
        try
        {
            TaskStatusEntity taskStatuse = new()
            {
                TaskId = idTask
            };
            await dbContext.TaskStatuse.AddAsync(taskStatuse);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<TaskStatusEntity>> GetTaskStatuses()
    {
        return await dbContext.TaskStatuse.ToListAsync();
    }

    public async Task<bool> UpdateTaskStatus(TaskStatusEntity taskStatus)
    {
        try
        {
            dbContext.TaskStatuse.Update(taskStatus);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
