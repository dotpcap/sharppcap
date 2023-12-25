using NUnit.Framework;
using PacketDotNet;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Test.Tunneling
{
    class UdpTester : IDisposable
    {
        private readonly IPAddress LocalIp;
        private readonly UdpClient Client;

        internal static readonly ushort Port = 4422;
        private static readonly PhysicalAddress BroadcastMac = PhysicalAddress.Parse("FFFFFFFFFFFF");

        public byte[] LastReceivedData { get; private set; }

        public UdpTester(IPAddress localIp)
        {
            LocalIp = localIp;
            Client = new UdpClient(new IPEndPoint(localIp, Port));
            Client.EnableBroadcast = true;
            Task.Run(ReceiveLoop);
        }

        public void Dispose()
        {
            Client.Dispose();
        }

        public void Broadcast(byte[] data)
        {
            var remote = new IPEndPoint(IPAddress.Broadcast, Port);
            Client.Send(data, data.Length, remote);
        }

        /// <summary>
        /// Check that given packet matches what we would send back
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void AssertMatches(Packet packet, byte[] data)
        {
            Assert.That(packet, Is.Not.Null);
            var ip = packet.Extract<IPv4Packet>();
            var udp = packet.Extract<UdpPacket>();

            Assert.That(ip, Is.Not.Null);
            Assert.That(udp, Is.Not.Null);

            Assert.That(Port, Is.EqualTo(udp.SourcePort));
            Assert.That(Port, Is.EqualTo(udp.DestinationPort));
            Assert.That(ip.SourceAddress, Is.EqualTo(LocalIp));

            Assert.That(udp.PayloadData, Is.EqualTo(data).AsCollection);
        }

        /// <summary>
        /// Create packet that would be replied to by this listener if correctly injected
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Packet GetReceivablePacket(byte[] data)
        {
            var ipBytes = LocalIp.GetAddressBytes();
            ipBytes[3]++;
            var fakeIp = new IPAddress(ipBytes);
            var fakeMac = PhysicalAddress.Parse("001122334455");
            var eth = new EthernetPacket(fakeMac, BroadcastMac, EthernetType.IPv6);
            var ip = new IPv4Packet(fakeIp, LocalIp);
            var udp = new UdpPacket(Port, Port);

            eth.PayloadPacket = ip;
            ip.PayloadPacket = udp;
            udp.PayloadData = data;

            udp.UpdateCalculatedValues();
            ip.UpdateCalculatedValues();

            udp.UpdateUdpChecksum();
            ip.UpdateIPChecksum();

            return eth;
        }

        private void ReceiveLoop()
        {
            try
            {
                while (true)
                {
                    var remote = new IPEndPoint(IPAddress.Any, 0);
                    var receiveBytes = Client.Receive(ref remote);
                    if (!LocalIp.Equals(remote.Address))
                    {
                        LastReceivedData = receiveBytes;
                    }
                }
            }
            catch
            {
                // end of connection
            }
        }
    }

}
