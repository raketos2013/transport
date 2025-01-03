using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum ResultOperation
    {
        [Display(Name = "Успешно")]
        Success,
        [Display(Name = "Ошибка")]
        Error,
        [Display(Name = "Переименован")]
        Rename
		/*        [Display(Name = "Скопирован")]
	    Copy,*/
	}
}
