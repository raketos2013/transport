using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface ILockService
{
    public bool Lock(string taskId, string userId);
    public bool Unlock(string taskId);
    public LockInfoEntity? IsLocked(string taskId);
}
