using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IUserLogService
{
    Task AddLog(string username, string action, string data);
    Task<List<UserLogEntity>> GetAllLogs();
}
