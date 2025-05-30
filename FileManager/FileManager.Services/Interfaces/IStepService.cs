using FileManager.Domain.Entity;

namespace FileManager.Services.Interfaces;

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
}
