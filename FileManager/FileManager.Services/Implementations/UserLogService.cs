using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;

namespace FileManager.Services.Implementations;

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
