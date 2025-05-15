using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;


namespace FileManager_Server.Operations
{
    public class Clrbuf(TaskStepEntity step, 
                        TaskOperation? operation, 
                        ITaskLogger taskLogger, 
                        AppDbContext appDbContext, 
                        IMailSender mailSender) 
                : StepOperation(step, operation, taskLogger, appDbContext, mailSender)
    {
        public override void Execute(List<string>? bufferFiles)
        {
            _taskLogger.StepLog(TaskStep, $"ОЧИСТКА БУФЕРА: {TaskStep.Source}");
            int countFiles = 0;
            if (bufferFiles != null)
            {
                countFiles = bufferFiles.Count;
                bufferFiles = null;
            }
            _taskLogger.StepLog(TaskStep, $"Удалено файлов из буфера: {countFiles}");

            _nextStep?.Execute(bufferFiles);
        }
    }
}
