using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum ResultOperation
    {
        [Display(Name = "")]
        N,
        [Display(Name = "Инфо")]
        I,
        [Display(Name = "Ошибка")]
        E,
        [Display(Name = "Предупреждение")]
        W
	}
}
