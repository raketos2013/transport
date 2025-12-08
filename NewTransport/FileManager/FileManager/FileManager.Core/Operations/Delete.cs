using FileManager.Core.Entities;
using FileManager.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace FileManager.Core.Operations;

public class Delete(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScope scopeFactory)
            : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"УДАЛЕНИЕ: {TaskStep.Source} => {TaskStep.Destination}");
        await _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileName;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];

        files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
        if (files.Length == 0 && TaskStep.IsBreak)
        {
            await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
            _nextStep = null;
        }
        await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Length}");

        OperationDeleteEntity? operation = await _operationService.GetDeleteByStepId(TaskStep.StepId);
        if (operation != null)
        {
            if (operation.InformSuccess && files.Length > 0)
            {
                var addressesAsync = await _addresseeService.GetAllAddressees();
                addresses = addressesAsync.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                      x.IsActive == true).ToList();
            }
        }

        foreach (string file in files)
        {
            fileName = Path.GetFileName(file);
            File.Delete(file);
            await _taskLogger.StepLog(TaskStep, "Файл успешно удалён", fileName);
            successFiles.Add(fileName);
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            await _mailSender.Send(TaskStep, addresses, successFiles);
        }

        if (_nextStep != null)
        {
            await _nextStep.Execute(bufferFiles);
        }
    }
}
