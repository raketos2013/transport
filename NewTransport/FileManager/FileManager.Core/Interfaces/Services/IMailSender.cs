using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IMailSender
{
    void Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files);
    void SendOffSteps(string taskId, List<AddresseeEntity> addresses, List<int> numberSteps);
}
