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
using System.Runtime.InteropServices;

namespace Tamir.IPLib
{
	/// <summary>
	/// Summary description for IPHelper.
	/// </summary>
	public class IPHelper
	{
		private const	int		MAX_ADAPTER_NAME				= 128;
		private const	int		MAX_ADAPTER_NAME_LENGTH			= 256;
		private const	int		MAX_ADAPTER_DESCRIPTION_LENGTH	= 128;
		private const	int		MAX_ADAPTER_ADDRESS_LENGTH		= 8;
		private const	int		ERROR_OK						= 0;
		private static	uint	ERROR_BUFFER_OVERFLOW			= (uint)111;

		public const int MIB_IF_OPER_STATUS_NON_OPERATIONAL = 0 ;
		public const int MIB_IF_OPER_STATUS_UNREACHABLE     = 1;
		public const int MIB_IF_OPER_STATUS_DISCONNECTED    = 2;
		public const int MIB_IF_OPER_STATUS_CONNECTING      = 3;
		public const int MIB_IF_OPER_STATUS_CONNECTED       = 4;
		public const int MIB_IF_OPER_STATUS_OPERATIONAL     = 5;

		public const int MIB_IF_TYPE_OTHER                  = 1;
		public const int MIB_IF_TYPE_ETHERNET               = 6;
		public const int MIB_IF_TYPE_TOKENRING              = 9;
		public const int MIB_IF_TYPE_FDDI                   = 15;
		public const int MIB_IF_TYPE_PPP                    = 23;
		public const int MIB_IF_TYPE_LOOPBACK               = 24;
		public const int MIB_IF_TYPE_SLIP                   = 28;

		public const int MIB_IF_ADMIN_STATUS_UP             = 1;
		public const int MIB_IF_ADMIN_STATUS_DOWN           = 2;
		public const int MIB_IF_ADMIN_STATUS_TESTING        = 3;

		#region Structs
			
		/// <summary>
		/// Represent a row in an INTF_TABLE (Interfaces Table)
		/// </summary>
		[ComVisible(false),StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
			internal struct INTF_ROW
		{
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=256)]
			internal String		Name;
			internal uint		Index;
			internal uint		Type;
			internal uint		Mtu;
			internal uint		Speed;
			internal uint		PhysAddrLen;//MAC Address length

			[MarshalAs(UnmanagedType.ByValArray,SizeConst=MAX_ADAPTER_ADDRESS_LENGTH)]
			internal byte[]     PhysAddr;//The MAC address
			internal uint		AdminStatus;
			internal uint		OperStatus;
			internal uint		LastChange;
			internal uint		InOctets;
			internal uint		dwInUcastPkts ;
			internal uint		dwInNUcastPkts ;
			internal uint		dwInDiscards;
			internal uint		dwInErrors ;
			internal uint		dwInUnknownProtos ;
			internal uint		dwOutOctets;
			internal uint		dwOutUcastPkts;
			internal uint		dwOutNUcastPkts ;
			internal uint		dwOutDiscards;
			internal uint		dwOutErrors ;
			internal uint		dwOutQLen;
			internal uint		dwDescrLen ;

			[MarshalAs(UnmanagedType.ByValArray,SizeConst=256)]
			internal byte[]     bDescr;//The description of this interface
		}

