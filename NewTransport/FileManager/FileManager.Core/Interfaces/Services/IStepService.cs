using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IStepService
{
    List<TaskStepEntity> GetAllSteps();
    TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber);
    TaskStepEntity? GetStepByStepId(int stepId);
    List<TaskStepEntity> GetAllStepsByTaskId(string taskId);
    bool CreateStep(TaskStepEntity taskStep);
    bool EditStep(TaskStepEntity taskStep);
    bool ReplaceSteps(string taskId, string numberStep, string operation);
    bool ActivatedStep(int stepId);
    bool DeleteStep(int stepId);
    bool CopyStep(int stepId, int newNumber);
}
