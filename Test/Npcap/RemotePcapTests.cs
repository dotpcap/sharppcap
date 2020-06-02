using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SharpPcap.Npcap;
using System.Threading;
using System.Net;
using System.DirectoryServices.AccountManagement;
using SharpPcap.LibPcap;
using SharpPcap;

namespace Test.Npcap
{
    [TestFixture]
    [Category("RemotePcap")]
    [Platform("Win")]
    public class RemotePcapTests
    {
        private const string NullAuthArgs = "-n";
        private const string PwdAuthArgs = "";
        private const string Username = "SharpPcap.Test.User";
        private const string Password = "password";
        public static readonly IPEndPoint LoopbackSource = new IPEndPoint(IPAddress.Loopback, 2002);

        public static readonly ICredentials[] NullAuthCredentials = new ICredentials[]
        {
            null,
            new PlainCredential(null, null, null),
            new PlainCredential("foo", "bar", "null"),
            new PlainCredential("foo", "bar", "0")
        };

        [Test]
        public void PcapInterfaceNullAuthTest(
            [ValueSource(nameof(NullAuthCredentials))] ICredentials credentials
        )
        {
            using (new RemotePcapServer(NullAuthArgs))
            {
                var list = PcapInterface.GetAllPcapInterfaces(LoopbackSource, credentials);
                CollectionAssert.IsNotEmpty(list);
            }
        }


        [Test]
        public void NpcapDeviceListNullAuthTest()
        {
            using (new RemotePcapServer(NullAuthArgs))
            {
                var auth = new RemoteAuthentication(AuthenticationTypes.Null, null, null);
                CollectionAssert.IsNotEmpty(NpcapDeviceList.Devices(IPAddress.Loopback, 2002, auth));
                CollectionAssert.IsNotEmpty(NpcapDeviceList.Devices(IPAddress.Loopback, 2002, null));
            }
        }

        /// <summary>
        /// since creating a user is a time consuming operation,
        /// all logic related to testing password based rpcap is grouped here
        /// if the test gets too long, it would be moved to its own file
        /// </summary>
        [Test]
        public void PwdAuthTest()
        {
            try
            {
                if (!CreateTestUser())
                {
                    Assert.Inconclusive("Please rerun the test as administrator.");
                }
                var goodCred = new RemoteAuthentication(AuthenticationTypes.Password, Username, Password);
                var badCred = new RemoteAuthentication(AuthenticationTypes.Password, "foo", "bar");
                using (new RemotePcapServer(PwdAuthArgs))
                {
                    var pcapIfs = PcapInterface.GetAllPcapInterfaces("rpcap://localhost/", goodCred);
                    var npcapDevices = NpcapDeviceList.Devices(IPAddress.Loopback, NpcapDeviceList.RpcapdDefaultPort, goodCred);
                    CollectionAssert.IsNotEmpty(npcapDevices);

                    var devices = new PcapDevice[]{
                        // using NpcapDevice
                        npcapDevices[0],
                        // using rpcap with LibPcapLiveDevice should be possible
                        new LibPcapLiveDevice(pcapIfs[0])
                    };
                    foreach (var device in devices)
                    {
                        // repassing the auth to Open() should be optional
                        device.Open();
                        Assert.IsTrue(device.Opened);
                        device.Close();
                    }

                    Assert.Throws<PcapException>(
                        () => npcapDevices[0].Open(OpenFlags.NoCaptureRemote, 1, badCred),
                        "Credentials provided to Open() method takes precedence"
                    );

                    Assert.Throws<PcapException>(
                        () => PcapInterface.GetAllPcapInterfaces("rpcap://localhost/", badCred)
                    );
                }
            }
            finally
            {
                DeleteTestUser();
            }
        }

        private static bool CreateTestUser()
        {
            try
            {
                DeleteTestUser();
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

        private static void DeleteTestUser()
        {
            var ctx = new PrincipalContext(ContextType.Machine);
            var user = UserPrincipal.FindByIdentity(ctx, Username);
            user?.Delete();
        }

    }

    class PlainCredential : NetworkCredential
    {
        public PlainCredential(string username, string password, string domain)
           : base(username, password, domain)
        {

        }

        public override string ToString()
        {
            var str = UserName;
            if (!string.IsNullOrEmpty(Domain))
            {
                str += "@" + Domain;
            }
            return str;
        }
    }

    class RemotePcapServer : IDisposable
    {
        private readonly Process process;

        public RemotePcapServer(string args)
        {
            var binFile = new[]
            {
                @"C:\Program Files (x86)\WinPcap\rpcapd.exe",
                @"C:\Program Files\WinPcap\rpcapd.exe"
            }.FirstOrDefault(File.Exists) ?? "rpcapd";
            process = Process.Start(new ProcessStartInfo
            {
                FileName = binFile,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            // wait until the process has started up
            Thread.Sleep(500);
        }
        public void Dispose()
        {
            process.Kill();
            process.Dispose();
        }
    }
}
