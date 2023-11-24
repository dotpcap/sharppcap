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
/*
 * Copyright 2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Text;
using System.IO;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Read a pcap capture file
    /// </summary>
    public class CaptureFileReaderDevice : CaptureReaderDevice
    {
        private readonly string m_pcapFile;

        /// <value>
        /// The name of the capture file
        /// </value>
        public override string Name
        {
            get
            {
                return m_pcapFile;
            }
        }

        /// <value>
        /// Description of the device
        /// </value>
        public override string Description
        {
            get
            {
                return "Capture file reader device";
            }
        }

        /// <value>
        /// Number of bytes in the capture file
        /// </value>
        public long FileSize
        {
            get
            {
                return new FileInfo(Name).Length;
            }
        }

        /// <summary>
        /// The underlying pcap file name
        /// </summary>
        public string FileName
        {
            get { return System.IO.Path.GetFileName(this.Name); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="captureFilename">
        /// A <see cref="string"/>
        /// </param>
        public CaptureFileReaderDevice(string captureFilename)
        {
            m_pcapFile = captureFilename;
        }

        /// <summary>
        /// Open the device
        /// </summary>
        public override void Open(DeviceConfiguration configuration)
        {
            // holds errors
            StringBuilder errbuf = new StringBuilder(Pcap.PCAP_ERRBUF_SIZE); //will hold errors

            PcapHandle adapterHandle;

            // Check if we need to open with a defined precision
            var has_offline_with_tstamp_precision_support = Pcap.LibpcapVersion >= new Version(1, 5, 1);
            var resolution = configuration.TimestampResolution ?? TimestampResolution.Microsecond;
            if (has_offline_with_tstamp_precision_support)
            {
                adapterHandle = LibPcapSafeNativeMethods.pcap_open_offline_with_tstamp_precision(m_pcapFile, (uint)resolution, errbuf);
            }
            else
            {
                // notify the user that they asked for a non-standard resolution but their libpcap
                // version lacks the necessary function
                if (resolution != TimestampResolution.Microsecond)
                {
                    configuration.RaiseConfigurationFailed(
                        nameof(configuration.TimestampResolution),
                        PcapError.PlatformNotSupported,
                        "pcap version is < 1.5.1, needs pcap_open_offline_with_tstamp_precision()"
                    );
                }

                adapterHandle = LibPcapSafeNativeMethods.pcap_open_offline(m_pcapFile, errbuf);
            }

            // handle error
            if (adapterHandle.IsInvalid)
            {
                string err = "Unable to open offline adapter: " + errbuf.ToString();
                throw new PcapException(err);
            }

            // set the device handle
            Handle = adapterHandle;

            base.Open(configuration);
        }
    }
}

