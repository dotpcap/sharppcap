using System;
using NUnit.Framework;
using SharpPcap.Packets;

namespace Test
{
    [TestFixture]
    public class IPPacketTest
    {
        /// <summary>
        /// Test creating an IPPacket from values
        /// </summary>
        [Test]
        public void IPPacketConstructor()
        {
            byte[] srcHwAddressBytes = new byte[EthernetFields_Fields.MAC_ADDRESS_LENGTH];
            for(int i = 0; i < srcHwAddressBytes.Length; i++)
            {
                srcHwAddressBytes[i] = (byte)i;
            }

            byte[] dstHwAddressBytes = new byte[EthernetFields_Fields.MAC_ADDRESS_LENGTH];
            for(int i = 0; i < dstHwAddressBytes.Length; i++)
            {
                dstHwAddressBytes[i] = (byte)(dstHwAddressBytes.Length - i);
            }

            var srcHwAddress = new System.Net.NetworkInformation.PhysicalAddress(srcHwAddressBytes);
            var dstHwAddress = new System.Net.NetworkInformation.PhysicalAddress(dstHwAddressBytes);

            var srcIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            var dstIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
            var ipPayload = new byte[4];
            ipPayload[0] = 10;
            ipPayload[1] = 5;
            ipPayload[2] = 2;
            ipPayload[3] = 12;
            var ethernetPacket = new EthernetPacket(srcHwAddress, dstHwAddress, EthernetPacketType.None, null);
            var ipPacket = new IPPacket(IPPacket.IPVersions.IPv4,
                                        IPProtocol.IPProtocolType.NONE,
                                              srcIpAddress,
                                              dstIpAddress,
                                              ethernetPacket,
                                              ipPayload);

            Console.WriteLine("build a packet of: {0}", ipPacket.ToString());

            // compare the generated packet with the values we passed in
            Assert.AreEqual(ethernetPacket.DestinationHwAddress, ipPacket.DestinationHwAddress);
            Assert.AreEqual(ethernetPacket.SourceHwAddress, ipPacket.SourceHwAddress);
            Assert.AreEqual(srcIpAddress, ipPacket.SourceAddress);
            Assert.AreEqual(dstIpAddress, ipPacket.DestinationAddress);
            for(int i = 0; i < ipPacket.IPData.Length; i++)
            {
                Console.WriteLine(ipPacket.IPData[i]);
            }
            Assert.AreEqual(ipPayload, ipPacket.IPData);
        }
    }
}
