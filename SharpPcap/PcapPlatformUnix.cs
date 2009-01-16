using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Tamir.IPLib
{
	/// <summary>
	/// Summary description for PcapPlatformWin32.
	/// </summary>
	public class PcapPlatformUnix
	{
		[DllImport("libpcap.so", CharSet=CharSet.Auto)]
		internal extern static int pcap_findalldevs(ref IntPtr /* pcap_if_t** */ alldevs, StringBuilder /* char* */ errbuf);

		/// <summary>Create a list of network devices that can be opened with pcap_open().</summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int pcap_findalldevs_ex (string /*char **/source, IntPtr /*pcap_rmtauth **/auth, ref IntPtr /*pcap_if_t ** */alldevs, StringBuilder /*char * */errbuf);
	
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static void	pcap_freealldevs(IntPtr /* pcap_if_t * */ alldevs);

		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open_live(string dev, int packetLen, short mode, short timeout, StringBuilder errbuf);

		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open_offline( string/*const char* */ fname, StringBuilder/* char* */ errbuf ); 

		/// <summary>Open a file to write packets. </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*pcap_dumper_t * */ pcap_dump_open (IntPtr /*pcap_t * */adaptHandle, string /*const char * */fname);
	
		/// <summary>
		///  Save a packet to disk.  
		/// </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static void  pcap_dump (IntPtr /*u_char * */user, IntPtr /*const struct pcap_pkthdr * */h, IntPtr /*const u_char * */sp) ;
	
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /* pcap_t* */ pcap_open(string dev, int packetLen, short mode, short timeout,IntPtr auth, StringBuilder errbuf);

		/// <summary> close the files associated with p and deallocates resources.</summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static void  pcap_close (IntPtr /*pcap_t **/adaptHandle) ;			 

		/// <summary>
		/// To avoid callback, this returns one packet at a time
		/// </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
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
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int pcap_sendpacket(IntPtr /* pcap_t* */ adaptHandle, IntPtr  data, int size);

		/// <summary>
		/// Allocate a send queue. 
		/// </summary>
		/// <param name="memsize">The size of the queue</param>
		/// <returns>A pointer to the allocated buffer</returns>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*pcap_send_queue * */pcap_sendqueue_alloc(int memsize) ;

		/// <summary>
		/// Destroy a send queue. 
		/// </summary>
		/// <param name="queue">A pointer to the queue start address</queue>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static void pcap_sendqueue_destroy(IntPtr /* pcap_send_queue * */queue) ;


		/// <summary>
		/// Add a packet to a send queue. 
		/// </summary>
		/// <param name="queue">A pointer to a queue</param>
		/// <param name="header">The pcap header of the packet to send</param>
		/// <param name="data">The packet data</param>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
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
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int pcap_sendqueue_transmit(IntPtr/*pcap_t * */p, IntPtr /* pcap_send_queue * */queue, int sync);

		/// <summary>
		/// Compile a packet filter, converting an high level filtering expression (see Filtering expression syntax) in a program that can be interpreted by the kernel-level filtering engine. 
		/// </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int pcap_compile (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp, string /*char * */str, int optimize, UInt32 netmask);

		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int  pcap_setfilter (IntPtr /* pcap_t* */ adaptHandle, IntPtr /*bpf_program **/fp);

		/// <summary>
		/// return the error text pertaining to the last pcap library error.
		/// </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static IntPtr pcap_geterr (IntPtr /*pcap_t * */ adaptHandle);

		/// <summary>Returns a pointer to a string giving information about the version of the libpcap library being used; note that it contains more information than just a version number. </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static string /*const char **/  pcap_lib_version ();
	
		/// <summary>return the standard I/O stream of the 'savefile' opened by pcap_dump_open().</summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static IntPtr /*FILE **/  pcap_dump_file (IntPtr /*pcap_dumper_t **/p);
	
		/// <summary>Flushes the output buffer to the ``savefile,'' so that any packets 
		/// written with pcap_dump() but not yet written to the ``savefile'' will be written. 
		/// -1 is returned on error, 0 on success. </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int pcap_dump_flush (IntPtr /*pcap_dumper_t **/p);
		
		/// <summary>Closes a savefile. </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static void  pcap_dump_close (IntPtr /*pcap_dumper_t **/p);
			
		/// <summary> Return the link layer of an adapter. </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int pcap_datalink(IntPtr /* pcap_t* */ adaptHandle);

		/// <summary>
		/// Set the working mode of the interface p to mode. 
		///	Valid values for mode are MODE_CAPT (default capture mode) 
		///	and MODE_STAT (statistical mode). See the tutorial 
		///	"\ref wpcap_tut9" for details about statistical mode. 
		/// </summary>
		[DllImport("libpcap.so", CharSet=CharSet.Ansi)]
		internal extern static int pcap_setmode  ( IntPtr/* pcap_t * */ p, int  mode );
	}
	
}
