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
    public class Delete : StepOperation
    {
        public Delete(TaskStepEntity step, TaskOperation operation, ITaskLogger taskLogger, AppDbContext appDbContext)
            : base(step, operation, taskLogger, appDbContext)
        {
        }

        public override void Execute()
        {
            _taskLogger.StepLog(TaskStep, $"Удаление: {TaskStep.Source} => {TaskStep.Destination}");
            _taskLogger.OperationLog(TaskStep);

            string[] files = [];
            string  fileName;

            files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
            _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Count()}");

            OperationDeleteEntity operation = _appDbContext.OperationDelete.First(x => x.StepId == TaskStep.StepId);

            foreach (string file in files)
            {
                fileName = Path.GetFileName(file);

                File.Delete(file);
                _taskLogger.StepLog(TaskStep, "Файл успешно удалён", fileName);
            }

            if (_nextStep != null)
            {
                _nextStep.Execute();
            }
        }
    }
}
