using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Utilities;

namespace FileManager.Core.Services;

public class TaskLogger(IUnitOfWork unitOfWork) : ITaskLogger
{
    public async Task OperationLog(TaskStepEntity step)
    {
        if (step.OperationId == 0)
        {
            return;
        }
        TaskLogEntity taskLog = new()
        {
            OperationId = step.OperationId,
            StepId = step.StepId,
            TaskId = step.TaskId,
            StepNumber = step.StepNumber,
            OperationName = step.OperationName.ToString()
        };

        switch (step.OperationName)
        {
            case OperationName.Copy:
                OperationCopyEntity? operation = await unitOfWork.OperationRepository.GetCopyByOperationId(step.OperationId);
                if (operation != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Сортировка: {operation.Sort.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Контроль дублирования по журналу: {operation.FileInLog.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Контроль Файл есть в источнике: {operation.FileInSource.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Контроль Файл есть в назначении: {operation.FileInDestination.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Аттрибуты файла: {operation.FileAttribute.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                }
                break;
            case OperationName.Move:
                OperationMoveEntity? move = await unitOfWork.OperationRepository.GetMoveByOperationId(step.OperationId);
                if (move != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Контроль Файл есть в назначении: {move.FileInDestination.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Контроль дублирования по журналу: {move.FileInLog.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Сортировка: {move.Sort.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Аттрибуты файла: {move.FileAttribute.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                }
                break;
            case OperationName.Read:
                OperationReadEntity? operationRead = await unitOfWork.OperationRepository.GetReadByOperationId(step.OperationId);
                if (operationRead != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Кодировка: {operationRead.Encode}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"--Контроль Файл есть в назначении: {operationRead.FileInSource.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                }
                break;
            case OperationName.Exist:
                OperationExistEntity? exist = await unitOfWork.OperationRepository.GetExistByOperationId(step.OperationId);
                if (exist != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Ожидаемый результат: {exist.ExpectedResult.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Прервать задачу: {exist.BreakTaskAfterError}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                }
                break;
            case OperationName.Rename:
                OperationRenameEntity? rename = await unitOfWork.OperationRepository.GetRenameByOperationId(step.OperationId);
                if (rename != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Шаблон: {rename.OldPattern}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                    taskLog.ResultText = $"---Новый шаблон: {rename.NewPattern}";
                    taskLog.DateTimeLog = DateTime.Now;
                    await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
                    await unitOfWork.SaveAsync();
                }
                break;
            case OperationName.Delete:
                break;
            case OperationName.Clrbuf:
                break;
        }
    }

    public async Task StepLog(TaskStepEntity step, string text, string filename = "", ResultOperation resultOperation = ResultOperation.I)
    {
        TaskLogEntity taskLog = new()
        {
            DateTimeLog = DateTime.Now,
            OperationId = step.OperationId,
            ResultText = text,
            StepId = step.StepId,
            TaskId = step.TaskId,
            StepNumber = step.StepNumber,
            OperationName = step.OperationName.ToString()
        };
        if (filename != "")
        {
            taskLog.FileName = filename;
        }
        taskLog.ResultOperation = resultOperation;

        await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
        await unitOfWork.SaveAsync();
    }

    public async Task TaskLog(string TaskId, string text, ResultOperation? resultOperation = null)
    {
        TaskLogEntity taskLog = new()
        {
            DateTimeLog = DateTime.Now,
            TaskId = TaskId,
            ResultText = text,
            ResultOperation = resultOperation
        };

        await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
        await unitOfWork.SaveAsync();
    }
}
