using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.OperationFactory;
using FileManager.Core.Services;
using Quartz;

namespace FileManager.Jobs;

[DisallowConcurrentExecution]
public class JobForTask(ILogger<JobForTask> logger,
                        ITaskLogger taskLogger,
                        IMailSender mailSender,
                        ISchedulerFactory jobFactory,
                        ITaskService taskService,
                        IStepService stepService,
                        IOperationService operationService,
                        IAddresseeService addresseeService,
                        ITaskLogService taskLogService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var hz = jobFactory.GetScheduler().Result;
        var taskChecked = taskService.GetTaskById(context.JobDetail.Key.Name);
        if (taskChecked == null || !taskChecked.IsActive)
        {
            return;
        }
        taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Начало работы задачи {context.JobDetail.Key.Name} >>>");



        TaskStatusEntity status = taskService.GetTaskStatuses().First(x => x.TaskId == context.JobDetail.Key.Name); ;
        if (status != null)
        {
            status.IsProgress = true;
            status.IsError = false;
            status.DateLastExecute = DateTime.Now;
            taskService.UpdateTaskStatus(status);
        }
        if (context.RefireCount > 5)
        {
            logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - RefireCount > 5");
        }
        try
        {
            TaskEntity? taskEntity = taskService.GetTaskById(context.JobDetail.Key.Name);
            if (taskEntity is null)
            {
                throw new ArgumentNullException(nameof(taskEntity));
            }
            List<TaskStepEntity> taskSteps = stepService.GetAllStepsByTaskId(taskEntity.TaskId)
                                                            .OrderBy(x => x.StepNumber)
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
                var addresses = addresseeService.GetAllAddressees()
                                                    .Where(x => x.AddresseeGroupId == taskEntity.AddresseeGroupId &&
                                                                x.IsActive == true).ToList();
                if (addresses.Count > 0)
                {
                    mailSender.SendOffSteps(taskEntity.TaskId, addresses, offSteps);
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
                            operation = operationService.GetCopyByStepId(step.StepId);
                            CreatorFactoryMethod copyCreator = new CopyCreator();
                            steps.Add(copyCreator.FactoryMethod(step, operation, taskLogger, mailSender,
                                                                operationService, addresseeService, taskLogService));
                            break;
                        case OperationName.Move:
                            operation = operationService.GetMoveByStepId(step.StepId);
                            CreatorFactoryMethod moveCreator = new MoveCreator();
                            steps.Add(moveCreator.FactoryMethod(step, operation, taskLogger, mailSender,
                                                                operationService, addresseeService, taskLogService));
                            break;
                        case OperationName.Read:
                            operation = operationService.GetReadByStepId(step.StepId);
                            CreatorFactoryMethod readCreator = new ReadCreator();
                            steps.Add(readCreator.FactoryMethod(step, operation, taskLogger, mailSender,
                                                                operationService, addresseeService, taskLogService));
                            break;
                        case OperationName.Exist:
                            operation = operationService.GetExistByStepId(step.StepId);
                            CreatorFactoryMethod existCreator = new ExistCreator();
                            steps.Add(existCreator.FactoryMethod(step, operation, taskLogger, mailSender,
                                                                operationService, addresseeService, taskLogService));
                            break;
                        case OperationName.Rename:
                            operation = operationService.GetRenameByStepId(step.StepId);
                            CreatorFactoryMethod renameCreator = new RenameCreator();
                            steps.Add(renameCreator.FactoryMethod(step, operation, taskLogger, mailSender,
                                                                operationService, addresseeService, taskLogService));
                            break;
                        case OperationName.Delete:
                            operation = operationService.GetDeleteByStepId(step.StepId);
                            CreatorFactoryMethod deleteCreator = new DeleteCreator();
                            steps.Add(deleteCreator.FactoryMethod(step, operation, taskLogger, mailSender,
                                                                operationService, addresseeService, taskLogService));
                            break;
                        case OperationName.Clrbuf:
                            operation = operationService.GetClrbufByStepId(step.StepId);
                            CreatorFactoryMethod clrbufCreator = new ClrbufCreator();
                            steps.Add(clrbufCreator.FactoryMethod(step, operation, taskLogger, mailSender,
                                                                operationService, addresseeService, taskLogService));
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
                steps[0].Execute(bufferFiles);
            }
            await Task.CompletedTask;

            taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Окончание работы задачи {context.JobDetail.Key.Name} >>>");
        }
        catch (Exception ex)
        {
            try
            {
                TaskStatusEntity status2 = taskService.GetTaskStatuses().First(x => x.TaskId == context.JobDetail.Key.Name);
                if (status != null)
                {
                    status.IsProgress = false;
                    status.IsError = true;
                    status.DateLastExecute = DateTime.Now;
                    taskService.UpdateTaskStatus(status2);
                }
                TaskEntity task = taskService.GetTaskById(context.JobDetail.Key.Name);
                if (task != null)
                {
                    task.IsActive = false;
                    taskService.EditTask(task);
                }
                logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex.Message}");
            }
            catch (Exception ex2)
            {
                logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex2.Message}");
            }


            //throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
        }
    }
}


