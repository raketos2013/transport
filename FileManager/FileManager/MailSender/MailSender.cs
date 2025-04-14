using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.MailSender
{
    public class MailSender : IMailSender
    {
        public void Send()
        {
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress("FileTransport@lotus.asb.by")
            };




            throw new NotImplementedException();
        }
    }
}
