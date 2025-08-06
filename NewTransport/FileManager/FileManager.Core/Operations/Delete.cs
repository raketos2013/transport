using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Operations;

public class Delete(TaskStepEntity step,
                    TaskOperation? operation,
                    ITaskLogger taskLogger,
                    IMailSender mailSender,
                    IOperationService operationService,
                    IAddresseeService addresseeService,
                    ITaskLogService taskLogService)
            : StepOperation(step, operation, taskLogger, mailSender, operationService, addresseeService, taskLogService)
{
    public override void Execute(List<string>? bufferFiles)
    {
        _taskLogger.StepLog(TaskStep, $"УДАЛЕНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileName;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
        if (files.Length == 0 && TaskStep.IsBreak)
        {
            _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
            throw new Exception("Операция Delete: найдено 0 файлов");
        }
        _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Length}");

        OperationDeleteEntity? operation = _operationService.GetDeleteByStepId(TaskStep.StepId);
            //_appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == TaskStep.StepId);
        if (operation != null)
        {
            if (operation.InformSuccess && files.Length > 0)
            {
                addresses = _addresseeService.GetAllAddressees()
                                                    .Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                                x.IsActive == true).ToList();
                /*_appDbContext.Addressee.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                            x.IsActive == true).ToList();*/
            }
        }

        foreach (string file in files)
        {
            fileName = Path.GetFileName(file);

            File.Delete(file);
            _taskLogger.StepLog(TaskStep, "Файл успешно удалён", fileName);
            successFiles.Add(fileName);
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            _mailSender.Send(TaskStep, addresses, successFiles);
        }

        _nextStep?.Execute(bufferFiles);
    }
}
