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
    public class Delete : StepOperation
    {
        public Delete(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext, IMailSender mailSender)
            : base(step, operation, taskLogger, appDbContext, mailSender)
        {
        }

        public override void Execute(List<string>? bufferFiles)
        {
            _taskLogger.StepLog(TaskStep, $"Удаление: {TaskStep.Source} => {TaskStep.Destination}");
            _taskLogger.OperationLog(TaskStep);

            string[] files = [];
            string fileName;
            List<AddresseeEntity> addresses = new List<AddresseeEntity>();
            List<string> successFiles = new List<string>();

            files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
            _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Count()}");

            OperationDeleteEntity? operation = _appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == TaskStep.StepId);
            if (operation != null)
            {
                if (operation.InformSuccess && files.Count() > 0)
                {
                    addresses = _appDbContext.Addressee.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                                    x.IsActive == true).ToList();
                }
            }

            foreach (string file in files)
            {
                fileName = Path.GetFileName(file);

                File.Delete(file);
                _taskLogger.StepLog(TaskStep, "Файл успешно удалён", fileName);
                successFiles.Add(fileName);
            }

            if (addresses.Count > 0)
            {
                _mailSender.Send(TaskStep, addresses, successFiles);
            }

            if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
