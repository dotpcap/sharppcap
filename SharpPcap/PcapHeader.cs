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
 */

using System;

namespace SharpPcap
{
    /// <summary>
    ///  A wrapper class for libpcap's pcap_pkthdr structure
    /// </summary>
    public class PcapHeader
    {
        /// <summary>
        /// The underlying pcap_pkthdr structure
        /// </summary>
        internal PcapUnmanagedStructures.pcap_pkthdr m_pcap_pkthdr;

        /// <summary>
        /// Constructs a new PcapHeader
        /// </summary>
        public PcapHeader()
        {
            m_pcap_pkthdr = new PcapUnmanagedStructures.pcap_pkthdr();
        }

        /// <summary>
        /// Constructs a new PcapHeader
        /// </summary>
        /// <param name="m_pcap_pkthdr">The underlying pcap_pkthdr structure</param>
        internal PcapHeader( PcapUnmanagedStructures.pcap_pkthdr m_pcap_pkthdr )
        {
            this.m_pcap_pkthdr = m_pcap_pkthdr;
        }

        /// <summary>
        /// Constructs a new PcapHeader
        /// </summary>
        /// <param name="seconds">The seconds value of the packet's timestamp</param>
        /// <param name="microseconds">The microseconds value of the packet's timestamp</param>
        /// <param name="packetLength">The actual length of the packet</param>
        /// <param name="captureLength">The length of the capture</param>
        public PcapHeader( ulong seconds, ulong microseconds, uint packetLength, uint captureLength )
        {
            this.m_pcap_pkthdr.ts.tv_sec    =   (IntPtr)seconds;
            this.m_pcap_pkthdr.ts.tv_usec   =   (IntPtr)microseconds;
            this.m_pcap_pkthdr.len          =   packetLength;
            this.m_pcap_pkthdr.caplen       =   captureLength;
        }

        /// <summary>
        /// The seconds value of the packet's timestamp
        /// </summary>
        public ulong Seconds
        {
            get{return (ulong)m_pcap_pkthdr.ts.tv_sec;}
            set{m_pcap_pkthdr.ts.tv_sec = (IntPtr)value;}
        }

        /// <summary>
        /// The microseconds value of the packet's timestamp
        /// </summary>
        public ulong MicroSeconds
        {
            get{return (ulong)m_pcap_pkthdr.ts.tv_usec;}
            set{m_pcap_pkthdr.ts.tv_usec = (IntPtr)value;}
        }

        /// <summary>
        /// The actual length of the packet
        /// </summary>
        public uint PacketLength
        {
            get {return m_pcap_pkthdr.len;}
            set {m_pcap_pkthdr.len=value;}
        }
        /// <summary>
        /// The length of the capture
        /// </summary>
        public uint CaptureLength
        {
            get{return m_pcap_pkthdr.caplen;}
            set{m_pcap_pkthdr.caplen=value;}
        }
        
        /// <summary>
        /// Return the DateTime value of this pcap header
        /// </summary>
        virtual public System.DateTime Date
        {
            get
            {
                DateTime time = new DateTime(1970,1,1); 
                time = time.AddSeconds(Seconds); 
                time = time.AddMilliseconds(MicroSeconds / 1000); 
                return time.ToLocalTime();
            }           
        }
    }
}
