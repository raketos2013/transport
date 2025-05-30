using System.ComponentModel.DataAnnotations;

namespace FileManager.Domain.Entity
{
    public class AddresseeGroupEntity
    {
        [Key]
        [Display(Name = "Номер группы")]
        public int Id { get; set; }
        [Display(Name = "Наименование группы")]
        public string Name { get; set; }

        List<AddresseeEntity>? Addressees { get; set; }
        List<TaskEntity>? Tasks { get; set; }
    }
}
