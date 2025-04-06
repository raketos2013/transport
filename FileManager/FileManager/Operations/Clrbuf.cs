﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.Operations
{
    public class Clrbuf : StepOperation
    {
        public Clrbuf(TaskStepEntity step, TaskOperation operation, ITaskLogger taskLogger, AppDbContext appDbContext)
            : base(step, operation, taskLogger, appDbContext)
        {
        }

        public override void Execute()
        {
            if (_nextStep != null)
            {
                _nextStep.Execute();
            }
        }
    }
}
