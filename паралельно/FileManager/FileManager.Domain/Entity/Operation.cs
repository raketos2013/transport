using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
	public abstract class Operation
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OperationId { get; set; }
		
		public int StepId { get; set; }
		[ForeignKey(nameof(StepId))]
		public TaskStepEntity? Step { get; set; }


		[Display(Name = "Номер шага")]
		public int StepNumber { get; set; }
		[Display(Name = "Информировать об успехе")]
		public bool InformSuccess { get; set; }
		[Display(Name = "Группа рассылки")]
		public int Group { get; set; }
		[Display(Name = "Дополнительный текст")]
		public string? AdditionalText { get; set; }


	}
}
