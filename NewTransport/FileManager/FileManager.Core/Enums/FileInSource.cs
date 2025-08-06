using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum FileInSource
{
    [Display(Name = "Всегда")]
    [Description("Всегда")]
    Always = 1,
    [Display(Name = "Раз в день")]
    [Description("Раз в день")]
    OneDay = 2
}
