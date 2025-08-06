using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IStepRepository
{
    List<TaskStepEntity> GetAllSteps();
    TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber);
    TaskStepEntity? GetStepByStepId(int stepId);
    List<TaskStepEntity> GetAllStepsByTaskId(string taskId);
    bool CreateStep(TaskStepEntity taskStep);
    bool EditStep(TaskStepEntity taskStep);
    bool UpdateRangeSteps(List<TaskStepEntity> steps);
    bool ActivatedStep(int stepId);
    bool DeleteStepsByTaskId(string taskId);
    bool DeleteStep(int stepId);
}
