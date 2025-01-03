using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
    public class MailGroups
    {
        [Key]
        [Display(Name = "Номер группы")]
        public int Id { get; set; }
        [Display(Name = "Наименование группы")]
        public string Name { get; set; }
        
        List<MailList>? MailLists { get; set; }
    }
}
