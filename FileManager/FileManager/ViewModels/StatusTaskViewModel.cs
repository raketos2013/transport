using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.ViewModels;

public enum StatusTaskViewModel
{
    [Display(Name = "")]
    [Description("")]
    NOFILTER = 0,
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
