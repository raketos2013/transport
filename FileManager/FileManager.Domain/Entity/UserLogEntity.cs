using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileManager.Domain.Entity
{
    public class UserLogEntity
    {

        //public int Id { get; set; }
        [Display(Name = "Дата время события")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss.ffffff}")]
        public DateTime DateTimeLog { get; set; }
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }
        [Display(Name = "Действие")]
        public string Action { get; set; }
        [Display(Name = "Объект")]
        [Column(TypeName = "jsonb")]
        public string Data { get; set; }
    }
}
