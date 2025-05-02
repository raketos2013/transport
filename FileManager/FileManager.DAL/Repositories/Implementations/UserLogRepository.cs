using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;

namespace FileManager.DAL.Repositories.Implementations
{
    public class UserLogRepository(AppDbContext appDbContext) : IUserLogRepository
    {
        public void AddUserLog(UserLogEntity userLog)
        {
            appDbContext.UserLog.Add(userLog);
            appDbContext.SaveChanges();
        }

        public List<UserLogEntity> GetAllLogs()
        {
            return appDbContext.UserLog.ToList();
        }
    }
}
