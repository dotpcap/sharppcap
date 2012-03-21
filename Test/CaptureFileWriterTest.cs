using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;

namespace Test
{
    [TestFixture]
    public class CaptureFileWriterTest
    {
        public CaptureFileWriterTest ()
        {
        }

        /// <summary>
        /// Tests that we can create, close and delete
        /// a capture file
        /// </summary>
        [Test]
        public void TestFileCreationAndDeletion()
        {
            var wd = new CaptureFileWriterDevice(@"abc.pcap");
            wd.Write(new byte[] {1, 2, 3, 4});
            wd.Close();
            System.IO.File.Delete(@"abc.pcap");
        }
    }
}

