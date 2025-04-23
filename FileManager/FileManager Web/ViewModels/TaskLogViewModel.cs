using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using X.PagedList;
using System.ComponentModel.DataAnnotations;

namespace FileManager_Web.ViewModels
{
    public class TaskLogViewModel
    {
        [Display(Name ="Записей на странице")]
        public int PageSize { get; set; }
        [Display(Name = "Задача")]
        public string? TaskId { get; set; }
        [Display(Name = "Дата С")]
        public DateTime DateFrom { get; set; }
        [Display(Name = "Дата По")]
        public DateTime DateTo { get; set; }
        [Display(Name = "Номер шага")]
        public int StepNumber { get; set; }
        [Display(Name = "Операция")]
        public OperationName OperationName { get; set; }
        [Display(Name = "Результат")]
        public ResultOperation ResultOperation { get; set; }
        [Display(Name = "Имя файла")]
        public string? FileName { get; set; }
        [Display(Name = "Сообщение")]
        public string? Text { get; set; }
        public int? PageNumber {  get; set; }
        public IPagedList<TaskLogEntity> Logs {  get; set; }
    }
}
