using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
    public class MailList
    {
        [Display(Name = "Номер группы")]
        public int MailGroupsId { get; set; }
        [Display(Name = "Адрес электронной почты")]
        public string EMail { get; set; }

        public MailGroups? MailGroups { get; set; }

        
    }
}
