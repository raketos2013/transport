using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IStepService
{
    Task<List<TaskStepEntity>> GetAllSteps();
    Task<TaskStepEntity?> GetStepByTaskId(string taskId, int stepNumber);
    Task<TaskStepEntity?> GetStepByStepId(int stepId);
    Task<List<TaskStepEntity>> GetAllStepsByTaskId(string taskId);
    Task<bool> CreateStep(TaskStepEntity taskStep);
    Task<bool> EditStep(TaskStepEntity taskStep);
    Task<bool> ReplaceSteps(string taskId, string numberStep, string operation);
    Task<bool> ActivatedStep(int stepId);
    Task<bool> DeleteStep(int stepId);
    Task<bool> CopyStep(int stepId, int newNumber);
    Task<int> CountFiles(int stepId);
}
