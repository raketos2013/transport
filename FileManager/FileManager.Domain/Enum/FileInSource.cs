using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum FileInSource
    {
        [Display(Name = "Всегда")]
        [Description("Всегда")]
        Always = 1,
        [Display(Name = "Раз в день")]
        [Description("Раз в день")]
        OneDay = 2
    }
}
