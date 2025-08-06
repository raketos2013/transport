using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Entities;

public class OperationExistEntity : TaskOperation
{
    [Display(Name = "Ожидаемый результат")]
    public ExpectedResult ExpectedResult { get; set; }
    [Display(Name = "Прервать задачу")]
    public bool BreakTaskAfterError { get; set; }

}
