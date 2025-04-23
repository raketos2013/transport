using FileManager.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
    public class TaskLogEntity
    {
        [Display(Name = "Дата время события")]
        public DateTime DateTimeLog { get; set; }
        [Display(Name = "Id задачи")]
        public string TaskId { get; set; }
        [Display(Name = "Id шага")]
        public int? StepId { get; set; }
        [Display(Name = "Номер шага")]
        public int? StepNumber { get; set; }
        [Display(Name = "Id операции")]
        public int? OperationId { get; set; }
        [Display(Name = "Операция")]
        public string? OperationName { get; set; }
        [Display(Name = "Результат операции")]
        public ResultOperation? ResultOperation { get; set; }
        [Display(Name = "Имя файла")]
        public string? FileName { get; set; }
        [Display(Name = "Сообщение")]
        public string? ResultText { get; set; }
    }
}
