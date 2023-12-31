// Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
// Copyright 2009 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

using System;
using NUnit.Framework;
using SharpPcap;

namespace Test
{
    [TestFixture]
    public class PcapTest
    {
        // Test that we can retrieve the Pcap version without any errors
        [Test]
        public void Version()
        {
            Console.WriteLine(Pcap.Version);
        }

        [Test]
        public void LibpcapVersionTest()
        {
            var libpcap = "libpcap version 1.10.0";
            var versions = new[] {
                // https://github.com/nmap/nmap/issues/1566
                "libpcap version 1.9.0 (packet.dll version 0.992)",
                // Npcap
                "Npcap version 0.991, based on libpcap version 1.8.1",
                // Libpcap
                libpcap + " (with TPACKET_V3)",
                libpcap + " (SNF-only)",
                libpcap + " (Septel-only)",
                libpcap+ " (DPDK-only)",
                "DOS-" + libpcap,
                // WinPcap (legacy)
                "WinPcap version 4.1.3 (packet.dll version 4.1.0.2980), based on libpcap version 1.0 branch 1_0_rel0b (20091008)",
                // Currently installed
                Pcap.Version
            };
            foreach (var ver in versions)
            {
                var version = Pcap.GetLibpcapVersion(ver);
                Assert.That(version, Is.GreaterThanOrEqualTo(new Version(1, 0)));
            }
            Assert.That(new Version(0, 0), Is.EqualTo(Pcap.GetLibpcapVersion("invalid")));
        }
    }
}
