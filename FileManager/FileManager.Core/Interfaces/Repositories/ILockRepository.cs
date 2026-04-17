using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface ILockRepository
{
    Task<LockInfoEntity> Create(LockInfoEntity entity);
    Task<bool> DeleteByTaskId(string taskId);
    Task<LockInfoEntity?> GetByTaskId(string taskId);
    Task<List<LockInfoEntity>> GetLockedTasks();
}
