using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.WinDivert;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Test.WinDivert
{
    [TestFixture]
    [Category("WinDivert")]
    [Platform("Win", Reason = "WinDivert driver is only available for Windows")]
    public class WinDivertDeviceTest
    {

        /// <summary>
        /// Modify PATH var to include our WinDivert DLL's so that the LoadLibrary function will
        /// find whatever WinDivert dll required for the current architecture.
        /// </summary>
        [OneTimeSetUp]
        public void InstallWinDivert()
        {
            var version = "2.2.0";
            var arch = IntPtr.Size == 8 ? "x64" : "x86";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var driverPath = Path.Combine(baseDir, $"WinDivert-{version}-A\\" + arch);
            // Download driver if not already there
            if (!File.Exists(driverPath + "/WinDivert.dll"))
            {
                var zipFile = Path.Combine(baseDir, "windivert.zip");
                using (var client = new WebClient())
                {
                    client.DownloadFile(
                        $"https://github.com/basil00/Divert/releases/download/v{version}/WinDivert-{version}-A.zip",
                        zipFile
                    );
                }
                ZipFile.ExtractToDirectory(zipFile, baseDir);
            }
            // Patch PATH env
            var oldPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            string newPath = oldPath + Path.PathSeparator + driverPath;
            Environment.SetEnvironmentVariable("PATH", newPath);
        }


        [Test]
        public void TestBadFilter()
        {
            var device = new WinDivertDevice();
            var badFilter = "tcp.DstPort == 443 and foo";
            Assert.Throws<PcapException>(() => device.Filter = badFilter);
        }

        [Test]
        public void TestGetNextPacket()
        {
            using var device = new WinDivertDevice
            {
                Filter = "!loopback and tcp"
            };
            PacketCapture e;
            device.Open();
            device.GetNextPacket(out e);
            AssertTcp(e.GetPacket());
        }

        [Test]
        public void TestCapture()
        {
            using var device = new WinDivertDevice
            {
                Filter = "!loopback and tcp"
            };
            device.Open();
            var received = new List<RawCapture>();
            device.OnPacketArrival += (s, e) =>
            {
                received.Add(e.GetPacket());
            };
            device.StartCapture();
            WebHelper.WebFetch();
            Thread.Sleep(250);
            device.StopCapture();
            Assert.That(received, Has.Count.AtLeast(2));
            foreach (var capture in received)
            {
                AssertTcp(capture);
            }
        }

        [Test]
        public void SetGetParam()
        {
            using var device = new WinDivertDevice();
            device.Open();

            var originalValue = device.GetParam(WinDivertParam.QueueLen);
            var targetValue = originalValue + 1000;
            device.SetParam(WinDivertParam.QueueLen, targetValue);
            var actualValue = device.GetParam(WinDivertParam.QueueLen);
            Assert.AreEqual(targetValue, actualValue);
        }

        [Test]
        public void Properties()
        {
            using var device = new WinDivertDevice();
            Assert.AreEqual("WinDivert", device.Name);
            Assert.AreEqual("WinDivert Packet Driver", device.Description);
            Assert.AreEqual(LinkLayers.Raw, device.LinkType);
            Assert.AreEqual(null, device.MacAddress);
            Assert.AreEqual(SharpPcap.TimestampResolution.Microsecond, device.TimestampResolution);
            Assert.IsNull(device.Statistics);
        }

        private void AssertTcp(RawCapture capture)
        {
            Assert.NotNull(capture);
            var packet = Packet.ParsePacket(capture.LinkLayerType, capture.Data);
            Assert.IsInstanceOf<RawIPPacket>(packet);
            Assert.IsInstanceOf<IPPacket>(packet.PayloadPacket);
            Assert.IsInstanceOf<TcpPacket>(packet.PayloadPacket.PayloadPacket);
        }

        [Test]
        public void TestSend()
        {
            var dst = IPAddress.Parse("8.8.8.8");
            var nic = IpHelper.GetBestInterface(dst);
            Assert.NotNull(nic, "No internet connected interface found");
            var src = nic.GetIPProperties().UnicastAddresses
                .Select(addr => addr.Address)
                .FirstOrDefault(addr => addr.AddressFamily == AddressFamily.InterNetwork);
            var ifIndex = nic.GetIPProperties().GetIPv4Properties().Index;
            Console.WriteLine($"Using NIC {nic.Name} [{ifIndex}]");
            Console.WriteLine($"Sending from {src} to {dst}");
            using var device = new WinDivertDevice();
            device.Open();
            var udp = new UdpPacket(5000, 5000);
            udp.PayloadData = new byte[100];
            var ip = IPv4Packet.RandomPacket();
            ip.PayloadPacket = udp;

            ip.SourceAddress = src;
            ip.DestinationAddress = dst;

            device.SendPacket(ip);
        }

    }

}
