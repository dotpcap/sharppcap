﻿namespace SharpPcap.WinTap
{
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
        config_dhcp_set_Opt = 9,

        /* Added in 8.2 */

        /** obsoletes ConfigPointToPoint */
        ConfigTun = 10,

        /** Control whether 802.1Q headers are added for priority */
        PriorityBehavior = 11,

    }
}
