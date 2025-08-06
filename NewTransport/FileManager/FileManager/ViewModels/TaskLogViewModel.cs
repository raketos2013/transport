using FileManager.Core.Entities;
using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace FileManager.ViewModels;

public class TaskLogViewModel
{
    [Display(Name = "Записей на странице")]
    public int PageSize { get; set; }
    [Display(Name = "Задача")]
    public string? TaskId { get; set; }
    [Display(Name = "Дата С")]
    public DateTime DateFrom { get; set; }
    [Display(Name = "Дата По")]
    public DateTime DateTo { get; set; }
    [Display(Name = "Время С")]
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime TimeFrom { get; set; }
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    [Display(Name = "Время По")]
    public DateTime TimeTo { get; set; }
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
    [Display(Name = "Поле для сортировки")]
    public FieldSortLogs FieldSortLogs { get; set; }
    [Display(Name = "Сортировка")]
    public SortLogs SortLogs { get; set; }

    public int? PageNumber { get; set; }
    public IPagedList<TaskLogEntity> Logs { get; set; }
}
