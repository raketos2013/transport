using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface ILockRepository
{
    bool Create(LockInfoEntity entity);
    bool DeleteByTaskId(string taskId);
    LockInfoEntity? GetByTaskId(string taskId);
}
