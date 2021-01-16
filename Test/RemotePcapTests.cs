using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using SharpPcap.Npcap;
using System.Threading;
using System.Net;
using SharpPcap.LibPcap;
using SharpPcap;

namespace Test
{
    [TestFixture]
    [Category("RemotePcap")]
    public class RemotePcapTests
    {
        private const string NullAuthArgs = "-n";
        private const string PwdAuthArgs = "";

        public static readonly IPEndPoint LoopbackSource = new IPEndPoint(IPAddress.Loopback, 2002);

        public static readonly RemoteAuthentication[] NullAuthCredentials = new RemoteAuthentication[]
        {
            null,
            new PlainCredential(AuthenticationTypes.Null, null, null),
            new PlainCredential(AuthenticationTypes.Null, "foo", "bar")
        };

        [Test]
        public void PcapInterfaceNullAuthTest(
            [ValueSource(nameof(NullAuthCredentials))] RemoteAuthentication credentials
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
        [Platform("Win")]
        public void PwdAuthTest()
        {
            try
            {
                if (!TestUser.Create())
                {
                    Assert.Inconclusive("Please rerun the test as administrator.");
                }
                var goodCred = new RemoteAuthentication(AuthenticationTypes.Password, TestUser.Username, TestUser.Password);
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
                TestUser.Delete();
            }
        }

    }

    /// <summary>
    /// Used to provide more detailed information in the test runner gui by overriding ToString(), which
    /// the test runner uses when displaying the values for a ValueSource
    /// </summary>
    class PlainCredential : RemoteAuthentication
    {
        public PlainCredential(AuthenticationTypes Type,
                                     string Username,
                                     string Password)
           : base(Type, Username, Password)
        {

        }

        public override string ToString()
        {
            return String.Format("Username:{0} Password: {1} Type:{2}", Username, Password, Type.ToString());
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
                @"C:\Program Files\WinPcap\rpcapd.exe",
                "/usr/local/sbin/rpcapd"
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
