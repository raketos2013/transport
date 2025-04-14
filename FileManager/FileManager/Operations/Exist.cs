﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Services;
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

            operation = _appDbContext.OperationExist.FirstOrDefault(x => x.StepId == TaskStep.StepId);
            if (operation != null)
            {
                //_taskLogger.StepLog(TaskStep, $"Ожидаемый результат - {operation.ExpectedResult.GetDescription()}");
                switch (operation.ExpectedResult)
                {
                    case ExpectedResult.Success:
                        if (files.Count() == 0)
                        {
                            if (operation.BreakTaskAfterError)
                            {

                            }
                            throw new Exception("Exist 0 files");
                        }
                        break;
                    case ExpectedResult.Error:
                        if (files.Count() > 0 && operation.BreakTaskAfterError)
                        {
                            throw new Exception("Exist > 0 files");
                        }
                        break;
                    case ExpectedResult.Any:

                        break;
                    default:
                        break;
                }
            }

            if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
