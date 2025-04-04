using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        List<TaskEntity> GetAllTasks();
        TaskEntity GetTaskById(string idTask);
        bool CreateTask(TaskEntity task);
        bool UpdateTask(TaskEntity task);
        bool DeleteTask(string idTask);
        List<TaskEntity> GetTasksByGroup(int idGroup);
        TaskGroupEntity GetTaskGroupByName(string groupName);
        List<TaskGroupEntity> GetAllGroups();

    }
}
