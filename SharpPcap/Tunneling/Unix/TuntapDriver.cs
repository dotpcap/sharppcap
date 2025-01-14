// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers.Binary;
using System.ComponentModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace SharpPcap.Tunneling.Unix
{
    /// <summary>
    /// Helper methods to communicate with tap device file
    /// </summary>
    internal class TuntapDriver : ITunnelDriver
    {
        internal static readonly ITunnelDriver Instance = new TuntapDriver();
        public FileStream Open(NetworkInterface networkInterface, IPAddressConfiguration address, DeviceConfiguration configuration)
        {
            var bufferSize = configuration.BufferSize ?? 4096;
            var stream = new FileStream("/dev/net/tun", FileMode.Open, FileAccess.ReadWrite, default, bufferSize);
            try
            {
                SetIff(stream.SafeFileHandle, networkInterface.Id, IffFlags.Tap | IffFlags.NoPi);
                SetAddress(networkInterface.Id, address);
                BringUp(networkInterface.Id, configuration.Mode.HasFlag(DeviceModes.Promiscuous));
            }
            catch (Exception)
            {
                stream.Dispose();
                throw;
            }
            return stream;
        }

        public Version GetVersion(NetworkInterface networkInterface, SafeFileHandle handle)
        {
            return null;
        }

        public bool IsTunnelInterface(NetworkInterface networkInterface)
        {
            return File.Exists($"/sys/class/net/{networkInterface.Name}/tun_flags");
        }

        [DllImport("libc", SetLastError = true)]
        private extern static int ioctl(int fd, uint request, ref IfReq data);

        private static void IOControl(IntPtr handle, SocketIoctl request, ref IfReq data)
        {
            var retval = ioctl(handle.ToInt32(), (uint)request, ref data);
            if (retval != 0)
            {
                var errno = Marshal.GetLastWin32Error();
                throw new Win32Exception(errno);
            }
        }

        internal static void SetIff(SafeFileHandle handle, string ifr_name, IffFlags ifr_flags)
        {
            IfReq ifr = default;
            ifr.ifr_name = ifr_name;
            ifr.ifr_flags = (short)ifr_flags;

            IOControl(handle.DangerousGetHandle(), (SocketIoctl)TUNSETIFF, ref ifr);
        }
        private void SetAddress(string ifr_name, IPAddressConfiguration address)
        {
            if (address.Address == null)
            {
                return;
            }
            IfReq ifr = default;
            ifr.ifr_name = ifr_name;
            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
            {

                ifr.ifr_addr.sa_family = (ushort)address.Address.AddressFamily;
                ifr.ifr_addr.sin_addr.s_addr = BitConverter.ToUInt32(address.Address.GetAddressBytes(), 0);
                IOControl(sock.Handle, SocketIoctl.SIOCSIFADDR, ref ifr);
                ifr.ifr_addr.sin_addr.s_addr = BitConverter.ToUInt32(address.IPv4Mask.GetAddressBytes(), 0);
                IOControl(sock.Handle, SocketIoctl.SIOCSIFNETMASK, ref ifr);
            }
        }

        internal static void BringUp(string ifr_name, bool promiscuous)
        {
            IfReq ifr = default;
            ifr.ifr_name = ifr_name;
            ifr.ifr_flags = (short)(NetDeviceFlags.Up | NetDeviceFlags.AllMulti | NetDeviceFlags.Running);
            if (promiscuous)
            {
                ifr.ifr_flags |= (short)NetDeviceFlags.Promisc;
            }
            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
            {
                IOControl(sock.Handle, SocketIoctl.SIOCSIFFLAGS, ref ifr);
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
