using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileManager.Core.Entities;

public class AddresseeEntity
{

    [Display(Name = "Табельный номер")]
    public string PersonalNumber { get; set; }
    [Display(Name = "Номер группы")]

    public int AddresseeGroupId { get; set; }
    [ForeignKey(nameof(AddresseeGroupId))]
    public AddresseeGroupEntity? AddresseeGroup { get; set; }
    [Display(Name = "Адрес электронной почты")]
    public string EMail { get; set; }
    [Display(Name = "ФИО")]
    public string Fio { get; set; }
    [Display(Name = "Структурное подразделение")]
    public string StructuralUnit { get; set; }
    [Display(Name = "Активно")]
    public bool IsActive { get; set; }
    [Display(Name = "Примечание")]
    public string Note { get; set; }
}
