using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IMailSender
{
    Task Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files);
    Task SendOffSteps(string taskId, List<AddresseeEntity> addresses, List<int> numberSteps);
}
