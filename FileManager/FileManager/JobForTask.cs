using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Factory;
using FileManager_Server.Loggers;
using FileManager_Server.Operations;
using Quartz;


namespace FileManager_Server
{

	[DisallowConcurrentExecution]
	public class JobForTask : IJob
	{
		private readonly ILogger<JobForTask> _logger;
		private readonly ITaskLogger _taskLogger;
		private readonly AppDbContext _appDbContext;

		public JobForTask(AppDbContext appDbContext, ILogger<JobForTask> logger, ITaskLogger taskLogger)
		{
			_appDbContext = appDbContext;
			_logger = logger;
			_taskLogger = taskLogger;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			_taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Начало работы задачи {context.JobDetail.Key.Name} >>>");
			if (context.RefireCount > 5)
			{
				_logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - RefireCount > 5");
			}
			try
			{
                TaskEntity? taskEntity = _appDbContext.Task.First(x => x.TaskId == context.JobDetail.Key.Name);
                if (taskEntity is null)
                {
                    throw new ArgumentNullException(nameof(taskEntity));
                }
                List<TaskStepEntity> taskSteps = _appDbContext.TaskStep.Where(x => x.TaskId == taskEntity.TaskId).OrderBy(x => x.StepNumber).ToList();

                List<IStepOperation> steps = new List<IStepOperation>();
                List<string> bufferFiles = new List<string>();
                int numberChainLink = 0;
                TaskOperation? operation;
                foreach (var step in taskSteps)
                {
                    if (step.IsActive)
                    {
                        switch (step.OperationName)
                        {
                            case OperationName.Copy:
                                operation = _appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == step.StepId);
                                CreatorFactoryMethod copyCreator = new CopyCreator();
                                steps.Add(copyCreator.FactoryMethod(step, operation, _taskLogger, _appDbContext));
                                break;
                            case OperationName.Move:
                                operation = _appDbContext.OperationMove.FirstOrDefault(x => x.StepId == step.StepId);
                                CreatorFactoryMethod moveCreator = new MoveCreator();
                                steps.Add(moveCreator.FactoryMethod(step, operation, _taskLogger, _appDbContext));
                                break;
                            case OperationName.Read:
                                operation = _appDbContext.OperationRead.FirstOrDefault(x => x.StepId == step.StepId);
                                CreatorFactoryMethod readCreator = new ReadCreator();
                                steps.Add(readCreator.FactoryMethod(step, operation, _taskLogger, _appDbContext));
                                break;
                            case OperationName.Exist:
                                operation = _appDbContext.OperationExist.FirstOrDefault(x => x.StepId == step.StepId);
                                CreatorFactoryMethod existCreator = new ExistCreator();
                                steps.Add(existCreator.FactoryMethod(step, operation, _taskLogger, _appDbContext));
                                break;
                            case OperationName.Rename:
								operation = _appDbContext.OperationRename.FirstOrDefault(x => x.StepId == step.StepId);
                                CreatorFactoryMethod renameCreator = new RenameCreator();
                                steps.Add(renameCreator.FactoryMethod(step, operation, _taskLogger, _appDbContext));
                                break;
                            case OperationName.Delete:
                                operation = _appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == step.StepId);
                                CreatorFactoryMethod deleteCreator = new DeleteCreator();
                                steps.Add(deleteCreator.FactoryMethod(step, operation, _taskLogger, _appDbContext));
                                break;
							case OperationName.Clrbuf:
                                operation = _appDbContext.OperationRead.FirstOrDefault(x => x.StepId == step.StepId);
                                CreatorFactoryMethod clrbufCreator = new ClrbufCreator();
                                steps.Add(clrbufCreator.FactoryMethod(step, operation, _taskLogger, _appDbContext));
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

                _taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Окончание работы задачи {context.JobDetail.Key.Name} >>>");
			}
			catch (Exception ex)
			{
				/*using (var scope = _serviceProvider.CreateScope())
				{
					try
					{

						var service = scope.ServiceProvider.GetService<AppDbContext>();
						if (service != null)
						{
							TaskStatusEntity status = service.TaskStatuse.First(x => x.TaskId == context.JobDetail.Key.Name);
							if (status != null)
							{
								status.IsProgress = false;
								status.IsError = true;
								status.DateLastExecute = DateTime.Now;
								service.TaskStatuse.Update(status);
								service.SaveChanges();
							}
						}
						_logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex.Message}");
					}
					catch (Exception ex2)
					{
						_logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex2.Message}");
					}

				}
				throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);*/
			}
		}
	}
}


