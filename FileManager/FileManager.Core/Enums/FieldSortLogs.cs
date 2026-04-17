using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum FieldSortLogs
{
    [Description("Дата")]
    [Display(Name = "Дата")]
    DateTimeLog = 1,
    [Description("Задача")]
    [Display(Name = "Задача")]
    TaskId = 2,
    [Description("Операция")]
    [Display(Name = "Операция")]
    OperationName = 3,
    [Description("Результат")]
    [Display(Name = "Результат")]
    ResultOperation = 4,
    [Description("Имя файла")]
    [Display(Name = "Имя файла")]
    FileName = 5
}
