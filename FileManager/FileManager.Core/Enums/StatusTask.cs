using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum StatusTask
{
    [Display(Name = "Выполняется")]
    [Description("Выполняется")]
    Process = 1,
    [Display(Name = "Ошибка")]
    [Description("Ошибка")]
    Error = 2,
    [Display(Name = "Ожидание")]
    [Description("Ожидание")]
    Wait = 3,
    [Display(Name = "Завершена")]
    [Description("Завершена")]
    Complete = 4
}
