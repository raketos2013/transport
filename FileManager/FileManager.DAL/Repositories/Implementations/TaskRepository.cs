using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _appDbContext;
		public TaskRepository(AppDbContext dbContext)
		{
			_appDbContext = dbContext;
		}

		public bool CreateTask(TaskEntity task)
		{
			try
			{
                task.LastModified = DateTime.Now;
                _appDbContext.Task.Add(task);
                TaskStatusEntity taskStatus = new TaskStatusEntity();
                taskStatus.TaskId = task.TaskId;
                _appDbContext.TaskStatuse.Add(taskStatus);
				_appDbContext.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool DeleteTask(string idTask)
		{
			throw new NotImplementedException();
		}

		public List<TaskGroupEntity> GetAllGroups()
		{
			return _appDbContext.TaskGroup.ToList();
		}

		public List<TaskEntity> GetAllTasks()
		{
			return _appDbContext.Task.ToList();
		}

		public TaskEntity GetTaskById(string idTask)
		{
			return _appDbContext.Task.FirstOrDefault(x => x.TaskId == idTask);
		}

		public TaskGroupEntity GetTaskGroupByName(string groupName)
		{
			return _appDbContext.TaskGroup.FirstOrDefault(x => x.Name == groupName);
		}

		public List<TaskEntity> GetTasksByGroup(int idGroup)
		{
			return _appDbContext.Task.Where(x => x.TaskGroupId == idGroup).ToList();
		}

		public bool EditTask(TaskEntity task)
		{

			TaskEntity? oldTask = _appDbContext.Task.FirstOrDefault(x => x.TaskId == task.TaskId);
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
				_appDbContext.Task.Update(oldTask);
				_appDbContext.SaveChanges();
			}
			return false;
		}

        public bool UpdateLastModifiedTask(string idTask)
        {
			try
			{
				TaskEntity? task = _appDbContext.Task.FirstOrDefault(x =>x.TaskId == idTask);
				if (task != null) 
				{
					task.LastModified = DateTime.Now;
                    _appDbContext.Task.Update(task);
                    _appDbContext.SaveChanges();
                    return true;
                }
				return false;
				
			}
			catch (Exception)
			{
				return false;
			}
        }

		public bool CreateTaskGroup(string name)
		{
			try
			{
				TaskGroupEntity? taskGroups = _appDbContext.TaskGroup.FirstOrDefault(x => x.Name == name);
				if (taskGroups == null)
				{
					TaskGroupEntity taskGroupEntity = new TaskGroupEntity();
					taskGroupEntity.Name = name;
					_appDbContext.TaskGroup.Add(taskGroupEntity);
					_appDbContext.SaveChanges();
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool DeleteTaskGroup(int idGroup)
		{
			try
			{
				List<TaskEntity> tasks = _appDbContext.Task.Where(x => x.TaskGroupId == idGroup).ToList();
				foreach (var task in tasks)
				{
					task.TaskGroupId = 0;
				}
				_appDbContext.SaveChanges();
				TaskGroupEntity? taskGroup = _appDbContext.TaskGroup.FirstOrDefault(x => x.Id == idGroup);
				if (taskGroup != null)
				{
					_appDbContext.TaskGroup.Remove(taskGroup);
					_appDbContext.SaveChanges();
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool ActivatedTask(string idTask)
		{
			try
			{
				TaskEntity? task = _appDbContext.Task.FirstOrDefault(x => x.TaskId == idTask);
				if (task != null)
				{
					task.IsActive = !task.IsActive;
					task.LastModified = DateTime.Now;
					_appDbContext.Task.Update(task);
					_appDbContext.SaveChanges();
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

        public bool CreateTaskStatuse(string idTask)
		{
			try
			{
                TaskStatusEntity taskStatuse = new TaskStatusEntity();
                taskStatuse.TaskId = idTask;
                _appDbContext.TaskStatuse.Add(taskStatuse);
                _appDbContext.SaveChanges();
                return true;
			}
			catch (Exception)
			{
				return false;
			}
        }

    }
}
