using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FileManager.Domain.Enum;

namespace FileManager.Domain.Entity
{
	public class TaskStepEntity
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Display(Name = "Id шага")]
		public int StepId { get; set; }
		[Display(Name = "Идентификатор задачи")]
		public string TaskId { get; set; }
		[ForeignKey(nameof(TaskId))]
		public TaskEntity? Task { get; set; }

        [Display(Name = "Идентификатор операции")]
        public int OperationId { get; set; }
/*        [ForeignKey(nameof(OperationId))]
        public TaskOperation Operation { get; set; }*/
		[Display(Name = "Операция")]
		public OperationName OperationName { get; set; }
		[Display(Name = "Номер шага")]
		public int StepNumber { get; set; }
		[Display(Name = "Описание")]
		public string Description { get; set; }
		[Display(Name = "Маска файла")]
		public string FileMask { get; set; }
		[Display(Name = "Источник")]
		public string Source { get; set; }
		[Display(Name = "Назначение")]
		public string? Destination { get; set; }
		[Display(Name = "Прерывание")]
		public bool IsBreak {  get; set; }
		[Display(Name = "Активно")]
		public bool IsActive { get; set; }

	}
}
