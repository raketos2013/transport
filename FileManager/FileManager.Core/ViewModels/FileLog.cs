using FileManager.Core.Enums;

namespace FileManager.Core.ViewModels;

public class FileLog
{
    public DateTime DateTimeLog { get; set; }
    public string Text { get; set; }
    public string FileName { get; set; }
    public ResultOperation ResultOperation { get; set; }
}
