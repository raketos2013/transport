using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum OperationName
    {
        /*[Display(Name = "")]
        None = 0,*/
        [Display(Name = "Копировать")]
        Copy = 1,
        [Display(Name = "Переместить")]
        Move = 2,
        [Display(Name = "Прочитать")]
        Read = 3,
        [Display(Name = "Наличие")]
        Exist = 4,
        [Display(Name = "Переименовать")]
        Rename = 5,
        [Display(Name = "Удалить")]
        Delete = 6,
    }
}
