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
    /* Name                                 Data type       Units
     * ----                                 ---------       -----
     *
     * IEEE80211_RADIOTAP_TSFT              u_int64_t       microseconds
     *
     *      Value in microseconds of the MAC's 64-bit 802.11 Time
     *      Synchronization Function timer when the first bit of the
     *      MPDU arrived at the MAC. For received frames, only.
     *
     * IEEE80211_RADIOTAP_CHANNEL           2 x u_int16_t   MHz, bitmap
     *
     *      Tx/Rx frequency in MHz, followed by flags (see below).
     *
     * IEEE80211_RADIOTAP_FHSS              u_int16_t       see below
     *
     *      For frequency-hopping radios, the hop set (first byte)
     *      and pattern (second byte).
     *
     * IEEE80211_RADIOTAP_RATE              u_int8_t        500kb/s
     *
     *      Tx/Rx data rate
     *
     * IEEE80211_RADIOTAP_DBM_ANTSIGNAL     int8_t          decibels from
     *                                                      one milliwatt (dBm)
     *
     *      RF signal power at the antenna, decibel difference from
     *      one milliwatt.
     *
     * IEEE80211_RADIOTAP_DBM_ANTNOISE      int8_t          decibels from
     *                                                      one milliwatt (dBm)
     *
     *      RF noise power at the antenna, decibel difference from one
     *      milliwatt.
     *
     * IEEE80211_RADIOTAP_DB_ANTSIGNAL      u_int8_t        decibel (dB)
     *
     *      RF signal power at the antenna, decibel difference from an
     *      arbitrary, fixed reference.
     *
     * IEEE80211_RADIOTAP_DB_ANTNOISE       u_int8_t        decibel (dB)
     *
     *      RF noise power at the antenna, decibel difference from an
     *      arbitrary, fixed reference point.
     *
     * IEEE80211_RADIOTAP_LOCK_QUALITY		u_int16_t       unitless
     *
     *      Quality of Barker code lock. Unitless. Monotonically
     *      nondecreasing with "better" lock strength. Called "Signal
     *      Quality" in datasheets.  (Is there a standard way to measure
     *      this?)
     *
     * IEEE80211_RADIOTAP_TX_ATTENUATION    u_int16_t       unitless
     *
     *      Transmit power expressed as unitless distance from max
     *      power set at factory calibration.  0 is max power.
     *      Monotonically nondecreasing with lower power levels.
     *
     * IEEE80211_RADIOTAP_DB_TX_ATTENUATION u_int16_t       decibels (dB)
     *
     *      Transmit power expressed as decibel distance from max power
     *      set at factory calibration.  0 is max power.  Monotonically
     *      nondecreasing with lower power levels.
     *
     * IEEE80211_RADIOTAP_DBM_TX_POWER      int8_t          decibels from
     *                                                      one milliwatt (dBm)
     *
     *      Transmit power expressed as dBm (decibels from a 1 milliwatt
     *      reference). This is the absolute power level measured at
     *      the antenna port.
     *
     * IEEE80211_RADIOTAP_FLAGS             u_int8_t        bitmap
     *
     *      Properties of transmitted and received frames. See flags
     *      defined below.
     *
     * IEEE80211_RADIOTAP_ANTENNA           u_int8_t        antenna index
     *
     *      Unitless indication of the Rx/Tx antenna for this packet.
     *      The first antenna is antenna 0.
     *
     * IEEE80211_RADIOTAP_FCS           	u_int32_t       data
     *
     *	FCS from frame in network byte order.
     */
    /* ethereal does NOT handle the following:
        IEEE80211_RADIOTAP_FHSS:
        IEEE80211_RADIOTAP_LOCK_QUALITY:
        IEEE80211_RADIOTAP_TX_ATTENUATION:
        IEEE80211_RADIOTAP_DB_TX_ATTENUATION:
    */
    [Flags]
    public enum AirPcapRadioTapType : int
    {
        IEEE80211_RADIOTAP_TSFT = 0,
        IEEE80211_RADIOTAP_FLAGS = 1,
        IEEE80211_RADIOTAP_RATE = 2,
        IEEE80211_RADIOTAP_CHANNEL = 3,
        IEEE80211_RADIOTAP_FHSS = 4,
        IEEE80211_RADIOTAP_DBM_ANTSIGNAL = 5,
        IEEE80211_RADIOTAP_DBM_ANTNOISE = 6,
        IEEE80211_RADIOTAP_LOCK_QUALITY = 7,
        IEEE80211_RADIOTAP_TX_ATTENUATION = 8,
        IEEE80211_RADIOTAP_DB_TX_ATTENUATION = 9,
        IEEE80211_RADIOTAP_DBM_TX_POWER = 10,
        IEEE80211_RADIOTAP_ANTENNA = 11,
        IEEE80211_RADIOTAP_DB_ANTSIGNAL = 12,
        IEEE80211_RADIOTAP_DB_ANTNOISE = 13,
        IEEE80211_RADIOTAP_FCS = 14,
        IEEE80211_RADIOTAP_EXT = 31,
    };
}
