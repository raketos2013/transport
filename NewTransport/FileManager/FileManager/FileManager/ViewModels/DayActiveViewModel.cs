using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.ViewModels;

public enum DayActiveViewModel
{
    [Display(Name = "")]
    [Description("")]
    NOFILTER,
    [Display(Name = "Рабочие")]
    [Description("Рабочие")]
    WORK = 1,
    [Display(Name = "Выходные")]
    [Description("Выходные")]
    HOLIDAY = 2,
    [Display(Name = "Все")]
    [Description("Все")]
    ALL = 3,
}
