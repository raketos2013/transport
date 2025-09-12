namespace FileManager.Core.Interfaces.Services;

public interface IAuthService
{
    Task<(bool, string)> AuthenticateUser(string username, string password);
}
