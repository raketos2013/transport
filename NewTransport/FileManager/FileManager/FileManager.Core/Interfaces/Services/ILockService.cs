using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface ILockService
{
    Task<bool> Lock(string taskId);
    Task<bool> Unlock(string taskId);
    Task<LockInfoEntity?> IsLocked(string taskId);
    Task<List<LockInfoEntity>> GetLockedTasks();
}
