using FileManager.DAL;
using FileManager.Domain.Entity;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http.Json;
using FileManager.Domain.Enum;


namespace FileManager_Server
{
    public class DoSomething
    {

        /*private readonly ILogger<DoSomething> _logger;
        private readonly IServiceProvider _serviceProvider;
        public DoSomething(ILogger<DoSomething> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public void DoCopy(TaskEntity taskEntity, TaskOperationEntity destinationEntity, string[] files, ref List<string> badfiles)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
                {
                    if (dbContext == null)
                    {
                        throw new ArgumentNullException(nameof(dbContext));
                    }
                    if (destinationEntity is null)
                    {
                        throw new ArgumentNullException(nameof (destinationEntity));
                    }
                    
                    int initialCountBadFiles = badfiles.Count;
                    FileInfo fileInfo;
                    List<MailList> mailListTask = dbContext.MailLists.Where(x => x.MailGroupsId == taskEntity.Group).ToList();
                    List<MailList> mailListDestination = dbContext.MailLists.Where(x => x.MailGroupsId == destinationEntity.Group).ToList();
                    Dictionary<string, string> successCopiedFiles = new Dictionary<string, string>();
                    Dictionary<string, Dictionary<string, string>> notCopiedFiles = new Dictionary<string, Dictionary<string, string>>();
                    TransportTaskLogEntity transportTaskLogEntity;
                    List<TransportTaskLogEntity> dublFileNames = new();
                    string fileName;
                    string fileNameExt;
                    string optionCopy = "";
                    if (destinationEntity.DublDest == FileInDestination.ERR)
                    {
                        optionCopy = "без перезаписи";
                    }
                    else if (destinationEntity.DublDest == FileInDestination.OVR)
                    {
                        optionCopy = "с перезаписью";
                    }
                    else if (destinationEntity.DublDest == FileInDestination.RNM)
                    {
                        optionCopy = "переименовать при существовании";
                    }

                    //протоколирование копирования в файлов в каталог назначения
                    transportTaskLogEntity = new TransportTaskLogEntity();
                    transportTaskLogEntity.FileName = taskEntity.FileMask;
                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                    transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                    transportTaskLogEntity.ResultText = $"Начало копирования ({optionCopy}) в {destinationEntity.OperationId}: {taskEntity.SourceCatalog} => {destinationEntity.DestinationDirectory}";
                    dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                    dbContext.SaveChanges();

                    transportTaskLogEntity = new TransportTaskLogEntity();
                    transportTaskLogEntity.FileName = taskEntity.FileMask;
                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                    transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                    transportTaskLogEntity.ResultText = $"Количество найденных файлов по маске {taskEntity.FileMask}: {files.Length}";
                    dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                    dbContext.SaveChanges();


                    foreach (string s in files)
                    {
                        fileInfo = new FileInfo(s);
                        string destinationDirectory = String.Format(destinationEntity.DestinationDirectory, DateTime.Now, fileInfo.CreationTime);

                        fileName = Path.GetFileName(s);

                        //переименование файла если нужно
                        if (destinationEntity.IsRename && destinationEntity.TemplateFileName is not null && destinationEntity.NewTemplateFileName is not null)
                        {
                            //проверка правильности destinationEntity.TemplateFileName
                            if (Regex.IsMatch(Path.GetFileName(s), destinationEntity.TemplateFileName, RegexOptions.IgnoreCase))
                            {
                                fileName = RenameFileNew(Path.GetFileName(s), destinationEntity.TemplateFileName, destinationEntity.NewTemplateFileName);
                            }
                            else
                            {
                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = Path.GetFileName(s);
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                                transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                                transportTaskLogEntity.ResultText = $"Не соответствие имени файла шаблону для переименования {destinationEntity.TemplateFileName}";
                                dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                                dbContext.SaveChanges();

                                badfiles.Add(s);
                                notCopiedFiles.Add(s, new Dictionary<string, string>() { { destinationDirectory, $"Не соответствие имени файла шаблону для переименования {destinationEntity.TemplateFileName}" } });
                                _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId}; Не соответствие имени файла шаблону для переименования {destinationEntity.TemplateFileName}");
                                continue;
                            }
                        }




                        //контроль на повтор обработки в течении дня
                        dublFileNames = dbContext.TransportTaskLogs.Where(x => x.FileName == fileName && DateOnly.FromDateTime(x.DateTimeLog) == DateOnly.FromDateTime(DateTime.Now)).ToList();
                        if (dublFileNames.Count > 0 && destinationEntity.DublNameJr)
                        {
                            transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = fileName;
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                            transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                            transportTaskLogEntity.ResultText = "Не пройден контроль на повторную обработку за текущий день";
                            dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext.SaveChanges();

                            badfiles.Add(s);
                            notCopiedFiles.Add(s, new Dictionary<string, string>() { { destinationDirectory, "Не пройден контроль на повторную обработку за текущий день" } });
                            _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId}; Не пройден контроль на повторную обработку файла за текущий день: {s} в {destinationEntity.DestinationDirectory}");
                            continue;
                        }


                        try
                        {
                            if (!Directory.Exists(destinationDirectory))
                            {
                                Directory.CreateDirectory(destinationDirectory);

                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = fileName;
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                                transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                                transportTaskLogEntity.ResultText = $"Создан каталог назначения {destinationDirectory}";
                                dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                                dbContext.SaveChanges();
                            }

                            if (destinationEntity.DublDest == FileInDestination.OVR)
                            {
                                File.Copy(s, Path.Combine(destinationDirectory, fileName), true);
                                successCopiedFiles.Add(s, destinationDirectory);
                                _logger.LogInformation($"{DateTime.Now} задача: {taskEntity.TaskId} - {Path.GetFileName(s)} скопирован усшешно в {destinationDirectory}");
                            }
                            else if (destinationEntity.DublDest == FileInDestination.ERR)
                            {
                                File.Copy(s, Path.Combine(destinationDirectory, fileName), false);
                                successCopiedFiles.Add(fileName, destinationDirectory);
                                _logger.LogInformation($"{DateTime.Now} задача: {taskEntity.TaskId} - {Path.GetFileName(s)} скопирован усшешно в {destinationDirectory}");
                            }
                            else if (destinationEntity.DublDest == FileInDestination.RNM)
                            {
                                FileInfo fileInfoInDestination = new FileInfo(Path.Combine(destinationDirectory, fileName));
                                if (fileInfoInDestination.Exists)
                                {
                                    fileNameExt = Path.Combine(destinationDirectory, string.Concat(fileName, DateTime.Now.ToString("_ddMMyyyy_HHmmss")));

                                    transportTaskLogEntity = new TransportTaskLogEntity();
                                    transportTaskLogEntity.FileName = fileName;
                                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                    transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                                    transportTaskLogEntity.ResultOperation = ResultOperation.Rename;
                                    transportTaskLogEntity.ResultText = $"Файл {fileName} существует в {destinationDirectory} и будет переименован в {Path.GetFileName(fileNameExt)}";
                                    dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                                    dbContext.SaveChanges();

                                    File.Copy(s, fileNameExt, false);
                                    _logger.LogInformation($"{DateTime.Now} задача: {taskEntity.TaskId} - {fileName} скопирован усшешно в {destinationEntity} с именем {Path.GetFileName(fileNameExt)} ");
                                }
                                else
                                {
                                    fileNameExt = Path.Combine(destinationDirectory, fileName);
                                    File.Copy(s, fileNameExt, false);
                                    _logger.LogInformation($"{DateTime.Now} задача: {taskEntity.TaskId} - {fileName} скопирован усшешно в {destinationEntity}");
                                }
                            }

                            transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = fileName;
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                            transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                            transportTaskLogEntity.ResultText = $"Файл успешно скопирован в {destinationDirectory}";
                            dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = fileName;
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                            transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                            transportTaskLogEntity.ResultText = $"Ошибка при копировании файла: {ex.Message}";
                            dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext.SaveChanges();
                            badfiles.Add(s);
                            notCopiedFiles.Add(s, new Dictionary<string, string>() { { destinationDirectory, ex.Message } });
                            _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId}; Ошибка при копировании файла:{s} в {destinationEntity.DestinationDirectory} - {ex.Message}");
                        }
                    }
                    SendMailForDestination(successCopiedFiles, $"Назначение: {destinationEntity.OperationId}", "выполнена успешно", destinationEntity, mailListDestination);
                    if (notCopiedFiles.Count > 0)
                    {
                        SendErrorMailForDestination(notCopiedFiles, $"Назначение: {destinationEntity.OperationId}", "завершена с ошибкой", destinationEntity, mailListTask);
                    }
                    transportTaskLogEntity = new TransportTaskLogEntity();
                    transportTaskLogEntity.FileName = taskEntity.FileMask;
                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                    transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                    transportTaskLogEntity.ResultText = $"Завершение копирования ({optionCopy}) в назначение {destinationEntity.OperationId}";
                    dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                    dbContext.SaveChanges();

                }
            }
        }


        private string RenameFileNew(string filename, string old_pattern, string new_pattern)
        {
            StringBuilder stringBuilder = new StringBuilder(new_pattern);

            if (Regex.IsMatch(filename, old_pattern, RegexOptions.IgnoreCase))
            {
                Regex regex = new Regex(old_pattern, RegexOptions.IgnoreCase);

                MatchCollection matches = regex.Matches(filename);
                foreach (Match match in matches)
                {
                    foreach (Group item in match.Groups)
                    {
                        if (item.Name != "0")
                        {
                            stringBuilder.Replace($"[{item.Name}]", $"{item.Value}");
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }





        public void DoCopyArchFiles(string[] files, List<string> badfiles, TaskEntity taskEntity)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
                {

                    string archCatalog;
                    string badArchCatalog;

                    foreach (var file in files)
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            archCatalog = String.Format(taskEntity.ArchiveCatalog, DateTime.Now, fileInfo.CreationTime);
                            badArchCatalog = String.Format(taskEntity.BadArchiveCatalog, DateTime.Now, fileInfo.CreationTime);
                            TransportTaskLogEntity transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = Path.GetFileName(file);
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = "0";

                            if (!badfiles.Contains(file))
                            {
                                if (taskEntity.IsDeleteSource)
                                {


                                    File.Move(file, Path.Combine(archCatalog, Path.GetFileName(file)), true);

                                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                                    transportTaskLogEntity.ResultText = $"Файл {Path.GetFileName(file)} успешно перемещен в архив: {archCatalog}";
                                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                    _logger.LogInformation($"{transportTaskLogEntity.DateTimeLog} задача: {taskEntity.TaskId} - {Path.GetFileName(file)} успешно перемещен в архив: {archCatalog}");
                                }
                                else
                                {
                                    File.Copy(file, Path.Combine(archCatalog, Path.GetFileName(file)), true);

                                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                                    transportTaskLogEntity.ResultText = $"Файл {Path.GetFileName(file)} успешно скопирован в архив: {archCatalog}";
                                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                    _logger.LogInformation($"{transportTaskLogEntity.DateTimeLog} задача: {taskEntity.TaskId} - {Path.GetFileName(file)} успешно скопирован в архив: {archCatalog}");

                                }
                            }
                            else
                            {
                                if (taskEntity.IsDeleteSource)
                                {
                                    File.Move(file, Path.Combine(badArchCatalog, Path.GetFileName(file)), true);

                                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                                    transportTaskLogEntity.ResultText = $"Файл {Path.GetFileName(file)} перемещен в архив ошибок: {badArchCatalog}";
                                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                    _logger.LogInformation($"{transportTaskLogEntity.DateTimeLog} задача: {taskEntity.TaskId} - {Path.GetFileName(file)} перемещен в архив ошибок: {badArchCatalog}");

                                }
                                else
                                {
                                    File.Copy(file, Path.Combine(badArchCatalog, Path.GetFileName(file)), true);

                                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                                    transportTaskLogEntity.ResultText = $"Файл {Path.GetFileName(file)} скопирован в архив ошибок: {badArchCatalog}";
                                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                    _logger.LogInformation($"{transportTaskLogEntity.DateTimeLog} задача: {taskEntity.TaskId} - {Path.GetFileName(file)} скопирован в архив ошибок: {badArchCatalog}");

                                }
                            }

                            dbContext?.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext?.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            TransportTaskLogEntity transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = Path.GetFileName(file);
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = "0";
                            transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                            transportTaskLogEntity.ResultText = ex.Message;
                            dbContext?.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext?.SaveChanges();

                            _logger.LogError(@$"{DateTime.Now} задача: {taskEntity.TaskId} - ошибка архивирования: {Path.GetFileName(file)}");
                        }
                    }




                }
            }
        }



        public void SendMail(string message, string Subject, List<MailList> recipients, string? attachFile = null)
        {
            try
            {
                if (recipients.Count > 0)
                {
                    MailMessage mail = new MailMessage()
                    {
                        From = new MailAddress("FileTransport@lotus.asb.by")
                    };

                    foreach (var recp in recipients)
                    {
                        var regexEmail = new Regex(@"\w.+@lotus\.asb\.by");
                        if (regexEmail.IsMatch(recp.EMail))
                        {
                            mail.To.Add(new MailAddress(recp.EMail));
                        }
                    }
                    mail.Subject = Subject;
                    mail.Body = message;

                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        mail.Attachments.Add(new Attachment(attachFile));
                    }

                    SmtpClient client = new SmtpClient()
                    {
                        Host = "lotus.asb.by",
                        Port = 25,
                        EnableSsl = false,
                        UseDefaultCredentials = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    if (mail.To.Count > 0)
                    {
                        client.Send(mail);
                    }
                    mail.Dispose();
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при отправке e-mail: {ex.Message}");
            }
        }



        public void SendMailForDestination(Dictionary<string, string> copiedFiles, string Subject, string caption, TaskOperationEntity destinationEntity, List<MailList> recipients, string? attachFile = null)
        {
            StringBuilder message = new StringBuilder();
            try
            {
                if (recipients.Count > 0)
                {
                    MailMessage mail = new MailMessage()
                    {
                        From = new MailAddress("FileTransport@lotus.asb.by")
                    };
                    mail.IsBodyHtml = true;

                    foreach (var recp in recipients)
                    {
                        var regexEmail = new Regex(@"\w.+@lotus\.asb\.by");
                        if (regexEmail.IsMatch(recp.EMail))
                        {
                            mail.To.Add(new MailAddress(recp.EMail));
                        }
                    }
                    mail.Subject = Subject;
                    message.Append("<table>");
                    message.Append($"<tr><th colspan=3>Операция копирование {caption} ( Задача:{destinationEntity.TaskId}, назначение {destinationEntity.OperationId} )</th></tr>");
                    message.Append("<tr><td>Источник</td><td>Назначение</td><td>Файл</td></tr>");
                    foreach (var file in copiedFiles)
                    {
                        message.Append($"<tr><td>{Path.GetDirectoryName(file.Key)}</td><td>{file.Value}</td><td>{Path.GetFileName(file.Key)}</td></tr>");
                    }
                    message.Append("</table>");
                    message.Append($"<b>{destinationEntity.AdditionalText}</b>");
                    mail.Body = message.ToString();

                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        mail.Attachments.Add(new Attachment(attachFile));
                    }

                    SmtpClient client = new SmtpClient()
                    {
                        Host = "lotus.asb.by",
                        Port = 25,
                        EnableSsl = false,
                        UseDefaultCredentials = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    if (mail.To.Count > 0)
                    {
                        client.Send(mail);
                    }
                    mail.Dispose();
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка отправки e-mail: {ex.Message}");
            }
        }


        public void SendErrorMailForDestination(Dictionary<string, Dictionary<string, string>> notCopiedFiles, string Subject, string caption, TaskOperationEntity destinationEntity, List<MailList> recipients, string? attachFile = null)
        {
            StringBuilder message = new StringBuilder();
            try
            {
                if (recipients.Count > 0)
                {
                    MailMessage mail = new MailMessage()
                    {
                        From = new MailAddress("FileTransport@lotus.asb.by")
                    };
                    mail.IsBodyHtml = true;

                    foreach (var recp in recipients)
                    {
                        var regexEmail = new Regex(@"\w.+@lotus\.asb\.by");
                        if (regexEmail.IsMatch(recp.EMail))
                        {
                            mail.To.Add(new MailAddress(recp.EMail));
                        }
                    }
                    mail.Subject = Subject;
                    message.Append("<table>");
                    message.Append($"<tr><th colspan=4>Операция копирование {caption} ( Задача:{destinationEntity.TaskId}, назначение {destinationEntity.OperationId} )</th></tr>");
                    message.Append("<tr><td>Источник</td><td>Назначение</td><td>Файл</td><td>Ошибка</td></tr>");
                    foreach (var file in notCopiedFiles)
                    {
                        foreach (var dest in file.Value)
                        {
                            message.Append($"<tr><td>{Path.GetDirectoryName(file.Key)}</td><td>{dest.Key}</td><td>{Path.GetFileName(file.Key)}</td><td>{dest.Value}</td></tr>");
                        }
                    }
                    message.Append("</table>");
                    message.Append($"<b>{destinationEntity.AdditionalText}</b>");
                    mail.Body = message.ToString();

                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        mail.Attachments.Add(new Attachment(attachFile));
                    }

                    SmtpClient client = new SmtpClient()
                    {
                        Host = "lotus.asb.by",
                        Port = 25,
                        EnableSsl = false,
                        UseDefaultCredentials = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    if (mail.To.Count > 0)
                    {
                        client.Send(mail);
                    }
                    mail.Dispose();
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Mail.Send: {ex.Message}");
            }
        }




        public void UploadFiles(TaskEntity taskEntity, TaskOperationEntity destinationEntity, string[] files, ref List<string> badfiles)
        {


            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
                {
                    if (dbContext == null)
                    {
                        throw new ArgumentNullException(nameof(dbContext));
                    }
                    if (destinationEntity is null)
                    {
                        throw new ArgumentNullException(nameof (destinationEntity));
                    }
                    int initialCountBadFiles = badfiles.Count;
                    List<MailList> mailListTask = dbContext.MailLists.Where(x => x.MailGroupsId == taskEntity.Group).ToList();

                    FileInfo fileInfo;
                    List<MailList> mailListDestination = dbContext.MailLists.Where(x => x.MailGroupsId == destinationEntity.Group).ToList();
                    Dictionary<string, string> successCopiedFiles = new Dictionary<string, string>();
                    Dictionary<string, Dictionary<string, string>> notCopiedFiles = new Dictionary<string, Dictionary<string, string>>();
                    TransportTaskLogEntity transportTaskLogEntity;
                    List<TransportTaskLogEntity> dublFileNames = new();


                    //протоколирование копирования в файлов в каталог назначения
                    transportTaskLogEntity = new TransportTaskLogEntity();
                    transportTaskLogEntity.FileName = taskEntity.FileMask;
                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                    transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                    transportTaskLogEntity.ResultText = $"Начало отправки  файлов {destinationEntity.OperationId}: {taskEntity.SourceCatalog} => {destinationEntity.DestinationDirectory}";
                    dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                    dbContext.SaveChanges();

                    transportTaskLogEntity = new TransportTaskLogEntity();
                    transportTaskLogEntity.FileName = taskEntity.FileMask;
                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                    transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                    transportTaskLogEntity.ResultText = $"Количество найденных файлов по маске {taskEntity.FileMask}: {files.Length}";
                    dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                    dbContext.SaveChanges();



                    string fileName;
                    foreach (string s in files)
                    {

                        fileInfo = new FileInfo(s);
                        string destinationDirectory = String.Format(destinationEntity.DestinationDirectory);

                        fileName = Path.GetFileName(s);

                        //переименование файла если нужно
                        if (destinationEntity.IsRename && destinationEntity.TemplateFileName is not null && destinationEntity.NewTemplateFileName is not null)
                        {
                            //проверка правильности destinationEntity.TemplateFileName
                            if (Regex.IsMatch(Path.GetFileName(s), destinationEntity.TemplateFileName, RegexOptions.IgnoreCase))
                            {
                                fileName = RenameFileNew(Path.GetFileName(s), destinationEntity.TemplateFileName, destinationEntity.NewTemplateFileName);
                            }
                            else
                            {
                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = Path.GetFileName(s);
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                                transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                                transportTaskLogEntity.ResultText = $"Не соответствие имени файла шаблону для переименования {destinationEntity.TemplateFileName}";
                                dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                                dbContext.SaveChanges();

                                badfiles.Add(s);
                                notCopiedFiles.Add(s, new Dictionary<string, string>() { { destinationDirectory, $"Не соответствие имени файла шаблону для переименования {destinationEntity.TemplateFileName}" } });
                                _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId}; Не соответствие имени файла шаблону для переименования {destinationEntity.TemplateFileName}");
                                continue;
                            }
                        }


                        //контроль на повтор обработки в течении дня
                        dublFileNames = dbContext.TransportTaskLogs.Where(x => x.FileName == fileName && DateOnly.FromDateTime(x.DateTimeLog) == DateOnly.FromDateTime(DateTime.Now)).ToList();
                        if (dublFileNames.Count > 0 && destinationEntity.DublNameJr)
                        {
                            transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = fileName;
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                            transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                            transportTaskLogEntity.ResultText = "Не пройден контроль на повторную обработку за текущий день";
                            dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext.SaveChanges();

                            badfiles.Add(s);
                            notCopiedFiles.Add(s, new Dictionary<string, string>() { { destinationDirectory, "Не пройден контроль на повторную обработку за текущий день" } });
                            _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId}; Не пройден контроль на повторную обработку файла за текущий день: {s} в {destinationEntity.DestinationDirectory}");
                            continue;
                        }




                        try
                        {

                            var result = UploadAsync(destinationEntity.DestinationDirectory, s, fileName);
                            if (result.IsSuccessStatusCode)
                            {
                                successCopiedFiles.Add(s, destinationDirectory);


                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = Path.GetFileName(fileName);
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                                transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                                transportTaskLogEntity.ResultText = $"Файл {fileName} успешно отправлен";
                                dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                badfiles.Add(s);

                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = Path.GetFileName(fileName);
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                                transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                                using (var reader = new StreamReader(result.Content.ReadAsStream()))
                                {
                                    String message = reader.ReadToEnd();
                                    transportTaskLogEntity.ResultText = $"Код ошибки: {result.StatusCode} {message}";
                                    _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId} - {s}, код ошибки: {result.StatusCode} {message}");
                                    notCopiedFiles.Add(s, new Dictionary<string, string>() { { destinationDirectory, message } });
                                }
                                dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                                dbContext.SaveChanges();
                            }

                        }
                        catch (Exception ex)
                        {
                            transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = Path.GetFileName(fileName);
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                            transportTaskLogEntity.ResultOperation = ResultOperation.Error;
                            transportTaskLogEntity.ResultText = $"Ошибка отправки файла {fileName} - {ex.Message}";
                            dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext.SaveChanges();
                            badfiles.Add(s);
                            _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId} - {s} {ex.Message}");

                        }
                    }


                    SendMailForDestination(successCopiedFiles, $"Назначение: {destinationEntity.OperationId}", "выполнена успешно", destinationEntity, mailListDestination);
                    if (notCopiedFiles.Count > 0)
                    {
                        SendErrorMailForDestination(notCopiedFiles, $"Назначение: {destinationEntity.OperationId}", "завершена с ошибкой", destinationEntity, mailListTask);
                    }
                    transportTaskLogEntity = new TransportTaskLogEntity();
                    transportTaskLogEntity.FileName = taskEntity.FileMask;
                    transportTaskLogEntity.DateTimeLog = DateTime.Now;
                    transportTaskLogEntity.TaskId = taskEntity.TaskId;
                    transportTaskLogEntity.OperationId = destinationEntity.OperationId;
                    transportTaskLogEntity.ResultOperation = ResultOperation.Success;
                    transportTaskLogEntity.ResultText = $"Завершение отправки файлов в назначение {destinationEntity.OperationId}";
                    dbContext.TransportTaskLogs.Add(transportTaskLogEntity);
                    dbContext.SaveChanges();

                }
            }
        }


        private HttpResponseMessage UploadAsync(string url, string file, string filename)
        {

            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())

                {
                    using (var fileStreamContent = new StreamContent(new FileStream(file, FileMode.Open)))
                    {
                        var url_api = new Uri(url);

                        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        client.BaseAddress = url_api;

                        formData.Add(fileStreamContent, "file", filename);

                        var response = client.PostAsync(url_api.AbsolutePath, formData).Result;
                        return response;
                    }
                }
            }
        }





        public bool FitsMask(string sFileName, string sFileMask)
        {
            string[] fileMasks = sFileMask.Split([';']);

            foreach (string fileMask in fileMasks)
            {
                Regex mask = new Regex(fileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."), RegexOptions.IgnoreCase);
                if (mask.IsMatch(sFileName))
                {
                    return true;
                }
            }
            return false;
        }






        public void DoMoveSplit(TaskEntity taskEntity, List<FileInformation> files, ref List<string> badfiles)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
                {
                    if (taskEntity is null)
                    {
                        throw new ArgumentNullException(nameof(taskEntity));
                    }                    
                    if (taskEntity.TmpCatalog is null)
                    {
                        throw new ArgumentNullException($"{nameof(taskEntity)} TmpCatalog");
                    }
                    string tmpCatalog;
                    FileInfo fileInfo;
                    TransportTaskLogEntity transportTaskLogEntity = new TransportTaskLogEntity();

                    foreach (var file in files)
                    {
                        try
                        {
                            fileInfo = new FileInfo(file.Name);
                            tmpCatalog = String.Format(taskEntity.TmpCatalog, DateTime.Now, fileInfo.CreationTime);
                            if (!Directory.Exists(tmpCatalog))
                            {
                                Directory.CreateDirectory(tmpCatalog);

                                transportTaskLogEntity = new TransportTaskLogEntity();
                                transportTaskLogEntity.FileName = Path.GetFileName(file.Name);
                                transportTaskLogEntity.DateTimeLog = DateTime.Now;
                                transportTaskLogEntity.TaskId = taskEntity.TaskId;
                                transportTaskLogEntity.OperationId = "0";
                                transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                                transportTaskLogEntity.ResultText = $"Создан каталог {tmpCatalog} для перемещения файлов исключенных по содержимому";
                                dbContext?.TransportTaskLogs.Add(transportTaskLogEntity);
                                dbContext?.SaveChanges();
                            }
                            File.Move(file.Name, Path.Combine(tmpCatalog, Path.GetFileName(file.Name)), true);
                            _logger.LogInformation($"{DateTime.Now} задача: {taskEntity.TaskId} - {Path.GetFileName(file.Name)} усшешно перемещен в {tmpCatalog}");

                            transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = Path.GetFileName(file.Name);
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = "0";
                            transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Success;
                            transportTaskLogEntity.ResultText = $"Файл {Path.GetFileName(file.Name)} усшешно перемещен в {tmpCatalog}";
                            dbContext?.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext?.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            transportTaskLogEntity = new TransportTaskLogEntity();
                            transportTaskLogEntity.FileName = Path.GetFileName(file.Name);
                            transportTaskLogEntity.DateTimeLog = DateTime.Now;
                            transportTaskLogEntity.TaskId = taskEntity.TaskId;
                            transportTaskLogEntity.OperationId = "0";
                            transportTaskLogEntity.ResultOperation = FileManager.Domain.Enum.ResultOperation.Error;
                            transportTaskLogEntity.ResultText = $"Ошибка перемещения файла {Path.GetFileName(file.Name)} в {String.Format(taskEntity.TmpCatalog, DateTime.Now, new FileInfo(file.Name).CreationTime)} -> {ex.Message}";
                            dbContext?.TransportTaskLogs.Add(transportTaskLogEntity);
                            dbContext?.SaveChanges();
                            badfiles.Add(file.Name);
                            _logger.LogError($"{DateTime.Now} задача: {taskEntity.TaskId} - ошибка перемещения файла {file.Name} в {String.Format(taskEntity.TmpCatalog, DateTime.Now, new FileInfo(file.Name).CreationTime)} -> {ex.Message}");

                        }
                    }
                }
            }
        }

*/






    }
}
