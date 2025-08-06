using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Entities;

public class OperationReadEntity : TaskOperation
{

    [Display(Name = "Файл есть в источнике")]
    public FileInSource FileInSource { get; set; }
    [Display(Name = "Кодировка")]
    public Encode Encode { get; set; }
    [Display(Name = "Поиск по шаблону")]
    public bool SearchRegex { get; set; }
    [Display(Name = "Строка поиска")]
    public string FindString { get; set; }
    [Display(Name = "Ожидаемый результат")]
    public ExpectedResult ExpectedResult { get; set; }
    [Display(Name = "Прервать задачу")]
    public bool BreakTaskAfterError { get; set; }

}
