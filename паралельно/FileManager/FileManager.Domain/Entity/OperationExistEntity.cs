using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
	public class OperationExistEntity : Operation
	{
		[Display(Name = "Ожидаемый результат")]
		public string ExpectedResult { get; set; }
		[Display(Name = "Прервать задачу")]
		public bool BreakTaskAfterError { get; set; }
	}
}
