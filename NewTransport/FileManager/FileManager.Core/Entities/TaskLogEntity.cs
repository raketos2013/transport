using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Entities;

public class TaskLogEntity
{
    [Display(Name = "Дата время события")]
    public DateTime DateTimeLog { get; set; }
    [Display(Name = "Id задачи")]
    public string TaskId { get; set; }
    [Display(Name = "Id шага")]
    public int? StepId { get; set; }
    [Display(Name = "Номер шага")]
    public int? StepNumber { get; set; }
    [Display(Name = "Id операции")]
    public int? OperationId { get; set; }
    [Display(Name = "Операция")]
    public string? OperationName { get; set; }
    [Display(Name = "Результат операции")]
    public ResultOperation? ResultOperation { get; set; }
    [Display(Name = "Имя файла")]
    public string? FileName { get; set; }
    [Display(Name = "Сообщение")]
    public string? ResultText { get; set; }
}
