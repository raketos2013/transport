using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Loggers;
using Microsoft.EntityFrameworkCore;

namespace FileManager_Server.Operations
{
    public class Copy : StepOperation
    {
        public Copy(TaskStepEntity step, TaskOperation operation, ITaskLogger taskLogger, AppDbContext appDbContext)
            : base(step, operation, taskLogger, appDbContext)
        {
        }


        public override void Execute()
        {
            _taskLogger.StepLog(TaskStep, $"Копирование: {TaskStep.Source} => {TaskStep.Destination}");
            _taskLogger.OperationLog(TaskStep);

            string[] files = [];
            string fileNameDestination, fileName;
            bool isCopyFile = true;

            files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
            _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Count()}");
            OperationCopyEntity operation = _appDbContext.OperationCopy.First(x => x.StepId == TaskStep.StepId);

            // список файлов с атрибутами
            List<FileInfo> infoFiles = new List<FileInfo>();
            foreach (var file in files)
            {
                infoFiles.Add(new FileInfo(file));
            }

            if (operation != null)
            {
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
            bool isOverwriteFile = false;
            foreach (var file in infoFiles)
            {
                FileAttributes attributs = File.GetAttributes(file.FullName);

                fileName = Path.GetFileName(file.FullName);

                if (operation != null)
                {
                    isCopyFile = true;

                    // дубль по журналу и файл в источнике
                    TaskLogEntity taskLogs = _appDbContext.TaskLog.FirstOrDefault(x => x.StepId == TaskStep.StepId &&
                                                                                    x.FileName == fileName);
                    if (taskLogs != null)
                    {
                        if (operation.FileInSource == FileInSource.OneDay && operation.FileInLog == true)
                        {
                            isCopyFile = false;
                        }
                        else if (operation.FileInSource == FileInSource.Always && operation.FileInLog == true)
                        {
                            // stop task

                        }
                        else if (operation.FileInSource == FileInSource.OneDay && operation.FileInLog == false)
                        {
                            isCopyFile = false;
                        }
                    }

                    // файл в назначении
                    if (operation.FileInDestination == FileInDestination.OVR)
                    {
                        isOverwriteFile = true;
                    }
                    else if (operation.FileInDestination == FileInDestination.RNM)
                    {
                        isOverwriteFile = true;
                    }
                    else if (operation.FileInDestination == FileInDestination.ERR)
                    {
                        isOverwriteFile = false;
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
                else
                {
                    isCopyFile = true;
                }

                if (isCopyFile)
                {
                    fileNameDestination = Path.Combine(TaskStep.Destination, fileName);
                    FileInfo destinationFileInfo = new FileInfo(fileNameDestination);
                    if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
                    {
                        destinationFileInfo.IsReadOnly = false;
                        File.Copy(file.FullName, fileNameDestination, isOverwriteFile);
                        _taskLogger.StepLog(TaskStep, "Файл успешно скопирован", fileName);
                        destinationFileInfo.IsReadOnly = true;
                    }
                    else
                    {
                        File.Copy(file.FullName, fileNameDestination, isOverwriteFile);
                        _taskLogger.StepLog(TaskStep, "Файл успешно скопирован", fileName);
                    }
                }

            }

            if (_nextStep != null)
            {
                _nextStep.Execute();
            }
        }
    }
}
