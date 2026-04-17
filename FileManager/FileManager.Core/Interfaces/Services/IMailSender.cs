using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IMailSender
{
    Task SendRename(TaskStepEntity step, List<AddresseeEntity> addresses, List<(string OldFileName, string NewFileName)> files);
    Task Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files);
    Task SendOffSteps(string taskId, List<AddresseeEntity> addresses, List<int> numberSteps);
}
