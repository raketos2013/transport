using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum DayActive
    {
        [Display(Name = "Рабочие")]
        [Description("Рабочие")]
        WORK = 1,
        [Display(Name = "Выходные")]
        [Description("Выходные")]
        HOLIDAY = 2,
        [Display(Name = "Все")]
        [Description("Все")]
        ALL = 3,
    }
}
