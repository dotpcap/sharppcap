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
 * Copyright 2010-2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Runtime.InteropServices;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Packet header
    /// </summary>
    public class AirPcapPacketHeader
    {
        /// <summary>
        /// Seconds field
        /// </summary>
        public ulong TsSec
        {
            get;
            set;
        }

        /// <summary>
        /// Microseconds field
        /// </summary>
        public ulong TsUsec
        {
            get;
            set;
        }

        /// <summary>
        /// Number of bytes captured
        /// </summary>
        public long Caplen
        {
            get;
            set;
        }

        /// <summary>
        /// On-line packet size in bytes
        /// </summary>
        public long Originallen
        {
            get;
            set;
        }

        /// <summary>
        /// Header length in bytes
        /// </summary>
        public long Hdrlen
        {
            get;
            set;
        }

        internal AirPcapPacketHeader(IntPtr packetHeader)
        {
            var pkthdr = (AirPcapUnmanagedStructures.AirpcapBpfHeader)Marshal.PtrToStructure(packetHeader,
                                                                                             typeof(AirPcapUnmanagedStructures.AirpcapBpfHeader));

            this.TsSec          = (ulong)pkthdr.TsSec;
            this.TsUsec         = (ulong)pkthdr.TsUsec;
            this.Caplen         = (long)pkthdr.Caplen;
            this.Originallen    = (long)pkthdr.Originallen;
            this.Hdrlen         = (long)pkthdr.Hdrlen;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("TsSec {0}, TsUSec {1}, Caplen {2}, Originallen {3}, Hdrlen {4}",
                                 TsSec,
                                 TsUsec,
                                 Caplen,
                                 Originallen,
                                 Hdrlen);
        }
    }
}
