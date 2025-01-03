using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
    public class AddresseeEntity
	{
		[Key]
		[Display(Name = "Табельный номер")]
		public string PersonalNumber { get; set; }
		[Display(Name = "Номер группы")]
        public int MailGroupId { get; set; }
		[ForeignKey(nameof(MailGroupId))]
		public AddresseeGroupEntity? MailGroup { get; set; }
		[Display(Name = "Адрес электронной почты")]
        public string EMail { get; set; }
		[Display(Name = "ФИО")]
		public string Fio {  get; set; }
        
		[Display(Name = "Структурное подразделение")]
		public string StructuralUnit { get; set; }
        

        
    }
}
