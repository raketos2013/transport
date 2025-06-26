using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Web.Loggers;
using FileManager_Web.MailSender;
using FileManager_Web.Operations;


namespace FileManager_Web.Factory;

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
