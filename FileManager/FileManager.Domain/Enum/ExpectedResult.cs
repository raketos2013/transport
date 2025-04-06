using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
