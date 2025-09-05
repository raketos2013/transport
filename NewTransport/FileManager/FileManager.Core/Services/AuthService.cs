using FileManager.Core.Interfaces.Services;
using Novell.Directory.Ldap;

namespace FileManager.Core.Services;

public class AuthService : IAuthService
{
    public (bool, string) AuthenticateUser(string username, string password)
    {
        bool isAuthenticated = false;
        string? givenName = "";
		try
		{
            using var ldapConnection = new LdapConnection();
            string domainName = "bb.asb";
            string userDn = $"{username}@{domainName}";
            int ldapVersion = LdapConnection.LdapV3;

            ldapConnection.ConnectionTimeout = 0;
            //ldapConnection.Connect(domainName, LdapConnection.DefaultPort);
        }
		catch (Exception)
		{

			throw;
		}
        throw new NotImplementedException();
    }
}
