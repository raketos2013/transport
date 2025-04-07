﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.Operations;


namespace FileManager_Server.Factory
{
    public class CopyCreator : CreatorFactoryMethod
    {
        internal override IStepOperation FactoryMethod(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext dbContext)
        {
            return new Copy(step, operation, taskLogger, dbContext);
        }
    }
}
