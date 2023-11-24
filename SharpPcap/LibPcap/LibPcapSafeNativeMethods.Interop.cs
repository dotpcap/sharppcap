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
using System.IO;
using System.Reflection;
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
    internal static partial class LibPcapSafeNativeMethods
    {
        // NOTE: For mono users on non-windows platforms a .config file is used to map
        //       the windows dll name to the unix/mac library name
        //       This file is called $assembly_name.dll.config and is placed in the
        //       same directory as the assembly
        //       See http://www.mono-project.com/Interop_with_Native_Libraries#Library_Names
        private const string PCAP_DLL = "wpcap";

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_init(
            uint opts,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /* char* */ errbuf
        );

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_findalldevs(
            ref IntPtr /* pcap_if_t** */ alldevs,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /* char* */ errbuf
        );

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_findalldevs_ex(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string /*char **/source,
            ref pcap_rmtauth /*pcap_rmtauth **/auth,
            ref IntPtr /*pcap_if_t ** */alldevs,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /*char * */errbuf
        );

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void pcap_freealldevs(IntPtr /* pcap_if_t * */ alldevs);

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapHandle /* pcap_t* */ pcap_open(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string dev,
            int packetLen,
            int flags,
            int read_timeout,
            ref pcap_rmtauth rmtauth,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder errbuf
        );

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapHandle /* pcap_t* */ pcap_create(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string dev,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder errbuf
        );

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapHandle /* pcap_t* */ pcap_open_offline(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string/*const char* */ fname,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder/* char* */ errbuf
        );

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapHandle /* pcap_t* */ pcap_open_dead(int linktype, int snaplen);

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapError pcap_set_buffer_size(PcapHandle /* pcap_t */ adapter, int bufferSizeInBytes);

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapError pcap_set_immediate_mode(PcapHandle /* pcap_t */ adapter, int immediate_mode);

        /// <summary>Open a file to write packets. </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr /* pcap_dumper_t * */ pcap_dump_open(
            PcapHandle /*pcap_t * */adaptHandle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string /*const char * */ fname
        );

        /// <summary>Append a file to write packets. </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr /* pcap_dumper_t * */ pcap_dump_open_append(
            PcapHandle /*pcap_t * */adaptHandle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string /*const char * */ fname
        );

        /// <summary>
        ///  Save a packet to disk.  
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void pcap_dump(IntPtr /*u_char * */ user, IntPtr /*const struct pcap_pkthdr * */h, IntPtr /*const u_char * */sp);

        /// <summary> close the files associated with p and deallocates resources.</summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void pcap_close(IntPtr /*pcap_t * */ adaptHandle);

        /// <summary>
        /// To avoid callback, this returns one packet at a time
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_next_ex(PcapHandle /* pcap_t* */ adaptHandle, ref IntPtr /* **pkt_header */ header, ref IntPtr data);

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
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_sendpacket(PcapHandle /* pcap_t* */ adaptHandle, IntPtr data, int size);

        /// <summary>
        /// Compile a packet filter, converting an high level filtering expression (see Filtering expression syntax) in a program that can be interpreted by the kernel-level filtering engine. 
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_compile(
            PcapHandle /* pcap_t* */ adaptHandle, BpfProgram fp,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string /*char * */str,
            int optimize, UInt32 netmask
        );

        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setfilter(PcapHandle /* pcap_t* */ adaptHandle, BpfProgram fp);

        /// <summary>
        /// Returns if a given filter applies to an offline packet. 
        /// </summary>
        /// <returns></returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_offline_filter(BpfProgram prog, IntPtr /* pcap_pkthdr* */ header, IntPtr pkt_data);

        /// <summary>
        /// Free up allocated memory pointed to by a bpf_program struct generated by pcap_compile()
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void pcap_freecode(IntPtr fp);

        /// <summary>
        /// return the error text pertaining to the last pcap library error.
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler), MarshalCookie = "no_free")]
        internal extern static string pcap_geterr(PcapHandle /*pcap_t * */ adaptHandle);

        /// <summary>Returns a pointer to a string giving information about the version of the libpcap library being used; note that it contains more information than just a version number. </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler), MarshalCookie = "no_free")]
        internal extern static string /* const char * */  pcap_lib_version();

        /// <summary>return the standard I/O stream of the 'savefile' opened by pcap_dump_open().</summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr /*FILE **/  pcap_dump_file(IntPtr /*pcap_dumper_t **/p);

        /// <summary>Flushes the output buffer to the 'savefile', so that any packets 
        /// written with pcap_dump() but not yet written to the 'savefile' will be written. 
        /// -1 is returned on error, 0 on success. </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_dump_flush(IntPtr /*pcap_dumper_t **/p);

        /// <summary>Closes a savefile. </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void pcap_dump_close(IntPtr /*pcap_dumper_t **/p);

        /// <summary> Return the link layer of an adapter. </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_datalink(PcapHandle /* pcap_t* */ adaptHandle);

        /// <summary>
        /// Set nonblocking mode. pcap_loop() and pcap_next() doesnt work in  nonblocking mode!
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setnonblock(
            PcapHandle /* pcap_if_t** */ adaptHandle,
            int nonblock,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /* char* */ errbuf
        );

        /// <summary>
        /// Get nonblocking mode, returns allways 0 for savefiles.
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_getnonblock(
            PcapHandle /* pcap_if_t** */ adaptHandle,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /* char* */ errbuf
        );

        /// <summary>
        /// Read packets until cnt packets are processed or an error occurs.
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_dispatch(PcapHandle /* pcap_t* */ adaptHandle, int count, pcap_handler callback, IntPtr ptr);

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
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_get_selectable_fd(PcapHandle /* pcap_t* */ adaptHandle);

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
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_stats(PcapHandle /* pcap_t* */ adapter, IntPtr /* struct pcap_stat* */ stat);

        /// <summary>
        /// Returns the snapshot length
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_snapshot(PcapHandle /* pcap_t * */ adapter);

        /// <summary>
        /// pcap_set_rfmon() sets whether monitor mode should be set on a capture handle when the handle is activated.
        /// If rfmon is non-zero, monitor mode will be set, otherwise it will not be set.  
        /// </summary>
        /// <param name="p">A <see cref="PcapHandle"/></param>
        /// <param name="rfmon">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        [DllImport(PCAP_DLL, EntryPoint = "pcap_set_rfmon", CallingConvention = CallingConvention.Cdecl)]
        private extern static PcapError _pcap_set_rfmon(PcapHandle /* pcap_t* */ p, int rfmon);

        /// <summary>
        /// pcap_set_snaplen() sets the snapshot length to be used on a capture handle when the handle is activated to snaplen.  
        /// </summary>
        /// <param name="p">A <see cref="PcapHandle"/></param>
        /// <param name="snaplen">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapError pcap_set_snaplen(PcapHandle /* pcap_t* */ p, int snaplen);

        /// <summary>
        /// pcap_set_promisc() sets whether promiscuous mode should be set on a capture handle when the handle is activated. 
        /// If promisc is non-zero, promiscuous mode will be set, otherwise it will not be set.  
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <param name="promisc">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapError pcap_set_promisc(PcapHandle /* pcap_t* */ p, int promisc);

        /// <summary>
        /// pcap_set_timeout() sets the packet buffer timeout that will be used on a capture handle when the handle is activated to to_ms, which is in units of milliseconds.
        /// </summary>
        /// <param name="p">A <see cref="IntPtr"/></param>
        /// <param name="to_ms">A <see cref="int"/></param>
        /// <returns>Returns 0 on success or PCAP_ERROR_ACTIVATED if called on a capture handle that has been activated.</returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapError pcap_set_timeout(PcapHandle /* pcap_t* */ p, int to_ms);

        /// <summary>
        /// pcap_activate() is used to activate a packet capture handle to look at packets on the network, with the options that were set on the handle being in effect.  
        /// </summary>
        /// <param name="p">A <see cref="PcapHandle"/></param>
        /// <returns>Returns 0 on success without warnings, a non-zero positive value on success with warnings, and a negative value on error. A non-zero return value indicates what warning or error condition occurred.</returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapError pcap_activate(PcapHandle /* pcap_t* */ p);

        /// <summary>
        /// Force a pcap_dispatch() or pcap_loop() call to return
        /// </summary>
        /// <param name="p">A <see cref="PcapHandle"/></param>
        /// <returns>Returns 0 on success without warnings, a non-zero positive value on success with warnings, and a negative value on error. A non-zero return value indicates what warning or error condition occurred.</returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_breakloop(PcapHandle /* pcap_t* */ p);

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
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_fileno(PcapHandle /* pcap_t* p */ adapter);
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
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_sendqueue_transmit(PcapHandle /*pcap_t * */p, ref pcap_send_queue queue, int sync);
        #endregion

        #region Timestamp related functions
        /// <summary>
        /// Available since libpcap 1.5
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, EntryPoint = "pcap_set_tstamp_precision", CallingConvention = CallingConvention.Cdecl)]
        private extern static PcapError _pcap_set_tstamp_precision(PcapHandle /* pcap_t* p */ adapter, int precision);

        /// <summary>
        /// Available since libpcap 1.5
        /// </summary>
        /// <param name="adapter"></param>
        [DllImport(PCAP_DLL, EntryPoint = "pcap_get_tstamp_precision", CallingConvention = CallingConvention.Cdecl)]
        private extern static int _pcap_get_tstamp_precision(PcapHandle /* pcap_t* p */ adapter);

        /// <summary>
        /// Available since libpcap 1.2
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="tstamp_type"></param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapError pcap_set_tstamp_type(PcapHandle /* pcap_t* p */ adapter, int tstamp_type);

        /// <summary>
        /// Available since libpcap 1.2
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_list_tstamp_types(PcapHandle /* pcap_t* p */ adapter, ref IntPtr types_pointer_pointer);

        /// <summary>
        /// Since libpcap 1.2
        /// </summary>
        /// <param name="types_pointer"></param>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void pcap_free_tstamp_types(IntPtr types_pointer);

        // pcap_tstamp_type_name_to_val is unused, compile it out to remove it from
        // being considered in code coverage analysis
