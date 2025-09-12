using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FileManager.Core.Operations;

public class Move(TaskStepEntity step,
                    TaskOperation? operation,
                    //ITaskLogger taskLogger,
                    //IMailSender mailSender,
                    //IOptions<AuthTokenConfiguration> authTokenConfigurations,
                    //IOperationService operationService,
                    //IAddresseeService addresseeService,
                    //ITaskLogService taskLogService,
                    //IHttpClientFactory httpClientFactory
                    IServiceScopeFactory scopeFactory)
            : StepOperation(step, operation, 
                            //taskLogger, mailSender, authTokenConfigurations, 
                            //operationService, addresseeService, taskLogService, httpClientFactory
                            scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"ПЕРЕМЕЩЕНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        await _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileNameDestination, fileName;
        bool isMoveFile = true;
        List<FileInfo> infoFiles = [];
        OperationMoveEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

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
        await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count}");

        if (infoFiles.Count > 0)
        {
            operation = await _operationService.GetMoveByStepId(TaskStep.StepId);
            //_appDbContext.OperationMove.FirstOrDefault(x => x.StepId == TaskStep.StepId);
            // список файлов с атрибутами

            /*foreach (var file in files)
				{
					infoFiles.Add(new FileInfo(file));
				}*/
            if (operation != null)
            {

                if (operation.InformSuccess)
                {
                    var addressesAsync = await _addresseeService.GetAllAddressees();
                    addresses = addressesAsync
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
                await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                throw new Exception("Операция Move: найдено 0 файлов");
            }
        }



        bool isOverwriteFile = false;
        foreach (var file in infoFiles)
        {

            FileInUse(file);


            FileAttributes attributs = File.GetAttributes(file.FullName);
            fileName = Path.GetFileName(file.FullName);
            isMoveFile = true;

            if (operation != null)
            {
                // дубль по журналу
                TaskLogEntity? taskLogs = null;
                //_taskLogService.GetLogsByTaskId(TaskStep.TaskId)
                //                                            .FirstOrDefault(x => x.StepId == TaskStep.StepId &&
                //                                                                    x.FileName == fileName);
                if (taskLogs != null)
                {
                    if (operation.FileInLog == DoubleInLog.INADAY)
                    {
                        isMoveFile = false;
                    }
                    else
                    {
                        isMoveFile = true;
                    }
                }

                // атрибуты
                switch (operation.FileAttribute)
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
            }

            if (isMoveFile)
            {



                var destFileName = fileName;

                // файл в назначении
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


                fileNameDestination = Path.Combine(TaskStep.Destination, destFileName);
                FileInfo destinationFileInfo = new(fileNameDestination);

                

                if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
                {
                    destinationFileInfo.IsReadOnly = false;
                    File.Move(file.FullName, fileNameDestination, isOverwriteFile);
                    await _taskLogger.StepLog(TaskStep, "Файл успешно перемещён", fileName);
                    destinationFileInfo.IsReadOnly = true;
                    successFiles.Add(fileName);
                }
                else if (destinationFileInfo.Exists && isOverwriteFile || !destinationFileInfo.Exists)
                {
                   File.Move(file.FullName, fileNameDestination, isOverwriteFile);
                    await _taskLogger.StepLog(TaskStep, "Файл успешно перемещён", fileName);
                    successFiles.Add(fileName);
                }
            }
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            await _mailSender.Send(TaskStep, addresses, successFiles);
        }

        _nextStep?.Execute(bufferFiles);
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
}
