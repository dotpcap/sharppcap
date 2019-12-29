using NUnit.Framework;
using NUnit.Framework.Constraints;
using PacketDotNet;
using SharpPcap;
using SharpPcap.WinDivert;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;

namespace Test.WinDivert
{
    [TestFixture]
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
            var device = new WinDivertDevice
            {
                Filter = "!loopback and tcp"
            };
            device.Open();
            var capture = device.GetNextPacket();
            device.Close();
            AssertTcp(capture);
        }

        [Test]
        public void TestCapture()
        {
            var device = new WinDivertDevice
            {
                Filter = "!loopback and tcp"
            };
            device.Open();
            var received = new List<RawCapture>();
            device.OnPacketArrival += (s, e) =>
            {
                received.Add(e.Packet);
            };
            device.StartCapture();
            Thread.Sleep(10000);
            device.StopCapture();
            device.Close();
            Assert.That(received, Has.Count.AtLeast(2));
            foreach (var capture in received)
            {
                AssertTcp(capture);
            }
        }

        private void AssertTcp(RawCapture capture)
        {
            Assert.NotNull(capture);
            var packet = Packet.ParsePacket(capture.LinkLayerType, capture.Data);
            Assert.IsInstanceOf<RawIPPacket>(packet);
            Assert.IsInstanceOf<IPPacket>(packet.PayloadPacket);
            Assert.IsInstanceOf<TcpPacket>(packet.PayloadPacket.PayloadPacket);
        }
    }

}
