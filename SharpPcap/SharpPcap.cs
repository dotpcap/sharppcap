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
using System.Runtime.InteropServices;

namespace Tamir.IPLib
{
	/// <summary>
	/// Summary description for SharpPcap.
	/// </summary>
	/// <author>Tamir Gal</author>
	/// <version>  $Revision: 1.2 $ </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-16 08:11:35 $ </lastModifiedAt>
	public class SharpPcap
	{
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
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_findalldevs_ex (string /*char **/source, IntPtr /*pcap_rmtauth **/auth, ref IntPtr /*pcap_if_t ** */alldevs, StringBuilder /*char * */errbuf);
		
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static void	pcap_freealldevs(IntPtr /* pcap_if_t * */ alldevs);

		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open_live(string dev, int packetLen, short mode, short timeout, StringBuilder errbuf);

		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open_offline( string/*const char* */ fname, StringBuilder/* char* */ errbuf ); 

		/// <summary>Open a file to write packets. </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*pcap_dumper_t * */ pcap_dump_open (IntPtr /*pcap_t * */adaptHandle, string /*const char * */fname);
		
		/// <summary>
		///  Save a packet to disk.  
		/// </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static void  pcap_dump (IntPtr /*u_char * */user, IntPtr /*const struct pcap_pkthdr * */h, IntPtr /*const u_char * */sp) ;
		
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open(string dev, int packetLen, short mode, short timeout,IntPtr auth, StringBuilder errbuf);

		/// <summary> close the files associated with p and deallocates resources.</summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static void  pcap_close (IntPtr /*pcap_t **/adaptHandle) ;			 

		/// <summary>
		/// To avoid callback, this returns one packet at a time
		/// </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
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
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*pcap_send_queue * */pcap_sendqueue_alloc(int memsize) ;

		/// <summary>
		/// Destroy a send queue. 
		/// </summary>
		/// <param name="queue">A pointer to the queue start address</queue>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
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
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_sendqueue_transmit(IntPtr/*pcap_t * */p, IntPtr /* pcap_send_queue * */queue, int sync);

		/// <summary>
		/// Compile a packet filter, converting an high level filtering expression (see Filtering expression syntax) in a program that can be interpreted by the kernel-level filtering engine. 
		/// </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_compile (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp, string /*char * */str, int optimize, UInt32 netmask);

		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int  pcap_setfilter (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp);

		/// <summary>
		/// return the error text pertaining to the last pcap library error.
		/// </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr pcap_geterr (IntPtr /*pcap_t * */ adaptHandle);

		/// <summary>Returns a pointer to a string giving information about the version of the libpcap library being used; note that it contains more information than just a version number. </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static string /*const char **/  pcap_lib_version ();
		
		/// <summary>return the standard I/O stream of the 'savefile' opened by pcap_dump_open().</summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*FILE **/  pcap_dump_file (IntPtr /*pcap_dumper_t **/p);
		
		/// <summary>Flushes the output buffer to the 'savefile', so that any packets 
		/// written with pcap_dump() but not yet written to the 'savefile' will be written. 
		/// -1 is returned on error, 0 on success. </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
		internal extern static int pcap_dump_flush (IntPtr /*pcap_dumper_t **/p);
			
		/// <summary>Closes a savefile. </summary>
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
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
		[DllImport("wpcap.dll", CharSet=CharSet.Ansi)]
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
				PCAP_PKTHDR PktInfo = (PCAP_PKTHDR)Marshal.PtrToStructure( header, typeof(PCAP_PKTHDR) );
				/* convert the timestamp to readable format */
				tm = new DateTime( (PktInfo.tv_usec) );				
			
