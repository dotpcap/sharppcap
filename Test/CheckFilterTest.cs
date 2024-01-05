// Copyright 2009-2017 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;

namespace Test
{
    [TestFixture]
    public class CheckFilterTest
    {
        [Test]
        public void TestFilters()
        {
            // test a known failing filter
            Assert.That(LibPcapLiveDevice.CheckFilter("some bogus filter", out string errorString), Is.False);
            Assert.That(errorString, Is.Not.Null);
            Assert.That(errorString, Is.Not.Empty);

            // test a known working filter
            Assert.That(LibPcapLiveDevice.CheckFilter("port 23", out errorString), Is.True);
            Assert.That(errorString, Is.Null);
        }

        /// <summary>
        /// Test BpfProgram.Matches()
        /// </summary>
        [Test]
        public void BpfProgramMatches()
        {
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile("arp_with_vlan.pcap"));
            device.Open();

            var f = "(dst host 192.168.42.1) and (arp or tcp dst port 40499)";
            // Make filter work with or without VLAN
            using var bpfProgram = BpfProgram.Create(LinkLayers.Ethernet, $"({f}) or (vlan and ({f}))");
            Assert.That(bpfProgram.IsInvalid, Is.False);

            device.GetNextPacket(out var packet);
            Assert.That(bpfProgram.Matches(packet.Data), Is.True);
        }
        /// <summary>
        /// Test BpfProgram.Matches() when creating BpfProgram without device
        /// </summary>
        [Test]
        public void BpfProgramNoDeviceMatches()
        {
            using var device = new CaptureFileReaderDevice(TestHelper.GetFile("tcp.pcap"));
            device.Open();

            using var bpfProgram = BpfProgram.Create(device.Handle, "tcp");
            Assert.That(bpfProgram.IsInvalid, Is.False);

            device.GetNextPacket(out var packet);
            Assert.That(bpfProgram.Matches(packet.Data), Is.True);
        }

        /// <summary>
        /// Test BpfProgram.Matches() and other filter related methods
        /// </summary>
        [Test]
        public void FilterMethods()
        {
            using var device = TestHelper.GetPcapDevice();
            device.Open();

            var filterExpression = "arp";
            using var bpfProgram = BpfProgram.Create(device.Handle, filterExpression);
            Assert.That(bpfProgram.IsInvalid, Is.False);

            var arp = new ARP(device);
            var destinationIP = new System.Net.IPAddress(new byte[] { 8, 8, 8, 8 });

            // Note: We don't care about the success or failure here
            arp.Resolve(destinationIP);

            // retrieve some packets, looking for the arp
            var header = IntPtr.Zero;
            var data = IntPtr.Zero;
            var foundBpfMatch = false;
            var packetsToTry = 10;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (packetsToTry > 0)
            {
                if (sw.ElapsedMilliseconds > 2000)
                {
                    break;
                }

                var retval = device.GetNextPacketPointers(ref header, ref data);

                if (retval == 1)
                {
                    packetsToTry--;

                    Assert.That(header, Is.Not.EqualTo(IntPtr.Zero));
                    Assert.That(data, Is.Not.EqualTo(IntPtr.Zero));

                    // and test it against the bpf filter to confirm an exception is not thrown
                    Assert.DoesNotThrow(() =>
                        {
                            // we expect a match as we are sending an arp packet
                            if (bpfProgram.Matches(header, data))
                            {
                                foundBpfMatch = true;
                            }
                        }
                    );
                }
            }

            Assert.That(foundBpfMatch, Is.True);
        }
    }
}
