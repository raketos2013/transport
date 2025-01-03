using FileManager.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
	public class OperationCopyEntity : TaskOperation
	{
		[Display(Name = "Файл есть в источнике")]
		public FileInSource FileInSource { get; set; }
		[Display(Name = "Файл есть в назначении")]
		public FileInDestination FileInDestination { get; set; }
		[Display(Name = "Дубль по журналу")]
		public bool FileInLog { get; set; }
		[Display(Name = "Сортировка")]
		public SortFiles Sort {  get; set; }
		[Display(Name = "Кол-во файлов")]
		public int FilesForProcessing { get; set; }
		[Display(Name = "Аттрибуты")]
		public AttributeFile FileAttribute { get; set; }
    }
}
