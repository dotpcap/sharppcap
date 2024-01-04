// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Tunneling;
using System;
using System.IO;
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
                Assert.That(device.Version.Major, Is.GreaterThanOrEqualTo(9));
            }
            Assert.That(device.Name, Is.Not.Null);
            Assert.That(device.FriendlyName, Is.Not.Null);
            Assert.That(device.Description, Is.Not.Null);
            Assert.That(device.LastError, Is.Null);
            Assert.That(device.Filter, Is.Null);
            Assert.That(device.LinkType, Is.EqualTo(LinkLayers.Ethernet));
            Assert.That(device.MacAddress, Is.Not.Null);
            Assert.That(device.TimestampResolution, Is.EqualTo(TimestampResolution.Microsecond));
            Assert.That(device.Statistics, Is.Null);
        }

        [Test]
        public void TestArpTunnel()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            using var tapDevice = GetTunnelDevice(nic);
            // Open TAP device first to ensure the virutal device is connected
            tapDevice.Open(DeviceModes.Promiscuous);
            var tapIp = IpHelper.GetIPAddress(nic);

            // we need to provide our own IP and MAC, otherwise OS will ignore its own requests
            var ipBytes = tapIp.GetAddressBytes();
            ipBytes[3]++;
            var testIp = new IPAddress(ipBytes);
            var testMac = PhysicalAddress.Parse("001122334455");

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


            Assert.That(mac, Is.EqualTo(tapDevice.MacAddress));
        }

        /// <summary>
        /// Test injection of packets from and to OS
        /// </summary>
        [Test]
        [Retry(3)]
        public void TestUdpTunnel()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            using var tapDevice = GetTunnelDevice(nic);
            // Open TAP device first to ensure the virutal device is connected
            try
            {
                tapDevice.Open(DeviceModes.Promiscuous);
            }
            catch (SystemException ex)
            {
                Assert.Fail(ex.Message);
            }
            var tapIp = IpHelper.GetIPAddress(nic);

            using var tester = new UdpTester(tapIp);

            tapDevice.Filter = "udp port " + UdpTester.Port;

            // Send from OS, and receive in tunnel
            var seq1 = new byte[] { 1, 2, 3 };
            tester.Broadcast(seq1);
            var retval = tapDevice.GetNextPacket(out var p1);
            Assert.That(retval, Is.EqualTo(GetPacketStatus.PacketRead));
            tester.AssertMatches(p1.GetPacket().GetPacket(), seq1);

            // Send from tunnel, and receive in OS
            var seq2 = new byte[] { 4, 5, 6 };
            var packet = tester.GetReceivablePacket(seq2);
            tapDevice.SendPacket(packet);
            retval = tapDevice.GetNextPacket(out var p2);
            // TAP don't receive its own traffic
            Assert.That(retval, Is.EqualTo(GetPacketStatus.ReadTimeout));
            Assert.That(tester.LastReceivedData, Is.EqualTo(seq2).AsCollection);
        }

        /// <summary>
        /// Inject packets with TAP, and check them being received by Libpcap
        /// </summary>
        [Test]
        public void TestPcapTapExchange()
        {
            var nic = TunnelDevice.GetTunnelInterfaces().First();
            using var tapDevice = GetTunnelDevice(nic);
            // Open TAP device first to ensure the virutal device is connected
            tapDevice.Open();
            // Wait for interface to be fully up
            Thread.Sleep(1000);
            var pcapInterface = PcapInterface.GetAllPcapInterfaces()
                .First(pIf => pIf.FriendlyName == nic.Name);
            using var pcapDevice = new LibPcapLiveDevice(pcapInterface);
            PcapDeviceTest.CheckExchange(tapDevice, pcapDevice);
        }

        private static TunnelDevice GetTunnelDevice(NetworkInterface nic)
        {
            var config = new IPAddressConfiguration
            {
                // Pick a range that no CI is likely to use
                Address = IPAddress.Parse("10.225.255.100"),
                IPv4Mask = IPAddress.Parse("255.255.255.0"),
            };
            return new TunnelDevice(nic, config);
        }


    }

}
