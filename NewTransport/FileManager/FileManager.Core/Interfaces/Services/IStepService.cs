using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IStepService
{
    Task<List<TaskStepEntity>> GetAllSteps();
    Task<TaskStepEntity?> GetStepByTaskId(string taskId, int stepNumber);
    Task<TaskStepEntity?> GetStepByStepId(int stepId);
    Task<List<TaskStepEntity>> GetAllStepsByTaskId(string taskId);
    Task<TaskStepEntity> CreateStep(TaskStepEntity taskStep);
    Task<TaskStepEntity> EditStep(TaskStepEntity taskStep);
    Task<bool> ReplaceSteps(string taskId, string numberStep, string operation);
    Task<TaskStepEntity> ActivatedStep(int stepId);
    Task<bool> DeleteStep(int stepId);
    Task<TaskStepEntity> CopyStep(int stepId, int newNumber);
    Task<int> CountFiles(int stepId);
}
