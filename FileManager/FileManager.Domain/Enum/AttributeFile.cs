using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Enum
{
    public enum AttributeFile
    {
        [Display(Name = "Видимый, кроме 'H'")]
        [Description("Видимый, кроме 'H'")]
        V = 1,
        [Display(Name = "Скрытый")]
        [Description("Скрытый")]
        H = 2,
        [Display(Name = "Архивный")]
        [Description("Архивный")]
        A = 3,
        [Display(Name = "Для чтения")]
        [Description("Для чтения")]
        R = 4,
        [Display(Name = "Все")]
        [Description("Все")]
        X = 5
    }
}
