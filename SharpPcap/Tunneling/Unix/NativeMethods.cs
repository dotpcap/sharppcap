using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace SharpPcap.Tunneling.Unix
{
    /// <summary>
    /// Helper methods to communicate with tap device file
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("libc", SetLastError = true)]
        private extern static int ioctl(int fd, uint request, ref Ifreq data);

        private static void IOControl(IntPtr handle, uint request, ref Ifreq data)
        {
            var retval = ioctl(handle.ToInt32(), request, ref data);
            if (retval != 0)
            {
                var errno = Marshal.GetLastWin32Error();
                throw new Win32Exception(errno);
            }
        }

        internal static void SetIff(SafeFileHandle handle, string ifr_name, IffFlags ifr_flags)
        {
            Ifreq ifr = default;
            ifr.ifr_name = ifr_name;
            ifr.ifr_flags = (short)ifr_flags;

            IOControl(handle.DangerousGetHandle(), TUNSETIFF, ref ifr);
        }

        internal static void BringUp(string ifr_name)
        {
            var SIOCSIFFLAGS = 0x8914U;
            Ifreq ifr = default;
            ifr.ifr_name = ifr_name;
            ifr.ifr_flags = (short)(NetDeviceFlags.Up | NetDeviceFlags.AllMulti | NetDeviceFlags.Running);
            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
            {
                IOControl(sock.Handle, SIOCSIFFLAGS, ref ifr);
            }
        }

        /* Ioctl defines */
        private static uint GetIoc(IocDir dir, char type, uint nr, uint size)
        {
            return ((uint)dir << 28) | ((uint)type << 8) | (nr << 0) | (size << 16);
        }

        /// <summary>
        /// See https://github.com/torvalds/linux/blob/master/include/uapi/linux/if_tun.h
        /// </summary>
        static readonly uint TUNSETIFF = GetIoc(IocDir.Write, 'T', 202, sizeof(int));
        static readonly uint TUNSETPERSIST = GetIoc(IocDir.Write, 'T', 203, sizeof(int));
        static readonly uint TUNSETOWNER = GetIoc(IocDir.Write, 'T', 204, sizeof(int));
        static readonly uint TUNSETLINK = GetIoc(IocDir.Write, 'T', 205, sizeof(int));
        static readonly uint TUNSETGROUP = GetIoc(IocDir.Write, 'T', 206, sizeof(int));

    }
}
