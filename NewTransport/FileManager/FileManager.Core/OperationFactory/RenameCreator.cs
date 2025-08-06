using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Operations;

namespace FileManager.Core.OperationFactory;

public class RenameCreator : CreatorFactoryMethod
{
    public override IStepOperation FactoryMethod(TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    ITaskLogger taskLogger,
                                                    IMailSender mailSender,
                                                    IOperationService operationService,
                                                    IAddresseeService addresseeService,
                                                    ITaskLogService taskLogService)
    {
        return new Rename(step, operation, taskLogger, mailSender, operationService, addresseeService, taskLogService);
    }
}
