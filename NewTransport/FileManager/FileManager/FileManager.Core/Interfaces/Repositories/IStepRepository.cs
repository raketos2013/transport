using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IStepRepository
{
    Task<List<TaskStepEntity>> GetAllSteps();
    Task<TaskStepEntity?> GetStepByTaskId(string taskId, int stepNumber);
    Task<TaskStepEntity?> GetStepByStepId(int stepId);
    Task<List<TaskStepEntity>> GetAllStepsByTaskId(string taskId);
    Task<TaskStepEntity> CreateStep(TaskStepEntity taskStep);
    TaskStepEntity EditStep(TaskStepEntity taskStep);
    List<TaskStepEntity> UpdateRangeSteps(List<TaskStepEntity> steps);
    Task<TaskStepEntity> ActivatedStep(int stepId);
    Task<bool> DeleteStepsByTaskId(string taskId);
    Task<bool> DeleteStep(int stepId);
}
