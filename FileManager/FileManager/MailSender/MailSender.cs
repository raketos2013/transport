using FileManager.Domain.Entity;
using FileManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileManager_Server.MailSender
{
    public class MailSender : IMailSender
    {
        public void Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files)
        {
            StringBuilder message = new();
            try
            {
                MailMessage mail = new()
                {
                    From = new MailAddress("FileTransport@lotus.asb.by"),
                    IsBodyHtml = true
                };
                foreach (var item in addresses)
                {
                    var regexEmail = new Regex(@"\w.+@lotus\.asb\.by");
                    if (regexEmail.IsMatch(item.EMail))
                    {
                        mail.To.Add(new MailAddress(item.EMail));
                    }
                }
                mail.Subject = "Транспорт файлов";

                message.Append("---------------------------------------------------------------------------------------");
                message.Append($"<br><b>Операция {step.OperationName.GetDescription()} выполнена успешно ( Задача: {step.TaskId}, Шаг: {step.StepNumber})</b>");
                message.Append("<br>---------------------------------------------------------------------------------------");
                message.Append($"<br><b>Источник:</b> {step.Source}");
                message.Append($"<br><b>Назначение:</b> {step.Destination}");
                message.Append("<br><b>Данные:</b>");
                foreach (var item in files)
                {
                    message.Append($"<br>{item.ToString()}");
                }

                mail.Body = message.ToString();


                SmtpClient client = new()
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
            catch (Exception)
            {
                Console.WriteLine("Ошбика при отправке почты");
            }

        }
    }
}
