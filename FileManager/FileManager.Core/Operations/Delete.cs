using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace FileManager.Core.Operations;

public class Delete(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScope scopeFactory)
            : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles, CancellationToken cancellationToken)
    {
        await _taskLogger.StepLog(TaskStep, $"УДАЛЕНИЕ: {TaskStep.Source}");
        await _taskLogger.OperationLog(TaskStep);

        if (!Directory.Exists(TaskStep.Source))
        {
            await _taskLogger.StepLog(TaskStep, "Каталог источника не найден", "", ResultOperation.E);
            throw new DomainException("Каталог источника не найден");
        }

        string[] files = [];
        string fileName;
        List<AddresseeEntity> addresses = [];
        List<string> successFiles = [];
        List<FileLog> logs = [];

        files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
        await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {files.Length}");
        if (files.Length == 0 && TaskStep.IsBreak)
        {
            await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
            _nextStep = null;
            return;
        }

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
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            fileName = Path.GetFileName(file);
            File.Delete(file);
            //await _taskLogger.StepLog(TaskStep, "Файл успешно удалён", fileName);
            logs.Add(new FileLog()
            {
                DateTimeLog = DateTime.Now,
                Text = "Файл успешно удалён",
                FileName = fileName,
                ResultOperation = ResultOperation.I
            });
            successFiles.Add(fileName);
            if (logs.Count >= 1000)
            {
                await _taskLogger.LogFiles(TaskStep, logs);
                logs.Clear();
            }
        }
        if (logs.Count > 0)
        {
            await _taskLogger.LogFiles(TaskStep, logs);
            logs.Clear();
        }

        if (addresses.Count > 0 && successFiles.Count > 0)
        {
            await _mailSender.Send(TaskStep, addresses, successFiles);
        }

        if (_nextStep != null)
        {
            await _nextStep.Execute(bufferFiles, cancellationToken);
        }
    }
}
