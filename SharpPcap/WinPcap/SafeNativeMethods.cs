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
 */

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace SharpPcap.WinPcap
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

        #region WinPcap specific
        /// <summary>
        /// Extended pcap_open() method that is WinPcap specific that
        /// provides extra flags and functionality
        /// See http://www.winpcap.org/docs/docs_40_2/html/group__wpcapfunc.html#g2b64c7b6490090d1d37088794f1f1791
        /// </summary>
        /// <param name="dev">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="packetLen">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="flags">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="read_timeout">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="rmtauth">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="errbuf">
        /// A <see cref="StringBuilder"/>
        /// </param>
        /// <returns>
        /// A <see cref="IntPtr"/>
        /// </returns>
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr /* pcap_t* */ pcap_open(string dev,
                                                              int packetLen,
                                                              int flags,
                                                              int read_timeout,
                                                              IntPtr rmtauth,
                                                              StringBuilder errbuf);

        /// <summary>Create a list of network devices that can be opened with pcap_open().</summary>
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_findalldevs_ex (string /*char **/source,
                                                        IntPtr /*pcap_rmtauth **/auth,
                                                        ref IntPtr /*pcap_if_t ** */alldevs,
                                                        StringBuilder /*char * */errbuf);

        /// <summary>
        /// Set the working mode of the interface p to mode. 
        /// Valid values for mode are MODE_CAPT (default capture mode) 
        /// and MODE_STAT (statistical mode). See the tutorial 
        /// "\ref wpcap_tut9" for details about statistical mode.
        /// WinPcap specific method
        /// </summary>
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
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
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setbuff(IntPtr /* pcap_t */ adapter, int bufferSizeInBytes);

        /// <summary>
        /// changes the minimum amount of data in the kernel buffer that causes 
        /// a read from the application to return (unless the timeout expires)
        /// See http://www.winpcap.org/docs/docs_412/html/group__wpcapfunc.html#gab14ceacbf1c2f63026416dd73f80dc0d
        /// </summary>
        /// <param name="adapter">
        /// A <see cref="IntPtr"/>
        /// </param>
        /// <param name="sizeInBytes">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_setmintocopy(IntPtr /* pcap_t */ adapter, int sizeInBytes);

        /// <summary>
        /// Returns the AirPcap handler associated with an adapter. This handler can be used to change the
        /// wireless-related settings of the CACE Technologies AirPcap wireless capture adapters.
        ///
        /// Note: THIS FUNCTION SHOULD BE CONSIDERED PROVISIONAL, AND MAY BE REPLACED IN THE FUTURE BY A
        /// MORE COMPLETE SET OF FUNCTIONS FOR WIRELESS SUPPORT.
        /// pcap_get_airpcap_handle() allows to obtain the airpcap handle of an open adapter. This handle
        /// can be used with the AirPcap API functions to perform wireless-releated operations, e.g. changing
        /// the channel or enabling WEP decryption. For more details about the AirPcap wireless capture adapters,
        /// see http://www.cacetech.com/products/airpcap.html
        ///
        /// Parameters:
        ///   p,: handle to an open libpcap adapter
        /// Returns:
        ///   a PAirpcapHandle pointer to an open AirPcap handle, used internally by the libpcap open adapter.
        ///   NULL if the libpcap adapter doesn't have wireless support through AirPcap.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DllImport(PCAP_DLL, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr pcap_get_airpcap_handle(IntPtr /* pcap_t* */ p);

        #region Send queue functions
        /// <summary>
        /// Allocate a send queue. 
        /// </summary>
        /// <param name="memsize">The size of the queue</param>
        /// <returns>A pointer to the allocated buffer</returns>
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr /*pcap_send_queue * */pcap_sendqueue_alloc(int memsize) ;

        /// <summary>
        /// Destroy a send queue. 
        /// </summary>
        /// <param name="queue">A pointer to the queue start address</param>
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static void pcap_sendqueue_destroy(IntPtr /* pcap_send_queue * */queue) ;

        /// <summary>
        /// Add a packet to a send queue. 
        /// </summary>
        /// <param name="queue">A pointer to a queue</param>
        /// <param name="header">The pcap header of the packet to send</param>
        /// <param name="data">The packet data</param>
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
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
        [DllImport(PCAP_DLL, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        internal extern static int pcap_sendqueue_transmit(IntPtr/*pcap_t * */p, IntPtr /* pcap_send_queue * */queue, int sync);
        #endregion

        #endregion
    }
}

