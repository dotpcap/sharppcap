// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap.Tunneling.WinTap
{
    /// <summary>
    /// See https://github.com/OpenVPN/tap-windows6/blob/master/src/tap-windows.h
    /// </summary>
    enum TapIoControl
    {
        /* Present in 8.1 */

        GetMac = 1,
        GetVersion = 2,
        GetMtu = 3,
        GetInfo = 4,
        ConfigPointToPoint = 5,
        SetMediaStatus = 6,
        ConfigDhcpMasq = 7,
        GetLogLine = 8,
        ConfigDhcpSetOpt = 9,

        /* Added in 8.2 */

        /** obsoletes ConfigPointToPoint */
        ConfigTun = 10,

        /** Control whether 802.1Q headers are added for priority */
        PriorityBehavior = 11,

    }
}
