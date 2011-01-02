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
    /// Type of keys in the adapter
    /// </summary>
    public enum AirPcapKeyType : uint
    {
        /// <summary>
        /// Key type: WEP. The key can have an arbitrary length smaller than 32 bytes.
        /// </summary>
        Wep = 0,

        /// <summary>
        /// Key type: TKIP (WPA). NOT SUPPORTED YET by AirPcap
        /// </summary>
        Tkip = 1,

        /// <summary>
        /// Key type: CCMP (WPA2). NOT SUPPORTED YET by AirPcap
        /// </summary>
        Ccmp = 2
    };
}
