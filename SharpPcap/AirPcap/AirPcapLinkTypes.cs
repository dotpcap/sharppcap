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
    /// Link type
    /// </summary>
    public enum AirPcapLinkTypes : int
    {
        /// <summary>
        /// plain 802.11 link type. Every packet in the buffer contains the raw 802.11 frame, including MAC FCS.
        /// </summary>
        _802_11 = 1,

        /// <summary>
        /// 802.11 plus radiotap link type. Every packet in the buffer contains a radiotap header followed by the 802.11 frame. MAC FCS is included.
        /// </summary>
        _802_11_PLUS_RADIO = 2,

        /// <summary>
        /// Unknown link type, should be seen only in error
        /// </summary>
        UNKNOWN = 3,

        /// <summary>
        /// 802.11 plus PPI header link type. Every packet in the buffer contains a PPI header followed by the 802.11 frame. MAC FCS is included.
        /// </summary>
        _802_11_PLUS_PPI = 4
    };
}
