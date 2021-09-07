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
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2021 Ayoub Kaanich <kayoub5@live.com>
 */

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SharpPcap.LibPcap
{
    public class BpfProgram : SafeHandleZeroOrMinusOneIsInvalid
    {
        public static BpfProgram TryCreate(PcapHandle pcapHandle, string filter, int optimize = 1, uint netmask = 0)
        {
            var bpfProgram = new BpfProgram();
            //compile the expressions
            var result = LibPcapSafeNativeMethods.pcap_compile(pcapHandle,
                                                     bpfProgram,
                                                     filter,
                                                     optimize,
                                                     netmask);

            if (result < 0)
            {
                // Don't use Dispose since we don't want pcap_freecode to be called here
                Marshal.FreeHGlobal(bpfProgram.handle);
                bpfProgram.SetHandle(IntPtr.Zero);
                bpfProgram = null;
            }
            return bpfProgram;
        }


        public static BpfProgram Create(PcapHandle pcapHandle, string filter, int optimize = 1, uint netmask = 0)
        {
            var bpfProgram = TryCreate(pcapHandle, filter, optimize, netmask);
            if (bpfProgram == null)
            {
                throw new PcapException(PcapDevice.GetLastError(pcapHandle));
            }
            return bpfProgram;
        }

        private BpfProgram()
            : base(true)
        {
            var bpfProgram = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(PcapUnmanagedStructures.bpf_program)));
            SetHandle(bpfProgram);
        }

        /// <summary>
        /// Runs the program and returns if a given filter applies to the packet
        /// </summary>
        /// <param name="bpfProgram">
        /// A <see cref="IntPtr"/>
        /// </param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool Run(IntPtr header, IntPtr data)
        {
            var result = LibPcapSafeNativeMethods.pcap_offline_filter(this, header, data);
            return result != 0;
        }

        /// <summary>
        /// Runs the program and returns if a given filter applies to the packet
        /// </summary>
        /// <param name="bpfProgram">
        /// A <see cref="IntPtr"/>
        /// </param>
        public bool Run(ReadOnlySpan<byte> data)
        {
            var header = new PcapHeader()
            {
                CaptureLength = (uint)data.Length,
                PacketLength = (uint)data.Length,
            };
            IntPtr hdrPtr = header.MarshalToIntPtr();
            int result;
            unsafe
            {
                fixed (byte* p_packet = data)
                {
                    result = LibPcapSafeNativeMethods.pcap_offline_filter(this, hdrPtr, new IntPtr(p_packet));
                }
            }
            Marshal.FreeHGlobal(hdrPtr);
            return result != 0;
        }

        protected override bool ReleaseHandle()
        {
            LibPcapSafeNativeMethods.pcap_freecode(handle);
            //Alocate an unmanaged buffer
            Marshal.FreeHGlobal(handle);
            return true;
        }
    }
}
