using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.IO;
using System.Runtime.InteropServices;

namespace Test
{
    [TestFixture]
    public class CaptureHandleReaderDeviceTest
    {
        private static int capturedPackets;

        private SafeHandle GetTestFileHandle(string filename)
        {
            filename = TestHelper.GetFile(filename);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // On Windows, we need an actual OS handle.
                return File.OpenHandle(filename);
            }

            // On other platforms, libpcap is not very interop-friendly and expects a FILE*.
#if !WINDOWS
            return SafeCFileHandle.Wrap(Mono.Unix.Native.Stdlib.fopen(filename, "rb"));
#else
            return null;
#endif
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

#if !WINDOWS
    internal class SafeCFileHandle : SafeHandle
    {
        private SafeCFileHandle(bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
        }

        internal static SafeCFileHandle Wrap(IntPtr handleToWrap)
        {
            var result = new SafeCFileHandle(true);
            result.SetHandle(handleToWrap);
            return result;
        }

        protected override bool ReleaseHandle()
        {
            return Mono.Unix.Native.Stdlib.fclose(handle) == 0;
        }

        public override bool IsInvalid => handle == IntPtr.Zero;
    }
#endif
}
