using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class UserLogService(IUserLogRepository userLogRepository) : IUserLogService
{
    public void AddLog(string username, string action, string data)
    {
        UserLogEntity log = new()
        {
            Action = action,
            Data = data,
            UserName = username,
            DateTimeLog = DateTime.Now
        };
        userLogRepository.AddUserLog(log);
    }

    public List<UserLogEntity> GetAllLogs()
    {
        return userLogRepository.GetAllLogs();
    }
}
