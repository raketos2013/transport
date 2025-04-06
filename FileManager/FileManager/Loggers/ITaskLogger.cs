using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.Loggers
{
    public interface ITaskLogger
    {
        void TaskLog(string TaskId, string text);
        void StepLog(TaskStepEntity step ,string text, string filename = "");
        void OperationLog(TaskStepEntity step);
    }
}
