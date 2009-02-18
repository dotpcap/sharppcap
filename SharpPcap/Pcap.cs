/*
Copyright (c) 2005 Tamir Gal, http://www.tamirgal.com, All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

	1. Redistributions of source code must retain the above copyright notice,
		this list of conditions and the following disclaimer.

	2. Redistributions in binary form must reproduce the above copyright 
		notice, this list of conditions and the following disclaimer in 
		the documentation and/or other materials provided with the distribution.

	3. The names of the authors may not be used to endorse or promote products
		derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpPcap.PcapUnmanagedStructures;

namespace SharpPcap
{
	/// <summary>
	/// Summary description for SharpPcap.
	/// </summary>
	public class Pcap
	{
        // NOTE: For mono users on non-windows platforms a .config file is used to map
        //       the windows dll name to the unix/mac library name
        //       This file is called $assembly_name.dll.config and is placed in the
        //       same directory as the assembly
        //       See http://www.mono-project.com/Interop_with_Native_Libraries#Library_Names
        private const string PCAP_DLL = "wpcap.dll";

		/// <summary>A delegate for Packet Arrival events</summary>
		public delegate void PacketArrivalEvent(object sender, Packets.Packet packet);

		/// <summary>
		/// A delegate for delivering network statistics
		/// </summary>
		public delegate void PcapStatisticsEvent(object sender, PcapStatistics statistics);

		/// <summary>
		/// A delegate fornotifying of a capture stopped event
		/// </summary>
		public delegate void PcapCaptureStoppedEvent(object sender, bool error);

		/// <summary>Represents the infinite number for packet captures </summary>
		public const int INFINITE = -1;

		/// <summary>A string value that prefixes avery pcap device name </summary>
		internal const string PCAP_NAME_PREFIX = @"\Device\NPF_";

		[DllImport("wpcap.dll", CharSet=CharSet.Auto)]
		private extern static int pcap_findalldevs(ref IntPtr /* pcap_if_t** */ alldevs, StringBuilder /* char* */ errbuf);

		/// <summary>Create a list of network devices that can be opened with pcap_open().</summary>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static int pcap_findalldevs_ex (string /*char **/source, IntPtr /*pcap_rmtauth **/auth, ref IntPtr /*pcap_if_t ** */alldevs, StringBuilder /*char * */errbuf);
		
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static void	pcap_freealldevs(IntPtr /* pcap_if_t * */ alldevs);

		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open_live(string dev, int packetLen, short mode, short timeout, StringBuilder errbuf);

		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open_offline( string/*const char* */ fname, StringBuilder/* char* */ errbuf ); 

		/// <summary>Open a file to write packets. </summary>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*pcap_dumper_t * */ pcap_dump_open (IntPtr /*pcap_t * */adaptHandle, string /*const char * */fname);
		
		/// <summary>
		///  Save a packet to disk.  
		/// </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static void  pcap_dump (IntPtr /*u_char * */user, IntPtr /*const struct pcap_pkthdr * */h, IntPtr /*const u_char * */sp) ;
		
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open(string dev, int packetLen, short mode, short timeout,IntPtr auth, StringBuilder errbuf);

		/// <summary> close the files associated with p and deallocates resources.</summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static void  pcap_close (IntPtr /*pcap_t **/adaptHandle) ;			 

		/// <summary>
		/// To avoid callback, this returns one packet at a time
		/// </summary>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static int pcap_next_ex(IntPtr /* pcap_t* */ adaptHandle, ref IntPtr /* **pkt_header */ header , ref IntPtr  data);

		/// <summary>
		/// Send a raw packet.<br>
		/// This function allows to send a raw packet to the network. 
		/// The MAC CRC doesn't need to be included, because it is transparently calculated
		///  and added by the network interface driver.	/// </summary>
		/// <param name="adaptHandle">the interface that will be used to send the packet</param>
		/// <param name="data">contains the data of the packet to send (including the various protocol headers)</param>
		/// <param name="size">the dimension of the buffer pointed by data</param>
		/// <returns>0 if the packet is succesfully sent, -1 otherwise.</returns>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_sendpacket(IntPtr /* pcap_t* */ adaptHandle, IntPtr  data, int size);

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
		/// <param name="queue">A pointer to the queue start address</queue>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static void pcap_sendqueue_destroy(IntPtr /* pcap_send_queue * */queue) ;


		/// <summary>
		/// Add a packet to a send queue. 
		/// </summary>
		/// <param name="queue">A pointer to a queue</param>
		/// <param name="header">The pcap header of the packet to send</param>
		/// <param name="data">The packet data</param>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
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

		/// <summary>
		/// Compile a packet filter, converting an high level filtering expression (see Filtering expression syntax) in a program that can be interpreted by the kernel-level filtering engine. 
		/// </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_compile (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp, string /*char * */str, int optimize, UInt32 netmask);

		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static int  pcap_setfilter (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp);

		/// <summary>
		/// return the error text pertaining to the last pcap library error.
		/// </summary>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static IntPtr pcap_geterr (IntPtr /*pcap_t * */ adaptHandle);

		/// <summary>Returns a pointer to a string giving information about the version of the libpcap library being used; note that it contains more information than just a version number. </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static string /*const char **/  pcap_lib_version ();
		
		/// <summary>return the standard I/O stream of the 'savefile' opened by pcap_dump_open().</summary>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*FILE **/  pcap_dump_file (IntPtr /*pcap_dumper_t **/p);
		
		/// <summary>Flushes the output buffer to the 'savefile', so that any packets 
		/// written with pcap_dump() but not yet written to the 'savefile' will be written. 
		/// -1 is returned on error, 0 on success. </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_dump_flush (IntPtr /*pcap_dumper_t **/p);
			
		/// <summary>Closes a savefile. </summary>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static void  pcap_dump_close (IntPtr /*pcap_dumper_t **/p);
				
		/// <summary> Return the link layer of an adapter. </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_datalink(IntPtr /* pcap_t* */ adaptHandle);

		/// <summary>
		/// Set the working mode of the interface p to mode. 
		///	Valid values for mode are MODE_CAPT (default capture mode) 
		///	and MODE_STAT (statistical mode). See the tutorial 
		///	"\ref wpcap_tut9" for details about statistical mode. 
		/// </summary>
		[DllImport(PCAP_DLL, CharSet=CharSet.Ansi)]
		internal extern static int pcap_setmode  ( IntPtr/* pcap_t * */ p, int  mode );

		/* interface is loopback */
		internal const uint		PCAP_IF_LOOPBACK				= 0x00000001;
		private  const int		MAX_ADAPTER_NAME_LENGTH			= 256;		
		private  const int		MAX_ADAPTER_DESCRIPTION_LENGTH	= 128;
		internal const int		MAX_PACKET_SIZE					= 65536;
		internal const int		PCAP_ERRBUF_SIZE				= 256;
		internal const int		MODE_CAPT						=	0;
		internal const int		MODE_STAT						=	1;
		internal const string	PCAP_SRC_IF_STRING				= "rpcap://";

		// Constants for address families
		// These are set in a Pcap static initializer because the values
		// differ between Windows and Linux
		private static int      AF_INET;
        private static int      AF_PACKET;
		private static int      AF_INET6;

		#region Callback Implementation ( Not Working from some reason, Bug?)

		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		private extern static int pcap_loop(IntPtr /* pcap_t* */ adaptHandle, short count, PcapHandler callback, IntPtr ptr);

		/// <summary>
		/// From some reason the Callback inplementation doesn't work.
		/// It fires for one time and then throws null pointer exception
		/// </summary>
		private delegate void PcapHandler(IntPtr param, IntPtr /* pcap_pkthdr* */ header, IntPtr pkt_data);

		private void MyPacketHandler(IntPtr param, IntPtr /* pcap_pkthdr* */ header, IntPtr pkt_data)
		{
			DateTime tm;
			
			if (header != IntPtr.Zero)
			{
				pcap_pkthdr PktInfo =
                    (pcap_pkthdr)Marshal.PtrToStructure( header,
                                                                                typeof(pcap_pkthdr) );
				/* convert the timestamp to readable format */
				tm = new DateTime( (long)(PktInfo.ts.tv_usec) );				
			
				Console.WriteLine("{0}, len: {1}", tm.ToShortTimeString(), PktInfo.len);
			}
		}

		#endregion Callback Implementation (Not Working)

		// managed version of pcap_addr
		public class PcapAddress
		{
			public System.Net.IPAddress Addr;      /* address */
			public System.Net.IPAddress Netmask;   /* netmask for that address */
			public System.Net.IPAddress Broadaddr; /* broadcast address for that address */
			public System.Net.IPAddress Dstaddr;   /* P2P destination address for that address */

			public PcapAddress(pcap_addr pcap_addr)
			{
				if(pcap_addr.Addr != IntPtr.Zero)
					Addr = Pcap.GetPcapAddress( pcap_addr.Addr );
				if(pcap_addr.Netmask != IntPtr.Zero)
					Netmask = Pcap.GetPcapAddress( pcap_addr.Netmask );
				if(pcap_addr.Broadaddr !=IntPtr.Zero)
					Broadaddr = Pcap.GetPcapAddress( pcap_addr.Broadaddr );
				if(pcap_addr.Dstaddr != IntPtr.Zero)
					Dstaddr = Pcap.GetPcapAddress( pcap_addr.Dstaddr );
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();

				if(Addr != null)
					sb.AppendFormat("Addr:      {0}\n", Addr.ToString());

				if(Netmask != null)
					sb.AppendFormat("Netmask:   {0}\n", Netmask.ToString());

				if(Broadaddr != null)
					sb.AppendFormat("Broadaddr: {0}\n", Broadaddr.ToString());

				if(Dstaddr != null)
					sb.AppendFormat("Dstaddr:   {0}\n", Dstaddr.ToString());

				return sb.ToString();
			}
		}

		// managed version of pcap_if
		// NOTE: we can't use pcap_if directly because the class contains
		//       a pointer to pcap_if that will be freed when the
		//       device memory is freed, so instead convert the unmanaged structure
		//       to a managed one to avoid this issue
		public class PcapInterface
		{
			public string            Name;        /* name to hand to "pcap_open_live()" */				
			public string            Description; /* textual description of interface */
			public List<PcapAddress> Addresses;
			public uint              Flags;       /* PCAP_IF_ interface flags */

			public PcapInterface(pcap_if pcapIf)
			{
				Name = pcapIf.Name;
				Description = pcapIf.Description;
				Flags = pcapIf.Flags;

				// retrieve addresses
				Addresses = new List<PcapAddress>();
				IntPtr address = pcapIf.Addresses;
				while(address != IntPtr.Zero)
				{
					//A sockaddr struct
					pcap_addr pcap_addr;

					//Marshal memory pointer into a struct
					pcap_addr = (pcap_addr)Marshal.PtrToStructure(address, typeof(pcap_addr));

					Addresses.Add(new PcapAddress(pcap_addr));

					address = pcap_addr.Next; // move to the next address
				}
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("Name: {0}\n", Name);
				sb.AppendFormat("Description: {0}\n", Description);
				foreach(PcapAddress addr in Addresses)
				{
					sb.AppendFormat("Addresses:\n{0}\n", addr);
				}
				sb.AppendFormat("Flags: {0}\n", Flags);
				return sb.ToString();
			}
		}

		public static string Version
		{
			get
			{
				try
				{
					return pcap_lib_version();
				}
				catch
				{
					return "pcap version can't be identified, you are either using "+
						"an older version, or pcap is not installed.";
				}
			}
		}

		private static bool isUnix()
		{
			int p = (int) Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
			{
                return true;
            } else {
                return false;
            }
		}

		static Pcap()
		{
            // happens to have the same value on Windows and Linux
            AF_INET = 2;

            // AF_PACKET = 17 on Linux, AF_NETBIOS = 17 on Windows
            // FIXME: need to resolve the discrepency at some point
            AF_PACKET = 17;

            if(isUnix())
            {
                AF_INET6 = 10; // value for linux from socket.h
            } else
            {
                AF_INET6 = 23; // value for windows from winsock.h
            }
		}

		/// <summary>
		/// Returns all pcap network devices available on this machine.
		/// </summary>
		public static List<PcapDevice> GetAllDevices()
		{
			IntPtr ptrDevs = IntPtr.Zero; // pointer to a PCAP_IF struct
			IntPtr next = IntPtr.Zero;    // pointer to a PCAP_IF struct
			StringBuilder errbuf = new StringBuilder( 256 ); //will hold errors
			List<PcapDevice> deviceList = new List<PcapDevice>();

			/* Retrieve the device list */
			int res = pcap_findalldevs(ref ptrDevs, errbuf);
			if (res == -1)
			{
				string err = "Error in WinPcap.GetAllDevices(): " + errbuf;
				throw new Exception( err );
			}
			else
			{	// Go through device structs and add to list
				next = ptrDevs;
				while (next != IntPtr.Zero)
				{
                    //Marshal memory pointer into a struct
                    pcap_if pcap_if_unmanaged =
                        (pcap_if)Marshal.PtrToStructure(next,
                                                        typeof(pcap_if));
					PcapInterface pcap_if = new PcapInterface(pcap_if_unmanaged);
					deviceList.Add(new PcapDevice(pcap_if));
					next = pcap_if_unmanaged.Next;
				}
			}
			pcap_freealldevs( ptrDevs );  // free buffers
			return deviceList;
		}

		/// <summary>
		/// Returns a PCAP_IF struct representing a pcap network device
		/// </summary>
		/// <param name="pcapName">The name of a device.<br>
		/// Can be either in pcap device format or windows network
		/// device format</param>
		/// <returns></returns>
		internal static PcapDevice GetPcapDeviceStruct(string pcapName)
		{
			if( !pcapName.StartsWith( PCAP_NAME_PREFIX ) )
			{
				pcapName = PCAP_NAME_PREFIX+pcapName;
			}

			List<PcapDevice> devices = GetAllDevices();
			foreach(PcapDevice d in devices)
			{
				if(d.PcapName.Equals(pcapName))
				{
					return d;
				}
			}

			throw new Exception("Device not found: "+pcapName);
		}

        internal static System.Net.IPAddress GetPcapAddress(IntPtr sockaddrPtr)
        {
            System.Net.IPAddress address;

            // A sockaddr struct. We use this to determine the address family
            sockaddr saddr;

            // Marshal memory pointer into a struct
            saddr = (sockaddr)Marshal.PtrToStructure(sockaddrPtr,
                                                     typeof(sockaddr));
            byte[] addressBytes;
            if((saddr.sa_family == AF_INET) || (saddr.sa_family == AF_PACKET))
            {
                sockaddr_in saddr_in = (sockaddr_in)Marshal.PtrToStructure(sockaddrPtr,
                                                                           typeof(sockaddr_in));
                address = new System.Net.IPAddress(saddr_in.sin_addr.s_addr);
            } else if(saddr.sa_family == AF_INET6)
            {
                addressBytes = new byte[16];
                sockaddr_in6 sin6 =
                    (sockaddr_in6)Marshal.PtrToStructure(sockaddrPtr,
                                                         typeof(sockaddr_in6));
                Array.Copy(sin6.sin6_addr, addressBytes, addressBytes.Length);
                address = new System.Net.IPAddress(addressBytes);
            } else
            {
                throw new System.NotImplementedException("sa_family of " + saddr.sa_family + " not supported");
            }

            return address;
        }
		
		public static PcapOfflineDevice GetPcapOfflineDevice(string pcapFileName)
		{
			return new PcapOfflineDevice( pcapFileName );
		}

		public static PcapDevice GetPcapDevice( string pcapDeviceName )
		{
			return GetPcapDeviceStruct(pcapDeviceName);
		}
	}
}
