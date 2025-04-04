﻿using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Interfaces
{
    public interface IStepRepository
    {
        List<TaskStepEntity> GetAllSteps();
        TaskStepEntity GetStepByTaskId(string taskId, int stepNumber);
        List<TaskStepEntity> GetAllStepsByTaskId(string taskId);
        bool CreateStep(TaskStepEntity taskStep);
        bool EditStep(TaskStepEntity taskStep);

    }
}
