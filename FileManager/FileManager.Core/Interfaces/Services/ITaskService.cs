using FileManager.Core.Entities;
using FileManager.Core.ViewModels;

namespace FileManager.Core.Interfaces.Services;

public interface ITaskService
{
    Task<List<TaskEntity>> GetAllTasks();
    Task<TaskEntity?> GetTaskById(string idTask);
    Task<TaskEntity> CreateTask(TaskEntity task);
    Task<TaskEntity> EditTask(TaskEntity task);
    Task<bool> DeleteTask(string idTask);
    Task<TaskEntity> ActivatedTask(string idTask);
    Task<TaskEntity?> CopyTask(string idTask, string newIdTask, string isCopySteps,
                                    List<CopyStepViewModel> copyStep, bool isActivate);
    Task<TaskStatusEntity> CreateTaskStatus(string idTask);
    Task<List<TaskStatusEntity>> GetTaskStatuses();
    Task<TaskStatusEntity> UpdateTaskStatus(TaskStatusEntity taskStatus);
}
