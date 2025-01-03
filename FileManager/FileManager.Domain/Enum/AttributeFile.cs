using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum AttributeFile
    {
        [Display(Name = "Видимый, кроме 'H'")]
        V = 1,
        [Display(Name = "Скрытый")]
        H = 2, 
        [Display(Name = "Архивный")]
        A = 3,
        [Display(Name = "Для чтения")]
        R = 4,
        [Display(Name = "Все")]
        X = 5
    }
}
