using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<List<TaskEntity>> GetAllTasks();
    Task<TaskEntity> GetTaskById(string idTask);
    Task<TaskEntity> CreateTask(TaskEntity task);
    Task<bool> EditTask(TaskEntity task);
    Task<bool> DeleteTask(string idTask);
    Task<List<TaskEntity>> GetTasksByGroup(int idGroup);
    Task<TaskGroupEntity> GetTaskGroupByName(string groupName);
    Task<List<TaskGroupEntity>> GetAllGroups();
    Task<bool> UpdateLastModifiedTask(string idTask);
    Task<TaskGroupEntity?> CreateTaskGroup(string name);
    Task<bool> DeleteTaskGroup(int idGroup);
    Task<bool> ActivatedTask(string idTask);
    Task<bool> CreateTaskStatuse(string idTask);

    Task<List<TaskStatusEntity>> GetTaskStatuses();
    Task<bool> UpdateTaskStatus(TaskStatusEntity taskStatus);
}
