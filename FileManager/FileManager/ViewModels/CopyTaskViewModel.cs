using FileManager.Core.ViewModels;

namespace FileManager.ViewModels;

public class CopyTaskViewModel
{
    public string TaskId { get; set; }
    public string NewTaskId { get; set; }
    public bool IsCopySteps { get; set; }
    public bool IsActivate { get; set; }
    public List<CopyStepViewModel> CopySteps { get; set; }
}
