/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Read a pcap capture from a file handle (e.g., a pipe).
    /// </summary>
    public class CaptureHandleReaderDevice : PcapDevice
    {
        /// <value>
        /// File handle the object was created with.
        /// </value>
        public IntPtr FileHandle { get; }

        /// <value>
        /// The name of the capture file.
        /// </value>
        public override string Name => "<file handle>";

        /// <value>
        /// Description of the device.
        /// </value>
        public override string Description => "Capture handle reader device";

        /// <summary>
        /// Creates a new <c>CaptureHandleReaderDevice</c>.
        /// </summary>
        /// <param name="handle">
        /// On Windows, a native Windows handle. On other systems, a pointer to a C runtime <c>FILE</c> object.
        /// </param>
        public CaptureHandleReaderDevice(IntPtr handle)
        {
            FileHandle = handle;
        }

        /// <summary>
        /// Opens the device.
        /// </summary>
        public override void Open(DeviceConfiguration configuration)
        {
            // holds errors
            StringBuilder errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE);

            var resolution = configuration.TimestampResolution ?? TimestampResolution.Microsecond;
            var adapterHandle = LibPcapSafeNativeMethods.pcap_open_handle_offline_with_tstamp_precision(FileHandle, (uint)resolution, errbuf);

            // handle error
            if (adapterHandle.IsInvalid)
            {
                string err = "Unable to open offline adapter: " + errbuf;
                throw new PcapException(err);
            }

            // set the device handle
            Handle = adapterHandle;

            base.Open(configuration);
        }

        /// <summary>
        /// Retrieves pcap statistics.
        ///
        /// Not supported for this device.
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override ICaptureStatistics Statistics => null;

    }
}

