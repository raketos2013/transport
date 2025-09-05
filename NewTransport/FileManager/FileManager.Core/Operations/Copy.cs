using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using System.Net.Http.Headers;

namespace FileManager.Core.Operations;

public class Copy(TaskStepEntity step,
                    TaskOperation? operation,
                    ITaskLogger taskLogger,
                    IMailSender mailSender,
                    IOperationService operationService,
                    IAddresseeService addresseeService,
                    ITaskLogService taskLogService)
            : StepOperation(step, operation, taskLogger, mailSender, operationService, addresseeService, taskLogService)
{
    public override void Execute(List<string>? bufferFiles)
    {
        _taskLogger.StepLog(TaskStep, $"КОПИРОВАНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileNameDestination, fileName;
        bool isCopyFile = true;
        List<FileInfo> infoFiles = [];
        List<string> successFiles = [];
        OperationCopyEntity? operation = null;

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
        List<AddresseeEntity> addresses = [];
        _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count}");
        if (infoFiles.Count > 0)
        {
            operation = _operationService.GetCopyByStepId(TaskStep.StepId);
            //operation = _appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == TaskStep.StepId);

            if (operation != null)
            {

                if (operation.InformSuccess)
                {
                    addresses = _addresseeService.GetAllAddressees()
                                                    .Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                                x.IsActive == true).ToList();
                }

                // сортировка
                switch (operation.Sort)
                {
                    case SortFiles.NoSortFiles:
                        break;
                    case SortFiles.NameAscending:
                        infoFiles = infoFiles.OrderBy(o => o.Name).ToList();
                        break;
                    case SortFiles.NameDescending:
                        infoFiles = infoFiles.OrderByDescending(o => o.Name).ToList();
                        break;
                    case SortFiles.TimeAscending:
                        infoFiles = infoFiles.OrderBy(o => o.CreationTime).ToList();
                        break;
                    case SortFiles.TimeDescending:
                        infoFiles = infoFiles.OrderByDescending(o => o.CreationTime).ToList();
                        break;
                    case SortFiles.SizeAscending:
                        infoFiles = infoFiles.OrderBy(o => o.Length).ToList();
                        break;
                    case SortFiles.SizeDescending:
                        infoFiles = infoFiles.OrderByDescending(o => o.Length).ToList();
                        break;
                    default:
                        break;
                }
                // макс файлов
                if (operation.FilesForProcessing != 0 & operation.FilesForProcessing < infoFiles.Count - 2)
                {
                    infoFiles.RemoveRange(operation.FilesForProcessing, infoFiles.Count - 2);
                }
            }
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                throw new Exception("Операция Copy: найдено 0 файлов");
            }
        }

        bool isOverwriteFile = false;
        foreach (var file in infoFiles)
        {
            FileAttributes attributs = File.GetAttributes(file.FullName);
            fileName = Path.GetFileName(file.FullName);
            isCopyFile = true;

            if (operation != null)
            {
                // дубль по журналу и файл в источнике
                TaskLogEntity? taskLogs = _taskLogService.GetLogsByTaskId(TaskStep.TaskId)
                                                            .FirstOrDefault(x => x.StepId == TaskStep.StepId &&
                                                                                    x.FileName == fileName);
                if (taskLogs != null)
                {
                    if (operation.FileInSource == FileInSource.OneDay && operation.FileInLog == DoubleInLog.INADAY)
                    {
                        isCopyFile = false;
                    }
                    else if (operation.FileInSource == FileInSource.Always && operation.FileInLog == DoubleInLog.INADAY)
                    {
                        // stop task
                        _taskLogger.StepLog(TaskStep, "Сработал контроль: \"Дублирование по журналу\"", fileName);
                        throw new Exception("Дублирование файла по журналу!");

                    }
                    else if (operation.FileInSource == FileInSource.OneDay && operation.FileInLog == DoubleInLog.NOCTRL)
                    {
                        isCopyFile = false;
                    }
                }

                

                // атрибуты
                if (isCopyFile)
                {
                    switch (operation.FileAttribute)
                    {
                        case AttributeFile.H:
                            isCopyFile = false;
                            if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                isCopyFile = true;
                            }
                            break;
                        case AttributeFile.A:
                            isCopyFile = false;
                            if ((attributs & FileAttributes.Compressed) == FileAttributes.Compressed)
                            {
                                isCopyFile = true;
                            }
                            break;
                        case AttributeFile.R:
                            isCopyFile = false;
                            if ((attributs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                isCopyFile = true;
                            }
                            break;
                        case AttributeFile.X:
                            isCopyFile = true;
                            break;
                        case AttributeFile.V:
                            isCopyFile = false;
                            if ((attributs & FileAttributes.Archive) == FileAttributes.Archive)
                            {
                                isCopyFile = true;
                            }
                            if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                isCopyFile = false;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (isCopyFile)
            {

                // файл в назначении
                var destFileName = fileName;
                if (operation.FileInDestination == FileInDestination.OVR)
                {
                    isOverwriteFile = true;
                }
                else if (operation.FileInDestination == FileInDestination.RNM)
                {
                    destFileName = destFileName + DateTime.Now.ToString("_yyyyMMdd_HHmmss");
                    isOverwriteFile = false;
                }
                else if (operation.FileInDestination == FileInDestination.ERR)
                {
                    isOverwriteFile = false;
                }

                if (TaskStep.Destination.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    HttpClient client = new();
                    byte[] fileBytes = File.ReadAllBytes(file.FullName);
                    HttpContent fileContent = new ByteArrayContent(fileBytes);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"file\"", 
                        FileName = $"\"{fileName}\""
                    };
                }

                fileNameDestination = Path.Combine(TaskStep.Destination, destFileName);
                FileInfo destinationFileInfo = new(fileNameDestination);
                if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
                {
                    destinationFileInfo.IsReadOnly = false;
                    File.Copy(file.FullName, fileNameDestination, isOverwriteFile);
                    _taskLogger.StepLog(TaskStep, "Файл успешно скопирован", fileName);
                    destinationFileInfo.IsReadOnly = true;
                    successFiles.Add(fileName);
                }
                else if (destinationFileInfo.Exists && isOverwriteFile || !destinationFileInfo.Exists)
                {
                    File.Copy(file.FullName, fileNameDestination, isOverwriteFile);
                    _taskLogger.StepLog(TaskStep, "Файл успешно скопирован", fileName);
                    successFiles.Add(fileName);
                }
            }

        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            _mailSender.Send(TaskStep, addresses, successFiles);
        }


        _nextStep?.Execute(bufferFiles);
    }
}
