using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FileManager.ViewModels;

public enum SortLogs
{
    [Description("По возрастанию")]
    [Display(Name = "По возрастанию")]
    Ascending,
    [Description("По убыванию")]
    [Display(Name = "По убыванию")]
    Descending,
}
