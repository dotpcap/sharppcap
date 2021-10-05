using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinTap;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace Test.WinTap
{
    [TestFixture]
    [Category("WinTap")]
    [Platform("Win", Reason = "WinpkFilter driver is only available for Windows")]
    public class WinTapDeviceTest
    {

        [Test]
        public void Properties()
        {
            var nic = WinTapDevice.GetTapInterfaces().First();
            using var device = new WinTapDevice(nic);

            Assert.GreaterOrEqual(device.Version.Major, 9);
            Assert.IsNotNull(device.Name);
            Assert.IsNotNull(device.FriendlyName);
            Assert.IsNull(device.Description);
            Assert.IsNull(device.LastError);
            Assert.IsNull(device.Filter);
            Assert.AreEqual(LinkLayers.Ethernet, device.LinkType);
            Assert.IsNotNull(device.MacAddress);
            Assert.AreEqual(TimestampResolution.Microsecond, device.TimestampResolution);
            Assert.IsNull(device.Statistics);
        }

        [Test]
        public void TestReceive()
        {
            var nic = WinTapDevice.GetTapInterfaces().First();
            var tapIp = GetIPAddress(nic);

            // we need to provide our own IP and MAC, otherwise OS will ignore its own requests
            var ipBytes = tapIp.GetAddressBytes();
            ipBytes[3]++;
            var testIp = new IPAddress(ipBytes);
            var testMac = PhysicalAddress.Parse("001122334455");


            using var tapDevice = new WinTapDevice(nic);
            // Open TAP device first to ensure the virutal device is connected
            tapDevice.Open();
            using var pcapDevice = GetLibPcapDevice(nic);

            tapDevice.Filter = "arp dst host " + tapIp;
            var arp = new ARP(pcapDevice);

            var mac = arp.Resolve(tapIp, testIp, testMac);
            Assert.AreEqual(mac, tapDevice.MacAddress);

            var retval = tapDevice.GetNextPacket(out var p);
            Assert.AreEqual(GetPacketStatus.PacketRead, retval);
            var arpPacket = Packet.ParsePacket(tapDevice.LinkType, p.Data.ToArray())
                .Extract<ArpPacket>();
            Assert.NotNull(arpPacket);
        }

        /// <summary>
        /// Inject packets with one driver, and check it being received by the other
        /// </summary>
        [Test]
        public void TestPcapTapExchange()
        {
            var nic = WinTapDevice.GetTapInterfaces().First();
            using var tapDevice = new WinTapDevice(nic);
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

        private static IPAddress GetIPAddress(NetworkInterface nic)
        {
            foreach (var ip in nic.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.Address;
                }
            }
            return null;
        }


    }

}
