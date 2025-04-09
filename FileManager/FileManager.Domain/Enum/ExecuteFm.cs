using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum ExecuteFm
    {
		
		[Display(Name = "Поиск по шаблону")]
		W,
		[Display(Name = "Поиск текста")]
		E,
		[Display(Name = "Проверка времени")]
		R,
		[Display(Name = "Задержка")]
		T
    }
}
