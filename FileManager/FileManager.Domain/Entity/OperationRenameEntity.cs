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
		public string Pattern { get; set; }
		
	}
}
