﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManagerServer.Loggers;
using FileManagerServer.MailSender;
using FileManagerServer.Operations;


namespace FileManagerServer.Factory;

public abstract class CreatorFactoryMethod
{
    internal abstract IStepOperation FactoryMethod(TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    ITaskLogger taskLogger,
                                                    AppDbContext appDbContext,
                                                    IMailSender mailSender);
}
