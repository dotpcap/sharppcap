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
using System.Threading;

namespace Test.Tunneling
{
    [TestFixture]
    [Category("Tunneling")]
    [Platform(Exclude = "MacOSX", Reason = "Not supported yet")]
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

        [Test]
        public void TestArpTunnel()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            var tapIp = IpHelper.EnsureIPv4Address(nic);

            // we need to provide our own IP and MAC, otherwise OS will ignore its own requests
            var ipBytes = tapIp.GetAddressBytes();
            ipBytes[3]++;
            var testIp = new IPAddress(ipBytes);
            var testMac = PhysicalAddress.Parse("001122334455");

            using var tapDevice = new TunnelDevice(nic);
            // Open TAP device first to ensure the virutal device is connected
            tapDevice.Open(DeviceModes.Promiscuous);
            PhysicalAddress mac = null;
            for (int i = 0; i < 5; i++)
            {
                mac = ARP.Resolve(tapDevice, tapIp, testIp, testMac, TimeSpan.FromSeconds(1));
                if (mac != null)
                {
                    break;
                }
                // Wait for interface to finish Gratuitous ARP
            }


            Assert.AreEqual(tapDevice.MacAddress, mac);
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

            tapDevice.Filter = "udp port " + UdpTester.Port;

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
        /// Inject packets with TAP, and check them being received by Libpcap
        /// </summary>
        [Test]
        public void TestPcapTapExchange()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            using var tapDevice = new TunnelDevice(nic);
            // Open TAP device first to ensure the virutal device is connected
            tapDevice.Open();
            // Wait for interface to be fully up
            Thread.Sleep(1000);
            var pcapInterface = PcapInterface.GetAllPcapInterfaces()
                .First(pIf => pIf.FriendlyName == nic.Name);
            using var pcapDevice = new LibPcapLiveDevice(pcapInterface);
            PcapDeviceTest.CheckExchange(tapDevice, pcapDevice);
        }


    }

}
