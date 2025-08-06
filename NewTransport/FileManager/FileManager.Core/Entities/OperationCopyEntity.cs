using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Entities;

public class OperationCopyEntity : TaskOperation
{
    [Display(Name = "Файл есть в источнике")]
    public FileInSource FileInSource { get; set; }
    [Display(Name = "Файл есть в назначении")]
    public FileInDestination FileInDestination { get; set; }
    [Display(Name = "Дубль по журналу")]
    public DoubleInLog FileInLog { get; set; }
    [Display(Name = "Сортировка")]
    public SortFiles Sort { get; set; }
    [Display(Name = "Кол-во файлов")]
    public int FilesForProcessing { get; set; }
    [Display(Name = "Аттрибуты")]
    public AttributeFile FileAttribute { get; set; }
}
