using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;

namespace Test
{
#if NET
    [SupportedOSPlatform("windows")]
#endif
    public static class TestUser
    {
        public const string Username = "SharpPcap.Test.User";
        public const string Password = "password";

        public static bool Create()
        {
            try
            {
                Delete();
                var ctx = new PrincipalContext(ContextType.Machine);
                using (var user = new UserPrincipal(ctx, Username, Password, true))
                {
                    user.Save();
                }
                return true;
            }
            catch (PrincipalException)
            {
                return false;
            }
        }

        public static void Delete()
        {
            var ctx = new PrincipalContext(ContextType.Machine);
            var user = UserPrincipal.FindByIdentity(ctx, Username);
            user?.Delete();
        }
    }
}
