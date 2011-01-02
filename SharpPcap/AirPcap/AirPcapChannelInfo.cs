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
    /// Channel info
    /// </summary>
    public class AirPcapChannelInfo
    {
        ///<summary>
        ///Channel frequency, in MHz
        ///</summary>
        uint Frequency { get; set; }

        /// <summary>
        /// 802.11n specific. Offset of the extension channel in case of 40MHz channels. 
        ///
        /// Possible values are -1, 0 +1: 
        /// - -1 means that the extension channel should be below the control channel (e.g. Control = 5 and Extension = 1)
        /// - 0 means that no extension channel should be used (20MHz channels or legacy mode)
        /// - +1 means that the extension channel should be above the control channel (e.g. Control = 1 and Extension = 5)
        ///
        /// In case of 802.11a/b/g channels (802.11n legacy mode), this field should be set to 0.
        ///
        /// </summary>
        sbyte ExtChannel { get; set; }

        /// <summary>
        /// Channel Flags. The only flag supported at this time is \ref AIRPCAP_CIF_TX_ENABLED.
        /// </summary>
        AirPcapChannelInfoFlags Flags { get; set; }

        internal AirPcapUnmanagedStructures.AirpcapChannelInfo UnmanagedInfo
        {
            get
            {
                var channelInfo = new AirPcapUnmanagedStructures.AirpcapChannelInfo();
                channelInfo.Frequency = Frequency;
                channelInfo.ExtChannel = ExtChannel;
                channelInfo.Flags = Flags;
                return channelInfo;
            }
        }

        internal AirPcapChannelInfo(AirPcapUnmanagedStructures.AirpcapChannelInfo channelInfo)
        {
            Frequency = channelInfo.Frequency;
            ExtChannel = channelInfo.ExtChannel;
            Flags = channelInfo.Flags;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("[AirPcapChannelInfo Frequency: {0}, ExtChannel: {1}, Flags: {2}]",
                                 Frequency, ExtChannel, Flags);
        }
    };
}
