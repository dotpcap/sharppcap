using System;
using System.Diagnostics;
using NUnit.Framework;
using SharpPcap.Npcap;

namespace Test.Npcap
{
    [TestFixture]
    [Category("RemotePcap")]
    [Platform("Win")]
    public class RemoteDeviceListTest
    {
        [Test]
        public void RemoteTest()
        {
            var noAuthenticationParameter = "-n";
            var exe1 = "c:\\Program Files (x86)\\WinPcap\\rpcapd.exe";
            var exe2 = "c:\\Program Files\\WinPcap\\rpcapd.exe";
            Process p;
            try
            {
                p = Process.Start(exe1, noAuthenticationParameter);
            }
            catch (Exception)
            {
                try
                {
                    p = Process.Start(exe2, noAuthenticationParameter);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            if (p == null)
            {
                throw new Exception("unable to start process");
            }

            // wait until the process has started up
            System.Threading.Thread.Sleep(500);

            // retrieve the device list
            var defaultPort = NpcapDeviceList.RpcapdDefaultPort;
            var deviceList = NpcapDeviceList.Devices(System.Net.IPAddress.Loopback, defaultPort, null);

            foreach (var d in deviceList)
            {
                Console.WriteLine(d.ToString());
            }

            System.Threading.Thread.Sleep(2000);

            p.Kill();
        }
    }
}
