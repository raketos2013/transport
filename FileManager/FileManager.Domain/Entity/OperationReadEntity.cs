using FileManager.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
	public class OperationReadEntity : TaskOperation
	{

		[Display(Name = "Файл есть в источнике")]
		public FileInSource FileInSource { get; set; }
		[Display(Name = "Кодировка")]
		public Encode Encode {  get; set; }
		[Display(Name = "Поиск по шаблону")]
		public bool SearchRegex { get; set; }
		[Display(Name = "Строка поиска")]
		public string FindString { get; set; }
		[Display(Name = "Ожидаемый результат")]
		public ExpectedResult ExpectedResult { get; set; }
		[Display(Name = "Прервать задачу")]
		public bool BreakTaskAfterError { get; set; }
		
	}
}
