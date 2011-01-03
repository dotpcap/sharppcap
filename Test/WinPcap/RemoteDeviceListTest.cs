using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using NUnit.Framework;
using SharpPcap.WinPcap;

namespace Test.WinPcap
{
    [TestFixture]
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
                p = System.Diagnostics.Process.Start(exe1, noAuthenticationParameter);
            } catch(Exception)
            {
                try
                {
                    p = System.Diagnostics.Process.Start(exe2, noAuthenticationParameter);
                } catch(Exception)
                {
                    throw;
                }
            }

            if(p == null)
            {
                throw new System.Exception("unable to start process");
            }

            // wait until the process has started up
            System.Threading.Thread.Sleep(500);

            // retrieve the device list
            var defaultPort = WinPcapDeviceList.RpcapdDefaultPort;
            var deviceList = WinPcapDeviceList.Devices(System.Net.IPAddress.Loopback, defaultPort, null);

            foreach (var d in deviceList)
            {
                Console.WriteLine(d.ToString());
            }

            System.Threading.Thread.Sleep(2000);

            p.Kill();
        }
    }
}
