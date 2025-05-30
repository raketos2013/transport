using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Factory;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using FileManager_Server.Operations;
using Quartz;


namespace FileManager_Server;


[DisallowConcurrentExecution]
public class JobForTask(AppDbContext appDbContext, ILogger<JobForTask> logger, ITaskLogger taskLogger, IMailSender mailSender) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Начало работы задачи {context.JobDetail.Key.Name} >>>");
        if (context.RefireCount > 5)
        {
            logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - RefireCount > 5");
        }
        try
        {
            TaskEntity? taskEntity = appDbContext.Task.First(x => x.TaskId == context.JobDetail.Key.Name);
            if (taskEntity is null)
            {
                throw new ArgumentNullException(nameof(taskEntity));
            }
            List<TaskStepEntity> taskSteps = appDbContext.TaskStep.Where(x => x.TaskId == taskEntity.TaskId).OrderBy(x => x.StepNumber).ToList();

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
                            operation = appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == step.StepId);
                            CreatorFactoryMethod copyCreator = new CopyCreator();
                            steps.Add(copyCreator.FactoryMethod(step, operation, taskLogger, appDbContext, mailSender));
                            break;
                        case OperationName.Move:
                            operation = appDbContext.OperationMove.FirstOrDefault(x => x.StepId == step.StepId);
                            CreatorFactoryMethod moveCreator = new MoveCreator();
                            steps.Add(moveCreator.FactoryMethod(step, operation, taskLogger, appDbContext, mailSender));
                            break;
                        case OperationName.Read:
                            operation = appDbContext.OperationRead.FirstOrDefault(x => x.StepId == step.StepId);
                            CreatorFactoryMethod readCreator = new ReadCreator();
                            steps.Add(readCreator.FactoryMethod(step, operation, taskLogger, appDbContext, mailSender));
                            break;
                        case OperationName.Exist:
                            operation = appDbContext.OperationExist.FirstOrDefault(x => x.StepId == step.StepId);
                            CreatorFactoryMethod existCreator = new ExistCreator();
                            steps.Add(existCreator.FactoryMethod(step, operation, taskLogger, appDbContext, mailSender));
                            break;
                        case OperationName.Rename:
                            operation = appDbContext.OperationRename.FirstOrDefault(x => x.StepId == step.StepId);
                            CreatorFactoryMethod renameCreator = new RenameCreator();
                            steps.Add(renameCreator.FactoryMethod(step, operation, taskLogger, appDbContext, mailSender));
                            break;
                        case OperationName.Delete:
                            operation = appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == step.StepId);
                            CreatorFactoryMethod deleteCreator = new DeleteCreator();
                            steps.Add(deleteCreator.FactoryMethod(step, operation, taskLogger, appDbContext, mailSender));
                            break;
                        case OperationName.Clrbuf:
                            operation = appDbContext.OperationRead.FirstOrDefault(x => x.StepId == step.StepId);
                            CreatorFactoryMethod clrbufCreator = new ClrbufCreator();
                            steps.Add(clrbufCreator.FactoryMethod(step, operation, taskLogger, appDbContext, mailSender));
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
                TaskStatusEntity status = appDbContext.TaskStatuse.First(x => x.TaskId == context.JobDetail.Key.Name);
                if (status != null)
                {
                    status.IsProgress = false;
                    status.IsError = true;
                    status.DateLastExecute = DateTime.Now;
                    appDbContext.TaskStatuse.Update(status);
                    appDbContext.SaveChanges();
                }
                TaskEntity task = appDbContext.Task.FirstOrDefault(x => x.TaskId == context.JobDetail.Key.Name);
                if (task != null)
                {
                    task.IsActive = false;
                }
                appDbContext.Task.Update(task);
                appDbContext.SaveChanges();

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


