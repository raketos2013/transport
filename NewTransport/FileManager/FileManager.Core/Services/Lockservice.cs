using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class Lockservice(ILockRepository lockRepository) : ILockService
{
    public LockInfoEntity? IsLocked(string taskId)
    {
        return lockRepository.GetByTaskId(taskId);
    }

    public bool Lock(string taskId, string userId)
    {
        LockInfoEntity lockInfo = new()
        {
            UserId = userId,
            EntityId = taskId,
            Created = DateTime.Now
        };
        return lockRepository.Create(lockInfo);
    }

    public bool Unlock(string taskId)
    {
        return lockRepository.DeleteByTaskId(taskId);
    }
}
