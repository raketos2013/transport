namespace FileManager.Core.Interfaces.Services;

public interface IAuthService
{
    (bool, string) AuthenticateUser(string username, string password);
}
