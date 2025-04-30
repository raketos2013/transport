using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;


namespace FileManager.DAL.Repositories.Implementations
{
    public class TaskRepository(AppDbContext dbContext) : ITaskRepository
    {
        public TaskEntity CreateTask(TaskEntity task)
        {
            task.LastModified = DateTime.Now;
            dbContext.Task.Add(task);
            dbContext.SaveChanges();
            CreateTaskStatuse(task.TaskId);
            return task;
        }

        public bool DeleteTask(string idTask)
        {
            throw new NotImplementedException();
        }

        public List<TaskGroupEntity> GetAllGroups()
        {
            return dbContext.TaskGroup.ToList();
        }

        public List<TaskEntity> GetAllTasks()
        {
            return dbContext.Task.ToList();
        }

        public TaskEntity GetTaskById(string idTask)
        {
            return dbContext.Task.FirstOrDefault(x => x.TaskId == idTask);
        }

        public TaskGroupEntity GetTaskGroupByName(string groupName)
        {
            return dbContext.TaskGroup.FirstOrDefault(x => x.Name == groupName);
        }

        public List<TaskEntity> GetTasksByGroup(int idGroup)
        {
            return dbContext.Task.Where(x => x.TaskGroupId == idGroup).ToList();
        }

        public bool EditTask(TaskEntity task)
        {

            TaskEntity? oldTask = dbContext.Task.FirstOrDefault(x => x.TaskId == task.TaskId);
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
                dbContext.Task.Update(oldTask);
                dbContext.SaveChanges();
            }
            return false;
        }

        public bool UpdateLastModifiedTask(string idTask)
        {
            try
            {
                TaskEntity? task = dbContext.Task.FirstOrDefault(x => x.TaskId == idTask);
                if (task != null)
                {
                    task.LastModified = DateTime.Now;
                    dbContext.Task.Update(task);
                    dbContext.SaveChanges();
                    return true;
                }
                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public TaskGroupEntity? CreateTaskGroup(string name)
        {
            try
            {
                TaskGroupEntity? taskGroups = dbContext.TaskGroup.FirstOrDefault(x => x.Name == name);
                if (taskGroups == null)
                {
                    TaskGroupEntity taskGroupEntity = new()
                    {
                        Name = name
                    };
                    dbContext.TaskGroup.Add(taskGroupEntity);
                    dbContext.SaveChanges();
                    return taskGroupEntity;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DeleteTaskGroup(int idGroup)
        {
            try
            {
                List<TaskEntity> tasks = dbContext.Task.Where(x => x.TaskGroupId == idGroup).ToList();
                foreach (var task in tasks)
                {
                    task.TaskGroupId = 0;
                }
                dbContext.SaveChanges();
                TaskGroupEntity? taskGroup = dbContext.TaskGroup.FirstOrDefault(x => x.Id == idGroup);
                if (taskGroup != null)
                {
                    dbContext.TaskGroup.Remove(taskGroup);
                    dbContext.SaveChanges();
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
                TaskEntity? task = dbContext.Task.FirstOrDefault(x => x.TaskId == idTask);
                if (task != null)
                {
                    task.IsActive = !task.IsActive;
                    task.LastModified = DateTime.Now;
                    dbContext.Task.Update(task);
                    dbContext.SaveChanges();
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
                TaskStatusEntity taskStatuse = new()
                {
                    TaskId = idTask
                };
                dbContext.TaskStatuse.Add(taskStatuse);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
