using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Services;


namespace FileManager_Server.Loggers;

class TaskLogger(AppDbContext appDbContext) : ITaskLogger
{
    public void OperationLog(TaskStepEntity step)
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
                OperationCopyEntity? operation = appDbContext.OperationCopy.FirstOrDefault(x => x.OperationId == step.OperationId);
                if (operation != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Сортировка: {operation.Sort.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Контроль дублирования по журналу: {operation.FileInLog}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Контроль Файл есть в назначении: {operation.FileInSource.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Аттрибуты файла: {operation.FileAttribute.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();

                }
                break;
            case OperationName.Move:
                OperationMoveEntity? move = appDbContext.OperationMove.FirstOrDefault(x => x.OperationId == step.OperationId);
                if (move != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Контроль дублирования по журналу: {move.FileInLog}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Сортировка: {move.Sort.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Аттрибуты файла: {move.FileAttribute.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                }
                break;
            case OperationName.Read:
                OperationReadEntity? operationRead = appDbContext.OperationRead.FirstOrDefault(x => x.OperationId == step.OperationId);
                if (operationRead != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Кодировка: {operationRead.Encode}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"--Контроль Файл есть в назначении: {operationRead.FileInSource.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                }
                break;
            case OperationName.Exist:
                OperationExistEntity? exist = appDbContext.OperationExist.FirstOrDefault(x => x.OperationId == step.OperationId);
                if (exist != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Ожидаемый результат: {exist.ExpectedResult.GetDescription()}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Прервать задачу: {exist.BreakTaskAfterError}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                }
                break;
            case OperationName.Rename:
                OperationRenameEntity? rename = appDbContext.OperationRename.FirstOrDefault(x => x.OperationId == step.OperationId);
                if (rename != null)
                {
                    taskLog.ResultText = "Свойства операции - ";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Шаблон: {rename.OldPattern}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                    taskLog.ResultText = $"---Новый шаблон: {rename.NewPattern}";
                    taskLog.DateTimeLog = DateTime.Now;
                    appDbContext.TaskLog.Add(taskLog);
                    appDbContext.SaveChanges();
                }
                break;
            case OperationName.Delete:
                //operation = _appDbContext.OperationDelete.FirstOrDefault();
                break;
            case OperationName.Clrbuf:
                break;
        }




    }

    public void StepLog(TaskStepEntity step, string text, string filename = "")
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
        taskLog.ResultOperation = ResultOperation.I;

        appDbContext.TaskLog.Add(taskLog);
        appDbContext.SaveChanges();
    }

    public void TaskLog(string TaskId, string text)
    {
        TaskLogEntity taskLog = new()
        {
            DateTimeLog = DateTime.Now,
            TaskId = TaskId,
            ResultText = text
        };

        appDbContext.TaskLog.Add(taskLog);
        appDbContext.SaveChanges();
    }
}
