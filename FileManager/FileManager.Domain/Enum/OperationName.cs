using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace FileManager.Domain.Enum
{
    public enum OperationName
    {
        [Display(Name = "")]
        None = 0,
        [Display(Name = "Копирование")]
        [Description("Копирование")]
        Copy = 1,
        [Display(Name = "Перемещение")]
        [Description("Перемещение")]
        Move = 2,
        [Display(Name = "Чтение")]
        [Description("Чтение")]
        Read = 3,
        [Display(Name = "Проверка наличия/отсутствия")]
        [Description("Проверка наличия/отсутствия")]
        Exist = 4,
        [Display(Name = "Переименование")]
        [Description("Переименование")]
        Rename = 5,
        [Display(Name = "Удаление")]
        [Description("Удаление")]
        Delete = 6,
        [Display(Name = "Очистка буфера")]
        [Description("Очистка буфера")]
        Clrbuf = 7
    }
}
