using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Unix.Native;
using Microsoft.Win32.SafeHandles;

namespace Test
{
    [TestFixture]
    [LibpcapVersion(">=1.5.0")]
    public class CaptureHandleReaderDeviceTest
    {
        private static int capturedPackets;

        private SafeHandle GetTestFileHandle(string filename)
        {
            filename = TestHelper.GetFile(filename);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // On Windows, we need an actual OS handle.
                // The FileStream is not disposed, which is alright because we take care of its handle.
                // Once .NET Framework is no longer supported, this can be simplified to File.OpenHandle().
                return File.Open(filename, FileMode.Open).SafeFileHandle;
            }
            // On other platforms, libpcap is not very interop-friendly and expects a FILE*.
            return new StdlibFileHandle(Stdlib.fopen(filename, "rb"), true);
        }

        [Category("Timestamp")]
        [TestCase(TimestampResolution.Nanosecond, "1186341404.189852000s")]
        [TestCase(TimestampResolution.Microsecond, "1186341404.189852s")]
        public void CaptureTimestampResolution(TimestampResolution resolution, string timeval)
        {
            const string filename = "ipv6_http.pcap";
            using var handle = GetTestFileHandle(filename);
            using var device = new CaptureHandleReaderDevice(handle);
            var configuration = new DeviceConfiguration
            {
                TimestampResolution = resolution
            };
            device.Open(configuration);
            Assert.AreEqual(resolution, device.TimestampResolution);
            device.GetNextPacket(out var packet);
            Assert.AreEqual(timeval, packet.Header.Timeval.ToString());
        }

        [Test]
        public void FileHandleInvalidUponClose()
        {
            const string filename = "ipv6_http.pcap";
            using var handle = GetTestFileHandle(filename);
            Assert.IsFalse(handle.IsInvalid);
            Assert.IsFalse(handle.IsClosed);
            {
                using var device = new CaptureHandleReaderDevice(handle);
                device.Open();
            }
            Assert.IsTrue(handle.IsClosed);
        }

        [Test]
        public void CaptureProperties()
        {
            const string filename = "ipv6_http.pcap";
            using var handle = GetTestFileHandle(filename);
            using var device = new CaptureHandleReaderDevice(handle);
            device.Open();
            Assert.IsNotEmpty(device.Description);
            Assert.AreEqual(handle, device.FileHandle);
        }

        /// <summary>
        /// Test that we can retrieve packets from a pcap file just as we would from
        /// a live capture device and that all packets are captured
        /// </summary>
        [Test]
        public void CaptureInfinite()
        {
            const string filename = "ipv6_http.pcap";
            using var handle = GetTestFileHandle(filename);
            using var device = new CaptureHandleReaderDevice(handle);
            device.OnPacketArrival += HandleDeviceOnPacketArrival;
            device.Open();

            var expectedPackets = 10;
            capturedPackets = 0;
            device.Capture();

            Assert.AreEqual(expectedPackets, capturedPackets);
        }

        /// <summary>
        /// Test that if we ask to capture a finite number of packets that
        /// only this number of packets will be captured
        /// </summary>
        [Test]
        public void CaptureFinite()
        {
            const string filename = "ipv6_http.pcap";
            using var handle = GetTestFileHandle(filename);
            using var device = new CaptureHandleReaderDevice(handle);
            device.OnPacketArrival += HandleDeviceOnPacketArrival;
            device.Open();

            var expectedPackets = 3;
            capturedPackets = 0;
            device.Capture(expectedPackets);

            Assert.AreEqual(expectedPackets, capturedPackets);
        }

        void HandleDeviceOnPacketArrival(object sender, PacketCapture e)
        {
            Console.WriteLine("got packet " + e.GetPacket().ToString());
            capturedPackets++;
        }

        /// <summary>
        /// Test that we get expected unsupport indication when attempting to retrieve
        /// Statistics from this device
        /// </summary>
        [Test]
        public void StatisticsUnsupported()
        {
            const string filename = "ipv6_http.pcap";
            using var handle = GetTestFileHandle(filename);
            using var device = new CaptureHandleReaderDevice(handle);
            device.Open();
            Assert.IsNull(device.Statistics);
        }

        [Test]
        public void SetFilter()
        {
            const string filename = "test_stream.pcap";
            using var handle = GetTestFileHandle(filename);
            using var device = new CaptureHandleReaderDevice(handle);

            device.Open();
            device.Filter = "port 53";

            RawCapture rawPacket;
            int count = 0;
            PacketCapture e;
            GetPacketStatus retval;
            do
            {
                retval = device.GetNextPacket(out e);
                if (retval == GetPacketStatus.PacketRead)
                {
                    rawPacket = e.GetPacket();
                    Packet p = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);
                    var udpPacket = p.Extract<UdpPacket>();
                    Assert.IsNotNull(udpPacket);
                    int dnsPort = 53;
                    Assert.AreEqual(dnsPort, udpPacket.DestinationPort);
                    count++;
                }
            } while (retval == GetPacketStatus.PacketRead);

            Assert.AreEqual(1, count);
        }
    }


    internal class StdlibFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public StdlibFileHandle(IntPtr preexistingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(preexistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return Stdlib.fclose(handle) == 0;
        }

    }

}
