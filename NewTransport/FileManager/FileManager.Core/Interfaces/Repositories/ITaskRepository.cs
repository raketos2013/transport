using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<List<TaskEntity>> GetAllTasks();
    Task<TaskEntity?> GetTaskById(string idTask);
    Task<TaskEntity> CreateTask(TaskEntity task);
    Task<TaskEntity> EditTask(TaskEntity task);
    Task<bool> DeleteTask(string idTask);
    Task<TaskStatusEntity> CreateTaskStatus(string idTask);
    Task<List<TaskStatusEntity>> GetTaskStatuses();
    TaskStatusEntity UpdateTaskStatus(TaskStatusEntity taskStatus);
}
