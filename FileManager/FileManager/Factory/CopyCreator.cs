﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManagerServer.Loggers;
using FileManagerServer.MailSender;
using FileManagerServer.Operations;


namespace FileManagerServer.Factory;

public class CopyCreator : CreatorFactoryMethod
{
    internal override IStepOperation FactoryMethod(TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    ITaskLogger taskLogger,
                                                    AppDbContext dbContext,
                                                    IMailSender mailSender)
    {
        return new Copy(step, operation, taskLogger, dbContext, mailSender);
    }
}
