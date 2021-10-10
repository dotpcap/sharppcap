using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Tunneling;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace Test.WinTap
{
    [TestFixture]
    [Category("Tunneling")]
    [Platform(Exclude = "MacOSX", Reason = "Not tested yet")]
    public class TunnelDeviceTest
    {

        [Test]
        public void Properties()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            using var device = new TunnelDevice(nic);
            device.Open();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.GreaterOrEqual(device.Version.Major, 9);
            }
            Assert.IsNotNull(device.Name);
            Assert.IsNotNull(device.FriendlyName);
            Assert.IsNotNull(device.Description);
            Assert.IsNull(device.LastError);
            Assert.IsNull(device.Filter);
            Assert.AreEqual(LinkLayers.Ethernet, device.LinkType);
            Assert.IsNotNull(device.MacAddress);
            Assert.AreEqual(TimestampResolution.Microsecond, device.TimestampResolution);
            Assert.IsNull(device.Statistics);
        }

        /// <summary>
        /// Test injection of packets from and to OS
        /// </summary>
        [Test]
        public void TestUdpTunnel()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            using var tapDevice = new TunnelDevice(nic);
            // Open TAP device first to ensure the virtual device is connected
            tapDevice.Open();
            var tapIp = IpHelper.EnsureIPv4Address(nic);
            using var tester = new UdpTester(tapIp);


            tapDevice.Filter = "udp port 4444";

            // Send from OS, and receive in tunnel
            var seq1 = new byte[] { 1, 2, 3 };
            tester.Broadcast(seq1);
            var retval = tapDevice.GetNextPacket(out var p1);
            Assert.AreEqual(GetPacketStatus.PacketRead, retval);
            tester.AssertMatches(p1.GetPacket().GetPacket(), seq1);

            // Send from tunnel, and receive in OS
            var seq2 = new byte[] { 4, 5, 6 };
            var packet = tester.GetReceivablePacket(seq2);
            tapDevice.SendPacket(packet);
            retval = tapDevice.GetNextPacket(out var p2);
            // TAP don't receive its own traffic
            Assert.AreEqual(GetPacketStatus.ReadTimeout, retval);
            CollectionAssert.AreEqual(seq2, tester.LastReceivedData);
        }

        /// <summary>
        /// Inject packets with one driver, and check it being received by the other
        /// </summary>
        [Test]
        public void TestPcapTapExchange()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            using var tapDevice = new TunnelDevice(nic);
            // Open TAP device first to ensure the virutal device is connected
            tapDevice.Open();
            using var pcapDevice = GetLibPcapDevice(nic);
            PcapDeviceTest.CheckExchange(tapDevice, pcapDevice);
        }

        private static LibPcapLiveDevice GetLibPcapDevice(NetworkInterface nic)
        {
            var pcapInterface = new PcapInterface(new pcap_if
            {
                Name = @"\Device\NPF_" + nic.Id,
            }, nic, null);
            return new LibPcapLiveDevice(pcapInterface);
        }


    }

}
