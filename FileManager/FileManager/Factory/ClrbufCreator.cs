﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManagerServer.Loggers;
using FileManagerServer.MailSender;
using FileManagerServer.Operations;


namespace FileManagerServer.Factory;

public class ClrbufCreator : CreatorFactoryMethod
{
    internal override IStepOperation FactoryMethod(TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    ITaskLogger taskLogger,
                                                    AppDbContext appDbContext,
                                                    IMailSender mailsender)
    {
        return new Clrbuf(step, operation, taskLogger, appDbContext, mailsender);
    }
}
