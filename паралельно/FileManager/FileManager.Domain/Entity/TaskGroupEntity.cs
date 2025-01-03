using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Entity
{
    public class TaskGroupEntity
    {
        [Key]
        [Display(Name = "Идентификатор группы")]
        public int Id { get; set; }
        [Display(Name = "Наименование группы")]
        public string Name { get; set; }
        public List<TaskEntity>? Tasks { get; set; }

    }
}
