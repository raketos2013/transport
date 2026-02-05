using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Enums;

public enum FilterOptions
{
    [Display(Name = "=")]
    [Description("=")]
    Equal = 0,
    [Display(Name = "!=")]
    [Description("!=")]
    NotEqual = 1,
    [Display(Name = ">")]
    [Description(">")]
    More = 2,
    [Display(Name = "<")]
    [Description("<")]
    Less = 3,
    [Display(Name = ">=")]
    [Description(">=")]
    MoreEqual = 4,
    [Display(Name = "<=")]
    [Description("<=")]
    LessEqual = 5
}
