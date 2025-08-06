using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IUserLogRepository
{
    void AddUserLog(UserLogEntity userLog);
    List<UserLogEntity> GetAllLogs();
}
