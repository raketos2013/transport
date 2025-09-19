using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FileManager.Core.Operations;

public abstract class StepOperation : IStepOperation
{
    protected IStepOperation? _nextStep;
    public TaskStepEntity TaskStep { get; set; }
    public TaskOperation? TaskOperation { get; set; }

    protected readonly ITaskLogger _taskLogger;
    protected readonly IMailSender _mailSender;
    protected readonly IOperationService _operationService;
    protected readonly IAddresseeService _addresseeService;
    protected readonly ITaskLogService _taskLogService;
    protected readonly IHttpClientFactory _httpClientFactory;
    protected readonly IOptions<AuthTokenConfiguration> _authTokenConfigurations;

    protected StepOperation(TaskStepEntity step,
                            TaskOperation? operation, 
                            IServiceScopeFactory scopeFactory)
    {
        TaskStep = step;
        TaskOperation = operation;
        using var scope = scopeFactory.CreateScope();
        _taskLogger = scope.ServiceProvider.GetRequiredService<ITaskLogger>();
        _mailSender = scope.ServiceProvider.GetRequiredService<IMailSender>();
        _operationService = scope.ServiceProvider.GetRequiredService<IOperationService>();
        _addresseeService = scope.ServiceProvider.GetRequiredService<IAddresseeService>();
        _taskLogService = scope.ServiceProvider.GetRequiredService<ITaskLogService>();
        _httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        _authTokenConfigurations = scope.ServiceProvider.GetRequiredService<IOptions<AuthTokenConfiguration>>();
    }

    public abstract Task Execute(List<string>? bufferFiles);

    public void SetNext(IStepOperation nextStep)
    {
        _nextStep = nextStep;
    }
}
