using FileManager.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Entity
{
    public class TaskEntity
    {

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
        [Display(Name = "Дни активности")]
        public DayActive DayActive { get; set; }
        [Display(Name = "Группа рассылки")]
        public int? AddresseeGroupId { get; set; }
        [Display(Name = "Активно")]
        public bool IsActive { get; set; }

        [Display(Name = "Последнее изменение")]
        public DateTime LastModified { get; set; }
        [Display(Name = "Шаги")]
        public List<TaskStepEntity>? Steps { get; set; }

        [Display(Name = "Статус задачи")]
        public TaskStatusEntity? TaskStatus { get; set; }

        [Display(Name = "Группа задач")]
        public int TaskGroupId { get; set; }
        [Display(Name = "Лимит выполнений")]
        public int ExecutionLimit { get; set; }
        [Display(Name = "Осталось выполнений")]
        public int ExecutionLeft { get; set; }
        [Display(Name = "Выполняется")]
        public bool IsProgress { get; set; }



    }
}
