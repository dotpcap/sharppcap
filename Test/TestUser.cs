using NUnit.Framework;
using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;

namespace Test
{
    public static class TestUser
    {
        public const string Username = "SharpPcap.Test.User";
        public const string Password = "password";

        public static bool Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Bash("adduser", Username);
                Bash("adduser", Username, "sudo");
                Bash("bash", "-c", $"\"echo -e {Username}:{Password} | chpasswd\"");
            }
            // OS not supported
            return false;
        }

        private static void Bash(string cmd, params string[] args)
        {
            var process = Process.Start(cmd, args);
            process.WaitForExit();
            Assert.AreEqual(process.ExitCode, 0);
        }

        public static void Delete()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var ctx = new PrincipalContext(ContextType.Machine);
                var user = UserPrincipal.FindByIdentity(ctx, Username);
                user?.Delete();
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Bash("userdel", Username);
            }
        }
    }
}
