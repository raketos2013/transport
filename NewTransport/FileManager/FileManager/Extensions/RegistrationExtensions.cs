using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Services;
using FileManager.Infrastructure.Repositories;

namespace FileManager.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IStepRepository, StepRepository>();
        services.AddScoped<IAddresseeRepository, AddresseeRepository>();
        services.AddScoped<IOperationRepository, OperationRepository>();
        services.AddScoped<ITaskLogRepository, TaskLogRepository>();
        services.AddScoped<IUserLogRepository, UserLogRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IStepService, StepService>();
        services.AddScoped<IAddresseeService, AddresseeService>();
        services.AddScoped<IOperationService, OperationService>();
        services.AddScoped<ITaskLogService, TaskLogService>();
        services.AddScoped<IUserLogService, UserLogService>();

        services.AddScoped<ITaskLogger, TaskLogger>();
        services.AddScoped<IMailSender, MailSender>();

        //services.AddScoped<IUserLogging, UserLogging>();
        return services;
    }
}
