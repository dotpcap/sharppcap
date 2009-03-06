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
using System.Runtime.InteropServices;
using SharpPcap.Packets;

namespace SharpPcap
{
    /// <summary>
    /// Summary description for PcapSendQueue.
    /// </summary>
    public class PcapSendQueue
    {
        IntPtr m_queue = IntPtr.Zero;

        /// <summary>
        /// Creates and allocates a new PcapSendQueue and 
        /// </summary>
        /// <param name="memSize">
        /// The maximun amount of memory (in bytes) 
        /// to allocate for the queue</param>
        public PcapSendQueue(int memSize)
        {
            m_queue = SafeNativeMethods.pcap_sendqueue_alloc( memSize );
            if(m_queue==IntPtr.Zero)
                throw new PcapException("Error creating PcapSendQueue");
        }

        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <param name="pcapHdr">The pcap header of the packet</param>
        /// <returns>True if success, else false</returns>
        internal bool Add( byte[] packet, PcapUnmanagedStructures.pcap_pkthdr pcapHdr )
        {
            if(m_queue==IntPtr.Zero)
            {
                throw new PcapException("Can't add packet, this queue is disposed");
            }

            if(pcapHdr.caplen==0)
                pcapHdr.caplen = (uint)packet.Length;//set the length in the header field

            //Marshal packet
            IntPtr pktPtr;
            pktPtr = Marshal.AllocHGlobal(packet.Length);
            Marshal.Copy(packet, 0, pktPtr, packet.Length);

            //Marshal header
            IntPtr hdrPtr;
            hdrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PcapUnmanagedStructures.pcap_pkthdr)));
            Marshal.StructureToPtr(pcapHdr, hdrPtr, true);

            int res = SafeNativeMethods.pcap_sendqueue_queue( m_queue, hdrPtr, pktPtr);

            Marshal.FreeHGlobal(pktPtr);
            Marshal.FreeHGlobal(hdrPtr);    
    
            return (res!=-1);
        }
        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <param name="pcapHdr">The pcap header of the packet</param>
        /// <returns>True if success, else false</returns>
        internal bool Add( byte[] packet, PcapHeader pcapHdr )
        {
            return this.Add( packet, pcapHdr.m_pcap_pkthdr);
        }
        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <returns>True if success, else false</returns>
        public bool Add( byte[] packet )
        {
            PcapUnmanagedStructures.pcap_pkthdr hdr = new SharpPcap.PcapUnmanagedStructures.pcap_pkthdr();
            return this.Add( packet, hdr );
        }
        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <returns>True if success, else false</returns>
        public bool Add( Packet packet )
        {
            return this.Add( packet.Bytes, packet.PcapHeader.m_pcap_pkthdr );
        }
        /// <summary>
        /// Add a packet to this send queue.
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <param name="seconds">The 'seconds' part of the packet's timestamp</param>
        /// <param name="miliseconds">The 'microseconds' part of the packet's timestamp</param>
        /// <returns>True if success, else false</returns>
        public bool Add( byte[] packet, int seconds, int microseconds )
        {
            PcapUnmanagedStructures.pcap_pkthdr hdr = new SharpPcap.PcapUnmanagedStructures.pcap_pkthdr();
            hdr.ts.tv_sec = (IntPtr)seconds;
            hdr.ts.tv_usec = (IntPtr)microseconds;
            return this.Add( packet, hdr );
        }

        /// <summary>
        /// Send a queue of raw packets to the network. 
        /// </summary>
        /// <param name="device">The PcapDevice on which to send the queue</param>
        /// <param name="synchronize">determines if the send operation must be synchronized: 
        /// if it is non-zero, the packets are sent respecting the timestamps, 
        /// otherwise they are sent as fast as possible
        /// <returns></returns>
        public int Transmit( PcapDevice device, bool synchronize)
        {
            if(!device.Opened)
                throw new PcapException("Can't transmit queue, the pcap device is closed.");

            if(m_queue==IntPtr.Zero)
            {
                throw new PcapException("Can't transmit queue, this queue is disposed");
            }

            int sync = synchronize ? 1 : 0;         
            return SafeNativeMethods.pcap_sendqueue_transmit(device.PcapHandle, m_queue, sync);
        }

        /// <summary>
        /// Destroy the send queue. 
        /// </summary>
        public void Dispose()
        {
            if(m_queue!=IntPtr.Zero)
            {
                SafeNativeMethods.pcap_sendqueue_destroy( m_queue );
            }
        }

        /// <summary>
        /// The current length in bytes of this queue
        /// </summary>
        public int CurrentLength
        {
            get
            {
                if(m_queue==IntPtr.Zero)
                {
                    throw new PcapException("Can't perform operation, this queue is disposed");
                }
                PcapUnmanagedStructures.pcap_send_queue q =
                    (PcapUnmanagedStructures.pcap_send_queue)Marshal.PtrToStructure
                    (m_queue, typeof(PcapUnmanagedStructures.pcap_send_queue));
                return (int)q.len;
            }
        }
    }
}
