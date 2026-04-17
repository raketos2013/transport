using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum DoubleInLog
{
    [Display(Name = "Не контролировать")]
    [Description("Не контролировать")]
    NOCTRL = 1,
    [Display(Name = "В течение дня")]
    [Description("В течение дня")]
    INADAY = 2
}
