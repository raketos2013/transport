using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum FileInDestination
    {
        [Display(Name = "Без перезаписи")]
        ERR,
        [Display(Name = "Перезапись")]
        OVR,
        [Display(Name = "Переименовать")]
        RNM
    }
}
