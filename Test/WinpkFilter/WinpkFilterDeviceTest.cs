// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.WinDivert;
using SharpPcap.WinpkFilter;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Test.WinpkFilter
{
    [TestFixture]
    [Category("WinpkFilter")]
    [Platform("Win", Reason = "WinpkFilter driver is only available for Windows")]
    public class WinpkFilterDeviceTest
    {

        /// <summary>
        /// Modify PATH var to include our WinpkFilter DLL's so that the LoadLibrary function will
        /// find whatever WinpkFilter dll required for the current architecture.
        /// </summary>
        [OneTimeSetUp]
        public void InstallWinpkFilter()
        {
            var arch = IntPtr.Size == 8 ? "x64" : "x86";
            var platform = IntPtr.Size == 8 ? "amd64" : "i386";
            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WinpkFilter");
            var driverPath = Path.Combine(baseDir, "tools", platform);
            // Download driver if not already there
            if (!File.Exists(driverPath + "/ndisapi.dll"))
            {
                var zipFile = $"WinpkFilter-{arch}.zip";
                using (var client = new WebClient())
                {
                    client.DownloadFile(
                        $"https://www.ntkernel.com/downloads/tools_bin_{arch}.zip",
                        zipFile
                    );
                }
                if (Directory.Exists(baseDir))
                {
                    Directory.Delete(baseDir, true);
                }
                Directory.CreateDirectory(baseDir);
                ZipFile.ExtractToDirectory(zipFile, baseDir);
            }
            // Patch PATH env
            var oldPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            string newPath = oldPath + Path.PathSeparator + driverPath;
            Environment.SetEnvironmentVariable("PATH", newPath);
        }

        [Test]
        public void Properties()
        {
            using var driver = WinpkFilterDriver.Open();

            Assert.That(driver.Version, Is.Not.Zero);

            foreach (var device in driver.GetNetworkDevices())
            {
                Assert.That(device.Name, Is.Not.Null);
                Assert.That(device.FriendlyName, Is.Not.Null);
                Assert.That(device.Description, Is.Null);
                Assert.That(device.LastError, Is.Null);
                Assert.That(device.Filter, Is.Null);
                Assert.That(device.LinkType, Is.EqualTo(LinkLayers.Ethernet));
                Assert.That(device.MacAddress, Is.Not.Null);
                Assert.That(device.TimestampResolution, Is.EqualTo(TimestampResolution.Microsecond));
                Assert.That(device.Statistics, Is.Null);
                if (device.IsValid)
                {
                    // unless explicitly changed by user, this is the default for Ethernet adapters
                    Assert.That(device.Mtu, Is.EqualTo(1500));

                }
            }
        }

        private static void AllowPacket(ILiveDevice device, WinpkFilterHeader header, Packet packet)
        {
            device.SendPacket(packet.Bytes, header);
        }


        private static void BlockPacket(ILiveDevice device, WinpkFilterHeader header, Packet packet)
        {
            var ip = packet.Extract<IPv4Packet>();
            var tcp = packet.Extract<TcpPacket>();
            if (WebHelper.Address.Equals(ip?.DestinationAddress)
                && tcp?.DestinationPort == 80)
            {
                // No packet shall pass
                return;
            }
            AllowPacket(device, header, packet);
        }

        private static void DirtyPacket(ILiveDevice device, WinpkFilterHeader header, Packet packet)
        {
            var ip = packet.Extract<IPv4Packet>();
            var tcp = packet.Extract<TcpPacket>();
            if (WebHelper.Address.Equals(ip?.DestinationAddress)
                && tcp?.DestinationPort == 80)
            {
                // Let's mess with the request a bit
                var data = Encoding.UTF8.GetString(tcp.PayloadData);
                data = data.Replace("HTTP", "HTML");
                tcp.PayloadData = Encoding.UTF8.GetBytes(data);
                ip.UpdateIPChecksum();
                tcp.UpdateTcpChecksum();
            }
            AllowPacket(device, header, packet);
        }

        static readonly FirewallCheck[] FirewallChecks =
        {
            new FirewallCheck("Allow", AllowPacket, WebExceptionStatus.Success),
            new FirewallCheck("Block", BlockPacket, WebExceptionStatus.Timeout),
            new FirewallCheck("Dirty", DirtyPacket, WebExceptionStatus.ProtocolError),
        };

        [Test]
        [TestCaseSource(nameof(FirewallChecks))]
        public void TestFirewall(FirewallCheck check)
        {
            var index = IpHelper.GetBestInterfaceIndex(WebHelper.Address);
            var name = NetworkInterface.GetAllNetworkInterfaces()
                .First(nic => nic.GetIPProperties().GetIPv4Properties().Index == index)
                .Name;
            using var driver = WinpkFilterDriver.Open();
            using var device = driver.GetNetworkDevices().First(d => d.FriendlyName == name);
            Assert.That(device.Started, Is.False);
            device.OnPacketArrival += check.OnPacketArrival;

            device.Open(DeviceModes.Promiscuous);
            device.AdapterMode = AdapterModes.Tunnel;
            device.StartCapture();
            // Wait for the capture thread to start
            Thread.Sleep(1000);
            Assert.That(device.Started, Is.True);
            var status = WebHelper.WebFetch();
            Assert.That(status, Is.EqualTo(check.ExpectedStatus));
        }

        public class FirewallCheck
        {
            private readonly string Name;
            private readonly Action<ILiveDevice, WinpkFilterHeader, Packet> PacketOperation;
            public WebExceptionStatus ExpectedStatus { get; }

            public FirewallCheck(
                string name,
                Action<ILiveDevice, WinpkFilterHeader, Packet> packetOperation,
                WebExceptionStatus expectedStatus
            )
            {
                Name = name;
                PacketOperation = packetOperation;
                ExpectedStatus = expectedStatus;
            }

            public override string ToString()
            {
                return Name;
            }

            internal void OnPacketArrival(object _, PacketCapture e)
            {
                PacketOperation((ILiveDevice)e.Device, (WinpkFilterHeader)e.Header, e.GetPacket().GetPacket());
            }
        }


    }

}