				Console.WriteLine("{0}, len: {1}", tm.ToShortTimeString(), PktInfo.len);
			}
		}

		#endregion Callback Implementation (Not Working)

		#region Unmanaged Structs Implementation

		/// <summary>
		/// Item in a list of interfaces.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			internal struct PCAP_IF 
		{
			public IntPtr /* pcap_if* */	Next;			
			public string					Name;			/* name to hand to "pcap_open_live()" */				
			public string					Description;	/* textual description of interface, or NULL */
			public IntPtr /*pcap_addr * */	Addresses;
			public uint						Flags;			/* PCAP_IF_ interface flags */
		};

		/// <summary>
		/// Representation of an interface address.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			internal struct PCAP_ADDR 
		{
			public IntPtr /* pcap_addr* */	Next;
			public IntPtr /* sockaddr * */	Addr;		/* address */
			public IntPtr /* sockaddr * */  Netmask;	/* netmask for that address */
			public IntPtr /* sockaddr * */	Broadaddr;	/* broadcast address for that address */
			public IntPtr /* sockaddr * */	Dstaddr;	/* P2P destination address for that address */
		};

		/// <summary>
		/// Structure used by kernel to store most addresses.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			internal struct SOCKADDR 
		{
			public int			sa_family;       /* address family */
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=14)]
			public byte[]		sa_data;         /* up to 14 bytes of direct address */
		};


		/// <summary>
		/// Each packet in the dump file is prepended with this generic header.
		/// This gets around the problem of different headers for different
		/// packet interfaces.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			public struct PCAP_PKTHDR 
		{											//timestamp
			public int		tv_sec;				///< seconds
			public int		tv_usec;			///< microseconds
			public int		caplen;			/* length of portion present */
			public int		len;			/* length this packet (off wire) */
		};

		/// <summary>
		/// Packet data bytes
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			internal struct PCAP_PKTDATA
		{	
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=MAX_PACKET_SIZE)]						
			public byte[]		bytes;
		};

		/// <summary>
		/// A BPF pseudo-assembly program for packet filtering
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			internal struct bpf_program 
		{
			public uint bf_len;                
			public IntPtr /* bpf_insn **/ bf_insns;  
		};

		/// <summary>
		/// A queue of raw packets that will be sent to the network with pcap_sendqueue_transmit()
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
			internal struct pcap_send_queue 
		{
			public uint maxlen;   
            public uint len;   
			public IntPtr /* char **/ ptrBuff;  
		};


		#endregion Unmanaged Structs Implementation

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

		/// <summary>
		/// Returns all pcap network devices available on this machine.
		/// </summary>
		public static PcapDeviceList GetAllDevices()
		{
			IntPtr ptrDevs = IntPtr.Zero; // pointer to a PCAP_IF struct
			IntPtr next = IntPtr.Zero;    // pointer to a PCAP_IF struct
			PCAP_IF pcap_if;
			StringBuilder errbuf = new StringBuilder( 256 ); //will hold errors
			PcapDeviceList deviceList = new PcapDeviceList();

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
					pcap_if = (PCAP_IF)Marshal.PtrToStructure(next, typeof(PCAP_IF)); //Marshal memory pointer into a struct
					if(NetworkDevice.IsNetworkDevice( pcap_if.Name ))
					{
						try
						{
							deviceList.Add( new NetworkDevice(pcap_if) );
						}
						catch
						{
							deviceList.Add( new PcapDevice(pcap_if) );
						}
					}
					else
					{
						deviceList.Add( new PcapDevice(pcap_if) );
					}
					
					next = pcap_if.Next;
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
		internal static PCAP_IF GetPcapDeviceStruct(string pcapName)
		{
			if( !pcapName.StartsWith( PCAP_NAME_PREFIX ) )
			{
				pcapName = PCAP_NAME_PREFIX+pcapName;
			}
			IntPtr ptrDevs = IntPtr.Zero; // pointer to a PCAP_IF struct
			IntPtr next = IntPtr.Zero;    // pointer to a PCAP_IF struct
			PCAP_IF pcap_if;
			StringBuilder errbuf = new StringBuilder( 256 ); //will hold errors

			/* Retrieve the device list */
			int res = pcap_findalldevs(ref ptrDevs, errbuf);
			if (res == -1)
			{
				string err = "Error in SharpPcap.GetAllDevices(): " + errbuf;
				throw new Exception( err );
			}
			else
			{	// Go through device structs and search for 'pcapName'
				next = ptrDevs;
				while (next != IntPtr.Zero)
				{
					pcap_if = (PCAP_IF)Marshal.PtrToStructure(next, typeof(PCAP_IF)); //Marshal memory pointer into a struct
					if(pcap_if.Name==pcapName)
					{
						pcap_freealldevs( ptrDevs );  // free buffers
						return pcap_if;
					}
					next = pcap_if.Next;
				}
			}
			pcap_freealldevs( ptrDevs );  // free buffers
			throw new Exception("Device not found: "+pcapName);
		}

		internal static PCAP_ADDR GetPcap_Addr(IntPtr pcap_addrPtr)
		{
			//A sockaddr struct
			PCAP_ADDR pcap_addr;
			//Marshal memory pointer into a struct
			pcap_addr = (PCAP_ADDR)Marshal.PtrToStructure(pcap_addrPtr, typeof(PCAP_ADDR));
			return pcap_addr;		
		}

		internal static int GetPcapAddress(IntPtr sockaddrPtr)
		{
			//A sockaddr struct
			SOCKADDR sockaddr;
			//Marshal memory pointer into a struct
			sockaddr = (SOCKADDR)Marshal.PtrToStructure(sockaddrPtr, typeof(SOCKADDR));
			return BitConverter.ToInt32( sockaddr.sa_data, 0);
		}
		
		public static PcapOfflineDevice GetPcapOfflineDevice(string pcapFileName)
		{
			return new PcapOfflineDevice( pcapFileName );
		}

		public static PcapDevice GetPcapDevice( string pcapDeviceName )
		{
			return new PcapDevice( GetPcapDeviceStruct( pcapDeviceName ) );
		}
	}
}
