/*
Copyright (c) 2005 Tamir Gal, http://www.tamirgal.com, All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice,
        this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright 
        notice, this list of conditions and the following disclaimer in 
        the documentation and/or other materials provided with the distribution.

    3. The names of the authors may not be used to endorse or promote products
        derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
        private PcapUnmanagedStructures.pcap_pkthdr m_pktHdr;
        /// <summary>
        /// This holds byte received and packets received
        /// </summary>
        private byte[]  m_pktData;
        /// <summary>
        /// Constructs a new Pcap Statistics strcuture
        /// </summary>
        /// <param name="pktHdr">Time value as PCAP_PKTHDR</param>
        /// <param name="pktData">Statistics values as PCAP_PKTDATA</param>
        internal PcapStatistics(PcapUnmanagedStructures.pcap_pkthdr pktHdr, PcapUnmanagedStructures.PCAP_PKTDATA pktData)
        {
            this.m_pktHdr   = pktHdr;
            this.m_pktData  = pktData.bytes;
        }

        internal PcapStatistics(Packets.Packet p)
        {
            this.m_pktHdr   = p.PcapHeader.m_pcap_pkthdr;
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
                return (ulong)m_pktHdr.ts.tv_sec;
            }           
        }

        /// <summary>
        /// The 'MicroSeconds' part of the timestamp
        /// </summary>
        public ulong MicroSeconds
        {
            get
            {
                return (ulong)m_pktHdr.ts.tv_usec;
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
