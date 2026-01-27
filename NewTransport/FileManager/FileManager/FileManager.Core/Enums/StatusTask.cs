using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum StatusTask
{
    [Display(Name = "Выполняется")]
    [Description("Выполняется")]
    Process,
    [Display(Name = "Ошибка")]
    [Description("Ошибка")]
    Error,
    [Display(Name = "Ожидание")]
    [Description("Ожидание")]
    Wait,
    [Display(Name = "Завершена")]
    [Description("Завершена")]
    Complete
}
