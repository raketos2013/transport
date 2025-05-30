using FileManager.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Entity
{
    public class OperationExistEntity : TaskOperation
    {
        [Display(Name = "Ожидаемый результат")]
        public ExpectedResult ExpectedResult { get; set; }
        [Display(Name = "Прервать задачу")]
        public bool BreakTaskAfterError { get; set; }

    }
}
