using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Extensions;
using System.ComponentModel.DataAnnotations;
//using X.PagedList;

namespace FileManager.ViewModels;

public class TaskLogViewModel
{
    [Display(Name = "Записей на странице")]
    public int PageSize { get; set; }
    [Display(Name = "Задача")]
    public string? TaskId { get; set; }
    public FilterOptions TaskIdOption { get; set; }
    [Display(Name = "Дата С")]
    public DateTime DateFrom { get; set; }
    public FilterOptions DateFromOption { get; set; }
    [Display(Name = "Дата По")]
    public DateTime DateTo { get; set; }
    public FilterOptions DateToOption { get; set; }
    [Display(Name = "Время С")]
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime TimeFrom { get; set; }
    public FilterOptions TimeFromOption { get; set; }
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    [Display(Name = "Время По")]
    public DateTime TimeTo { get; set; }
    public FilterOptions TimeToOption { get; set; }
    [Display(Name = "Номер шага")]
    public int StepNumber { get; set; }
    public FilterOptions StepNumberOption { get; set; }
    [Display(Name = "Операция")]
    public OperationName OperationName { get; set; }
    public FilterOptions OperationNameOption { get; set; }
    [Display(Name = "Результат")]
    public ResultOperation ResultOperation { get; set; }
    public FilterOptions ResultOperationOption { get; set; }
    [Display(Name = "Имя файла")]
    public string? FileName { get; set; }
    public FilterOptions FileNameOption { get; set; }
    [Display(Name = "Сообщение")]
    public string? Text { get; set; }
    public FilterOptions TextOption { get; set; }
    [Display(Name = "Поле для сортировки")]
    public FieldSortLogs FieldSortLogs { get; set; }
    [Display(Name = "Сортировка")]
    public SortLogs SortLogs { get; set; }

    public int? PageNumber { get; set; }
    public PagedList<TaskLogEntity> Logs { get; set; }

    public Dictionary<string, string> ToRouteDictionary()
    {
        return this.GetType().GetProperties()
            .Where(p => p.GetValue(this) != null)
            .ToDictionary(p => p.Name, p => p.GetValue(this)?.ToString() ?? "");
    }
}
