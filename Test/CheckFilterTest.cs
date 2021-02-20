using System;
using NUnit.Framework;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Test
{
    [TestFixture]
    public class CheckFilterTest
    {
        [Test]
        public void TestFilters()
        {
            // test a known failing filter
            Assert.IsFalse(LibPcapLiveDevice.CheckFilter("some bogus filter", out string errorString));
            Assert.IsNotNull(errorString);
            Assert.IsNotEmpty(errorString);

            // test a known working filter
            Assert.IsTrue(LibPcapLiveDevice.CheckFilter("port 23", out errorString));
            Assert.IsNull(errorString);
        }

        /// <summary>
        /// Test RunBpfProgram() and other filter related methods
        /// </summary>
        [Test]
        public void FilterMethods()
        {
            using var device = TestHelper.GetPcapDevice();
            device.Open();

            var filterExpression = "arp";
            var mask = (uint)0;
            var result = PcapDevice.CompileFilter(device.PcapHandle, filterExpression, mask, out IntPtr bpfProgram, out string errorString);
            Assert.IsTrue(result);

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
                if(sw.ElapsedMilliseconds > 2000)
                {
                    break;
                }

                var retval = device.GetNextPacketPointers(ref header, ref data);

                if (retval == 1)
                {
                    packetsToTry--;

                    Assert.AreNotEqual(IntPtr.Zero, header);
                    Assert.AreNotEqual(IntPtr.Zero, data);

                    // and test it against the bpf filter to confirm an exception is not thrown
                    Assert.DoesNotThrow(() =>
                        {
                            // we expect a match as we are sending an arp packet
                            if (PcapDevice.RunBpfProgram(bpfProgram, header, data))
                            {
                                foundBpfMatch = true;
                            }
                        }
                    );
                }
            }

            Assert.IsTrue(foundBpfMatch);

            PcapDevice.FreeBpfProgram(bpfProgram);
        }
    }
}
