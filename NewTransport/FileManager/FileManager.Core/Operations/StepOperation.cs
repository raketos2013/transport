using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Operations;

public abstract class StepOperation(TaskStepEntity step,
                                    TaskOperation? operation,
                                    ITaskLogger taskLogger,
                                    IMailSender mailSender,
                                    IOperationService operationService,
                                    IAddresseeService addresseeService,
                                    ITaskLogService taskLogService)
                    : IStepOperation
{
    protected IStepOperation? _nextStep;

    public TaskStepEntity TaskStep { get; set; } = step;

    public TaskOperation? TaskOperation { get; set; } = operation;

    protected readonly ITaskLogger _taskLogger = taskLogger;
    protected readonly IMailSender _mailSender = mailSender;
    protected readonly IOperationService _operationService = operationService;
    protected readonly IAddresseeService _addresseeService = addresseeService;
    protected readonly ITaskLogService _taskLogService = taskLogService;

    public abstract void Execute(List<string>? bufferFiles);

    public void SetNext(IStepOperation nextStep)
    {
        _nextStep = nextStep;
    }
}
