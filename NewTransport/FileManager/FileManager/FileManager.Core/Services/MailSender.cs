using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Utilities;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace FileManager.Core.Services;

public class MailSender : IMailSender
{
    public async Task Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files)
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

    public async Task SendOffSteps(string taskId, List<AddresseeEntity> addresses, List<int> numberSteps)
    {
        if (numberSteps.Count == 0)
        {
            return;
        } 
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
            message.Append($"<br><b>Задача {taskId} запущена:</b>");
            message.Append("<br>---------------------------------------------------------------------------------------");
            message.Append($"<br><b>Вылюченные шаги:</b>");
            foreach (var stepNumber in numberSteps)
            {
                message.Append($"<br>{stepNumber.ToString()}");
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
