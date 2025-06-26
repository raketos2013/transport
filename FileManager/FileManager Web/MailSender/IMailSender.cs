using FileManager.Domain.Entity;

namespace FileManager_Web.MailSender;

public interface IMailSender
{
    void Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files);
}
