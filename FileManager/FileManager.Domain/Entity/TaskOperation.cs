using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileManager.Domain.Entity
{
    public abstract class TaskOperation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OperationId { get; set; }

        public int StepId { get; set; }
        [ForeignKey(nameof(StepId))]
        public TaskStepEntity? Step { get; set; }

        [Display(Name = "Информировать об успехе")]
        public bool InformSuccess { get; set; }
        [Display(Name = "Группа рассылки")]
        public int? AddresseeGroupId { get; set; }
        [Display(Name = "Дополнительный текст")]
        public string? AdditionalText { get; set; }
    }
}
