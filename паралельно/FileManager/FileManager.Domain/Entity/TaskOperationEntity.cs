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
    public class TaskOperationEntity
    {
        [Key]
        [Display(Name = "Идентификатор назначения")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string OperationId { get; set; }
        [Display(Name = "Идентификатор задачи")]
        public string TaskId { get; set; }
        [ForeignKey(nameof(TaskId))]
        public TaskEntity? Task { get; set; }
        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string Description { get; set; }
        [Display(Name = "Путь назначения")]
        [Required(ErrorMessage = "Обязательное поле")]
        public string DestinationDirectory { get; set; }
        [Display(Name = "Переименовать файлы")]
        public bool IsRename { get; set; }
        [Display(Name = "Шаблон имени файла")]
        public string? TemplateFileName { get; set; }
        [Display(Name = "Шаблон нового имени файла")]
        public string? NewTemplateFileName { get; set; }
        [Display(Name = "Активно")]
        public bool IsActive { get; set; }
        [Display(Name = "Группа рассылки")]
        public int? Group { get; set; }
        [Display(Name = "Контроль дублирования")]
        public FileInDestination DublDest { get; set; }
        [Display(Name = "Контроль на повтор обработки")]
        public bool DublNameJr { get; set; }
        /*        [Display(Name = "Тип сортировки")]
                public TypeSort Prior { get; set; }
                [Display(Name = "Типы файлов")]
                public AttributForScaning ScanAttr { get; set; }
                [Display(Name = "Выполнено")]
                public bool IsComplit { get; set; }
                [Display(Name = "Последнее изменение")]
                public DateTime LastModified { get; set; }
        */
        [Display(Name = "Дополнительный текст")]
        public string? AdditionalText { get; set; }

    }
}
