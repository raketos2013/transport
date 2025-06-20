using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using System.Text;
using System.Text.RegularExpressions;


namespace FileManager_Server.Operations;

public class Read(TaskStepEntity step,
                    TaskOperation? operation,
                    ITaskLogger taskLogger,
                    AppDbContext appDbContext,
                    IMailSender mailSender)
            : StepOperation(step, operation, taskLogger, appDbContext, mailSender)
{
    public override void Execute(List<string>? bufferFiles)
    {
        _taskLogger.StepLog(TaskStep, $"ЧТЕНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileName;
        List<FileInfo> infoFiles = [];
        OperationReadEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        bufferFiles ??= [];

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
        _taskLogger.StepLog(TaskStep, $"Количество найденных файлов по маске '{TaskStep.FileMask}': {files.Length}");
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
                isReadFile = true;



                foreach (var file in files)
                {
                    FileInfo fileInfo = new(file);
                    isReadFile = true;
                    fileName = Path.GetFileName(fileInfo.FullName);

                    // файл в источнике
                    TaskLogEntity? taskLogs = _appDbContext.TaskLog.FirstOrDefault(x => x.StepId == TaskStep.StepId &&
                                                                                    x.FileName == fileName);

                    if (!string.IsNullOrEmpty(operation.FindString))
                    {
                        isReadFile = false;
                        using (StreamReader reader = new StreamReader(file))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (operation.SearchRegex)
                                {
                                    Regex regex = new(operation.FindString);
                                    MatchCollection matches = regex.Matches(line);
                                    if (matches.Count > 0)
                                    {
                                        isReadFile = true;
                                    }
                                }
                                else
                                {
                                    if (line.Contains("qweqwe"))
                                    {
                                        //Console.WriteLine($"Файл содержит строку: {line}");
                                        isReadFile = true;
                                    }
                                }
                                
                            }
                        }
                        //Console.WriteLine($"Текст 'qwqwe' не найден в файле.");
                    }

                    if (taskLogs != null)
                    {
                        if (operation.FileInSource == FileInSource.OneDay)
                        {
                            isReadFile = false;
                        }
                    }
                    if (isReadFile)
                    {
                       
                            bufferFiles.Add(file);
                            successFiles.Add(fileName);

                    } 
                }

                bool isBreakTask = false;
                switch (operation.ExpectedResult)
                {
                    case ExpectedResult.Success:
                        if (bufferFiles.Count > 0)
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = true;
                            }
                            else
                            {
                                isBreakTask = false;
                            }
                        }
                        else
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = false;
                            }
                            else
                            {
                                isBreakTask = true;
                            }
                        }
                        break;
                    case ExpectedResult.Error:
                        if (bufferFiles.Count == 0)
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = false;
                            }
                            else
                            {
                                isBreakTask = true;
                            }
                        }
                        else
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = true;
                            }
                            else
                            {
                                isBreakTask = false;
                            }
                        }
                        break;
                    case ExpectedResult.Any:
                        if (operation.BreakTaskAfterError)
                        {
                            isBreakTask = true;
                        }
                        else
                        {
                            isBreakTask = false;
                        }
                        break;
                    default:
                        break;
                }
                if (isBreakTask)
                {
                    _taskLogger.StepLog(TaskStep, $"Прерывание задачи: несоответствие ожидаемому результату", "", ResultOperation.W);
                    throw new Exception("Ошибка при операции Read: несоответствие ожидаемому результату");
                }

                _taskLogger.StepLog(TaskStep, $"{bufferFiles.Count} файлов добавлено в BUFFER");

            }
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                throw new Exception("Операция Read: найдено 0 файлов");
            }
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            _mailSender.Send(TaskStep, addresses, successFiles);
        }

        _taskLogger.StepLog(TaskStep, "Переход к следующему шагу");

        _nextStep?.Execute(bufferFiles);
    }
}
