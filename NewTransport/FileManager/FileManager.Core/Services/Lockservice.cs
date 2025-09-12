using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class Lockservice(ILockRepository lockRepository) : ILockService
{
    public async Task<LockInfoEntity?> IsLocked(string taskId)
    {
        return await lockRepository.GetByTaskId(taskId);
    }

    public async Task<bool> Lock(string taskId, string userId)
    {
        LockInfoEntity lockInfo = new()
        {
            UserId = userId,
            EntityId = taskId,
            Created = DateTime.Now
        };
        return await lockRepository.Create(lockInfo);
    }

    public async Task<bool> Unlock(string taskId)
    {
        return await lockRepository.DeleteByTaskId(taskId);
    }
}
