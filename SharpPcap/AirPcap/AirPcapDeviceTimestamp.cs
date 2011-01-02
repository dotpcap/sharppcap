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

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Defines the internal AirPcap device timestamp
    /// </summary>
    public class AirPcapDeviceTimestamp
    {
        /// <summary>Current value of the device counter, in microseconds.</summary>
        public UInt64 DeviceTimestamp;

        /// <summary>Value of the software counter used to timestamp packets before reading the device counter, in microseconds.</summary>
        public UInt64 SoftwareTimestampBefore;

        /// <summary>Value of the software counter used to timestamp packets after reading the device counter, in microseconds.</summary>
        public UInt64 SoftwareTimestampAfter;

        internal AirPcapDeviceTimestamp(AirPcapUnmanagedStructures.AirpcapDeviceTimestamp timestamp)
        {
            DeviceTimestamp = timestamp.DeviceTimestamp;
            SoftwareTimestampBefore = timestamp.SoftwareTimestampBefore;
            SoftwareTimestampAfter = timestamp.SoftwareTimestampAfter;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("[AirPcapDeviceTimestamp DeviceTimestamp {0}, SoftwareTimestampBefore {1}, SoftwareTimestampAfter {2}",
                                 DeviceTimestamp, SoftwareTimestampBefore, SoftwareTimestampAfter);
        }
    }
}
