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
    public class CaptureFileReaderDevice : PcapDevice
    {
        private string m_pcapFile;

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
                return new FileInfo( Name ).Length;
            }
        }

        /// <summary>
        /// The underlying pcap file name
        /// </summary>
        public string FileName
        {
            get { return System.IO.Path.GetFileName( this.Name ); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="captureFilename">
        /// A <see cref="System.String"/>
        /// </param>
        public CaptureFileReaderDevice (string captureFilename)
        {
            m_pcapFile = captureFilename;

            // holds errors
            StringBuilder errbuf = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors
            // opens offline pcap file
            IntPtr adapterHandle = LibPcapSafeNativeMethods.pcap_open_offline( captureFilename, errbuf);

            // handle error
            if ( adapterHandle == IntPtr.Zero)
            {
                string err = "Unable to open offline adapter: " + errbuf.ToString();
                throw new PcapException( err );
            }

            // set the device handle
            PcapHandle = adapterHandle;
        }

        /// <summary>
        /// Open the device
        /// </summary>
        public override void Open()
        {
            // Nothing to do here, device is already opened upon construction
        }

        /// <summary>
        /// Retrieves pcap statistics
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override ICaptureStatistics Statistics
        {
            get
            {
                throw new NotSupportedOnCaptureFileException("Statistics not supported on a capture file");
            }
        }
    }
}

