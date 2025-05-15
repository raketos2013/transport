using FileManager.Domain.Entity;

namespace FileManager.Services.Interfaces
{
    public interface IUserLogService
    {
        void AddLog(string username, string action, string data);
        List<UserLogEntity> GetAllLogs();
    }
}
