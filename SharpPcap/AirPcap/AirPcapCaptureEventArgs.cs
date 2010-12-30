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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Class modeled after CaptureEventArgs that has
    /// AirPcap specific contents
    /// </summary>
    public class AirPcapCaptureEventArgs : EventArgs
    {
        private AirPcapRawPacket packet;

        /// <summary>
        /// Packet that was captured
        /// </summary>
        public AirPcapRawPacket Packet
        {
            get { return packet; }
        }

        private ICaptureDevice device;

        /// <summary>
        /// Device this EventArgs was generated for
        /// </summary>
        public ICaptureDevice Device
        {
            get { return device; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packet">
        /// A <see cref="PacketDotNet.RawPacket"/>
        /// </param>
        /// <param name="device">
        /// A <see cref="PcapDevice"/>
        /// </param>
        public AirPcapCaptureEventArgs(AirPcapRawPacket packet, ICaptureDevice device)
        {
            this.packet = packet;
            this.device = device;
        }
    }
}
