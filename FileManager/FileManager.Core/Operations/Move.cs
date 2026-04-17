using FileManager.Core.Constants;
using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FileManager.Core.Operations;

public class Move(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScope scopeFactory)
            : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles, CancellationToken cancellationToken)
    {
        await _taskLogger.StepLog(TaskStep, $"ПЕРЕМЕЩЕНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        await _taskLogger.OperationLog(TaskStep);

        if (!Directory.Exists(TaskStep.Source))
        {
            await _taskLogger.StepLog(TaskStep, "Каталог источника не найден", "", ResultOperation.E);
            throw new DomainException("Каталог источника не найден");
        }

        if (string.IsNullOrEmpty(TaskStep.Destination))
        {
            await _taskLogger.StepLog(TaskStep, "Не заполнено поле \"Назначение\"", "", ResultOperation.E);
            throw new DomainException("Не заполнено поле \"Назначение\"");
        }
        if (!Directory.Exists(TaskStep.Destination))
        {
            await _taskLogger.StepLog(TaskStep, "Каталог назначения не найден", "", ResultOperation.E);
            throw new DomainException("Каталог назначения не найден");
        }


        string[] files = [];
        string fileNameDestination, fileName;
        bool isMoveFile = true;
        List<FileInfo> infoFiles = [];
        OperationMoveEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];
        List<FileLog> logs = [];

        if (TaskStep.FileMask == AppConstants.BUFFER_FILE_MASK)
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
        await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count}");

        if (infoFiles.Count > 0)
        {
            operation = await _operationService.GetMoveByStepId(TaskStep.StepId);
            if (operation != null)
            {
                if (operation.InformSuccess)
                {
                    var addressesAsync = await _addresseeService.GetAllAddressees();
                    addresses = addressesAsync.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                          x.IsActive == true).ToList();
                }
                // сортировка
                infoFiles = SortFilesList(infoFiles, operation.Sort);
                // макс файлов
                infoFiles = MaxFiles(infoFiles, operation.FilesForProcessing);
            }
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                _nextStep = null;
                return;
            }
        }

        //var taskLogsAsync = await _taskLogService.GetLogsByTaskId(TaskStep.TaskId);
        var taskLogs = _taskLogService.GetLogsByTaskId(TaskStep.TaskId)
                                        .Where(x => x.DateTimeLog.Date == DateTime.Now.Date &&
                                                x.StepId == TaskStep.StepId)
                                    .ToList();

        bool isOverwriteFile = false;
        foreach (var file in infoFiles)
        {

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            await FileInUse(file);

            FileAttributes attributs = File.GetAttributes(file.FullName);
            fileName = Path.GetFileName(file.FullName);
            isMoveFile = true;

            if (operation != null)
            {
                // дубль по журналу



                var fileLog = taskLogs.FirstOrDefault(x => x.FileName == fileName);

                if (fileLog != null)
                {
                    if (operation.FileInLog == DoubleInLog.INADAY)
                    {
                        isMoveFile = false;
                        //await _taskLogger.StepLog(TaskStep, "Сработал контроль: \"Дублирование по журналу\"", fileName, ResultOperation.E);
                        logs.Add(new FileLog()
                        {
                            DateTimeLog = DateTime.Now,
                            Text = "Сработал контроль: \"Дублирование по журналу\"",
                            FileName = fileName,
                            ResultOperation = ResultOperation.E
                        });
                        await _taskLogger.LogFiles(TaskStep, logs);
                        throw new Exception("Дублирование файла по журналу!");
                    }
                    else
                    {
                        isMoveFile = true;
                    }
                }

                // атрибуты
                if (isMoveFile)
                {
                    isMoveFile = CheckAttributeFile(operation.FileAttribute, file.FullName);
                }
            }

            if (isMoveFile)
            {
                // файл в назначении
                string destFileName = fileName;
                fileNameDestination = Path.Combine(TaskStep.Destination, destFileName);
                if (File.Exists(fileNameDestination))
                {
                    if (operation == null)
                    {
                        logs.Add(new FileLog()
                        {
                            DateTimeLog = DateTime.Now,
                            Text = "Не удалось переместить файл. Файл уже существует",
                            FileName = fileName,
                            ResultOperation = ResultOperation.E
                        });
                        await _taskLogger.LogFiles(TaskStep, logs);
                        throw new Exception("Файл уже существует!");
                    }
                    else
                    {
                        (isOverwriteFile, destFileName) = ExistInDestination(operation, fileName, logs);
                    }
                }

                fileNameDestination = Path.Combine(TaskStep.Destination, destFileName);
                FileInfo destinationFileInfo = new(fileNameDestination);

                if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
                {
                    destinationFileInfo.IsReadOnly = false;
                    File.Move(file.FullName, fileNameDestination, isOverwriteFile);
                    //await _taskLogger.StepLog(TaskStep, "Файл успешно перемещён", destFileName);
                    logs.Add(new FileLog()
                    {
                        DateTimeLog = DateTime.Now,
                        Text = "Файл успешно перемещён",
                        FileName = destFileName,
                        ResultOperation = ResultOperation.I
                    });
                    destinationFileInfo.IsReadOnly = true;
                    successFiles.Add(destFileName);
                }
                else if (destinationFileInfo.Exists && isOverwriteFile || !destinationFileInfo.Exists)
                {
                    File.Move(file.FullName, fileNameDestination, isOverwriteFile);
                    //await _taskLogger.StepLog(TaskStep, "Файл успешно перемещён", destFileName);
                    logs.Add(new FileLog()
                    {
                        DateTimeLog = DateTime.Now,
                        Text = "Файл успешно перемещён",
                        FileName = destFileName,
                        ResultOperation = ResultOperation.I
                    });
                    successFiles.Add(destFileName);
                }
            }
            if (logs.Count >= 200)
            {
                await _taskLogger.LogFiles(TaskStep, logs);
                logs.Clear();
            }
        }
        if (logs.Count > 0)
        {
            await _taskLogger.LogFiles(TaskStep, logs);
            logs.Clear();
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            await _mailSender.Send(TaskStep, addresses, successFiles);
        }

        if (_nextStep != null)
        {
            await _nextStep.Execute(bufferFiles, cancellationToken);
        }
    }

    public async Task FileInUse(FileInfo file)
    {
        try
        {
            var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            stream.Close();
        }
        catch (Exception)
        {
            await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: файл {file.Name} занят", "", ResultOperation.E);
            throw new Exception("Операция Move: файл недоступен");
        }
    }

    public List<FileInfo> SortFilesList(List<FileInfo> infoFiles, SortFiles sortParam)
    {
        switch (sortParam)
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
                infoFiles = [.. infoFiles.OrderByDescending(o => o.CreationTime)];
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
        return infoFiles;
    }

    public List<FileInfo> MaxFiles(List<FileInfo> infoFiles, int maxFiles)
    {
        if (maxFiles != 0 && maxFiles < infoFiles.Count)
        {
            infoFiles.RemoveRange(maxFiles, infoFiles.Count - maxFiles);
        }
        return infoFiles;
    }

    public bool CheckAttributeFile(AttributeFile fileAttribute, string fileName)
    {
        FileAttributes attributs = File.GetAttributes(fileName);
        bool isMoveFile = true;
        switch (fileAttribute)
        {
            case AttributeFile.H:
                isMoveFile = false;
                if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    isMoveFile = true;
                }
                break;
            case AttributeFile.A:
                isMoveFile = false;
                if ((attributs & FileAttributes.Compressed) == FileAttributes.Compressed)
                {
                    isMoveFile = true;
                }
                break;
            case AttributeFile.R:
                isMoveFile = false;
                if ((attributs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    isMoveFile = true;
                }
                break;
            case AttributeFile.X:
                isMoveFile = true;
                break;
            case AttributeFile.V:
                isMoveFile = false;
                if ((attributs & FileAttributes.Archive) == FileAttributes.Archive)
                {
                    isMoveFile = true;
                }
                if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    isMoveFile = false;
                }
                break;
            default:
                break;
        }
        return isMoveFile;
    }

    public (bool, string) ExistInDestination(OperationMoveEntity operation, string fileName, List<FileLog> logs)
    {
        var destFileName = fileName;
        if (operation.FileInDestination == FileInDestination.OVR)
        {
            //await _taskLogger.StepLog(TaskStep, $"Файл уже существует в Назначении. Файл будет перезаписан", fileName, ResultOperation.W);
            logs.Add(new FileLog()
            {
                DateTimeLog = DateTime.Now,
                Text = "Файл уже существует в Назначении. Файл будет перезаписан",
                FileName = fileName,
                ResultOperation = ResultOperation.W
            });
            return (true, destFileName);
        }
        else if (operation.FileInDestination == FileInDestination.RNM)
        {
            destFileName += DateTime.Now.ToString("_yyyyMMdd_HHmmss");
            //await _taskLogger.StepLog(TaskStep, $"Файл уже существует в Назначении. Переименование в {destFileName}", fileName, ResultOperation.W);
            logs.Add(new FileLog()
            {
                DateTimeLog = DateTime.Now,
                Text = "Файл уже существует в Назначении. Переименование в {destFileName}",
                FileName = fileName,
                ResultOperation = ResultOperation.W
            });
            return (false, destFileName);
        }
        else if (operation.FileInDestination == FileInDestination.ERR)
        {
            //_taskLogger.StepLog(TaskStep, "Не удалось переместить файл. Файл уже существует", fileName, ResultOperation.E);
            logs.Add(new FileLog()
            {
                DateTimeLog = DateTime.Now,
                Text = "Не удалось переместить файл. Файл уже существует",
                FileName = fileName,
                ResultOperation = ResultOperation.E
            });
            _taskLogger.LogFiles(TaskStep, logs);
            throw new Exception("Файл уже существует!");
        }
        return (true, destFileName);
    }
}
