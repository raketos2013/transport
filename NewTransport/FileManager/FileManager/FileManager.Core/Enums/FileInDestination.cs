using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum FileInDestination
{
    [Display(Name = "Без перезаписи")]
    [Description("Без перезаписи")]
    ERR,
    [Display(Name = "Перезапись")]
    [Description("Перезапись")]
    OVR,
    [Display(Name = "Переименовать")]
    [Description("Переименовать")]
    RNM
}
