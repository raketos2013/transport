using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class UserLogRepository(AppDbContext appDbContext) : IUserLogRepository
{
    public async Task AddUserLog(UserLogEntity userLog)
    {
        await appDbContext.UserLog.AddAsync(userLog);
    }

    public async Task<List<UserLogEntity>> GetAllLogs()
    {
        return await appDbContext.UserLog.ToListAsync();
    }
}
