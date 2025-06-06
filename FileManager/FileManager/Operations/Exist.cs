using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;


namespace FileManager_Server.Operations;

public class Exist(TaskStepEntity step,
                    TaskOperation? operation,
                    ITaskLogger taskLogger,
                    AppDbContext appDbContext,
                    IMailSender mailSender)
            : StepOperation(step, operation, taskLogger, appDbContext, mailSender)
{
    public override void Execute(List<string>? bufferFiles)
    {
        _taskLogger.StepLog(TaskStep, $"ПРОВЕРКА НАЛИЧИЯ: {TaskStep.Source} => {TaskStep.Destination}");
        _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileName;
        OperationExistEntity? operation = null;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
        if (files.Length == 0 && TaskStep.IsBreak)
        {
            _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
            throw new Exception("Операция Exist: найдено 0 файлов");
        }
        _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Length}");

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
                    if (files.Length > 0)
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
                    if (files.Length == 0)
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
                _taskLogger.StepLog(TaskStep, $"Прерывание задачи: несоответствие ожидаемому результату", "", ResultOperation.W);
                throw new Exception("Ошибка при операции Exist: несоответствие ожидаемому результату");
            }
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            foreach (var file in files)
            {
                successFiles.Add(file);
            }
            _mailSender.Send(TaskStep, addresses, successFiles);
        }

        _nextStep?.Execute(bufferFiles);
    }
}
