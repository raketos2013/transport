using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum ExecuteFm
{

    [Display(Name = "Поиск по шаблону")]
    W,
    [Display(Name = "Поиск текста")]
    E,
    [Display(Name = "Проверка времени")]
    R,
    [Display(Name = "Задержка")]
    T
}
