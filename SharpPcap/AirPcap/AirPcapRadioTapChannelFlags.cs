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
    [Flags]
    public enum AirPcapRadioTapChannelFlags
    {
        /// <summary>Turbo channel</summary>
        IEEE80211_CHAN_TURBO = 0x0010,
        ///<summary>CCK channel</summary>
        IEEE80211_CHAN_CCK = 0x0020,
        /// <summary>OFDM channel</summary>
        IEEE80211_CHAN_OFDM = 0x0040,
        /// <summary>2 GHz spectrum channel</summary>
        IEEE80211_CHAN_2GHZ = 0x0080,
        /// <summary>5 GHz spectrum channel</summary>
        IEEE80211_CHAN_5GHZ = 0x0100,
        /// <summary>Only passive scan allowed</summary>
        IEEE80211_CHAN_PASSIVE = 0x0200,
        /// <summary>Dynamic CCK-OFDM channel</summary>
        IEEE80211_CHAN_DYN = 0x0400,
        /// <summary>GFSK channel (FHSS PHY)</summary>
        IEEE80211_CHAN_GFSK = 0x0800,
        /// <summary>11a static turbo channel only</summary>
        IEEE80211_CHAN_STURBO = 0x2000
    };
}
