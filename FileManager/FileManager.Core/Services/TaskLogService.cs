using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Exceptions;
using FileManager.Core.Extensions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Queries;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Core.Services;

public class TaskLogService(IUnitOfWork unitOfWork)
            : ITaskLogService
{
    public async Task<TaskLogEntity> AddTaskLog(TaskLogEntity taskLog)
    {
        var createdLog = await unitOfWork.TaskLogRepository.AddTaskLog(taskLog);
        return await unitOfWork.SaveAsync() > 0 ? createdLog
                            : throw new DomainException("Ошибка добавления лога");
    }

    public async Task<PagedList<TaskLogEntity>> GetLogs(Query query)
    {
        var items = unitOfWork.TaskLogRepository.GetLogs()
                                .ApplyCompare(f => f.DateTimeLog.Date, query.DateFrom.Date, FilterOptions.MoreEqual)
                                .ApplyCompare(f => f.DateTimeLog.Date, query.DateTo.Date, FilterOptions.LessEqual);

        if (query.TimeFrom.TimeOfDay != new TimeSpan(0, 0, 0))
        {
            items = items.Where(x => x.DateTimeLog.TimeOfDay >=  query.TimeFrom.TimeOfDay);
        }
        if (query.TimeTo.TimeOfDay != new TimeSpan(0, 0, 0))
        {
            items = items.Where(x => x.DateTimeLog.TimeOfDay <= query.TimeTo.TimeOfDay);
        }

        if (query.TaskId != null)
        {
            items = items.ApplyString(f => f.TaskId, query.TaskId, query.TaskIdOption);
        }

        if (query.StepNumber != 0)
        {
            items = items.ApplyCompare(f => f.StepNumber, query.StepNumber, query.StepNumberOption);
        }

        if (query.ResultOperation != ResultOperation.N)
        {
            items = items.ApplyCompare(f => f.ResultOperation, query.ResultOperation, query.ResultOperationOption);
        }

        if (query.OperationName != OperationName.None)
        {
            items = items.ApplyString(f => f.OperationName, query.OperationName.ToString(), query.OperationNameOption);
        }

        if (string.IsNullOrEmpty(query.FileName))
        {
            if (query.FileNameOption == FilterOptions.NotEqual)
            {
                items = items.Where(x => !string.IsNullOrEmpty(x.FileName));
            }
        }
        else
        {
            items = items.ApplyString(f => f.FileName, query.FileName, query.FileNameOption);
        }

        if (string.IsNullOrEmpty(query.Text))
        {
            if (query.TextOption == FilterOptions.NotEqual)
            {
                items = items.Where(x => !string.IsNullOrEmpty(x.ResultText));
            }
        }
        else
        {
            items = items.ApplyString(f => f.ResultText, query.Text, query.TextOption);
        }

        items = items.ApplySorting(query.FieldSortLogs.ToString(), query.SortLogs);

        return await PagedList<TaskLogEntity>.ToPagedList(items, query.Skip, query.Take);
    }

    public async Task<List<TaskLogEntity>> GetLogs()
    {
        return await unitOfWork.TaskLogRepository.GetLogs().ToListAsync();
    }

    public IQueryable<TaskLogEntity> GetLogsByTaskId(string taskId)
    {
        return unitOfWork.TaskLogRepository.GetLogsByTaskId(taskId);
    }

    public async Task<List<TaskLogEntity>> GetLastLogTasks()
    {
        return await unitOfWork.TaskLogRepository.GetLastLogs();
    }
}
