using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Enum;

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