#if false
        /// <summary>
        /// Since libpcap 1.2
        /// </summary>
        /// <returns></returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_tstamp_type_name_to_val(
             [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string tstamp_name
        );
#endif

        /// <summary>
        /// Since libpcap 1.2
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler), MarshalCookie = "no_free")]
        internal extern static string /* const char* */ pcap_tstamp_type_val_to_name(int tstamp_val);

        /// <summary>
        /// Since libpcap 1.2
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler), MarshalCookie = "no_free")]
        internal extern static string /* const char* */ pcap_tstamp_type_val_to_description(int tstamp_val);

        /// <summary>
        /// Since libpcap 1.5.1
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="precision"></param>
        /// <param name="errbuf"></param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapHandle /* pcap_t* */ pcap_open_offline_with_tstamp_precision(
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] string /* const char* */ fname,
            uint precision,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /* char* */ errbuf
        );

        /// <summary>
        /// Since libpcap 1.5.1
        /// </summary>
        /// <param name="type"></param>
        /// <param name="snaplen"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static PcapHandle /* pcap_t* */ pcap_open_dead_with_tstamp_precision(int type, int snaplen, uint precision);
        #endregion

        /// <summary>
        /// This function is different from <see cref="pcap_set_buffer_size"/>.
        /// It's for kernel buffer size, and applicable only for Windows
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="bufferSizeInBytes"></param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, EntryPoint = "pcap_setbuff", CallingConvention = CallingConvention.Cdecl)]
        private extern static PcapError _pcap_setbuff(PcapHandle /* pcap_t */ adapter, int bufferSizeInBytes);

        /// <summary>
        /// Windows Only
        /// changes the minimum amount of data in the kernel buffer that causes 
        /// a read from the application to return (unless the timeout expires)
        /// Setting this to zero will put the device in immediate mode in Windows
        /// See https://www.tcpdump.org/manpages/pcap_set_immediate_mode.3pcap.html
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="PcapHandle"/>
        /// </param>
        /// <param name="sizeInBytes">
        /// A <see cref="int"/>
        /// </param>
        /// <returns>
        /// A <see cref="int"/>
        /// </returns>
        [DllImport(PCAP_DLL, EntryPoint = "pcap_setmintocopy", CallingConvention = CallingConvention.Cdecl)]
        private extern static PcapError _pcap_setmintocopy(PcapHandle /* pcap_t */ adapter, int sizeInBytes);

        /// <summary>
        /// Windows Only
        /// Set the working mode of the interface p to mode. 
        /// Valid values for mode are MODE_CAPT (default capture mode) 
        /// and MODE_STAT (statistical mode). See the tutorial 
        /// "\ref wpcap_tut9" for details about statistical mode.
        /// Npcap specific method
        /// </summary>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setmode(PcapHandle /* pcap_t * */ p, int mode);

        /// <summary>
        /// Windows Only
        /// Wraps a Pcap handle around an existing OS handle, e.g., a pipe.
        /// </summary>
        /// <param name="handle">Native Windows handle.</param>
        /// <param name="precision">Desired timestamp precision (micro/nano).</param>
        /// <param name="errbuf">Buffer that will receive an error description if an error occurs.</param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, EntryPoint = "pcap_hopen_offline_with_tstamp_precision", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr /* pcap_t* */ _pcap_hopen_offline_with_tstamp_precision(
            SafeHandle handle,
            uint precision,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /* char* */ errbuf
        );

        /// <summary>
        /// Non-Windows Only
        /// Wraps a Pcap handle around a C runtime FILE object.
        /// </summary>
        /// <param name="fileObject">Pointer to FILE as returned by fopen, etc.</param>
        /// <param name="precision">Desired timestamp precision (micro/nano).</param>
        /// <param name="errbuf">Buffer that will receive an error description if an error occurs.</param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, EntryPoint = "pcap_fopen_offline_with_tstamp_precision", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr /* pcap_t* */ _pcap_fopen_offline_with_tstamp_precision(
            SafeHandle fileObject,
            uint precision,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PcapStringMarshaler))] StringBuilder /* char* */ errbuf
        );
    }
}
