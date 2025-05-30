using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileManager.Domain.Entity
{
    public class TaskStatusEntity
    {
        /*[Key]
        [Display(Name = "Id статуса")]
        public int StatusId { get; set; }*/
        [Key]
        [Display(Name = "Идентификатор задачи")]
        public string TaskId { get; set; }

        [ForeignKey(nameof(TaskId))]
        public TaskEntity? Task { get; set; }

        [Display(Name = "Выполняется")]
        public bool IsProgress { get; set; }

        [Display(Name = "Ошибки при обработке")]
        public bool IsError { get; set; }

        [Display(Name = "Количество выполнений за день")]
        public int CountExecute { get; set; }

        [Display(Name = "Количество обработанных файлов за день")]
        public int CountProcessedFiles { get; set; }



        [Display(Name = "Дата последнего выполнения задачи")]
        public DateTime DateLastExecute { get; set; }

        [Display(Name = "Количество оставшихся файлов в каталоге")]
        public int? CountLeftFiles { get; set; }

    }
}
