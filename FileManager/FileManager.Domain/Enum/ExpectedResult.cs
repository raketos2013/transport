using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum ExpectedResult
    {
        [Display(Name = "Успех")]
        Success = 1,
        [Display(Name = "Ошибка")]
        Error = 2,
        [Display(Name = "Любой")]
        Any = 3
    }
}
