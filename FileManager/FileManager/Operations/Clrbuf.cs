using FileManager.DAL;
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
        public Clrbuf(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext)
            : base(step, operation, taskLogger, appDbContext)
        { }

        public override void Execute(List<string>? bufferFiles)
        {
            _taskLogger.StepLog(TaskStep, $"Очистка буфера: {TaskStep.Source}");
            int countFiles = 0;
            if ( bufferFiles != null )
            {
                countFiles = bufferFiles.Count;
                bufferFiles = null;
            }
            _taskLogger.StepLog(TaskStep, $"Удалено файлов из буфера: {countFiles}");

            if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
