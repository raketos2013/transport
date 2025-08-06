using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IUserLogService
{
    void AddLog(string username, string action, string data);
    List<UserLogEntity> GetAllLogs();
}
