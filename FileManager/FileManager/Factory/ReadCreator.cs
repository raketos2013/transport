﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using FileManager_Server.Operations;


namespace FileManager_Server.Factory
{
    public class ReadCreator : CreatorFactoryMethod
    {
        internal override IStepOperation FactoryMethod(TaskStepEntity step,
                                                        TaskOperation? operation,
                                                        ITaskLogger taskLogger,
                                                        AppDbContext appDbContext,
                                                        IMailSender mailSender)
        {
            return new Read(step, operation, taskLogger, appDbContext, mailSender);
        }
    }
}
