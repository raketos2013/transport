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
        [Display(Name = "Успешно")]
        [Description("Успешно")]
        Success,
        [Display(Name = "Ошибка")]
        [Description("Ошибка")]
        Error
	}
}
