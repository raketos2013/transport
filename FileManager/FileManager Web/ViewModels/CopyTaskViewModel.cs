using FileManager.Domain.ViewModels.Step;

namespace FileManager_Web.ViewModels;

public class CopyTaskViewModel
{
    public string TaskId { get; set; }
    public string NewTaskId { get; set; }
    public bool IsCopySteps { get; set; }
    public List<CopyStepViewModel> CopySteps { get; set; }
}