		[ComVisible(false),StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)]
			internal struct IP_ADAPTER_INDEX_MAP
		{
			internal uint		Index;
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=128)]
			internal String		Name;
		}


		[ComVisible(false),StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
			internal struct IP_ADAPTER_INFO 
		{
			internal int /* struct _IP_ADAPTER_INFO* */ Next;
			internal uint           ComboIndex;

			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=MAX_ADAPTER_NAME_LENGTH + 4)]
			internal String         AdapterName;

			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=MAX_ADAPTER_DESCRIPTION_LENGTH + 4)]
			internal String         Description;
			internal int           AddressLength;

			[MarshalAs(UnmanagedType.ByValArray,SizeConst=MAX_ADAPTER_ADDRESS_LENGTH)]
			internal byte[]         Address;
			internal int           Index;
			internal int           Type;
			internal int           DhcpEnabled;
			public uint           CurrentIpAddress; /* IP_ADDR_STRING* */
			internal IP_ADDR_STRING IpAddressList;
			internal IP_ADDR_STRING GatewayList;
			internal IP_ADDR_STRING DhcpServer;
			[MarshalAs(UnmanagedType.Bool)]
			internal bool           HaveWins;
			internal IP_ADDR_STRING PrimaryWinsServer;
			internal IP_ADDR_STRING SecondaryWinsServer;
			internal uint/*time_t*/ LeaseObtained;
			internal uint/*time_t*/ LeaseExpires;
		};

		/// <summary>
		/// MAC_ADDR_ARR struct for "iphlpapi.dll" invocation.
		/// </summary>
		[ComVisible(false),StructLayout(LayoutKind.Sequential)]
			internal struct MAC_ADDR_ARRAY 
		{
			[MarshalAs(UnmanagedType.ByValArray,SizeConst=6)]
			public byte[] bytes;
		};

		/// <summary>
		/// Represent a row in an IP address
		/// </summary>
		[ComVisible(false),StructLayout(LayoutKind.Sequential)]
			internal struct IPADDR
		{
			internal byte s_b1;
			internal byte s_b2;
			internal byte s_b3;
			internal byte s_b4;
		};

		/// <summary>
		/// Represent a row in an IP mask
		/// </summary>
		[ComVisible(false),StructLayout(LayoutKind.Sequential)]
			internal struct IPMASK
		{
			internal byte s_b1;
			internal byte s_b2;
			internal byte s_b3;
			internal byte s_b4;
		};

		[ComVisible(false),StructLayout(LayoutKind.Sequential)]
			internal struct  IPFORWARDROW 
		{
			internal int /*DWORD*/ dwForwardDest;  
			internal int /*DWORD*/ dwForwardMask;  
			internal int /*DWORD*/ dwForwardPolicy;  
			internal int /*DWORD*/ dwForwardNextHop;  
			internal int /*DWORD*/ dwForwardIfIndex;  
			internal int /*DWORD*/ dwForwardType;  
			internal int /*DWORD*/ dwForwardProto;  
			internal int /*DWORD*/ dwForwardAge;  
			internal int /*DWORD*/ dwForwardNextHopAS;  
			internal int /*DWORD*/ dwForwardMetric1;  
			internal int /*DWORD*/ dwForwardMetric2;  
			internal int /*DWORD*/ dwForwardMetric3;  
			internal int /*DWORD*/ dwForwardMetric4;  
			internal int /*DWORD*/ dwForwardMetric5;
		};

		[ComVisible(false),StructLayout(LayoutKind.Sequential)]
			internal struct IPFORWARDTABLE 
		{
			uint /*DWORD*/						dwNumEntries;  
			uint /*struct MIB_IPFORWARDROW*/	table;
		};

		/// <summary>
		/// IP_ADDRESS_STRING struct for "iphlpapi.dll" invocation.
		/// </summary>
		[ComVisible(false),StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
			internal struct IP_ADDRESS_STRING 
		{
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=16)]
			public string address;
		};

		/// <summary>
		/// IP_MASK_STRING struct for "iphlpapi.dll" invocation.
		/// </summary>
		[ComVisible(false),StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
			internal struct IP_MASK_STRING 
		{
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=16)]
			public string address;
		};

		/// <summary>
		/// IP_ADDR_STRING struct for "iphlpapi.dll" invocation.
		/// </summary>
		[ComVisible(false),StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
			internal struct IP_ADDR_STRING 
		{
			public int Next;      /* struct _IP_ADDR_STRING* */
			public IP_ADDRESS_STRING IpAddress;
			public IP_MASK_STRING IpMask;
			public uint Context;
		};


		#endregion Structs

		#region iphlpapi.dll external function

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static uint GetAdaptersInfo(IntPtr pAdapterInfo,ref int pOutBufLen);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int GetInterfaceInfo(IntPtr pIfTable,ref	int pOutBufLen);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static uint IpReleaseAddress(IntPtr AdapterInfo);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static uint IpRenewAddress(IntPtr AdapterInfo);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int GetIfTable(IntPtr pIfTable,ref int pdwSize,bool bOrder);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int GetIfEntry(IntPtr pIfRow);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int SetIfEntry(IntPtr /*PMIB_IFROW*/ pIfRow);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int SendARP(int DestIP,int SrcIP,IntPtr pMacAddr,ref int PhyAddrLen);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int AddIPAddress(IPADDR Address, IPMASK IpMask,	uint IfIndex, out ulong NTEContext, out ulong NTEInstance);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int DeleteIPAddress (ulong NTEContext);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int GetIpForwardTable(IntPtr /*PMIB_IPFORWARDTABLE*/ pIpForwardTable,ref int /*PULONG*/ pdwSize,bool bOrder);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int DeleteIpForwardEntry(IntPtr /*PMIB_IPFORWARDROW*/ pRoute);

		[DllImport("iphlpapi", CharSet=CharSet.Auto)]
		private extern static int CreateIpForwardEntry(IntPtr /*PMIB_IPFORWARDROW*/ pRoute);

		#endregion iphlpapi.dll external function


		internal static bool GetIfTableRow(uint index, ref INTF_ROW row)
		{
			bool res = false;
			IntPtr pBuf = IntPtr.Zero;  //A pointer to an unmanaged buffer that
			//will be filled by the intf Table.

			int nBufSize = 0;			//The unmanaged buffer size

			//Getting the required buffer size for 'GetIfTable' function
			GetIfTable( IntPtr.Zero, ref nBufSize, false);
			//Allocating mem for the required buffer size
			pBuf = Marshal.AllocHGlobal( nBufSize );
			//Filling the buffer with 'GetIfTable' data
			int r = GetIfTable( pBuf, ref nBufSize, false);	
			if ( r != 0 )
				throw new System.ComponentModel.Win32Exception( r );

			//size is the number of entries in the intf Table
			int size =Marshal.ReadInt32( pBuf );
			//A pointer to the first 'INTF_ROW' in the array
			IntPtr row_ptr = unchecked((IntPtr)((int)pBuf + 4));
			while ( size-- > 0 ) 
			{
				//importing the unmanaged 'INTF_ROW' into 'row'
				row = (INTF_ROW)Marshal.PtrToStructure(row_ptr, typeof(INTF_ROW));
				//If this is the required interface
				//MessageBox.Show(row.Name);
				if (row.Index == index)
				{
					res = true;
				}
				//Updating the pointer to the next 'IP_ADAPTER_INDEX_MAP'
				row_ptr = unchecked((IntPtr)((int)row_ptr + 
					Marshal.SizeOf(typeof(INTF_ROW))));
			}
			Marshal.FreeHGlobal( pBuf );
			return res;
		}

		internal static bool GetIfTableRow(string ifName, ref INTF_ROW row)
		{
			bool res = false;
			IntPtr pBuf = IntPtr.Zero;  //A pointer to an unmanaged buffer that
			//will be filled by the intf Table.

			int nBufSize = 0;			//The unmanaged buffer size

			//Getting the required buffer size for 'GetIfTable' function
			GetIfTable( IntPtr.Zero, ref nBufSize, false);
			//Allocating mem for the required buffer size
			pBuf = Marshal.AllocHGlobal( nBufSize );
			//Filling the buffer with 'GetIfTable' data
			int r = GetIfTable( pBuf, ref nBufSize, false);	
			if ( r != 0 )
				throw new System.ComponentModel.Win32Exception( r );

			//size is the number of entries in the intf Table
			int size =Marshal.ReadInt32( pBuf );
			//A pointer to the first 'INTF_ROW' in the array
			IntPtr row_ptr = unchecked((IntPtr)((int)pBuf + 4));
			while ( size-- > 0 ) 
			{
				//importing the unmanaged 'INTF_ROW' into 'row'
				row = (INTF_ROW)Marshal.PtrToStructure(row_ptr, typeof(INTF_ROW));
				//If this is the required interface
				//MessageBox.Show(row.Name);
				if (ifName.EndsWith(row.Name))
				{
					res = true;
				}
				//Updating the pointer to the next 'IP_ADAPTER_INDEX_MAP'
				row_ptr = unchecked((IntPtr)((int)row_ptr + 
					Marshal.SizeOf(typeof(INTF_ROW))));
			}
			Marshal.FreeHGlobal( pBuf );
			return res;
		}

		public static bool SetAdminStatus(int ifIndex, bool up)
		{
			int status;

			if(up) status	=	MIB_IF_ADMIN_STATUS_UP;
			else status		=	MIB_IF_ADMIN_STATUS_DOWN;

			INTF_ROW ifRow = new INTF_ROW();
			ifRow.Index=(uint)ifIndex;
			ifRow.AdminStatus = (uint)status;

			IntPtr buf = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(INTF_ROW)));
			Marshal.StructureToPtr(ifRow, buf, true);			
			status=SetIfEntry(buf);
			Marshal.FreeHGlobal(buf);
			return status==ERROR_OK;
		}

		public static bool GetAdminStatus(int ifIndex)
		{
			int status = -1;
			INTF_ROW ifRow = new INTF_ROW();
			ifRow.Index=(uint)ifIndex;

			IntPtr buf = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(INTF_ROW)));
			Marshal.StructureToPtr(ifRow, buf, true);			

			if(GetIfEntry(buf)==ERROR_OK)
			{
				ifRow = (INTF_ROW)Marshal.PtrToStructure((IntPtr)buf,typeof(INTF_ROW));
				status = (int)ifRow.AdminStatus;
			}
			Marshal.FreeHGlobal(buf);
			return status==1;
		}

		public static bool GetOperStatus(int ifIndex)
		{
			int status = -1;
			INTF_ROW ifRow = new INTF_ROW();
			ifRow.Index=(uint)ifIndex;

			IntPtr buf = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(INTF_ROW)));
			Marshal.StructureToPtr(ifRow, buf, true);			

			if(GetIfEntry(buf)==ERROR_OK)
			{
				ifRow = (INTF_ROW)Marshal.PtrToStructure((IntPtr)buf,typeof(INTF_ROW));
				status = (int)ifRow.OperStatus;
			}
			Marshal.FreeHGlobal(buf);
			return status==MIB_IF_OPER_STATUS_OPERATIONAL;
		}

		/// <summary>
		/// Returns an IP_ADAPTER_INFO struct that represents a network adapter on this machine
		/// </summary>
		internal static IP_ADAPTER_INFO GetAdapterInfo(string adapterName) 
		{
			int size = Marshal.SizeOf(typeof(IP_ADAPTER_INFO));
			IntPtr buffer = Marshal.AllocHGlobal(size);
			uint result = GetAdaptersInfo(buffer,ref size);

			if (result == ERROR_BUFFER_OVERFLOW) 
			{				
				Marshal.FreeHGlobal(buffer);
				buffer = Marshal.AllocHGlobal(size);
				result = GetAdaptersInfo(buffer,ref size);
			}

			if (result == ERROR_OK) 
			{				
				int next=(int)buffer;
				IP_ADAPTER_INFO info;
				while (next != 0) 
				{
					info = (IP_ADAPTER_INFO)Marshal.PtrToStructure((IntPtr)next,typeof(IP_ADAPTER_INFO));
					next=info.Next;
					if(info.AdapterName==adapterName)
					{
						Marshal.FreeHGlobal(buffer);
						return info;
					}
				}
				Marshal.FreeHGlobal(buffer);
				throw new IPHelper_DeviceDoesntExistsException("GetAdaptersInfo failed: adapter doesn't exists: " +
					adapterName);
			} 
			else 
			{
				Marshal.FreeHGlobal(buffer);
				throw new InvalidOperationException("GetAdaptersInfo failed: " +
					result);
			}
		}

		/// <summary>
		/// Return all network devices available on this machine through the IPHelper API
		/// </summary>
		public static NetworkDeviceList GetAllDevices()
		{
			int size = Marshal.SizeOf(typeof(IP_ADAPTER_INFO));
			IntPtr buffer = Marshal.AllocHGlobal(size);
			uint result = GetAdaptersInfo(buffer,ref size);

			NetworkDeviceList deviceList = new NetworkDeviceList();

			if (result == ERROR_BUFFER_OVERFLOW) 
			{				
				Marshal.FreeHGlobal(buffer);
				buffer = Marshal.AllocHGlobal(size);
				result = GetAdaptersInfo(buffer,ref size);
			}

			if (result == ERROR_OK) 
			{				
				int next=(int)buffer;
				IP_ADAPTER_INFO info;
				while (next != 0) 
				{
					info = (IP_ADAPTER_INFO)Marshal.PtrToStructure((IntPtr)next,typeof(IP_ADAPTER_INFO));
					next=info.Next;
					deviceList.Add( new NetworkDevice(info) );
				}
				return deviceList;
			} 
			else 
			{
				Marshal.FreeHGlobal(buffer);
				throw new InvalidOperationException("GetAdaptersInfo failed: " +
					result);
			}
		}
	}

	public class IPHelper_DeviceDoesntExistsException : Exception 
	{
		public IPHelper_DeviceDoesntExistsException(string msg):base(msg)
		{
		}
	}
}
