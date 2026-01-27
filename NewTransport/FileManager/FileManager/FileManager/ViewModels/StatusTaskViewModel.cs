using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.ViewModels;

public enum StatusTaskViewModel
{
    [Display(Name = "")]
    [Description("")]
    NOFILTER,
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
