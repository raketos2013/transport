
using Microsoft.EntityFrameworkCore;

namespace FileManager.Core.Extensions;

public class PagedList<T> : List<T>
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber == 0)
        {
            pageNumber = 1;
        }
        var count = source.Count(); 
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return new PagedList<T>(await items.ToListAsync(), count, pageNumber, pageSize);
    }
}
