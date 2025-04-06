using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.Operations;


namespace FileManager_Server.Factory
{
    public abstract class CreatorFactoryMethod
    {
        internal abstract IStepOperation FactoryMethod(TaskStepEntity step, TaskOperation operation, ITaskLogger taskLogger, AppDbContext appDbContext);
    }
}
