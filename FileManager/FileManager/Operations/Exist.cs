using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Services;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.Operations
{
    public class Exist : StepOperation
    {
        public Exist(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext, IMailSender mailSender)
            : base(step, operation, taskLogger, appDbContext, mailSender)
        {
        }

        public override void Execute(List<string>? bufferFiles)
        {
            _taskLogger.StepLog(TaskStep, $"ПРОВЕРКА НАЛИЧИЯ: {TaskStep.Source} => {TaskStep.Destination}");
            _taskLogger.OperationLog(TaskStep);

            string[] files = [];
            string fileName;
            OperationExistEntity? operation = null;
            List<AddresseeEntity> addresses = new List<AddresseeEntity>();
            List<string> successFiles = new List<string>();

            files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
            _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Count()}");

            operation = _appDbContext.OperationExist.FirstOrDefault(x => x.StepId == TaskStep.StepId);
            if (operation != null)
            {

                if (operation.InformSuccess)
                {
                    addresses = _appDbContext.Addressee.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                                    x.IsActive == true).ToList();
                }
                bool isBreakTask = false;
                //_taskLogger.StepLog(TaskStep, $"Ожидаемый результат - {operation.ExpectedResult.GetDescription()}");
                switch (operation.ExpectedResult)
                {
                    case ExpectedResult.Success:
                        if (files.Count() > 0)
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = true;
                            }
                            else
                            {
                                isBreakTask = false;
                            }
                        }
                        else
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = false;
                            }
                            else
                            {
                                isBreakTask = true;
                            }
                        }
                        break;
                    case ExpectedResult.Error:
                        if (files.Count() == 0)
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = false;
                            }
                            else
                            {
                                isBreakTask = true;
                            }
                        }
                        else
                        {
                            if (operation.BreakTaskAfterError)
                            {
                                isBreakTask = true;
                            }
                            else
                            {
                                isBreakTask = false;
                            }
                        }
                        break;
                    case ExpectedResult.Any:
                        if (operation.BreakTaskAfterError)
                        {
                            isBreakTask = true;
                        }
                        else
                        {
                            isBreakTask = false;
                        }
                        break;
                    default:
                        break;
                }
                if (isBreakTask)
                {
                    throw new Exception("Ошибка при операции Exist");
                }
            }

            if (addresses.Count > 0)
            {
                foreach (var file in files)
                {
                    successFiles.Add(file);
                }
                _mailSender.Send(TaskStep, addresses, successFiles);
            }

            if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
