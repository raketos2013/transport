using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace FileManager.Core.Services;

public class LockService(IUnitOfWork unitOfWork,
                        IHttpContextAccessor httpContextAccessor) : ILockService
{
    public async Task<List<LockInfoEntity>> GetLockedTasks()
    {
        return await unitOfWork.LockRepository.GetLockedTasks();
    }

    public async Task<LockInfoEntity?> IsLocked(string taskId)
    {
        return await unitOfWork.LockRepository.GetByTaskId(taskId);
    }

    public async Task<bool> Lock(string taskId)
    {
        LockInfoEntity lockInfo = new()
        {
            UserId = httpContextAccessor.HttpContext.User.Identity.Name,
            EntityId = taskId,
            Created = DateTime.Now
        };
        await unitOfWork.LockRepository.Create(lockInfo);
        return await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> Unlock(string taskId)
    {
        await unitOfWork.LockRepository.DeleteByTaskId(taskId);
        return await unitOfWork.SaveAsync() > 0;
    }
}
