using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Web.Loggers;
using FileManager_Web.MailSender;
using FileManager_Web.Operations;

namespace FileManager_Web.Factory;

public class ExistCreator : CreatorFactoryMethod
{
    internal override IStepOperation FactoryMethod(TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    ITaskLogger taskLogger,
                                                    AppDbContext appDbContext,
                                                    IMailSender mailSender)
    {
        return new Exist(step, operation, taskLogger, appDbContext, mailSender);
    }
}
