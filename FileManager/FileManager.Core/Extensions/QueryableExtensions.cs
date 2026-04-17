using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Queries;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace FileManager.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, SortLogs direction)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;

        // Находим свойство в объекте T по имени (регистронезависимо)
        var propertyInfo = typeof(T).GetProperty(sortBy,
            System.Reflection.BindingFlags.IgnoreCase |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        if (propertyInfo == null) return query;

        // Строим выражение: t => t.PropertyName
        var parameter = Expression.Parameter(typeof(T), "t");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var orderByExp = Expression.Lambda(propertyAccess, parameter);

        // Выбираем метод: OrderBy или OrderByDescending
        string methodName = direction switch
        {
            SortLogs.Ascending => "OrderBy",
            SortLogs.Descending => "OrderByDescending",
            _ => ""
        };
        if (methodName == "") return query;

        var resultExp = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), propertyInfo.PropertyType],
            query.Expression,
            Expression.Quote(orderByExp)
        );

        return query.Provider.CreateQuery<T>(resultExp);
    }

    // Универсальный фильтр для чисел и дат
    public static IQueryable<T> ApplyCompare<T, TVal>(this IQueryable<T> query,
        Expression<Func<T, TVal>> selector, TVal value, FilterOptions op)
    {
        if (value == null) return query;

        var param = selector.Parameters[0];
        var left = selector.Body;
        var right = Expression.Constant(value, typeof(TVal));

        Expression body = op switch
        {
            FilterOptions.Equal => Expression.Equal(left, right),
            FilterOptions.NotEqual => Expression.NotEqual(left, right),
            FilterOptions.MoreEqual => Expression.GreaterThanOrEqual(left, right),
            FilterOptions.LessEqual => Expression.LessThanOrEqual(left, right),
            FilterOptions.More => Expression.GreaterThan(left, right),
            FilterOptions.Less => Expression.LessThan(left, right),
            _ => Expression.Equal(left, right)
        };

        return query.Where(Expression.Lambda<Func<T, bool>>(body, param));
    }

    // Фильтр для строк (Contains, Equals и т.д.)
    public static IQueryable<T> ApplyString<T>(this IQueryable<T> query,
        Expression<Func<T, string>> selector, string? value, FilterOptions op)
    {
        if (string.IsNullOrWhiteSpace(value)) return query;

        var method = typeof(string).GetMethod(op.ToString(), [typeof(string)])
                     ?? typeof(string).GetMethod("Contains", [typeof(string)]);

        var call = Expression.Call(selector.Body, method!, Expression.Constant(value));
        return query.Where(Expression.Lambda<Func<T, bool>>(call, selector.Parameters));
    }


    //public static async Task<PagedList<TaskLogEntity>> SortSkipTakeAsync(this IQueryable<TaskLogEntity> items, Query query)
    //{
    //    if (query.TaskId != null)
    //    {
    //        switch (query.TaskIdOption)
    //        {
    //            case FilterOptions.Equal:
    //                items = items.Where(x => x.TaskId == query.TaskId);
    //                break;
    //            case FilterOptions.NotEqual:
    //                items = items.Where(x => x.TaskId != query.TaskId);
    //                break;
    //            case FilterOptions.More:
    //                items = items.Where(x => string.Compare(query.TaskId, x.TaskId) > 0);
    //                break;
    //            case FilterOptions.Less:
    //                items = items.Where(x => string.Compare(query.TaskId, x.TaskId) < 0);
    //                break;
    //            case FilterOptions.MoreEqual:
    //                items = items.Where(x => string.Compare(query.TaskId, x.TaskId) >= 0);
    //                break;
    //            case FilterOptions.LessEqual:
    //                items = items.Where(x => string.Compare(query.TaskId, x.TaskId) <= 0);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    if (query.OperationName != OperationName.None)
    //    {
    //        switch (query.OperationNameOption)
    //        {
    //            case FilterOptions.Equal:
    //                items = items.Where(x => x.OperationName == query.OperationName.ToString());
    //                break;
    //            case FilterOptions.NotEqual:
    //                items = items.Where(x => x.OperationName != query.OperationName.ToString());
    //                break;
    //            case FilterOptions.More:
    //                items = items.Where(x => string.Compare(query.OperationName.ToString(), x.OperationName) > 0);
    //                break;
    //            case FilterOptions.Less:
    //                items = items.Where(x => string.Compare(query.OperationName.ToString(), x.OperationName) < 0);
    //                break;
    //            case FilterOptions.MoreEqual:
    //                items = items.Where(x => string.Compare(query.OperationName.ToString(), x.OperationName) >= 0);
    //                break;
    //            case FilterOptions.LessEqual:
    //                items = items.Where(x => string.Compare(query.OperationName.ToString(), x.OperationName) <= 0);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    if (query.StepNumber != 0)
    //    {
    //        switch (query.StepNumberOption)
    //        {
    //            case FilterOptions.Equal:
    //                items = items.Where(x => x.StepNumber == query.StepNumber);
    //                break;
    //            case FilterOptions.NotEqual:
    //                items = items.Where(x => x.StepNumber != query.StepNumber);
    //                break;
    //            case FilterOptions.More:
    //                items = items.Where(x => x.StepNumber > query.StepNumber);
    //                break;
    //            case FilterOptions.Less:
    //                items = items.Where(x => x.StepNumber < query.StepNumber);
    //                break;
    //            case FilterOptions.MoreEqual:
    //                items = items.Where(x => x.StepNumber >= query.StepNumber);
    //                break;
    //            case FilterOptions.LessEqual:
    //                items = items.Where(x => x.StepNumber <= query.StepNumber);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    if (query.ResultOperation != ResultOperation.N)
    //    {
    //        switch (query.ResultOperationOption)
    //        {
    //            case FilterOptions.Equal:
    //                items = items.Where(x => x.ResultOperation == query.ResultOperation);
    //                break;
    //            case FilterOptions.NotEqual:
    //                items = items.Where(x => x.ResultOperation != query.ResultOperation);
    //                break;
    //            case FilterOptions.More:
    //                items = items.Where(x => string.Compare(query.ResultOperation.ToString(), x.ResultOperation.ToString()) > 0);
    //                break;
    //            case FilterOptions.Less:
    //                items = items.Where(x => string.Compare(query.ResultOperation.ToString(), x.ResultOperation.ToString()) < 0);
    //                break;
    //            case FilterOptions.MoreEqual:
    //                items = items.Where(x => string.Compare(query.ResultOperation.ToString(), x.ResultOperation.ToString()) >= 0);
    //                break;
    //            case FilterOptions.LessEqual:
    //                items = items.Where(x => string.Compare(query.ResultOperation.ToString(), x.ResultOperation.ToString()) <= 0);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    if (string.IsNullOrEmpty(query.FileName))
    //    {
    //        if (query.FileNameOption == FilterOptions.NotEqual)
    //        {
    //            items = items.Where(x => !string.IsNullOrEmpty(x.FileName));
    //        }
    //    }
    //    else
    //    {
    //        switch (query.FileNameOption)
    //        {
    //            case FilterOptions.Equal:
    //                items = items.Where(x => x.FileName == query.FileName);
    //                break;
    //            case FilterOptions.NotEqual:
    //                items = items.Where(x => x.FileName != query.FileName);
    //                break;
    //            case FilterOptions.More:
    //                items = items.Where(x => string.Compare(query.FileName, x.FileName) > 0);
    //                break;
    //            case FilterOptions.Less:
    //                items = items.Where(x => string.Compare(query.FileName, x.FileName) < 0);
    //                break;
    //            case FilterOptions.MoreEqual:
    //                items = items.Where(x => string.Compare(query.FileName, x.FileName) >= 0);
    //                break;
    //            case FilterOptions.LessEqual:
    //                items = items.Where(x => string.Compare(query.FileName, x.FileName) <= 0);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    if (string.IsNullOrEmpty(query.Text))
    //    {
    //        if (query.TextOption == FilterOptions.NotEqual)
    //        {
    //            items = items.Where(x => !string.IsNullOrEmpty(x.ResultText));
    //        }
    //    }
    //    else
    //    {
    //        switch (query.TextOption)
    //        {
    //            case FilterOptions.Equal:
    //                items = items.Where(x => x.ResultText == query.Text);
    //                break;
    //            case FilterOptions.NotEqual:
    //                items = items.Where(x => x.ResultText != query.Text);
    //                break;
    //            case FilterOptions.More:
    //                items = items.Where(x => string.Compare(query.Text, x.ResultText) > 0);
    //                break;
    //            case FilterOptions.Less:
    //                items = items.Where(x => string.Compare(query.Text, x.ResultText) < 0);
    //                break;
    //            case FilterOptions.MoreEqual:
    //                items = items.Where(x => string.Compare(query.Text, x.ResultText) >= 0);
    //                break;
    //            case FilterOptions.LessEqual:
    //                items = items.Where(x => string.Compare(query.Text, x.ResultText) <= 0);
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    switch (query.FieldSortLogs)
    //    {
    //        case FieldSortLogs.Date:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.DateTimeLog);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.DateTimeLog);
    //            }
    //            break;
    //        case FieldSortLogs.Task:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.TaskId);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.TaskId);
    //            }
    //            break;
    //        case FieldSortLogs.Operation:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.OperationName);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.OperationName);
    //            }
    //            break;
    //        case FieldSortLogs.Result:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.ResultOperation);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.ResultOperation);
    //            }
    //            break;
    //        case FieldSortLogs.FileName:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.FileName);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.FileName);
    //            }
    //            break;
    //        default:
    //            break;
    //    }
    //    switch (query.FieldSortLogs)
    //    {
    //        case FieldSortLogs.Date:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.DateTimeLog);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.DateTimeLog);
    //            }
    //            break;
    //        case FieldSortLogs.Task:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.TaskId);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.TaskId);
    //            }
    //            break;
    //        case FieldSortLogs.Operation:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.OperationName);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.OperationName);
    //            }
    //            break;
    //        case FieldSortLogs.Result:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.ResultOperation);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.ResultOperation);
    //            }
    //            break;
    //        case FieldSortLogs.FileName:
    //            if (query.SortLogs == SortLogs.Ascending)
    //            {
    //                items = items.OrderBy(x => x.FileName);
    //            }
    //            else
    //            {
    //                items = items.OrderByDescending(x => x.FileName);
    //            }
    //            break;
    //        default:
    //            break;
    //    }

    //    return await PagedList<TaskLogEntity>.ToPagedList(items, query.Skip, query.Take);
    //}
}
