using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum ResultOperation
{
    [Display(Name = "")]
    N,
    [Display(Name = "Инфо")]
    I,
    [Display(Name = "Ошибка")]
    E,
    [Display(Name = "Предупреждение")]
    W
}
