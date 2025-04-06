﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.Operations;


namespace FileManager_Server.Factory
{
    public class DeleteCreator : CreatorFactoryMethod
    {
        internal override IStepOperation FactoryMethod(TaskStepEntity step, TaskOperation operation, ITaskLogger taskLogger, AppDbContext appDbContext)
        {
            return new Delete(step, operation, taskLogger, appDbContext);
        }
    }
}
