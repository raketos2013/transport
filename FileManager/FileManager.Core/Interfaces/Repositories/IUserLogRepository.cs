using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IUserLogRepository
{
    Task AddUserLog(UserLogEntity userLog);
    Task<List<UserLogEntity>> GetAllLogs();
}
