using FileManager.Core.Constants;
using FileManager.Core.Entities;
using FileManager.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.RegularExpressions;

namespace FileManager.Core.Operations;

public class Read(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScope scopeFactory)
            : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"ЧТЕНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        await _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileName;
        List<FileInfo> infoFiles = [];
        OperationReadEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        bufferFiles ??= [];

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
        await _taskLogger.StepLog(TaskStep, $"Количество найденных файлов по маске '{TaskStep.FileMask}': {files.Length}");
        bool isReadFile = true;
        if (infoFiles.Count > 0)
        {
            operation = await _operationService.GetReadByStepId(TaskStep.StepId);
            if (operation != null)
            {

                if (operation.InformSuccess)
                {
                    var addressesAsync = await _addresseeService.GetAllAddressees();
                    addresses = addressesAsync.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
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
                    var taskLogsAsync = await _taskLogService.GetLogsByTaskId(TaskStep.TaskId);
                    var taskLogs = taskLogsAsync.FirstOrDefault(x => x.StepId == TaskStep.StepId &&
                                                                     x.FileName == fileName);

                    if (!string.IsNullOrEmpty(operation.FindString))
                    {
                        isReadFile = false;
                        using StreamReader reader = new(file);
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
                                if (line.Contains(operation.FindString))
                                {
                                    isReadFile = true;
                                }
                            }
                        }
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
                            await _taskLogger.StepLog(TaskStep, "Файл успешно добавлен в буфер", fileName);
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
                    await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: несоответствие ожидаемому результату", "", ResultOperation.W);
                    _nextStep = null;
                }

                await _taskLogger.StepLog(TaskStep, $"{bufferFiles.Count} файлов добавлено в BUFFER");

            }
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                _nextStep = null;
            }
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            await _mailSender.Send(TaskStep, addresses, successFiles);
        }

        if (_nextStep != null)
        {
            await _nextStep.Execute(bufferFiles);
        }
    }
}
