using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using System.Text;


namespace FileManager_Server.Operations
{
    public class Read : StepOperation
    {
        public Read(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext, IMailSender mailSender)
            : base(step, operation, taskLogger, appDbContext, mailSender)
        {
        }

        public override void Execute(List<string>? bufferFiles)
        {
            _taskLogger.StepLog(TaskStep, $"Чтение (проверка контента): {TaskStep.Source} => {TaskStep.Destination}");
            _taskLogger.OperationLog(TaskStep);

            string[] files = [];
            string fileName;
            List<FileInfo> infoFiles = new List<FileInfo>();
            OperationReadEntity? operation = null;
            List<AddresseeEntity> addresses = new List<AddresseeEntity>();
            List<string> successFiles = new List<string>();

            if (bufferFiles == null)
            {
                bufferFiles = new List<string>();
            }

            if (TaskStep.FileMask == "{BUFFER}")
            {
                if (bufferFiles != null)
                {
                    foreach (var file in bufferFiles)
                    {
                        infoFiles.Add(new FileInfo(file));
                    }
                }
            }
            else
            {
                files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
                foreach (var file in files)
                {
                    infoFiles.Add(new FileInfo(file));
                }
            }
            _taskLogger.StepLog(TaskStep, $"Количество найденных файлов по маске '{TaskStep.FileMask}': {files.Count()}");
            bool isReadFile = true;
            if (infoFiles.Count > 0)
            {
                operation = _appDbContext.OperationRead.FirstOrDefault(x => x.StepId == TaskStep.StepId);
                if (operation != null)
                {

                    if (operation.InformSuccess)
                    {
                        addresses = _appDbContext.Addressee.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                                        x.IsActive == true).ToList();
                    }

                    Encoding encoding = Encoding.Default;
                    string fileText = File.ReadAllText("d:\\transportFiles\\time.txt", encoding);
                    _taskLogger.StepLog(TaskStep, "Файл успешно прочитан", "time.txt");
                    isReadFile = true;

                    foreach (var file in files)
                    {
                        FileInfo fileInfo = new(file);
                        isReadFile = true;
                        fileName = Path.GetFileName(fileInfo.FullName);

                        // файл в источнике
                        TaskLogEntity? taskLogs = _appDbContext.TaskLog.FirstOrDefault(x => x.StepId == TaskStep.StepId &&
                                                                                        x.FileName == fileName);
                        if (taskLogs != null)
                        {
                            if (operation.FileInSource == FileInSource.OneDay)
                            {
                                isReadFile = false;
                            }
                        }

                        if (isReadFile)
                        {

                            if (operation.ExpectedResult == ExpectedResult.Success)
                            {
                                bufferFiles.Add(file);
                                successFiles.Add(fileName);
                            }
                        }
                    }
                    _taskLogger.StepLog(TaskStep, $"{bufferFiles.Count} файлов добавлено в BUFFER");

                }
            }

            if (addresses.Count > 0)
            {
                _mailSender.Send(TaskStep, addresses, successFiles);
            }

            _taskLogger.StepLog(TaskStep, "Переход к следующему шагу");
            if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
