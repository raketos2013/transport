using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Interfaces
{
    public interface IStepService
    {
        List<TaskStepEntity> GetAllSteps();
        TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber);
        List<TaskStepEntity> GetAllStepsByTaskId(string taskId);
        bool CreateStep(TaskStepEntity taskStep);
        bool EditStep(TaskStepEntity taskStep);
        bool ReplaceSteps(string taskId, string numberStep, string operation);
    }
}
