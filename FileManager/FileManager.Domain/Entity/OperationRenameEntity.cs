using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
	public class OperationRenameEntity : TaskOperation
	{
		[Display(Name = "Шаблон имени файла")]
		public string OldPattern { get; set; }
        [Display(Name = "Шаблон нового имени файла")]
        public string NewPattern { get; set; }

    }
}
