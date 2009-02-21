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
