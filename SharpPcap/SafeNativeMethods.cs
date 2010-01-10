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
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpPcap
{
    /// <summary>
    /// Per http://msdn.microsoft.com/en-us/ms182161.aspx 
    /// </summary>
    [SuppressUnmanagedCodeSecurityAttribute]   
    internal static class SafeNativeMethods   
    {
        // NOTE: For mono users on non-windows platforms a .config file is used to map
        //       the windows dll name to the unix/mac library name
        //       This file is called $assembly_name.dll.config and is placed in the
        //       same directory as the assembly
        //       See http://www.mono-project.com/Interop_with_Native_Libraries#Library_Names
        private const string PCAP_DLL = "wpcap";

        [DllImport(PCAP_DLL, CharSet=CharSet.Auto)]
        internal extern static int pcap_findalldevs(ref IntPtr /* pcap_if_t** */ alldevs, StringBuilder /* char* */ errbuf);

        /// <summary>Create a list of network devices that can be opened with pcap_open().</summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_findalldevs_ex (string /*char **/source, IntPtr /*pcap_rmtauth **/auth, ref IntPtr /*pcap_if_t ** */alldevs, StringBuilder /*char * */errbuf);
        
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static void pcap_freealldevs(IntPtr /* pcap_if_t * */ alldevs);

        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /* pcap_t* */ pcap_open_live(string dev, int packetLen, short mode, short timeout, StringBuilder errbuf);

        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /* pcap_t* */ pcap_open_offline( string/*const char* */ fname, StringBuilder/* char* */ errbuf ); 

        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /* pcap_t* */ pcap_open_dead(int linktype, int snaplen);

        /// <summary>Open a file to write packets. </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /*pcap_dumper_t * */ pcap_dump_open (IntPtr /*pcap_t * */adaptHandle, string /*const char * */fname);
        
        /// <summary>
        ///  Save a packet to disk.  
        /// </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static void  pcap_dump (IntPtr /*u_char * */user, IntPtr /*const struct pcap_pkthdr * */h, IntPtr /*const u_char * */sp) ;
        
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /* pcap_t* */ pcap_open(string dev, int packetLen, short mode, short timeout,IntPtr auth, StringBuilder errbuf);

        /// <summary> close the files associated with p and deallocates resources.</summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static void  pcap_close (IntPtr /*pcap_t **/adaptHandle) ;           

        /// <summary>
        /// To avoid callback, this returns one packet at a time
        /// </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_next_ex(IntPtr /* pcap_t* */ adaptHandle, ref IntPtr /* **pkt_header */ header , ref IntPtr  data);

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
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_sendpacket(IntPtr /* pcap_t* */ adaptHandle, IntPtr  data, int size);

        /// <summary>
        /// Compile a packet filter, converting an high level filtering expression (see Filtering expression syntax) in a program that can be interpreted by the kernel-level filtering engine. 
        /// </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_compile (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp, string /*char * */str, int optimize, UInt32 netmask);

        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int  pcap_setfilter (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp);

        /// <summary>
        /// Free up allocated memory pointed to by a bpf_program struct generated by pcap_compile()
        /// </summary>
        [DllImport(PCAP_DLL)]
        internal extern static void pcap_freecode(IntPtr /*bpf_program **/fp);

        /// <summary>
        /// return the error text pertaining to the last pcap library error.
        /// </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr pcap_geterr (IntPtr /*pcap_t * */ adaptHandle);

        /// <summary>Returns a pointer to a string giving information about the version of the libpcap library being used; note that it contains more information than just a version number. </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /*const char **/  pcap_lib_version ();
        
        /// <summary>return the standard I/O stream of the 'savefile' opened by pcap_dump_open().</summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /*FILE **/  pcap_dump_file (IntPtr /*pcap_dumper_t **/p);
        
        /// <summary>Flushes the output buffer to the 'savefile', so that any packets 
        /// written with pcap_dump() but not yet written to the 'savefile' will be written. 
        /// -1 is returned on error, 0 on success. </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_dump_flush (IntPtr /*pcap_dumper_t **/p);
            
        /// <summary>Closes a savefile. </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static void  pcap_dump_close (IntPtr /*pcap_dumper_t **/p);
                
        /// <summary> Return the link layer of an adapter. </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_datalink(IntPtr /* pcap_t* */ adaptHandle);

        /// <summary>
        /// Set nonblocking mode. pcap_loop() and pcap_next() doesnt work in  nonblocking mode!
        /// </summary>
        [DllImport(PCAP_DLL, CharSet = CharSet.Auto)]
        internal extern static int pcap_setnonblock(IntPtr /* pcap_if_t** */ adaptHandle, int nonblock, StringBuilder /* char* */ errbuf);

        /// <summary>
        /// Get nonblocking mode, returns allways 0 for savefiles.
        /// </summary>
        [DllImport(PCAP_DLL, CharSet = CharSet.Auto)]
        internal extern static int pcap_getnonblock(IntPtr /* pcap_if_t** */ adaptHandle, StringBuilder /* char* */ errbuf);

        /// <summary>
        /// Read packets until cnt packets are processed or an error occurs.
        /// </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_dispatch(IntPtr /* pcap_t* */ adaptHandle, int count, pcap_handler callback, IntPtr ptr);

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
        /// A <see cref="System.Int32"/>
        /// </returns>
        [DllImport(PCAP_DLL)]
        internal extern static int pcap_get_selectable_fd(IntPtr /* pcap_t* */ adaptHandle);

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
        /// A <see cref="System.Int32"/>
        /// </returns>
        [DllImport(PCAP_DLL)]
        internal extern static int pcap_stats(IntPtr /* pcap_t* */ adapter, IntPtr /* struct pcap_stat* */ stat);

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
        /// A <see cref="System.Int32"/>
        /// </returns>
        [DllImport(PCAP_DLL)]
        internal extern static int pcap_fileno(IntPtr /* pcap_t* p */ adapter);
        #endregion

        #region WinPcap specific
        /// <summary>
        /// Set the working mode of the interface p to mode. 
        /// Valid values for mode are MODE_CAPT (default capture mode) 
        /// and MODE_STAT (statistical mode). See the tutorial 
        /// "\ref wpcap_tut9" for details about statistical mode.
        /// WinPcap specific method
        /// </summary>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_setmode  ( IntPtr/* pcap_t * */ p, int  mode );

        /// <summary>
        /// WinPcap specific method for setting the kernel buffer size
        /// associated with this adapter. The old buffer is discarded
        /// when the buffer size is changed.
        /// See http://www.winpcap.org/docs/docs_40_2/html/group__wpcapfunc.html
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="bufferSizeInBytes">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        [DllImport(PCAP_DLL)]
        internal extern static int pcap_setbuff(IntPtr /* pcap_t */ adapter, int bufferSizeInBytes);

        #region Send queue functions
        /// <summary>
        /// Allocate a send queue. 
        /// </summary>
        /// <param name="memsize">The size of the queue</param>
        /// <returns>A pointer to the allocated buffer</returns>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static IntPtr /*pcap_send_queue * */pcap_sendqueue_alloc(int memsize) ;

        /// <summary>
        /// Destroy a send queue. 
        /// </summary>
        /// <param name="queue">A pointer to the queue start address</param>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static void pcap_sendqueue_destroy(IntPtr /* pcap_send_queue * */queue) ;

        /// <summary>
        /// Add a packet to a send queue. 
        /// </summary>
        /// <param name="queue">A pointer to a queue</param>
        /// <param name="header">The pcap header of the packet to send</param>
        /// <param name="data">The packet data</param>
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_sendqueue_queue(IntPtr /* pcap_send_queue * */queue, IntPtr /* **pkt_header */ header , IntPtr  data);

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
        [DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
        internal extern static int pcap_sendqueue_transmit(IntPtr/*pcap_t * */p, IntPtr /* pcap_send_queue * */queue, int sync);
        #endregion

        #endregion
    }
}
