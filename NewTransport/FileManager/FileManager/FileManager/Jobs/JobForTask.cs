using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.OperationFactory;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.Options;
using Quartz;

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

        var taskChecked = await taskService.GetTaskById(context.JobDetail.Key.Name);
        if (taskChecked == null || !taskChecked.IsActive)
        {
            return;
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
            status.IsProgress = true;
            status.IsError = false;
            status.DateLastExecute = DateTime.Now;
            await taskService.UpdateTaskStatus(status);
        }
        if (taskChecked.ExecutionLimit != 0 && taskChecked.ExecutionLimit - taskChecked.ExecutionCount <= 0)
        {
            await taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Выключение задачи, превышен лимит выполнений >>>", ResultOperation.W);
            await taskService.ActivatedTask(taskChecked.TaskId);
            return;
        }
        taskChecked.ExecutionCount++;
        await taskService.EditTask(taskChecked);
        await taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Начало работы задачи {context.JobDetail.Key.Name} >>>");

       
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
                await steps[0].Execute(bufferFiles);
            }
            //await Task.CompletedTask;

            await taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Окончание работы задачи {context.JobDetail.Key.Name} >>>");
        }
        catch (Exception ex)
        {
            try
            {
                var status2Async = await taskService.GetTaskStatuses();
                var status2 = status2Async.First(x => x.TaskId == context.JobDetail.Key.Name);
                if (status != null)
                {
                    status.IsProgress = false;
                    status.IsError = true;
                    status.DateLastExecute = DateTime.Now;
                    await taskService.UpdateTaskStatus(status2);
                }
                var task = await taskService.GetTaskById(context.JobDetail.Key.Name);
                if (task != null)
                {
                    task.IsActive = false;
                    await taskService.EditTask(task);
                }
                await taskLogger.TaskLog(context.JobDetail.Key.Name, $"Автозавершение (выключение) задачи. {ex.Message}", ResultOperation.W);
                logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex.Message}");
            }
            catch (Exception ex2)
            {
                logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex2.Message}");
            }
        }
    }
}


