using FileManager.Core.Entities;
using FileManager.Core.ViewModels;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskService
{
    Task<List<TaskEntity>> GetAllTasks();
    Task<TaskEntity> GetTaskById(string idTask);
    Task<TaskEntity> CreateTask(TaskEntity task);
    Task<bool> EditTask(TaskEntity task);
    Task<bool> DeleteTask(string idTask);
    Task<List<TaskEntity>> GetTasksByGroup(string nameGroup);
    Task<List<TaskGroupEntity>> GetAllGroups();
    Task<bool> UpdateLastModifiedTask(string idTask);
    Task<TaskGroupEntity?> CreateTaskGroup(string name);
    Task<bool> DeleteTaskGroup(int idGroup);
    Task<bool> ActivatedTask(string idTask);
    Task<bool> CopyTask(string idTask, string newIdTask, string isCopySteps,
                                    List<CopyStepViewModel> copyStep);
    Task<bool> CreateTaskStatuse(string idTask);
    Task<List<TaskStatusEntity>> GetTaskStatuses();
    Task<bool> UpdateTaskStatus(TaskStatusEntity taskStatus);
}
