// Copyright 2020 Ayoub Kaanich <kayoub5@live.com>
// Copyright 2020-2021 Chris Morgan <chmorgan@gmail.com>
// SPDX-License-Identifier: MIT

using NUnit.Framework;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Linq;
using static Test.TestHelper;
using static System.TimeSpan;

namespace Test
{
    [TestFixture]
    [NonParallelizable]
    [Category("SendPacket")]
    public class SendPacketTest
    {
        private const string Filter = "ether proto 0x1234";

        [Test]
        public void TestSendPacketTest()
        {
            var packet = EthernetPacket.RandomPacket();
            packet.Type = (EthernetType)0x1234;
            var received = RunCapture(Filter, (device) =>
            {
                // test all forms of SendPacket()
                device.SendPacket(packet);
                var rawCapture = new RawCapture(PacketDotNet.LinkLayers.Ethernet, new PosixTimeval(), packet.Bytes);
                device.SendPacket(rawCapture);
                device.SendPacket(packet, packet.TotalPacketLength);
                device.SendPacket(packet.Bytes, packet.Bytes.Length);
            });
            Assert.That(received, Has.Count.EqualTo(4));
            Assert.That(received[0].Data, Is.EquivalentTo(packet.Bytes));
        }

        [SetUp]
        public void SetUp()
        {
            TestHelper.ConfirmIdleState();
        }

        [TearDown]
        public void Cleanup()
        {
            TestHelper.ConfirmIdleState();
        }
    }
}
