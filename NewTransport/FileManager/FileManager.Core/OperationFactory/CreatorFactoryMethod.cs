using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.OperationFactory;

public abstract class CreatorFactoryMethod
{
    public abstract IStepOperation FactoryMethod(TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    ITaskLogger taskLogger,
                                                    IMailSender mailSender,
                                                    IOperationService operationService,
                                                    IAddresseeService addresseeService,
                                                    ITaskLogService taskLogService);
}
