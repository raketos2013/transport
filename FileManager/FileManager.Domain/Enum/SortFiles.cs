using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Enum;

public enum SortFiles
{
    [Display(Name = "Не сортировать")]
    [Description("Не сортировать")]
    NoSortFiles = 0,
    [Display(Name = "По имени")]
    [Description("По имени")]
    NameAscending = 1,
    [Display(Name = "По имени по убыванию")]
    [Description("По имени по убыванию")]
    NameDescending = 2,
    [Display(Name = "По времени")]
    [Description("По времени")]
    TimeAscending = 3,
    [Display(Name = "По времени по убыванию")]
    [Description("По времени по убыванию")]
    TimeDescending = 4,
    [Display(Name = "По размеру")]
    [Description("По размеру")]
    SizeAscending = 5,
    [Display(Name = "По размеру по убыванию")]
    [Description("По размеру по убыванию")]
    SizeDescending = 6,
}
