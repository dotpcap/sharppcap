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
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.IO;
using System.Text;

namespace SharpPcap
{
    /// <summary>
    /// Capture packets from an offline pcap file
    /// </summary>
    public class PcapOfflineDevice : PcapDevice
    {
        private string m_pcapFile;

        /// <summary>
        /// The description of this device
        /// </summary>
        private const string PCAP_OFFLINE_DESCRIPTION 
            = "Offline pcap file";

        /// <summary>
        /// Constructs a new offline device for reading 
        /// pcap files
        /// </summary>
        /// <param name="pcapFile"></param>
        public PcapOfflineDevice(string pcapFile)
        {
            m_pcapFile = pcapFile;
        }

        public override string Name
        {
            get
            {
                return m_pcapFile;
            }
        }

        public override string Description
        {
            get
            {
                return PCAP_OFFLINE_DESCRIPTION;
            }
        }

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
            get { return System.IO.Path.GetFileName( this.Name );}
        }

        /// <summary>
        /// Opens the device for capture
        /// </summary>
        public override void Open()
        {
            //holds errors
            StringBuilder errbuf = new StringBuilder( Pcap.PCAP_ERRBUF_SIZE ); //will hold errors
            //opens offline pcap file
            IntPtr adapterHandle = SafeNativeMethods.pcap_open_offline( this.Name, errbuf);

            //handle error
            if ( adapterHandle == IntPtr.Zero)
            {
                string err = "Unable to open offline adapter: " + errbuf.ToString();
                throw new Exception( err );
            }

            //set the local handle
            this.PcapHandle = adapterHandle;
        }

        /// <summary>
        /// Opens the device for capture
        /// </summary>
        /// <param name="promiscuous_mode">This parameter
        /// has no affect on this method since it's an 
        /// offline device</param>
        public override void Open(bool promiscuous_mode)
        {
            this.Open();
        }       

        /// <summary>
        /// Opens the device for capture
        /// </summary>
        /// <param name="promiscuous_mode">This parameter
        /// has no affect on this method since it's an 
        /// offline device</param>
        /// <param name="read_timeout">This parameter
        /// has no affect on this method since it's an 
        /// offline device</param>
        public override void Open(bool promiscuous_mode, int read_timeout)
        {
            this.Open();
        }

        /// <summary>
        /// Setting a capture filter on this offline device is not supported
        /// </summary>
        public override void SetFilter( string filter )
        {
            throw new PcapException("It is not possible to set a capture filter on an offline device");
        }

        /// <summary>
        /// Statistics are not supported for savefiles
        /// </summary>
        /// <returns>
        /// A <see cref="PcapStatistics"/>
        /// </returns>
        public override PcapStatistics Statistics ()
        {
            throw new PcapException("No statistics are stored in savefiles");
        }
    }
}
