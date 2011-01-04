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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace SharpPcap
{
    /// <summary>
    /// The mode used when opening a device
    /// </summary>
    public enum DeviceMode : short
    {
        /// <summary>
        /// Promiscuous mode.
        /// Instructs the OS that we want to receive all packets, even those not
        /// intended for the adapter. On non-switched networks this can result in
        /// a large amount of addtional traffic.
        /// NOTE: Devices in this mode CAN be detected via the network
        /// </summary>
        Promiscuous = 1,

        /// <summary>
        /// Not promiscuous mode
        /// </summary>
        Normal = 0
    }
}
