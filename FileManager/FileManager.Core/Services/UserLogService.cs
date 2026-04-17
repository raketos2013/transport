using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace FileManager.Core.Services;

public class UserLogService(IUnitOfWork unitOfWork,
                            IHttpContextAccessor httpContextAccessor) : IUserLogService
{
    public async Task AddLog(string action, string data, string userName = "")
    {
        if (userName == "")
        {
            userName = httpContextAccessor.HttpContext.User.Identity.Name;
        }
        UserLogEntity log = new()
        {
            Action = action,
            Data = data,
            UserName = userName,
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
