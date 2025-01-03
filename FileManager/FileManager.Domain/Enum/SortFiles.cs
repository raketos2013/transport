using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum SortFiles
    {
        [Display(Name = "Не сортировать")]
        NoSortFiles = 0,
        [Display(Name = "По имени")]
        NameAscending = 1,
        [Display(Name = "По имени по убыванию")]
        NameDescending = 2,
        [Display(Name = "По времени")]
        TimeAscending = 3,
        [Display(Name = "По времени по убыванию")]
        TimeDescending = 4,
        [Display(Name = "По размеру")]
        SizeAscending = 5,
        [Display(Name = "По размеру по убыванию")]
        SizeDescending = 6,
    }
}
