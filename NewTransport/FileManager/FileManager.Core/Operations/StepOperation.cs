using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FileManager.Core.Operations;

public abstract class StepOperation
    //(TaskStepEntity step,
    //                                TaskOperation? operation,
    //                                //ITaskLogger taskLogger,
    //                                //IMailSender mailSender,
    //                                //IOptions<AuthTokenConfiguration> authTokenConfigurations,
    //                                //IOperationService operationService,
    //                                //IAddresseeService addresseeService,
    //                                //ITaskLogService taskLogService,
    //                                //IHttpClientFactory httpClientFactory,
    //                                IServiceScopeFactory scopeFactory)
                    : IStepOperation
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
        this.TaskStep = step;
        this.TaskOperation = operation;
        using var scope = scopeFactory.CreateScope();
        this._taskLogger = scope.ServiceProvider.GetRequiredService<ITaskLogger>();
        this._mailSender = scope.ServiceProvider.GetRequiredService<IMailSender>();
        this._operationService = scope.ServiceProvider.GetRequiredService<IOperationService>();
        this._addresseeService = scope.ServiceProvider.GetRequiredService<IAddresseeService>();
        this._taskLogService = scope.ServiceProvider.GetRequiredService<ITaskLogService>();
        this._httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        this._authTokenConfigurations = scope.ServiceProvider.GetRequiredService<IOptions<AuthTokenConfiguration>>();
    }

    public abstract Task Execute(List<string>? bufferFiles);

    public void SetNext(IStepOperation nextStep)
    {
        _nextStep = nextStep;
    }
}
