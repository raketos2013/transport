using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum Encode
    {
        [Display(Name = "UTF-8")]
        UTF8 = 4110,
        [Display(Name = "WIN-1251")]
        WIN1251 = 1504,
        [Display(Name = "DOS-866")]
        DOS866 = 1503

    }
}
