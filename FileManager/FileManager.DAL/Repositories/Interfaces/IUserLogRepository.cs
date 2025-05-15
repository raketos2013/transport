using FileManager.Domain.Entity;

namespace FileManager.DAL.Repositories.Interfaces
{
    public interface IUserLogRepository
    {
        void AddUserLog(UserLogEntity userLog);
        List<UserLogEntity> GetAllLogs();
    }
}
