using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class LockRepository(AppDbContext context) : ILockRepository
{
    public async Task<LockInfoEntity> Create(LockInfoEntity entity)
    {
        await context.LockInfo.AddAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteByTaskId(string taskId)
    {
        var lockInfo = await GetByTaskId(taskId);
        if (lockInfo != null)
        {
            context.LockInfo.Remove(lockInfo);
            return true;
        }
        return false;
    }

    public async Task<LockInfoEntity?> GetByTaskId(string taskId)
    {
        return await context.LockInfo.FirstOrDefaultAsync(x => x.EntityId == taskId);
    }
}
