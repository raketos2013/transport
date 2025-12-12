using FileManager.Core.Interfaces.Services;
using Novell.Directory.Ldap;

namespace FileManager.Core.Services;

public class AuthService : IAuthService
{
    public async Task<(bool, string)> AuthenticateUser(string username, string password)
    {
        bool isAuthenticated = false;
        string? givenName = "";
		try
		{
            using var ldapConnection = new LdapConnection { SecureSocketLayer = false };
            string domainName = "bb.asb";
            string userDn = $"{username}@{domainName}";
            int ldapVersion = LdapConnection.LdapV3;

            ldapConnection.ConnectionTimeout = 0;
            ldapConnection.Connect("sca-dc1.bb.asb", LdapConnection.DefaultPort);

            if (ldapConnection.Connected)
            {
                ldapConnection.Bind(ldapVersion, userDn, password);
                if (ldapConnection.Bound)
                {
                    LdapSearchConstraints constraints = new()
                    {
                        MaxResults = 10,
                        ServerTimeLimit = 0,
                        TimeLimit = 10000,
                        BatchSize = 1,
                    };

                    string searchBase = @"OU=50000104,OU=Users,OU=BB,DC=bb,DC=asb";
                    string? searchFilter = $"Name={username}";
                    string[] attrs = ["memberOf", "displayName", "Name"];
                    LdapSearchResults? results = (LdapSearchResults)ldapConnection.Search(searchBase,
                                                                                                LdapConnection.ScopeSub,
                                                                                                searchFilter,
                                                                                                attrs,
                                                                                                false,
                                                                                                constraints);
                    Thread.Sleep(5000);

                    if (results != null)
                    {
                        if (results.Count > 0)
                        {
                            var userInfo = results.FirstOrDefault();
                            givenName = userInfo.GetAttribute("displayName").StringValue;
                            var groups = userInfo.GetAttribute("memberOf").StringValueArray;

                            //givenName = results?.FirstOrDefault(p => p.Dn.Contains(username))?.GetAttribute("displayName").StringValue;
                            if (groups == null)
                                throw new Exception();
                            List<string> groupsList = [];
                            foreach (var item in groups)
                            {
                                var group = item.Split(',')[0].Substring(3);
                                groupsList.Add(group);
                            }

                            isAuthenticated = true;
                            
                            ldapConnection.Disconnect();
                            return (isAuthenticated, givenName ?? "");
                        }
                    }
                    ldapConnection.Bind(null, null);
                }
            }
        }
		catch (Exception)
		{
            return (isAuthenticated, givenName ?? "");
        }
        return (isAuthenticated, givenName ?? "");
    }
}
