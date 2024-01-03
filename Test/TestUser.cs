using NUnit.Framework;
using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;

namespace Test
{
    public static class TestUser
    {
        public const string Username = "sharppcaptestuser";
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
            var arguments = string.Join(" ", args);
            var info = new ProcessStartInfo
            {
                FileName = cmd,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var process = Process.Start(info);

            process.OutputDataReceived += (s, e) => Console.Out.WriteLine(e.Data);
            process.ErrorDataReceived += (s, e) => Console.Error.WriteLine(e.Data);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            if (!process.WaitForExit(10000))
            {
                throw new TimeoutException($"Command '{cmd} {arguments}' timed out");
            }
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
