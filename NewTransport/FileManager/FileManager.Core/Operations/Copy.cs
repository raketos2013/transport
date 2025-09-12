using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FileManager.Core.Operations;

public class Copy(TaskStepEntity step,
                    TaskOperation? operation,
                    //ITaskLogger taskLogger,
                    //IMailSender mailSender,
                    //IOptions<AuthTokenConfiguration> authTokenConfigurations,
                    //IOperationService operationService,
                    //IAddresseeService addresseeService,
                    //ITaskLogService taskLogService,
                    //IHttpClientFactory httpClientFactory,
                    IServiceScopeFactory scopeFactory)
            : StepOperation(step, operation, 
                            //taskLogger, mailSender, authTokenConfigurations, 
                            //operationService, addresseeService, taskLogService, httpClientFactory,
                            scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"КОПИРОВАНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        await _taskLogger.OperationLog(TaskStep);

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
        await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count}");
        if (infoFiles.Count > 0)
        {
            operation = await _operationService.GetCopyByStepId(TaskStep.StepId);
            //operation = _appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == TaskStep.StepId);

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
                await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
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
                var taskLogsAsync = await _taskLogService.GetLogsByTaskId(TaskStep.TaskId);
                                                           var taskLogs = taskLogsAsync.FirstOrDefault(x => x.StepId == TaskStep.StepId &&
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
                        await _taskLogger.StepLog(TaskStep, "Сработал контроль: \"Дублирование по журналу\"", fileName);
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
                    destFileName += DateTime.Now.ToString("_yyyyMMdd_HHmmss");
                    isOverwriteFile = false;
                }
                else if (operation.FileInDestination == FileInDestination.ERR)
                {
                    isOverwriteFile = false;
                }

                if (TaskStep.Destination.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    
                    var client = _httpClientFactory.CreateClient("MMR");

                    var data = new Dictionary<string, string>
                    {
                        { "grant_type", "client_credentials" },
                        { "client_id", _authTokenConfigurations.Value.ClientId },
                        { "client_secret", _authTokenConfigurations.Value.ClientSecret }
                    };
                    var content = new FormUrlEncodedContent(data);
                    var response = await client.PostAsync(_authTokenConfigurations.Value.TokenUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(jsonResponse);

                        if (tokenResponse != null)
                        {
                            var form = new MultipartFormDataContent();
                            
                                var fileContent = new ByteArrayContent(File.ReadAllBytes(file.FullName));
                                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                                form.Add(fileContent, "file", Path.GetFileName(file.FullName));

                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.access_token);
                                var responseFile = await client.PostAsync(TaskStep.Destination, form);



                                if (responseFile.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("File uploaded successfully.");
                                }
                                else
                                {
                                    Console.WriteLine($"Error uploading file: {response.StatusCode}");
                                }
                            
                        }
                    }
                }
                else
                {
                    fileNameDestination = Path.Combine(TaskStep.Destination, destFileName);
                    FileInfo destinationFileInfo = new(fileNameDestination);
                    if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
                    {
                        destinationFileInfo.IsReadOnly = false;
                        File.Copy(file.FullName, fileNameDestination, isOverwriteFile);
                        await _taskLogger.StepLog(TaskStep, "Файл успешно скопирован", fileName);
                        destinationFileInfo.IsReadOnly = true;
                        successFiles.Add(fileName);
                    }
                    else if (destinationFileInfo.Exists && isOverwriteFile || !destinationFileInfo.Exists)
                    {
                        File.Copy(file.FullName, fileNameDestination, isOverwriteFile);
                        await _taskLogger.StepLog(TaskStep, "Файл успешно скопирован", fileName);
                        successFiles.Add(fileName);
                    }
                }

                    
            }

        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            await _mailSender.Send(TaskStep, addresses, successFiles);
        }


        _nextStep?.Execute(bufferFiles);
    }
}
