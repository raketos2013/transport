using FileManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FileManager.Core.Queries;

public class Query
{
    public int Take { get; set; }
    public int Skip { get; set; }

    public string? TaskId { get; set; }
    public FilterOptions TaskIdOption { get; set; }
    public DateTime DateFrom { get; set; }
    public FilterOptions DateFromOption { get; set; }
    public DateTime DateTo { get; set; }
    public FilterOptions DateToOption { get; set; }
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime TimeFrom { get; set; }
    public FilterOptions TimeFromOption { get; set; }
    [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime TimeTo { get; set; }
    public FilterOptions TimeToOption { get; set; }
    public int StepNumber { get; set; }
    public FilterOptions StepNumberOption { get; set; }
    public OperationName OperationName { get; set; }
    public FilterOptions OperationNameOption { get; set; }
    public ResultOperation ResultOperation { get; set; }
    public FilterOptions ResultOperationOption { get; set; }
    public string? FileName { get; set; }
    public FilterOptions FileNameOption { get; set; }
    public string? Text { get; set; }
    public FilterOptions TextOption { get; set; }
    public FieldSortLogs FieldSortLogs { get; set; }
    public SortLogs SortLogs { get; set; }
}
