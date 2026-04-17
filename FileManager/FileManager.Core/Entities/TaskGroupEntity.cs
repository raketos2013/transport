using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Entities;

public class TaskGroupEntity
{
    [Key]
    [Display(Name = "Идентификатор группы")]
    public int Id { get; set; }
    [Display(Name = "Наименование группы")]
    public string Name { get; set; }
    public List<TaskEntity>? Tasks { get; set; }

}
