using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IStepRepository
{
    Task<List<TaskStepEntity>> GetAllSteps();
    Task<TaskStepEntity?> GetStepByTaskId(string taskId, int stepNumber);
    Task<TaskStepEntity?> GetStepByStepId(int stepId);
    Task<List<TaskStepEntity>> GetAllStepsByTaskId(string taskId);
    Task<bool> CreateStep(TaskStepEntity taskStep);
    Task<bool> EditStep(TaskStepEntity taskStep);
    Task<bool> UpdateRangeSteps(List<TaskStepEntity> steps);
    Task<bool> ActivatedStep(int stepId);
    Task<bool> DeleteStepsByTaskId(string taskId);
    Task<bool> DeleteStep(int stepId);
}
