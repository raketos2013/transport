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
	public class TaskEntity
	{
		/*public string Id { get; set; }
        public string Name { get; set; }
        public TypeAction Action { get; set; }*/

		[Key]
		[Display(Name = "Идентификатор")]
		[Required(ErrorMessage = "Обязательное поле")]
		public string TaskId { get; set; }
		[Display(Name = "Наименование")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Name { get; set; }
		[Display(Name = "Время начала")]
		[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
		public TimeOnly TimeBegin { get; set; }
		[Display(Name = "Время окончания")]
		[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
		public TimeOnly TimeEnd { get; set; }
		[Display(Name = "Дни недели")]
		public DayActive DayActive { get; set; }
		[Display(Name = "Группа рассылки")]
		public int? Group { get; set; }
		[Display(Name = "Активно")]
		public bool IsActive { get; set; }
		[Display(Name = "Входящий каталог")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string SourceCatalog { get; set; }
		[Display(Name = "Маска файлов")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string FileMask { get; set; }
		[Display(Name = "Интервал обработки")]
		[DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
		public TimeOnly Delay { get; set; }
		[Display(Name = "Каталог архива")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string ArchiveCatalog { get; set; }
		[Display(Name = "Каталог ошибок")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string BadArchiveCatalog { get; set; }
		[Display(Name = "Удалять после обработки")]
		public bool IsDeleteSource { get; set; }
		[Display(Name = "Макс. файлов в порции")]
        [Required(ErrorMessage = "Обязательное поле")]
        public int MaxAmountFiles { get; set; }
		[Display(Name = "Контроль на повтор обработки")]
		public bool DublNameJr { get; set; }
		[Display(Name = "Последнее изменение")]
		public DateTime LastModified { get; set; }



		[Display(Name = "Исключить по содержимому")]
		public bool SplitFiles { get; set; }
		[Display(Name = "Регулярное выражение")]
		public bool IsRegex { get; set; }
		[Display(Name = "Строка поиска")]
		public string? ValueParameterOfSplit { get; set; }
		[Display(Name = "Перемещать в каталог")]
		public bool MoveToTmp { get; set; }
		[Display(Name = "Каталог")]
		public string? TmpCatalog { get; set; }
		[Display(Name = "Задержка перед выполнением")]
		[DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
		public TimeOnly DelayBeforeExecuting { get; set; }

		[Display(Name = "Исключить маску")]
		public string? SubMask { get; set; }
/*		[Display(Name = "Архив для подмаски")]
		public string? SubArchiveCatalog { get; set; }
*/


		[Display(Name = "Каталоги назначения")]
		public List<TaskOperationEntity>? Operations { get; set; } = new List<TaskOperationEntity>();
		[Display(Name = "Статус задачи")]
		public TaskStatusEntity? TaskStatus { get; set; }
		[Display(Name = "Группа задач")]
		public int TaskGroup { get; set; }
        /*[Display(Name = "Группа задач")]
        [ForeignKey(nameof(TaskGroupId))]
		public TaskGroupEntity TaskGroup { get; set; }*/
		

	}
}
