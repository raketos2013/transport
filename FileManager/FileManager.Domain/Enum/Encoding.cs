using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Enum
{
    public enum Encode
    {
        [Display(Name = "UTF-8")]
        [Description("UTF-8")]
        UTF8 = 4110,
        [Display(Name = "WIN-1251")]
        [Description("WIN-1251")]
        WIN1251 = 1504,
        [Display(Name = "DOS-866")]
        [Description("DOS-866")]
        DOS866 = 1503

    }
}
