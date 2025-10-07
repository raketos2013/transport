using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace FileManager.Core.Services;

public class UserLogService(IUnitOfWork unitOfWork,
                            IHttpContextAccessor httpContextAccessor) : IUserLogService
{
    public async Task AddLog(string action, string data)
    {
        UserLogEntity log = new()
        {
            Action = action,
            Data = data,
            UserName = httpContextAccessor.HttpContext.User.Identity.Name,
            DateTimeLog = DateTime.Now
        };
        await unitOfWork.UserLogRepository.AddUserLog(log);
        await unitOfWork.SaveAsync();
    }

    public async Task<List<UserLogEntity>> GetAllLogs()
    {
        return await unitOfWork.UserLogRepository.GetAllLogs();
    }
}
