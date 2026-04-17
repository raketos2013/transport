using FileManager.Core.Entities;
using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileManager.ViewModels;

public class UserLogViewModel
{
    [Display(Name = "Дата С")]
    [Required]
    public DateTime DateFrom { get; set; }
    [Display(Name = "Дата По")]
    [Required]
    public DateTime DateTo { get; set; }
    [Display(Name = "Время С")]
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime TimeFrom { get; set; }
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    [Display(Name = "Время По")]
    public DateTime TimeTo { get; set; }
    [Display(Name = "Пользователь")]
    public string User { get; set; }
    public FilterOptions UserOption { get; set; }
    [Display(Name = "Действие")]
    public string Action { get; set; }
    public FilterOptions ActionOption { get; set; }
    public List<UserLogEntity> Logs { get; set; }
}
