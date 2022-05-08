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

using PacketDotNet;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    public class SendQueue : IDisposable
    {
        public static readonly bool IsHardwareAccelerated = GetIsHardwareAccelerated();

        private static bool GetIsHardwareAccelerated()
        {
            using (var handle = LibPcapSafeNativeMethods.pcap_open_dead(1, 60))
            {
                try
                {
                    pcap_send_queue queue = default;
                    LibPcapSafeNativeMethods.pcap_sendqueue_transmit(handle, ref queue, 0);
                    return true;
                }
                catch (TypeLoadException)
                {
                    // Function pcap_sendqueue_transmit not found
                    return false;
                }
            }
        }

        private byte[] buffer;
        private readonly TimestampResolution TimeResolution;

        /// <summary>
        /// Number of bytes in the queue that are pending transmission
        /// </summary>
        public int CurrentLength { get; private set; }

        public SendQueue(int memSize, TimestampResolution timeResolution = TimestampResolution.Microsecond)
        {
            buffer = new byte[memSize];
            TimeResolution = timeResolution;
        }

        /// <summary>
        /// Add a packet to this send queue. The PcapHeader defines the packet length.
        /// </summary>
        /// <param name="header">The pcap header of the packet</param>
        /// <param name="packet">The packet bytes to add</param>
        /// <returns>True if success, else false</returns>
        public bool Add(PcapHeader header, byte[] packet)
        {
            if (buffer == null)
            {
                throw new ObjectDisposedException(nameof(SendQueue));
            }
            var hdrSize = PcapHeader.MemorySize;
            var pktSize = (int)header.CaptureLength;
            // the header defines the size to send
            if (pktSize > packet.Length)
            {
                var error = string.Format("pcapHdr.CaptureLength of {0} > packet.Length {1}",
                                          pktSize, packet.Length);
                throw new InvalidOperationException(error);
            }

            if (hdrSize + pktSize > buffer.Length - CurrentLength)
            {
                return false;
            }
            //Marshal header
            IntPtr hdrPtr = header.MarshalToIntPtr(TimeResolution);
            Marshal.Copy(hdrPtr, buffer, CurrentLength, hdrSize);
            Marshal.FreeHGlobal(hdrPtr);

            Buffer.BlockCopy(packet, 0, buffer, CurrentLength + hdrSize, pktSize);

            CurrentLength += hdrSize + pktSize;

            return true;
        }

        /// <summary>
        /// Send a queue of raw packets to the network. 
        /// </summary>
        /// <param name="device">
        /// The device on which to send the queue
        /// A <see cref="PcapDevice"/>
        /// </param>
        /// <param name="synchronized">
        /// Should the timestamps be respected
        /// </param>
        /// <returns>
        /// The number of bytes sent as an <see cref="int"/>
        /// </returns>
        public int Transmit(PcapDevice device, bool synchronized)
        {
            return Transmit(device, (synchronized == true) ? SendQueueTransmitModes.Synchronized : SendQueueTransmitModes.Normal);
        }

        /// <summary>
        /// Send a queue of raw packets to the network.
        /// </summary>
        /// <param name="device">
        /// The device on which to send the queue
        /// A <see cref="PcapDevice"/>
        /// </param>
        /// <param name="transmitMode">
        /// Should the timestamps be respected
        /// </param>
        /// <returns>
        /// The number of bytes sent as an <see cref="int"/>
        /// </returns>
        public int Transmit(PcapDevice device, SendQueueTransmitModes transmitMode)
        {
            if (buffer == null)
            {
                throw new ObjectDisposedException(nameof(SendQueue));
            }
            if (!device.Opened)
            {
                throw new DeviceNotReadyException("Can't transmit queue, the pcap device is closed");
            }
            if (IsHardwareAccelerated)
            {
                return NativeTransmit(device, transmitMode);
            }
            return ManagedTransmit(device, transmitMode);
        }

        protected unsafe int ManagedTransmit(PcapDevice device, SendQueueTransmitModes transmitMode)
        {
            if (CurrentLength == 0)
            {
                return 0;
            }
            var position = 0;
            var hdrSize = PcapHeader.MemorySize;
            var sw = new Stopwatch();
            fixed (byte* buf = buffer)
            {
                var bufPtr = new IntPtr(buf);
                var firstTimestamp = TimeSpan.FromTicks(PcapHeader.FromPointer(bufPtr, TimeResolution).Timeval.Date.Ticks);
                while (position < CurrentLength)
                {
                    // Extract packet from buffer
                    var header = PcapHeader.FromPointer(bufPtr + position, TimeResolution);
                    var pktSize = (int)header.CaptureLength;
                    var p = new ReadOnlySpan<byte>(buffer, position + hdrSize, pktSize);
                    if (transmitMode == SendQueueTransmitModes.Synchronized)
                    {
                        var timestamp = TimeSpan.FromTicks(header.Timeval.Date.Ticks);
                        var remainingTime = timestamp.Subtract(firstTimestamp);
                        while (sw.Elapsed < remainingTime)
                        {
                            // Wait for packet time
                            if (remainingTime.TotalMilliseconds > 50)
                            {
                                Thread.Yield();
                            } else
                            {
                                Thread.SpinWait(1);
                            }
                        }
                    }
                    // Send the packet
                    int res;
                    unsafe
                    {
                        fixed (byte* p_packet = p)
                        {
                            res = LibPcapSafeNativeMethods.pcap_sendpacket(device.Handle, new IntPtr(p_packet), p.Length);
                        }
                    }
                    // Start Stopwatch after sending first packet
                    sw.Start();
                    if (res < 0)
                    {
                        break;
                    }
                    position += hdrSize + pktSize;
                }
            }
            return position;
        }

        protected unsafe int NativeTransmit(PcapDevice device, SendQueueTransmitModes transmitMode)
        {
            if (CurrentLength == 0)
            {
                // Npcap does not properly check for 0 packets queue
                // See https://github.com/nmap/npcap/issues/287
                return 0;
            }
            int sync = (transmitMode == SendQueueTransmitModes.Synchronized) ? 1 : 0;
            fixed (byte* buf = buffer)
            {
                var pcap_queue = new pcap_send_queue
                {
                    maxlen = (uint)buffer.Length,
                    len = (uint)CurrentLength,
                    ptrBuff = new IntPtr(buf)
                };
                return LibPcapSafeNativeMethods.pcap_sendqueue_transmit(device.Handle, ref pcap_queue, sync);
            }
        }

        public void Dispose()
        {
            buffer = null;
        }
    }

    public static class SendQueueExtensions
    {
        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <returns>True if success, else false</returns>
        public static bool Add(this SendQueue queue, byte[] packet)
        {
            var header = new PcapHeader(0, 0, (uint)packet.Length, (uint)packet.Length);
            return queue.Add(header, packet);
        }

        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet bytes to add</param>
        /// <returns>True if success, else false</returns>
        public static bool Add(this SendQueue queue, Packet packet)
        {
            return queue.Add(packet.Bytes);
        }

        /// <summary>
        /// Add a packet to this send queue. 
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <returns>True if success, else false</returns>
        public static bool Add(this SendQueue queue, RawCapture packet)
        {
            var data = packet.Data;
            var timeval = packet.Timeval;
            var header = new PcapHeader((uint)timeval.Seconds, (uint)timeval.MicroSeconds,
                                        (uint)data.Length, (uint)data.Length);
            return queue.Add(header, data);
        }

        /// <summary>
        /// Add a packet to this send queue.
        /// </summary>
        /// <param name="packet">The packet to add</param>
        /// <param name="seconds">The 'seconds' part of the packet's timestamp</param>
        /// <param name="microseconds">The 'microseconds' part of the packet's timestamp</param>
        /// <returns>True if success, else false</returns>
        public static bool Add(this SendQueue queue, byte[] packet, int seconds, int microseconds)
        {
            var header = new PcapHeader((uint)seconds, (uint)microseconds,
                                        (uint)packet.Length, (uint)packet.Length);

            return queue.Add(header, packet);
        }
    }
}
