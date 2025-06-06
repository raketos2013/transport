﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Enum;

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
