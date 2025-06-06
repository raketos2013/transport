using FileManager.Domain.Entity;


namespace FileManager.DAL.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        List<TaskEntity> GetAllTasks();
        TaskEntity GetTaskById(string idTask);
        TaskEntity CreateTask(TaskEntity task);
        bool EditTask(TaskEntity task);
        bool DeleteTask(string idTask);
        List<TaskEntity> GetTasksByGroup(int idGroup);
        TaskGroupEntity GetTaskGroupByName(string groupName);
        List<TaskGroupEntity> GetAllGroups();
        bool UpdateLastModifiedTask(string idTask);
        TaskGroupEntity? CreateTaskGroup(string name);
        bool DeleteTaskGroup(int idGroup);
        bool ActivatedTask(string idTask);
        bool CreateTaskStatuse(string idTask);

        List<TaskStatusEntity> GetTaskStatuses();
    }
}
