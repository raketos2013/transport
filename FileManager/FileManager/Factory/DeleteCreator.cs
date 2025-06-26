using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManagerServer.Loggers;
using FileManagerServer.MailSender;
using FileManagerServer.Operations;


namespace FileManagerServer.Factory;

public class DeleteCreator : CreatorFactoryMethod
{
    internal override IStepOperation FactoryMethod(TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    ITaskLogger taskLogger,
                                                    AppDbContext appDbContext,
                                                    IMailSender mailSender)
    {
        return new Delete(step, operation, taskLogger, appDbContext, mailSender);
    }
}
