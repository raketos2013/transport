using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;

namespace FileManager.Infrastructure.Repositories;

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
