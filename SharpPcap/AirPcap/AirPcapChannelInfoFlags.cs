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
    /// Channel info flags
    /// </summary>
    [Flags]
    public enum AirPcapChannelInfoFlags : byte
    {
        /// <summary>
        /// No flags set
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Channel info flag: the channel is enabled for transmission, too.
        ///
        /// To comply with the electomagnetic emission regulations of the different countries, the AirPcap hardware can be programmed
        /// to block transmission on specific channels. This flag is set by AirpcapGetDeviceSupportedChannels() to indicate that a 
        /// channel in the list supports transmission.
        /// </summary>
        TxEnable = 0x1
    };
}
