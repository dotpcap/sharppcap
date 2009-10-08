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
    /// Holds network statistics for a Pcap Devices
    /// </summary>
    public class PcapStatistics
    {
        /// <summary>
        /// This holds time value
        /// </summary>
        private PcapHeader m_pktHdr;

        /// <summary>
        /// This holds byte received and packets received
        /// </summary>
        private byte[]  m_pktData;

        /// <summary>
        /// Constructs a new Pcap Statistics strcuture
        /// </summary>
        /// <param name="pktHdr">Time value as PCAP_PKTHDR</param>
        /// <param name="pktData">Statistics values as PCAP_PKTDATA</param>
        internal PcapStatistics(PcapHeader pktHdr, PcapUnmanagedStructures.PCAP_PKTDATA pktData)
        {
            this.m_pktHdr   = pktHdr;
            this.m_pktData  = pktData.bytes;
        }

        internal PcapStatistics(Packets.Packet p)
        {
            this.m_pktHdr   = p.PcapHeader;
            this.m_pktData  = p.Bytes;
        }

        /// <summary>
        /// Number of packets received since last sample
        /// </summary>
        public Int64 RecievedPackets
        {
            get
            {
                return BitConverter.ToInt64(m_pktData, 0);
            }
        }

        /// <summary>
        /// Number of bytes received since last sample
        /// </summary>
        public Int64 RecievedBytes
        {
            get
            {
                return BitConverter.ToInt64(m_pktData, 8);
            }
        }

        /// <summary>
        /// The 'Seconds' part of the timestamp
        /// </summary>
        public ulong Seconds
        {
            get
            {
                return m_pktHdr.Seconds;
            }           
        }

        /// <summary>
        /// The 'MicroSeconds' part of the timestamp
        /// </summary>
        public ulong MicroSeconds
        {
            get
            {
                return m_pktHdr.MicroSeconds;
            }           
        }

        /// <summary>
        /// The timestamps
        /// </summary>
        public System.DateTime Date
        {
            get
            {
                DateTime timeval = new DateTime(1970,1,1); 
                timeval = timeval.AddSeconds(Seconds); 
                timeval = timeval.AddMilliseconds(MicroSeconds / 1000); 
                return timeval.ToLocalTime();
            }           
        }
    }
}
