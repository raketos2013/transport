using DocumentFormat.OpenXml.InkML;
using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.OperationFactory;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.Options;
using Quartz;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace FileManager.Jobs;

[DisallowConcurrentExecution]
public class JobForTask(IServiceScopeFactory scopeFactory) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = scopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<JobForTask>>();
        var taskLogger = scope.ServiceProvider.GetRequiredService<ITaskLogger>();
        var mailSender = scope.ServiceProvider.GetRequiredService<IMailSender>();
        var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
        var stepService = scope.ServiceProvider.GetRequiredService<IStepService>();
        var operationService = scope.ServiceProvider.GetRequiredService<IOperationService>();
        var addresseeService = scope.ServiceProvider.GetRequiredService<IAddresseeService>();
        CancellationToken cancellationToken = context.CancellationToken;

        var taskChecked = await taskService.GetTaskById(context.JobDetail.Key.Name);
        if (taskChecked == null || !taskChecked.IsActive)
        {
            return;
        }

        logger.LogInformation($"<<< start task - {context.JobDetail.Key.Name}");

        logger.LogInformation($"<<< check calendar for task - {context.JobDetail.Key.Name}");
        var calendarDay = await CheckCalendar();
        logger.LogInformation($"<<< calendar - {calendarDay}");
        switch (taskChecked.DayActive)
        {
            case DayActive.WORK:
                if (!calendarDay)
                {
                    logger.LogInformation($"<<< task not work at this day - {context.JobDetail.Key.Name}");
                    return;
                }
                break;
            case DayActive.HOLIDAY:
                if (calendarDay)
                {
                    logger.LogInformation($"<<< task not work at this day - {context.JobDetail.Key.Name}");
                    return;
                }
                break;
            case DayActive.ALL:
                break;
            default:
                break;
        }

        var statusAsync = await taskService.GetTaskStatuses();
        var status = statusAsync.First(x => x.TaskId == context.JobDetail.Key.Name);
        if (status != null)
        {
            if (status.DateLastExecute.Date != DateTime.Now.Date)
            {
                taskChecked.ExecutionCount = 0;
                await taskService.EditTask(taskChecked);
            }
            //status.IsProgress = true;
            //status.IsError = false;
            status.Status = StatusTask.Process;
            status.DateLastExecute = DateTime.Now;
            await taskService.UpdateTaskStatus(status);
        }
        if (taskChecked.ExecutionLimit != 0 && taskChecked.ExecutionLimit - taskChecked.ExecutionCount <= 0)
        {
            //await taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Выключение задачи, превышен лимит выполнений >>>", ResultOperation.W);
            //await taskService.ActivatedTask(taskChecked.TaskId);


            if (status != null)
            {
                status.Status = StatusTask.Complete;
                await taskService.UpdateTaskStatus(status);
            }
            return;
        }
        taskChecked.ExecutionCount++;
        await taskService.EditTask(taskChecked);




        await taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Начало работы задачи {context.JobDetail.Key.Name} >>>");
        logger.LogInformation($"<<< Начало работы задачи {context.JobDetail.Key.Name} >>>");


        if (context.RefireCount > 5)
        {
            logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - RefireCount > 5");
        }
        try
        {
            TaskEntity? taskEntity = await taskService.GetTaskById(context.JobDetail.Key.Name);
            if (taskEntity is null)
            {
                throw new ArgumentNullException(nameof(taskEntity));
            }
            var taskStepsAsync = await stepService.GetAllStepsByTaskId(taskEntity.TaskId);
            var taskSteps = taskStepsAsync.OrderBy(x => x.StepNumber)
                                                            .ToList();
            List<int> offSteps = [];
            foreach (var taskStep in taskSteps)
            {
                if (!taskStep.IsActive)
                {
                    offSteps.Add(taskStep.StepNumber);
                }
            }
            if (offSteps.Count > 0)
            {
                var addressesAsync = await addresseeService.GetAllAddressees();
                var addresses = addressesAsync.Where(x => x.AddresseeGroupId == taskEntity.AddresseeGroupId &&
                                                                x.IsActive == true).ToList();
                if (addresses.Count > 0)
                {
                    await mailSender.SendOffSteps(taskEntity.TaskId, addresses, offSteps);
                }
            }

            List<IStepOperation> steps = [];
            List<string> bufferFiles = [];
            int numberChainLink = 0;
            TaskOperation? operation;
            foreach (var step in taskSteps)
            {
                if (step.IsActive)
                {
                    switch (step.OperationName)
                    {
                        case OperationName.Copy:
                            operation = await operationService.GetCopyByStepId(step.StepId);
                            CreatorFactoryMethod copyCreator = new CopyCreator();
                            steps.Add(copyCreator.FactoryMethod(step, operation, scope));
                            break;
                        case OperationName.Move:
                            operation = await operationService.GetMoveByStepId(step.StepId);
                            CreatorFactoryMethod moveCreator = new MoveCreator();
                            steps.Add(moveCreator.FactoryMethod(step, operation, scope));
                            break;
                        case OperationName.Read:
                            operation = await operationService.GetReadByStepId(step.StepId);
                            CreatorFactoryMethod readCreator = new ReadCreator();
                            steps.Add(readCreator.FactoryMethod(step, operation, scope));
                            break;
                        case OperationName.Exist:
                            operation = await operationService.GetExistByStepId(step.StepId);
                            CreatorFactoryMethod existCreator = new ExistCreator();
                            steps.Add(existCreator.FactoryMethod(step, operation, scope));
                            break;
                        case OperationName.Rename:
                            operation = await operationService.GetRenameByStepId(step.StepId);
                            CreatorFactoryMethod renameCreator = new RenameCreator();
                            steps.Add(renameCreator.FactoryMethod(step, operation, scope));
                            break;
                        case OperationName.Delete:
                            operation = await operationService.GetDeleteByStepId(step.StepId);
                            CreatorFactoryMethod deleteCreator = new DeleteCreator();
                            steps.Add(deleteCreator.FactoryMethod(step, operation, scope));
                            break;
                        case OperationName.Clrbuf:
                            operation = await operationService.GetClrbufByStepId(step.StepId);
                            CreatorFactoryMethod clrbufCreator = new ClrbufCreator();
                            steps.Add(clrbufCreator.FactoryMethod(step, operation, scope));
                            break;
                        default:
                            break;
                    }
                    if (numberChainLink != 0)
                    {
                        steps[numberChainLink - 1].SetNext(steps[numberChainLink]);
                    }
                    numberChainLink++;
                }
            }
            if (steps.Count > 0)
            {
                await steps[0].Execute(bufferFiles, cancellationToken);
            }
            //await Task.CompletedTask;

            await taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Окончание работы задачи {context.JobDetail.Key.Name} >>>");
            logger.LogInformation($"<<< Окончание работы задачи {context.JobDetail.Key.Name} >>>");

            var taskStatusAsync = await taskService.GetTaskStatuses();
            var taskStatus = statusAsync.First(x => x.TaskId == context.JobDetail.Key.Name);
            if (taskStatus != null)
            {
                status.Status = StatusTask.Wait;
                await taskService.UpdateTaskStatus(status);
            }


        }
        catch (OperationCanceledException canceledException)
        {
            logger.LogError($"{DateTime.Now} Выключение задачи : {context.JobDetail.Key.Name} пользователем");
            try
            {
                var status2Async = await taskService.GetTaskStatuses();
                var status2 = status2Async.First(x => x.TaskId == context.JobDetail.Key.Name);
                if (status2 != null)
                {
                    status2.Status = StatusTask.Wait;
                    status2.DateLastExecute = DateTime.Now;
                    await taskService.UpdateTaskStatus(status2);
                }
                var task = await taskService.GetTaskById(context.JobDetail.Key.Name);
                if (task != null)
                {
                    task.IsActive = false;
                    await taskService.EditTask(task);
                }
                await taskLogger.TaskLog(context.JobDetail.Key.Name, $"Выключение задачи пользователем", ResultOperation.W);
            }
            catch (Exception ex2)
            {
                logger.LogError($"{DateTime.Now} Ошибка задачи: {context.JobDetail.Key.Name} - {ex2.Message}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"{DateTime.Now} Автозавершение (выключение) задачи : {context.JobDetail.Key.Name} - {ex.Message}");
            try
            {
                var status2Async = await taskService.GetTaskStatuses();
                var status2 = status2Async.First(x => x.TaskId == context.JobDetail.Key.Name);
                if (status2 != null)
                {
                    status2.Status = StatusTask.Error;
                    status2.DateLastExecute = DateTime.Now;
                    await taskService.UpdateTaskStatus(status2);
                }
                var task = await taskService.GetTaskById(context.JobDetail.Key.Name);
                if (task != null)
                {
                    task.IsActive = false;
                    await taskService.EditTask(task);
                }
                await taskLogger.TaskLog(context.JobDetail.Key.Name, $"Автозавершение (выключение) задачи. {ex.Message}", ResultOperation.W);
            }
            catch (Exception ex2)
            {
                logger.LogError($"{DateTime.Now} Ошибка задачи: {context.JobDetail.Key.Name} - {ex2.Message}");
            }
        }
    }

    private async Task<bool> CheckCalendar()
    {
        using var scope = scopeFactory.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<JobForTask>>();
        IHttpClientFactory _httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        IOptions<AuthTokenConfiguration> _authTokenConfigurations = scope.ServiceProvider.GetRequiredService<IOptions<AuthTokenConfiguration>>();

        var client = _httpClientFactory.CreateClient("Calendar");

        var data = new Dictionary<string, string>
                    {
                        { "grant_type", "client_credentials" },
                        { "client_id", _authTokenConfigurations.Value.ClientId },
                        { "client_secret", _authTokenConfigurations.Value.ClientSecret }
                    };
        var content = new FormUrlEncodedContent(data);
        logger.LogInformation($"<<< Authorization for work day");
        var responseToken = await client.PostAsync(_authTokenConfigurations.Value.TokenUrl, content);
        logger.LogInformation($"<<< Status result Authorization for work day - {responseToken.StatusCode}");
        if (responseToken.IsSuccessStatusCode)
        {
            logger.LogInformation($"<<< Authorization is SUCCESSFULL");
            var jsonResponse = await responseToken.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

            if (tokenResponse != null)
            {
                var date = DateTime.Today.ToString("yyyy-MM-dd");
                string requestString = "http://sca-iis-t.bb.asb:8080/reference-book/calendar/working-day-rb/" + date;
                var request = new HttpRequestMessage(HttpMethod.Head, requestString);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Day is WORK");
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {

                    Console.WriteLine("Day is NOT WORK");
                    return false;
                }
            }
        }
        else
        {
            logger.LogInformation($"<<< Authorization is ERROR - {responseToken.StatusCode}");
        }
        return true;
    }
}


