using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test
{
    [TestFixture]
    public class CaptureFileWriterDeviceTest
    {
        static string filename = @"abc.pcap";

        public CaptureFileWriterDeviceTest()
        {
        }

        /// <summary>
        /// Tests that we can create, close and delete
        /// a capture file
        /// </summary>
        [Test]
        public void TestFileCreationAndDeletion()
        {
            using (var wd = new CaptureFileWriterDevice(filename))
            {
                wd.Open();
                Assert.AreEqual(filename, wd.Name);
                Assert.IsNotEmpty(wd.Description);
                var bytes = new byte[] { 1, 2, 3, 4 };
                wd.Write(bytes);

                var p = new RawCapture(PacketDotNet.LinkLayers.Ethernet, new PosixTimeval(), bytes);
                wd.Write(p);
            }
            System.IO.File.Delete(@"abc.pcap");
        }

        [Test]
        [LibpcapVersion("<1.7.2")]
        public void TestCreationOptions()
        {
            // valid arguments results in the object being created
            using var valid = new CaptureFileWriterDevice("somefilename.pcap", System.IO.FileMode.Open);
            valid.Open(linkLayerType: PacketDotNet.LinkLayers.Ethernet);

            // file mode of append should throw
            Assert.Throws<PlatformNotSupportedException>(() =>
            {
                using var wd = new CaptureFileWriterDevice("somefilename.pcap", System.IO.FileMode.Append);
                wd.Open(linkLayerType: PacketDotNet.LinkLayers.Ethernet);
            });
        }

        [Test]
        [LibpcapVersion(">=1.7.2")]
        public void TestCreationOption2()
        {
            // valid arguments results in the object being created
            using (var valid = new CaptureFileWriterDevice("somefilename.pcap", System.IO.FileMode.Open))
            {
                valid.Open(linkLayerType: PacketDotNet.LinkLayers.Ethernet);
            }

            using (var validAppend = new CaptureFileWriterDevice("somefilename.pcap", System.IO.FileMode.Append))
            {
                validAppend.Open(linkLayerType: PacketDotNet.LinkLayers.Ethernet);
            }
        }

        /// <summary>
        /// Test opening the writer device using another interface's linklayer type
        /// </summary>
        [Test]
        public void TestOpenFromInterface()
        {
            using var device = TestHelper.GetPcapDevice();
            device.Open();

            // valid arguments results in the object being created
            using var valid = new CaptureFileWriterDevice("somefilename.pcap", System.IO.FileMode.Open);
            valid.Open(device);
        }

        [Category("Timestamp")]
        [Test]
        public void TestTimestampCreation()
        {
            // setting timestamp resolution
            using var wd = new CaptureFileWriterDevice("simefilename.pcap", System.IO.FileMode.Open);
            var configuration = new DeviceConfiguration
            {
                TimestampResolution = TimestampResolution.Nanosecond
            };
            wd.Open(configuration);
        }

        [Test]
        public void StatisticsUnsupported()
        {
            using (var wd = new CaptureFileWriterDevice(filename))
            {
                wd.Open();
                Assert.IsNull(wd.Statistics);
            }
        }

        [Test]
        public void TestInjectable()
        {
            using (var wd = new CaptureFileWriterDevice(filename))
            {
                wd.Open();
                Assert.AreEqual(filename, wd.Name);
                Assert.IsNotEmpty(wd.Description);

                var bytes = new byte[] { 1, 2, 3, 4 };

                var injectionDevice = wd as IInjectionDevice;

                var p = new RawCapture(PacketDotNet.LinkLayers.Ethernet, new PosixTimeval(), bytes);
                injectionDevice.SendPacket(p);

                var span = new ReadOnlySpan<byte>(bytes, 0, bytes.Length);
                injectionDevice.SendPacket(span);
            }
            System.IO.File.Delete(@"abc.pcap");
        }

    }
}

