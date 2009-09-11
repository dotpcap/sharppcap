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
﻿using System;

namespace SharpPcap
{
    public class PcapCaptureEventArgs : EventArgs
    {
        private Packets.Packet packet;
        public Packets.Packet Packet
        {
            get { return packet; }
        }

        private PcapDevice device;
        public PcapDevice Device
        {
            get { return device; }
        }

        public PcapCaptureEventArgs(Packets.Packet packet, PcapDevice device)
        {
            this.packet = packet;
            this.device = device;
        }

    }

    public class PcapStatisticsEventArgs : PcapCaptureEventArgs
    {        
        public PcapStatisticsEventArgs(Packets.Packet packet, PcapDevice device) : base(packet, device)
        {

        }

        public PcapStatistics Statistics
        {
            get
            {
                return new PcapStatistics(base.Packet);
            }
        }
    }

}
