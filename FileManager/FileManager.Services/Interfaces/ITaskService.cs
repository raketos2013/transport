using FileManager.Domain.Entity;
using FileManager.Domain.ViewModels.Step;


namespace FileManager.Services.Interfaces;

public interface ITaskService
{
    List<TaskEntity> GetAllTasks();
    TaskEntity GetTaskById(string idTask);
    TaskEntity CreateTask(TaskEntity task);
    bool EditTask(TaskEntity task);
    bool DeleteTask(string idTask);
    List<TaskEntity> GetTasksByGroup(string nameGroup);
    List<TaskGroupEntity> GetAllGroups();
    bool UpdateLastModifiedTask(string idTask);
    TaskGroupEntity? CreateTaskGroup(string name);
    bool DeleteTaskGroup(int idGroup);
    bool ActivatedTask(string idTask);
    bool CopyTask(string idTask, string newIdTask, string isCopySteps,
                                    List<CopyStepViewModel> copyStep);

    bool CreateTaskStatuse(string idTask);
}
