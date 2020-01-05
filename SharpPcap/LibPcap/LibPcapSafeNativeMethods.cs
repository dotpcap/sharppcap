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
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 * Copyright 2008-2011 Chris Morgan <chmorgan@gmail.com>
 */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static SharpPcap.LibPcap.PcapUnmanagedStructures;

namespace SharpPcap.LibPcap
{
    /// <summary>
    /// Per http://msdn.microsoft.com/en-us/ms182161.aspx 
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class LibPcapSafeNativeMethods
    {
        static LibPcapSafeNativeMethods()
        {
            UseWindows = Environment.OSVersion.Platform != PlatformID.MacOSX &&
                Environment.OSVersion.Platform != PlatformID.Unix;
        }

        private static bool UseWindows { get; }

        internal static int pcap_findalldevs(ref IntPtr /* pcap_if_t** */ alldevs, StringBuilder /* char* */ errbuf)
        {
            return UseWindows ? Windows.pcap_findalldevs(ref alldevs, errbuf)
                : Unix.pcap_findalldevs(ref alldevs, errbuf);
        }

        internal static void pcap_freealldevs(IntPtr /* pcap_if_t * */ alldevs)
        {
            if (UseWindows)
            {
                Windows.pcap_freealldevs(alldevs);
            }
            else
            {
                Unix.pcap_freealldevs(alldevs);
            }
        }

        internal static IntPtr /* pcap_t* */ pcap_create(string dev, StringBuilder errbuf)
        {
            return UseWindows ? Windows.pcap_create(dev, errbuf) : Unix.pcap_create(dev, errbuf);
        }

        internal static IntPtr /* pcap_t* */ pcap_open_offline(string/*const char* */ fname, StringBuilder/* char* */ errbuf)
        {
            return UseWindows ? Windows.pcap_open_offline(fname, errbuf) : Unix.pcap_open_offline(fname, errbuf);
        }

        internal static IntPtr /* pcap_t* */ pcap_open_dead(int linktype, int snaplen)
        {
            return UseWindows ? Windows.pcap_open_dead(linktype, snaplen) : Unix.pcap_open_dead(linktype, snaplen);
        }

        internal static int pcap_set_buffer_size(IntPtr /* pcap_t */ adapter, int bufferSizeInBytes)
        {
            return UseWindows ? Windows.pcap_set_buffer_size(adapter, bufferSizeInBytes) : Unix.pcap_set_buffer_size(adapter, bufferSizeInBytes);
        }

        /// <summary>Open a file to write packets. </summary>
        internal static IntPtr /*pcap_dumper_t * */ pcap_dump_open(IntPtr /*pcap_t * */adaptHandle, string /*const char * */fname)
        {
            return UseWindows ? Windows.pcap_dump_open(adaptHandle, fname) : Unix.pcap_dump_open(adaptHandle, fname);
        }

        /// <summary>
        ///  Save a packet to disk.  
        /// </summary>
        internal static void pcap_dump(IntPtr /*u_char * */user, IntPtr /*const struct pcap_pkthdr * */h, IntPtr /*const u_char * */sp)
        {
            if (UseWindows)
            {
                Windows.pcap_dump(user, h, sp);
            }
            else
            {
                Unix.pcap_dump(user, h, sp);
            }
        }

        /// <summary> close the files associated with p and deallocates resources.</summary>
        internal static void pcap_close(IntPtr /*pcap_t **/adaptHandle)
        {
            if (UseWindows)
            {
                Windows.pcap_close(adaptHandle);
            }
            else
            {
                Unix.pcap_close(adaptHandle);
            }
        }

        /// <summary>
        /// To avoid callback, this returns one packet at a time
        /// </summary>
        internal static int pcap_next_ex(IntPtr /* pcap_t* */ adaptHandle, ref IntPtr /* **pkt_header */ header, ref IntPtr data)
        {
            return UseWindows ? Windows.pcap_next_ex(adaptHandle, ref header, ref data) : Unix.pcap_next_ex(adaptHandle, ref header, ref data);
        }

        /// <summary>
        /// Send a raw packet.<br/>
        /// This function allows to send a raw packet to the network. 
        /// The MAC CRC doesn't need to be included, because it is transparently calculated
        ///  and added by the network interface driver.
        /// </summary>
        /// <param name="adaptHandle">the interface that will be used to send the packet</param>
        /// <param name="data">contains the data of the packet to send (including the various protocol headers)</param>
        /// <param name="size">the dimension of the buffer pointed by data</param>
        /// <returns>0 if the packet is succesfully sent, -1 otherwise.</returns>
        internal static int pcap_sendpacket(IntPtr /* pcap_t* */ adaptHandle, IntPtr data, int size)
        {
            return UseWindows ? Windows.pcap_sendpacket(adaptHandle, data, size) : Unix.pcap_sendpacket(adaptHandle, data, size);
        }

        /// <summary>
        /// Compile a packet filter, converting an high level filtering expression (see Filtering expression syntax) in a program that can be interpreted by the kernel-level filtering engine. 
        /// </summary>
        internal static int pcap_compile(IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp, string /*char * */str, int optimize, UInt32 netmask)
        {
            return UseWindows ? Windows.pcap_compile(adaptHandle, fp, str, optimize, netmask) : Unix.pcap_compile(adaptHandle, fp, str, optimize, netmask);
        }

        internal static int pcap_setfilter(IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp)
        {
            return UseWindows ? Windows.pcap_setfilter(adaptHandle, fp) : Unix.pcap_setfilter(adaptHandle, fp);
        }

        /// <summary>
        /// Free up allocated memory pointed to by a bpf_program struct generated by pcap_compile()
        /// </summary>
        internal static void pcap_freecode(IntPtr /*bpf_program **/fp)
        {
            if (UseWindows)
            {
                Windows.pcap_freecode(fp);
            }
            else
            {
                Unix.pcap_freecode(fp);
            }
        }

        /// <summary>
        /// return the error text pertaining to the last pcap library error.
        /// </summary>
        internal static IntPtr pcap_geterr(IntPtr /*pcap_t * */ adaptHandle)
        {
            return UseWindows ? Windows.pcap_geterr(adaptHandle) : Unix.pcap_geterr(adaptHandle);
        }

        /// <summary>Returns a pointer to a string giving information about the version of the libpcap library being used; note that it contains more information than just a version number. </summary>
        internal static IntPtr /*const char **/  pcap_lib_version()
        {
            return UseWindows ? Windows.pcap_lib_version() : Unix.pcap_lib_version();
        }

        /// <summary>return the standard I/O stream of the 'savefile' opened by pcap_dump_open().</summary>
        internal static IntPtr /*FILE **/  pcap_dump_file(IntPtr /*pcap_dumper_t **/p)
        {
            return UseWindows ? Windows.pcap_dump_file(p) : Unix.pcap_dump_file(p);
        }

        /// <summary>Flushes the output buffer to the 'savefile', so that any packets 
        /// written with pcap_dump() but not yet written to the 'savefile' will be written. 
        /// -1 is returned on error, 0 on success. </summary>
        internal static int pcap_dump_flush(IntPtr /*pcap_dumper_t **/p)
        {
            return UseWindows ? Windows.pcap_dump_flush(p) : Unix.pcap_dump_flush(p);
        }

        /// <summary>Closes a savefile. </summary>
        internal static void pcap_dump_close(IntPtr /*pcap_dumper_t **/p)
        {
            if (UseWindows)
            {
                Windows.pcap_dump_close(p);
            }
            else
            {
                Unix.pcap_dump_close(p);
            }
        }

        /// <summary> Return the link layer of an adapter. </summary>
        internal static int pcap_datalink(IntPtr /* pcap_t* */ adaptHandle)
        {
            return UseWindows ? Windows.pcap_datalink(adaptHandle) : Unix.pcap_datalink(adaptHandle);
        }

        /// <summary>
        /// Set nonblocking mode. pcap_loop() and pcap_next() doesnt work in  nonblocking mode!
        /// </summary>
        internal static int pcap_setnonblock(IntPtr /* pcap_if_t** */ adaptHandle, int nonblock, StringBuilder /* char* */ errbuf)
        {
            return UseWindows ? Windows.pcap_setnonblock(adaptHandle, nonblock, errbuf) : Unix.pcap_setnonblock(adaptHandle, nonblock, errbuf);
        }

        /// <summary>
        /// Get nonblocking mode, returns allways 0 for savefiles.
        /// </summary>
        internal static int pcap_getnonblock(IntPtr /* pcap_if_t** */ adaptHandle, StringBuilder /* char* */ errbuf)
        {
            return UseWindows ? Windows.pcap_getnonblock(adaptHandle, errbuf) : Unix.pcap_getnonblock(adaptHandle, errbuf);
        }

        /// <summary>
        /// Read packets until cnt packets are processed or an error occurs.
        /// </summary>
        internal static int pcap_dispatch(IntPtr /* pcap_t* */ adaptHandle, int count, pcap_handler callback, IntPtr ptr)
        {
            return UseWindows ? Windows.pcap_dispatch(adaptHandle, count, callback, ptr) : Unix.pcap_dispatch(adaptHandle, count, callback, ptr);
        }

        /// <summary>
        /// The delegate declaration for PcapHandler requires an UnmanagedFunctionPointer attribute.
        /// Without this it fires for one time and then throws null pointer exception
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void pcap_handler(IntPtr param, IntPtr /* pcap_pkthdr* */ header, IntPtr pkt_data);

        /// <summary>
        /// Retrieves a selectable file descriptor
        /// </summary>
        /// <param name="adaptHandle">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        internal static int pcap_get_selectable_fd(IntPtr /* pcap_t* */ adaptHandle)
        {
            return UseWindows ? Windows.pcap_get_selectable_fd(adaptHandle) : Unix.pcap_get_selectable_fd(adaptHandle);
        }

        /// <summary>
        /// Fills in the pcap_stat structure passed to the function
        /// based on the pcap_t adapter
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="stat">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        internal static int pcap_stats(IntPtr /* pcap_t* */ adapter, IntPtr /* struct pcap_stat* */ stat)
        {
            return UseWindows ? Windows.pcap_stats(adapter, stat) : Unix.pcap_stats(adapter, stat);
        }

        /// <summary>
        /// Returns the snapshot length
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        internal static int pcap_snapshot(IntPtr /* pcap_t... */ adapter)
        {
            return UseWindows ? Windows.pcap_snapshot(adapter) : Unix.pcap_snapshot(adapter);
        }

        /// <summary>
        /// pcap_set_rfmon() sets whether monitor mode should be set on a capture handle when the handle is activated.
        /// If rfmon is non-zero, monitor mode will be set, otherwise it will not be set.  
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <param name="rfmon">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        internal static int pcap_set_rfmon(IntPtr /* pcap_t* */ p, int rfmon)
        {
            return UseWindows ? Windows.pcap_set_rfmon(p, rfmon) : Unix.pcap_set_rfmon(p, rfmon);
        }

        /// <summary>
        /// pcap_set_snaplen() sets the snapshot length to be used on a capture handle when the handle is activated to snaplen.  
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <param name="snaplen">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        internal static int pcap_set_snaplen(IntPtr /* pcap_t* */ p, int snaplen)
        {
            return UseWindows ? Windows.pcap_set_snaplen(p, snaplen) : Unix.pcap_set_snaplen(p, snaplen);
        }

        /// <summary>
        /// pcap_set_promisc() sets whether promiscuous mode should be set on a capture handle when the handle is activated. 
        /// If promisc is non-zero, promiscuous mode will be set, otherwise it will not be set.  
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <param name="promisc">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        internal static int pcap_set_promisc(IntPtr /* pcap_t* */ p, int promisc)
        {
            return UseWindows ? Windows.pcap_set_promisc(p, promisc) : Unix.pcap_set_promisc(p, promisc);
        }

        /// <summary>
        /// pcap_set_timeout() sets the packet buffer timeout that will be used on a capture handle when the handle is activated to to_ms, which is in units of milliseconds.
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <param name="to_ms">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        internal static int pcap_set_timeout(IntPtr /* pcap_t* */ p, int to_ms)
        {
            return UseWindows ? Windows.pcap_set_timeout(p, to_ms) : Unix.pcap_set_timeout(p, to_ms);
        }

        /// <summary>
        /// pcap_activate() is used to activate a packet capture handle to look at packets on the network, with the options that were set on the handle being in effect.  
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <returns>Returns 0 on success without warnings, a non-zero positive value on success with warnings, and a negative value on error. A non-zero return value indicates what warning or error condition occurred.</returns>
        internal static int pcap_activate(IntPtr /* pcap_t* */ p)
        {
            return UseWindows ? Windows.pcap_activate(p) : Unix.pcap_activate(p);
        }

        /// <summary>
        /// Force a pcap_dispatch() or pcap_loop() call to return
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static int pcap_breakloop(IntPtr /* pcap_t_* */ p)
        {
            return UseWindows ? Windows.pcap_breakloop(p) : Unix.pcap_breakloop(p);
        }

        #region libpcap specific
        /// <summary>
        /// Returns the file descriptor number from which captured packets are read,
        /// if a network device was opened with pcap_create() and pcap_activate() or
        /// with pcap_open_live(), or -1, if a ``savefile'' was opened with
        /// pcap_open_offline()
        /// Libpcap specific method
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        internal static int pcap_fileno(IntPtr /* pcap_t* p */ adapter)
        {
            return UseWindows ? Windows.pcap_fileno(adapter) : Unix.pcap_fileno(adapter);
        }
        #endregion

        #region Send queue functions

        /// <summary>
        /// Send a queue of raw packets to the network. 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="queue"></param>
        /// <param name="sync">determines if the send operation must be synchronized: 
        /// if it is non-zero, the packets are sent respecting the timestamps, 
        /// otherwise they are sent as fast as possible</param>
        /// <returns>The amount of bytes actually sent. 
        /// If it is smaller than the size parameter, an error occurred 
        /// during the send. The error can be caused by a driver/adapter 
        /// problem or by an inconsistent/bogus send queue.</returns>
        internal static int pcap_sendqueue_transmit(IntPtr/*pcap_t * */p, ref pcap_send_queue queue, int sync)
        {
            return UseWindows
                ? Windows.pcap_sendqueue_transmit(p, ref queue, sync)
                : Unix.pcap_sendqueue_transmit(p, ref queue, sync);
        }
        #endregion
    }
}
