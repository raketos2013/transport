using FileManager.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileManager.Core.Entities;

public class TaskLogEntity
{
    [Display(Name = "Id")]
    public Guid Id { get; set; }
    [Display(Name = "Дата время события")]
    public DateTime DateTimeLog { get; set; }
    [Display(Name = "Id задачи")]
    [Column(TypeName = "varchar(30)")]
    public string TaskId { get; set; }
    [Display(Name = "Id шага")]
    public int? StepId { get; set; }
    [Display(Name = "Номер шага")]
    public int? StepNumber { get; set; }
    [Display(Name = "Id операции")]
    public int? OperationId { get; set; }
    [Display(Name = "Операция")]
    [Column(TypeName = "varchar(20)")]
    public string? OperationName { get; set; }
    [Display(Name = "Результат операции")]
    public ResultOperation? ResultOperation { get; set; }
    [Display(Name = "Имя файла")]
    [Column(TypeName = "varchar(100)")]
    public string? FileName { get; set; }
    [Display(Name = "Сообщение")]
    [Column(TypeName = "varchar(300)")]
    public string? ResultText { get; set; }
}
