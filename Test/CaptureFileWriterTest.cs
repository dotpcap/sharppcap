using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test
{
    [TestFixture]
    public class CaptureFileWriterTest
    {
        static string filename = @"abc.pcap";

        public CaptureFileWriterTest()
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
        public void TestCreationOptions()
        {
            // valid arguments results in the object being created
            using var valid = new CaptureFileWriterDevice(PacketDotNet.LinkLayers.Ethernet, null, "somefilename.pcap", System.IO.FileMode.Open);

            // invalid snapshot length should throw
            Assert.Throws<InvalidOperationException>(() =>
            {
                using var wd = new CaptureFileWriterDevice(PacketDotNet.LinkLayers.Ethernet, 500000, "somefilename.pcap", System.IO.FileMode.Open);
            });

            // file mode of append should throw
            Assert.Throws<InvalidOperationException>(() =>
            {
                using var wd = new CaptureFileWriterDevice(PacketDotNet.LinkLayers.Ethernet, null, "somefilename.pcap", System.IO.FileMode.Append);
            });
        }

        [Test]
        public void SendPacket()
        {
            var bytes = new byte[] { 0x10, 0x20, 0x30 };

            using (var wd = new CaptureFileWriterDevice(filename))
            {
                Assert.Throws<NotSupportedOnCaptureFileException>(() => wd.SendPacket(bytes));
            }
        }

        [Test]
        public void Statistics()
        {
            using (var wd = new CaptureFileWriterDevice(filename))
            {
                Assert.Throws<NotSupportedOnCaptureFileException>(() => { var s = wd.Statistics; });
            }
        }
    }
}

