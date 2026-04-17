using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileManager.ViewModels;

public class TaskFilterViewModel
{
    [Display(Name = "Идентификатор")]
    public string TaskId { get; set; }
    [Display(Name = "Наименование")]
    public string Name { get; set; }
    [Display(Name = "Время начала")]
    public TimeOnly TimeBegin { get; set; }
    [Display(Name = "Время окончания")]
    public TimeOnly TimeEnd { get; set; }
    [Display(Name = "Группы рассылки")]
    public int AddresseeGroupId { get; set; }
    [Display(Name = "Дни активности")]
    public DayActiveViewModel DayActive { get; set; }
    [Display(Name = "Статус")]
    public StatusTaskViewModel Status {  get; set; }
}
