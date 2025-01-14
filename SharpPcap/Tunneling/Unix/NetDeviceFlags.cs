// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap.Tunneling.Unix
{
    internal enum NetDeviceFlags : ushort
    {
        Up = 1 << 0,  /* sysfs */
        Broadcast = 1 << 1,  /* volatile */
        Debug = 1 << 2,  /* sysfs */
        Loopback = 1 << 3,  /* volatile */
        PoinToPoint = 1 << 4,  /* volatile */
        NoTrailers = 1 << 5,  /* sysfs */
        Running = 1 << 6,  /* volatile */
        NoArp = 1 << 7,  /* sysfs */
        Promisc = 1 << 8,  /* sysfs */
        AllMulti = 1 << 9,  /* sysfs */
        Master = 1 << 10, /* volatile */
        Slave = 1 << 11, /* volatile */
        Multicast = 1 << 12, /* sysfs */
        Portsel = 1 << 13, /* sysfs */
        AutoMedia = 1 << 14, /* sysfs */
        Dynamic = 1 << 15, /* sysfs */
    };
}
