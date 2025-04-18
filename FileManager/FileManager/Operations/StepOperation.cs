using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.Operations
{
    public abstract class StepOperation : IStepOperation
    {
        protected IStepOperation? _nextStep;

        public TaskStepEntity TaskStep { get; set; }

        public TaskOperation? TaskOperation { get; set; }

        protected readonly ITaskLogger _taskLogger;
        protected readonly AppDbContext _appDbContext;
        protected readonly IMailSender _mailSender;

        public StepOperation(TaskStepEntity step,
                                TaskOperation? operation,
                                ITaskLogger taskLogger,
                                AppDbContext appDbContext,
                                IMailSender mailSender)
        {
            TaskStep = step;
            TaskOperation = operation;
            _taskLogger = taskLogger;
            _appDbContext = appDbContext;
            _mailSender = mailSender;
        }

        public abstract void Execute(List<string>? bufferFiles);

        public void SetNext(IStepOperation nextStep)
        {
            _nextStep = nextStep;
        }
    }
}
