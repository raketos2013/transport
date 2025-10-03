using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.ViewModels;

public enum FieldSortLogs
{
    [Description("Дата")]
    [Display(Name = "Дата")]
    Date = 1,
    [Description("Задача")]
    [Display(Name = "Задача")]
    Task = 2,
    [Description("Операция")]
    [Display(Name = "Операция")]
    Operation = 3,
    [Description("Результат")]
    [Display(Name = "Результат")]
    Result = 4,
    [Description("Имя файла")]
    [Display(Name = "Имя файла")]
    FileName = 5
}
