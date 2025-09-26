using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace FileManager.Core.Services;

public class UserLogService(IUserLogRepository userLogRepository,
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
        await userLogRepository.AddUserLog(log);
    }

    public async Task<List<UserLogEntity>> GetAllLogs()
    {
        return await userLogRepository.GetAllLogs();
    }
}
