using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Enum
{
    public enum ExpectedResult
    {
        [Display(Name = "Успех")]
        [Description("Успех")]
        Success = 1,
        [Display(Name = "Ошибка")]
        [Description("Ошибка")]
        Error = 2,
        [Display(Name = "Любой")]
        [Description("Любой")]
        Any = 3
    }
}
