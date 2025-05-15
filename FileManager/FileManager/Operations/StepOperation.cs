using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;


namespace FileManager_Server.Operations
{
    public abstract class StepOperation(TaskStepEntity step,
                                        TaskOperation? operation,
                                        ITaskLogger taskLogger,
                                        AppDbContext appDbContext,
                                        IMailSender mailSender) 
                        : IStepOperation
    {
        protected IStepOperation? _nextStep;

        public TaskStepEntity TaskStep { get; set; } = step;

        public TaskOperation? TaskOperation { get; set; } = operation;

        protected readonly ITaskLogger _taskLogger = taskLogger;
        protected readonly AppDbContext _appDbContext = appDbContext;
        protected readonly IMailSender _mailSender = mailSender;

        public abstract void Execute(List<string>? bufferFiles);

        public void SetNext(IStepOperation nextStep)
        {
            _nextStep = nextStep;
        }
    }
}
