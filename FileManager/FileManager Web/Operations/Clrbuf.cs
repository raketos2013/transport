﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Web.Loggers;
using FileManager_Web.MailSender;


namespace FileManager_Web.Operations;

public class Clrbuf(TaskStepEntity step,
                    TaskOperation? operation,
                    ITaskLogger taskLogger,
                    AppDbContext appDbContext,
                    IMailSender mailSender)
            : StepOperation(step, operation, taskLogger, appDbContext, mailSender)
{
    public override void Execute(List<string>? bufferFiles)
    {
        _taskLogger.StepLog(TaskStep, $"ОЧИСТКА БУФЕРА: {TaskStep.Source}");
        int countFiles = 0;
        if (bufferFiles != null)
        {
            countFiles = bufferFiles.Count;
            bufferFiles = null;
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                throw new Exception("Операция Clrbuf: найдено 0 файлов");
            }
        }
        _taskLogger.StepLog(TaskStep, $"Удалено файлов из буфера: {countFiles}");

        _nextStep?.Execute(bufferFiles);
    }
}
