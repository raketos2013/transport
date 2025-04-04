using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				_appDbContext.Task.Add(task);
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

		public bool UpdateTask(TaskEntity task)
		{
			throw new NotImplementedException();
		}
	}
}
