using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.MailSender
{
    public interface IMailSender
    {
        void Send(TaskStepEntity step, List<AddresseeEntity> addresses, List<string> files);
    }
}
