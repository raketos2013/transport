using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IUserLogService
{
    Task AddLog(string action, string data, string userName = "");
    Task<List<UserLogEntity>> GetAllLogs();
}
