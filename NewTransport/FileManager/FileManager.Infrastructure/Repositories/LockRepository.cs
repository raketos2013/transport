using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;

namespace FileManager.Infrastructure.Repositories;

public class LockRepository(AppDbContext context) : ILockRepository
{
    public bool Create(LockInfoEntity entity)
    {
        context.LockInfo.Add(entity);
        return context.SaveChanges() > 0;
    }

    public bool DeleteByTaskId(string taskId)
    {
        var lockInfo = GetByTaskId(taskId);
        if (lockInfo != null)
        {
            context.LockInfo.Remove(lockInfo);
            return context.SaveChanges() > 0;
        }
        return false;
    }

    public LockInfoEntity? GetByTaskId(string taskId)
    {
        return context.LockInfo.FirstOrDefault(x => x.EntityId == taskId);
    }
}
