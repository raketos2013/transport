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
    public class Exist : StepOperation
    {
        public Exist(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext)
            : base(step, operation, taskLogger, appDbContext)
        {
        }

		public override void Execute(List<string>? bufferFiles)
		{
			_taskLogger.StepLog(TaskStep, $"Проверка наличия: {TaskStep.Source} => {TaskStep.Destination}");
			_taskLogger.OperationLog(TaskStep);

			string[] files = [];
			string fileName;
			OperationExistEntity? operation = null;

			files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
			_taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Count()}");
			if (files.Count() > 0)
			{

			}
			
			/*if (infoFiles.Count > 0)
			{
				operation = _appDbContext.
			}*/
			if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
