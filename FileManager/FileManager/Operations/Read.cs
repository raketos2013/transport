using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;


namespace FileManager_Server.Operations
{
    public class Read : StepOperation
    {
        public Read(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext)
            : base(step, operation, taskLogger, appDbContext)
        {
        }

        public override void Execute(List<string>? bufferFiles)
        {
			_taskLogger.StepLog(TaskStep, $"Копирование: {TaskStep.Source} => {TaskStep.Destination}");
			_taskLogger.OperationLog(TaskStep);

			string[] files = [];
			string fileNameDestination, fileName;
			bool isCopyFile = true;
			List<FileInfo> infoFiles = new List<FileInfo>();
			OperationCopyEntity? operation = null;

			files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
			foreach (var file in files)
			{
				infoFiles.Add(new FileInfo(file));
			}
			_taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count()}");
			if (infoFiles.Count > 0)
			{
				operation = _appDbContext.
			}



			if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
