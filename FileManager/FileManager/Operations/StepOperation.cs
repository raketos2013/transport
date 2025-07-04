﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManagerServer.Loggers;
using FileManagerServer.MailSender;


namespace FileManagerServer.Operations;

public abstract class StepOperation(TaskStepEntity step,
                                    TaskOperation? operation,
                                    ITaskLogger taskLogger,
                                    AppDbContext appDbContext,
                                    IMailSender mailSender)
                    : IStepOperation
{
    protected IStepOperation? _nextStep;

    public TaskStepEntity TaskStep { get; set; } = step;

    public TaskOperation? TaskOperation { get; set; } = operation;

    protected readonly ITaskLogger _taskLogger = taskLogger;
    protected readonly AppDbContext _appDbContext = appDbContext;
    protected readonly IMailSender _mailSender = mailSender;

    public abstract void Execute(List<string>? bufferFiles);

    public void SetNext(IStepOperation nextStep)
    {
        _nextStep = nextStep;
    }
}
