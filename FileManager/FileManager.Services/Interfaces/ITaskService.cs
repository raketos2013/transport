using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Interfaces
{
    public interface ITaskService
    {
		List<TaskEntity> GetAllTasks();
		TaskEntity GetTaskById(string idTask);
		bool CreateTask(TaskEntity task);
		bool UpdateTask(TaskEntity task);
		bool DeleteTask(string idTask);
		List<TaskEntity> GetTasksByGroup(string nameGroup);
		List<TaskGroupEntity> GetAllGroups();
	}
}
