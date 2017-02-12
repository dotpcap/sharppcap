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
using System.Runtime.InteropServices;
using SharpPcap.LibPcap;

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// Interface to the WinPcap send queue extension methods
    /// </summary>
    public class SendQueue
    {
        IntPtr m_queue = IntPtr.Zero;

        /// <summary>
        /// Creates and allocates a new SendQueue
        /// </summary>
        /// <param name="memSize">
        /// The maximun amount of memory (in bytes) 
        /// to allocate for the queue</param>
        public SendQueue(int memSize)
        {
            // ensure that we are running under winpcap
            WinPcapDevice.ThrowIfNotWinPcap();

            m_queue = WinPcap.SafeNativeMethods.pcap_sendqueue_alloc( memSize );
            if(m_queue==IntPtr.Zero)
                throw new PcapException("Error creating PcapSendQueue");
        }

        /// <summary>
        /// Add a packet to this send queue. The PcapHeader defines the packet length.
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <param name="pcapHdr">The pcap header of the packet</param>
        /// <returns>True if success, else false</returns>
        internal bool AddInternal( byte[] packet, PcapHeader pcapHdr )
        {
            if(m_queue==IntPtr.Zero)
            {
                throw new PcapException("Can't add packet, this queue is disposed");
            }

            // the header defines the size to send
            if(pcapHdr.CaptureLength > packet.Length)
            {
                var error = string.Format("pcapHdr.CaptureLength of {0} > packet.Length {1}",
                                          pcapHdr.CaptureLength, packet.Length);
                throw new System.InvalidOperationException(error);
            }

            //Marshal packet
            IntPtr pktPtr;
            pktPtr = Marshal.AllocHGlobal(packet.Length);
            Marshal.Copy(packet, 0, pktPtr, packet.Length);

            //Marshal header
            IntPtr hdrPtr = pcapHdr.MarshalToIntPtr();

            int res = WinPcap.SafeNativeMethods.pcap_sendqueue_queue( m_queue, hdrPtr, pktPtr);

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
            return this.AddInternal( packet, pcapHdr);
        }

        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <returns>True if success, else false</returns>
        public bool Add( byte[] packet )
        {
            PcapHeader hdr = new PcapHeader();
            hdr.CaptureLength = (uint)packet.Length;
            return this.AddInternal( packet, hdr );
        }

        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <returns>True if success, else false</returns>
        public bool Add( RawCapture packet )
        {
            var data = packet.Data;
            var timeval = packet.Timeval;
            var header = new PcapHeader((uint)timeval.Seconds, (uint)timeval.MicroSeconds,
                                        (uint)data.Length, (uint)data.Length);
            return this.AddInternal(data, header);
        }

        /// <summary>
        /// Add a packet to this send queue.
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <param name="seconds">The 'seconds' part of the packet's timestamp</param>
        /// <param name="microseconds">The 'microseconds' part of the packet's timestamp</param>
        /// <returns>True if success, else false</returns>
        public bool Add( byte[] packet, int seconds, int microseconds )
        {
            var header = new PcapHeader((uint)seconds, (uint)microseconds,
                                        (uint)packet.Length, (uint)packet.Length);
            
            return this.Add( packet, header );
        }

        /// <summary>
        /// Send a queue of raw packets to the network. 
        /// </summary>
        /// <param name="device">
        /// The device on which to send the queue
        /// A <see cref="PcapDevice"/>
        /// </param>
        /// <param name="transmitMode">
        /// A <see cref="SendQueueTransmitModes"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public int Transmit( WinPcapDevice device, SendQueueTransmitModes transmitMode)
        {
            if(!device.Opened)
                throw new DeviceNotReadyException("Can't transmit queue, the pcap device is closed");

            if(m_queue==IntPtr.Zero)
            {
                throw new PcapException("Can't transmit queue, this queue is disposed");
            }

            int sync = (transmitMode == SendQueueTransmitModes.Synchronized) ? 1 : 0;
            return WinPcap.SafeNativeMethods.pcap_sendqueue_transmit(device.PcapHandle, m_queue, sync);
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
