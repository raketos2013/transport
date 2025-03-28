using FileManager.DAL;
using FileManager.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Quartz;
using System.IO;
using System.Text.RegularExpressions;

//using System.Xml.Linq;
using System.Xml;

namespace FileManager_Server
{

    [DisallowConcurrentExecution]
    public class JobForTask : IJob
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobForTask> _logger;
        //private readonly DoSomething _doSomething;
		private readonly TaskOperationService _taskOperationService;
		private Task? _mainTask;
        private Task? _splitTask;

        public JobForTask(IServiceProvider serviceProvider, ILogger<JobForTask> logger, TaskOperationService taskOperationService)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _taskOperationService = taskOperationService;
        }



        public async Task Execute(IJobExecutionContext context)
        {
            if (context.RefireCount > 5)
            {
                _logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - RefireCount > 5");
            }
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
                    {
                        if (dbContext == null)
                        {
                            throw new ArgumentNullException(nameof(dbContext));
                        }

						TaskEntity? taskEntity = dbContext.Task.First(x => x.TaskId == context.JobDetail.Key.Name);
						if (taskEntity is null)
						{
							throw new ArgumentNullException(nameof(taskEntity));
						}
                        List<TaskStepEntity> taskSteps = dbContext.TaskStep.Where(x => x.TaskId == taskEntity.TaskId).OrderBy(x => x.StepNumber).ToList();

                        foreach (var taskStep in taskSteps)
                        {
                            if (taskStep.IsActive)
                            {
                                switch (taskStep.OperationName)
                                {
                                    case FileManager.Domain.Enum.OperationName.Copy:
                                        _taskOperationService.Copy(taskStep);
                                        break;
                                    case FileManager.Domain.Enum.OperationName.Move:
                                        break;
                                    case FileManager.Domain.Enum.OperationName.Read:
                                        break;
                                    case FileManager.Domain.Enum.OperationName.Exist:
                                        break;
                                    case FileManager.Domain.Enum.OperationName.Rename:
                                        break;
                                    case FileManager.Domain.Enum.OperationName.Delete:
                                        break;
                                    default:
                                        break;
                                }
							}
						}





						string archCatalog;
                        string badArchCatalog;
                        FileInfo fileInfo;
                        TaskLogEntity transportTaskLogEntity;
                        //List<TaskOperationEntity> destinationsList = service.TaskOperations.Where(x => x.TaskId == context.JobDetail.Key.Name && x.IsActive == true).ToList();
                        //TaskStatusEntity status = service.TaskStatuse.First(x => x.TaskId == context.JobDetail.Key.Name);
                       
                        //List<MailList> mailList = service.MailLists.Where(x => x.MailGroupsId == taskEntity.Group).ToList();

                        
                        /*if (status != null)
                        {
                            if (DateOnly.FromDateTime(status.DateLastExecute) != DateOnly.FromDateTime(DateTime.Now))
                            {
                                status.CountExecute = 0;
                                status.CountProcessedFiles = 0;
                                status.IsError = false;
                            }
                            status.IsProgress = true;
                            service.TaskStatuse.Update(status);
                            service.SaveChanges();

                            JobDataMap? dataMap = context.JobDetail.JobDataMap;

                            string? jobName = dataMap?.GetString("JobName");
                            string? lastModified = dataMap?.GetString("LastModified");

                            //протоколирование начала выполнения задачи
                            _logger.LogInformation($"{DateTime.Now} задача: {context.JobDetail.Key.Name} {lastModified} - запуск");

                            transportTaskLogEntity = new TaskLogEntity();
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = "0";
                            transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                            transportTaskLogEntity.ResultText = $"<<<Начало работы задачи: {taskEntity.TaskId}>>>";
                            service.TaskLog.Add(transportTaskLogEntity);
                            service.SaveChanges();



                            List<FileInformation> filesSet = new();
                            List<FileInformation> outFilesSet = new();
                            List<FileInformation> sortedFilesSet = new();
                            List<TaskLogEntity> dublFileNames = new();
                            List<string> badfiles = new();

                            //проверка на существование каталога источника
                            List<string> files_raw = new();
                            string[] files_raw_with_sub_mask = [];
                            string sourceCatalog = "";// = String.Format(taskEntity.SourceCatalog, DateTime.Now);

                            *//*if (Directory.Exists(sourceCatalog))
                            {
                                files_raw_with_sub_mask = Directory.GetFiles(sourceCatalog, taskEntity.FileMask);
                                //исключим подмаску файлов
                                string? subMask = null;
                                if (taskEntity.SubMask != null && taskEntity.SubMask.Length > 0 && files_raw_with_sub_mask.Length > 0)
                                {
                                    subMask = taskEntity.SubMask;
                                    foreach (string file in files_raw_with_sub_mask)
                                    {
                                        if (!_doSomething.FitsMask(Path.GetFileName(file), subMask))
                                        {
                                            files_raw.Add(file);
                                        }
                                        else
                                        {
                                            transportTaskLogEntity = new TransportTaskLogEntity();
                                            transportTaskLogEntity.FileName = Path.GetFileName(file);
                                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                            transportTaskLogEntity.OperationId = "0";
                                            transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                                            transportTaskLogEntity.ResultText = $"Файл {Path.GetFileName(file)} исключен по маске {subMask}";
                                            service.TransportTaskLogs.Add(transportTaskLogEntity);
                                            service.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (string file in files_raw_with_sub_mask)
                                    {
                                        files_raw.Add(file);
                                    }
                                }
                                //протоколирование количества найденных файлов для обработки
                                _logger.LogInformation($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - количество найденных файлов по маске {taskEntity.FileMask}: {files_raw.Count}");

                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = taskEntity.FileMask;
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = "0";
                                transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                                transportTaskLogEntity.ResultText = $"Количество найденных файлов по маске {taskEntity.FileMask}: {files_raw.Count}";
                                service.TransportTaskLogs.Add(transportTaskLogEntity);
                                service.SaveChanges();

                            }
                            else
                            {
                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = taskEntity.FileMask;
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = "0";
                                transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Error;
                                transportTaskLogEntity.ResultText = $"Не существует каталог источник: {sourceCatalog}";
                                service.TransportTaskLogs.Add(transportTaskLogEntity);

                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = taskEntity.FileMask;
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = "0";
                                transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Error;
                                transportTaskLogEntity.ResultText = $">>>Работа задачи: {taskEntity.TaskId} завершена<<<";
                                service.TransportTaskLogs.Add(transportTaskLogEntity);

                                service.SaveChanges();

                                status.IsError = true;
                                status.IsProgress = false;
                                status.DateLastExecute = DateTime.Now;
                                status.CountLeftFiles = GetCountFiles(sourceCatalog, taskEntity);
                                service.TaskStatuses.Update(status);
                                service.SaveChanges();

                                _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId} - не существует каталог источник: {taskEntity.SourceCatalog}, работа задачи завершена");

                                if (mailList.Count > 0)
                                {
                                    _doSomething.SendMail($"Ошибка при выполнеии задачи: {jobName}. Не существует каталог источник: {taskEntity.SourceCatalog}", $"Задача: {jobName}", mailList);
                                }

                                return;
                            }*//*
                                                    
                            string[] files = [];
                            *//*if (files_raw != null && files_raw.Count > 0)
                            {

                                //список файлов с датой создания 
                                foreach (var file in files_raw)
                                {
                                    filesSet.Add(new FileInformation { Name = file, Creation = new FileInfo(file).CreationTime });
                                }


                                //выборка порции файлов для обработки
                                //протоколирование
                                */
                            /*if (taskEntity is not null)
                                {
                                    transportTaskLogEntity = new TaskLogEntity();
                                    
                                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                    transportTaskLogEntity.OperationId = "0";
                                    transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                                    transportTaskLogEntity.ResultText = $"Сортировка файлов по дате создания и ограничение количества файлов в порции: {taskEntity.MaxAmountFiles}";
                                    service.TaskLog.Add(transportTaskLogEntity);
                                    service.SaveChanges();


                                    sortedFilesSet = filesSet.OrderBy(o => o.Creation).ToList();
                                    *//*for (int i = 0; i < sortedFilesSet.Count; i++)
                                    {
                                        if (taskEntity.MaxAmountFiles > 0 && i >= taskEntity.MaxAmountFiles)
                                        {
                                            break;
                                        }

                                        Array.Resize(ref files, files.Length + 1);
                                        files[files.GetUpperBound(0)] = sortedFilesSet[i].Name;
                                    }*//*
                                }*//*
                            }*/
                            /*if (files != null && files.Length > 0 && taskEntity is not null)
                            {
                                transportTaskLogEntity = new TaskLogEntity();
                                
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = "0";
                                transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                                transportTaskLogEntity.ResultText = $"Отобрано файлов для обработки с учетом максимального в порции: {files.Length}";
                                service.TaskLog.Add(transportTaskLogEntity);
                                service.SaveChanges();

                                _logger.LogInformation($"{DateTime.Now} задача: {taskEntity.TaskId}, отобрано: {files.Length} файлов");
                                //запуск действий главной задачи
                                *//*_mainTask = Task.Factory.StartNew(() =>
                                {
                                    //задержка выполнения задачи
                                    if (taskEntity.DelayBeforeExecuting.Ticks == 0)
                                    {
                                        int milisec = (taskEntity.DelayBeforeExecuting.Hour * 3600 + taskEntity.DelayBeforeExecuting.Minute * 60 + taskEntity.DelayBeforeExecuting.Second) * 1000;

                                        if (milisec > 0)
                                        {
                                            transportTaskLogEntity = new TransportTaskLogEntity();
                                            transportTaskLogEntity.FileName = taskEntity.FileMask;
                                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                            transportTaskLogEntity.OperationId = "0";
                                            transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                                            transportTaskLogEntity.ResultText = $"Начало задержки выполнения задачи на: {milisec / 1000} секунд";
                                            service.TransportTaskLogs.Add(transportTaskLogEntity);
                                            service.SaveChanges();

                                            Task.Delay(milisec).Wait();
                                        }
                                    }
                                    //запуск подзадач для каждого каталога назначения
                                    foreach (var taskOperationEntity in destinationsList)
                                    {
                                        if (taskOperationEntity.DestinationDirectory.StartsWith("http"))
                                        {
                                            Task.Factory.StartNew(() => _doSomething.UploadFiles(taskEntity, taskOperationEntity, files, ref badfiles), TaskCreationOptions.AttachedToParent);
                                        }
                                        else
                                        {
                                            Task.Factory.StartNew(() => _doSomething.DoCopy(taskEntity, taskOperationEntity, files, ref badfiles), TaskCreationOptions.AttachedToParent);
                                        }
                                    }
                                });*//*
                                //ожидаем выполнения главной задачи
                                _mainTask.Wait();
                                
                            }*/

                            /*status = service.TaskStatuse.First(x => x.TaskId == context.JobDetail.Key.Name);
                            status.CountProcessedFiles += files.Length;
                            status.DateLastExecute = DateTime.Now;
                            status.IsProgress = false;
                            status.CountExecute += 1;
                            status.CountLeftFiles = GetCountFiles(sourceCatalog, taskEntity);
                            service.TaskStatuse.Update(status);
                            service.SaveChanges();

                            if (taskEntity != null)
                            {

                                transportTaskLogEntity = new TaskLogEntity();
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = "0";
                                transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                                transportTaskLogEntity.ResultText = $">>>Работа задачи: {taskEntity.TaskId} завершена<<<";
                                service.TaskLog.Add(transportTaskLogEntity);

                                service.SaveChanges();
                            }*//*



                            _doSomething.DoCopy(taskEntity, files, ref badfiles);



							_logger.LogInformation($"{status.DateLastExecute} задача: {context.JobDetail.Key.Name} {lastModified} - завершение");
                        }*/
                    }
                }
                await Task.CompletedTask;

            }
            catch (Exception ex)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    try
                    {

                        var service = scope.ServiceProvider.GetService<AppDbContext>();
                        if (service != null)
                        {
                            TaskStatusEntity status = service.TaskStatuse.First(x => x.TaskId == context.JobDetail.Key.Name);
                            if (status != null)
                            {
                                status.IsProgress = false;
                                status.IsError = true;
                                status.DateLastExecute = DateTime.Now;
                                service.TaskStatuse.Update(status);
                                service.SaveChanges();
                            }
                        }
                        _logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex.Message}");
                    }
                    catch (Exception ex2)
                    {
                        _logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex2.Message}");
                    }

                }
                throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
            }
        }


        /*private List<FileInformation> SplitFileSet(TaskEntity taskEntity, List<FileInformation> inSet, out List<FileInformation> outSet)
        {
            if (taskEntity.ValueParameterOfSplit is null)
            {
                throw new ArgumentNullException($"{nameof(taskEntity)} ValueParameterOfSplit");
            }

            List<FileInformation> resultSet = [.. inSet];
            outSet = new List<FileInformation>();
            Regex regex = new Regex(@$"{taskEntity.ValueParameterOfSplit}");

            foreach (FileInformation file in inSet)
            {
                if (!taskEntity.IsRegex)
                {

                    if (File.ReadLines(file.Name).Any(line => line.Contains(taskEntity.ValueParameterOfSplit)))
                    {
                        resultSet.Remove(file);
                        outSet.Add(file);
                    }
                }
                else
                {
                    if (File.ReadLines(file.Name).Any(line => regex.Matches(line).Count > 0))
                    {
                        resultSet.Remove(file);
                        outSet.Add(file);
                    }
                }
            }
            return resultSet;
        }*/



        private int GetCountFiles(string sourceCatalog, TaskEntity taskEntity)
        {
            List<string> files_raw = new List<string>();
            if (Directory.Exists(sourceCatalog))
            {
                //var files_raw_with_sub_mask = Directory.GetFiles(sourceCatalog, taskEntity.FileMask);
                //исключим подмаску файлов
                /*string? subMask = null;
                if (taskEntity.SubMask != null && taskEntity.SubMask.Length > 0 && files_raw_with_sub_mask.Length > 0)
                {
                    subMask = taskEntity.SubMask;
                    foreach (string file in files_raw_with_sub_mask)
                    {
                        if (!_doSomething.FitsMask(Path.GetFileName(file), subMask))
                        {
                            files_raw.Add(file);
                        }
                    }
                }
                else
                {
                    foreach (string file in files_raw_with_sub_mask)
                    {
                        files_raw.Add(file);
                    }
                }*/
            }
            return files_raw.Count;
        }


    }
}
