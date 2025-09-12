using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class UserLogService(IUserLogRepository userLogRepository) : IUserLogService
{
    public async Task AddLog(string username, string action, string data)
    {
        UserLogEntity log = new()
        {
            Action = action,
            Data = data,
            UserName = username,
            DateTimeLog = DateTime.Now
        };
        await userLogRepository.AddUserLog(log);
    }

    public async Task<List<UserLogEntity>> GetAllLogs()
    {
        return await userLogRepository.GetAllLogs();
    }
}
