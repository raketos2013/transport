using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Web.Loggers;
using FileManager_Web.MailSender;
using FileManager_Web.Operations;


namespace FileManager_Web.Factory;

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
