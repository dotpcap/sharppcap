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
using System.Net;
using System.Collections.Generic;
using SharpPcap.Util;

namespace SharpPcap.Win32
{
	/// <summary>
	/// Represents a physical Network Device on a Windows machine.
	/// </summary>
	public class NetworkDevice : PcapDevice
	{
		private List<IPAddress> m_ipAddressList;
		private List<IPAddress> m_ipMaskList;
        private List<String> m_gatewaysList;
		private IPHelper.IP_ADAPTER_INFO m_adapterInfo;


		/// <summary>
		/// Constructs a new Network Device based on a pcapIf.
		/// </summary>
		internal NetworkDevice(Pcap.PcapInterface pcapIf) : base(pcapIf)
		{
			Setup();
		}

		/// <summary>
		/// Constructs a new Network Device based on a IP_ADAPTER_INFO struct.
		/// </summary>
		internal NetworkDevice(IPHelper.IP_ADAPTER_INFO adapterInfo) :
            base(new PcapDeviceList()[adapterInfo.AdapterName].Interface)
		{
            SetAdapterInfo(adapterInfo);
		}

		/// <summary>
		/// The name of this Network Device
		/// </summary>
		public string Name
		{
			get{return m_adapterInfo.AdapterName;}
		}

		/// <summary>
		/// The Description of this Network Device
		/// </summary>
		public string Description
		{
			get{return m_adapterInfo.Description;}
		}

		/// <summary>
		/// Gets the device index of this network device
		/// </summary>
		public int Index
		{
			get{return m_adapterInfo.Index;}
		}

		/// <summary>
		/// Gets a hex string representing the MAC (physical) Address of this 
		/// Network Device.
		/// </summary>
		public string MacAddress
		{
			get{return Util.Convert.BytesToHex( m_adapterInfo.Address, 0, m_adapterInfo.AddressLength );}
		}

		/// <summary>
		/// Gets the MAC Address of this network device as a byte array.
		/// </summary>
		public byte[] MacAddressBytes
		{
			get
			{
				byte[] mac = new byte[m_adapterInfo.AddressLength];
				Array.Copy(m_adapterInfo.Address, 0, mac, 0, mac.Length);
				return mac;
			}
		}

		/// <summary>
		/// Gets the main IP address of this network device
		/// </summary>
		public System.Net.IPAddress IpAddress
		{
			get
			{
				if(IpAddressList==null||IpAddressList.Count==0)
					return null;
                return IpAddressList[0];
			}
		}

		/// <summary>
		/// Gets the subnet mask of the main IP address of this network device
		/// </summary>
		public IPAddress SubnetMask
		{
			get
			{
				if(IpMaskList==null||IpMaskList.Count==0)
					return null;
				return IpMaskList[0];
			}
		}

		/// <summary>
		/// Gets a list of all IP addresses and subnet masks of this network device
		/// </summary>
		public List<System.Net.IPAddress> IpAddressList
		{
			get{return m_ipAddressList;}
		}

		/// <summary>
		/// Gets a list of all IP subnet masks of this network device
		/// </summary>
		public List<System.Net.IPAddress> IpMaskList
		{
			get{return m_ipMaskList;}
		}


		/// <summary>
		/// Gets the primary default gateway of this network device
		/// </summary>
		public string DefaultGateway
		{
			get
			{
				if(DefaultGatewayList==null||DefaultGatewayList.Count==0)
					return null;
				return m_gatewaysList[0];
			}
		}

		/// <summary>
		/// Gets a list of all default gateways on this network device
		/// </summary>
		public List<string> DefaultGatewayList
		{
			get{return m_gatewaysList;}
		}

		/// <summary>
		/// Gets a status indicating whether DHCP is enabled on this network device
		/// </summary>
		public bool DhcpEnabled
		{
			get{return m_adapterInfo.DhcpEnabled==1;}
		}

		/// <summary>
		/// Gets the IP Address of the DHCP Server of this network device
		/// </summary>
		public string DhcpServer
		{
			get{return m_adapterInfo.DhcpServer.IpAddress.address;}
		}

		/// <summary>
		/// Gets the date/time of the DHCP lease
		/// </summary>
		public DateTime DhcpLeaseObtained
		{
			get{return Util.Convert.Time_T2DateTime(m_adapterInfo.LeaseObtained);}
		}

		/// <summary>
		/// Gets the date/time of the DHCP expiration
		/// </summary>
		public DateTime DhcpLeaseExpires
		{
			get{return Util.Convert.Time_T2DateTime(m_adapterInfo.LeaseExpires);}
		}

		/// <summary>
		/// Gets the primary WINS Server configured for this network device
		/// </summary>
		public string WinsServerPrimary
		{
			get{return m_adapterInfo.PrimaryWinsServer.IpAddress.address;}
		}

		/// <summary>
		/// Gets the secondary WINS Server configured for this network device
		/// </summary>
		public string WinsServerSecondary
		{
			get{return m_adapterInfo.SecondaryWinsServer.IpAddress.address;}
		}

		public bool AdminStatus
		{
			get
			{
				return IPHelper.GetAdminStatus(this.Index);
			}
			set
			{
				IPHelper.SetAdminStatus(this.Index, value);
			}
		}

		public bool MediaState
		{
			get
			{
				return IPHelper.GetOperStatus(this.Index);
			}
		}

		/// <summary>
		/// Returns a string representaion of this Network Device
		/// </summary>
		public override string ToString()
		{
			return (this.Name+" ["+this.Description+"]");
		}

		/// <summary>
		/// Returns a detailed string representaion of this Network Device
		/// </summary>
		public string ToStringDetailed()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(this.ToString());sb.Append("\n");
			sb.Append("-------------------------------");sb.Append("\n");
			sb.Append("Index: "+this.Index);sb.Append("\n");
			sb.Append("Pcap Name: "+this.Name);sb.Append("\n");
			sb.Append("Pcap Description: "+this.Description);sb.Append("\n");
			sb.Append("IP Address: "+this.IpAddress);sb.Append("\n");
			sb.Append("Subnet Mask: "+this.SubnetMask);sb.Append("\n");
			sb.Append("Default Gateway: "+this.DefaultGateway);sb.Append("\n");
			sb.Append("MAC Address: "+this.MacAddress);sb.Append("\n");
			sb.Append("DHCP Enabled: "+this.DhcpEnabled);sb.Append("\n");
			sb.Append("DHCP Server: "+this.DhcpServer);sb.Append("\n");
			sb.Append("Primary WINS Server: "+this.WinsServerPrimary);sb.Append("\n");
			sb.Append("Secondary WINS Server: "+this.WinsServerSecondary);sb.Append("\n");
			if(this.DhcpEnabled)
			{
				sb.Append("Lease Obtained: "+this.DhcpLeaseObtained);sb.Append("\n");
				sb.Append("Lease Expires: "+this.DhcpLeaseExpires);sb.Append("\n");
			}
			sb.Append("Admin Status: "+this.AdminStatus);sb.Append("\n");
			sb.Append("Media State: "+this.MediaState);
			return sb.ToString();
		}

		/***********************************/
		/***       Private Methods	       */
		/***********************************/


		private void Setup()
		{
			try
			{
				SetAdapterInfo( IPHelper.GetAdapterInfo( FromPcapName(Name) ) );
			}
			catch(IPHelper_DeviceDoesntExistsException ddee)
			{
				if(Name!=null)
				{
					m_adapterInfo.AdapterName = Name;
					m_adapterInfo.Description = "This is a pcap emulated device. Only pcap operations allowed. Inappropriate properties will hold a 'null' value.";
				}
				else
					throw ddee;
			}
			catch(Exception e)
			{
				throw e;
			}
		}

		private void SetAdapterInfo(IPHelper.IP_ADAPTER_INFO adapterInfo)
		{
			m_adapterInfo = adapterInfo;
			GetIpAddressList( adapterInfo.IpAddressList,
                              out m_ipAddressList,
                              out m_ipMaskList);
			m_gatewaysList = GetIpGateways( adapterInfo.GatewayList );
		}

		private void GetIpAddressList(IPHelper.IP_ADDR_STRING addr,
                                      out List<IPAddress> address,
                                      out List<IPAddress> mask)
		{
            address = new List<IPAddress>();
            mask = new List<IPAddress>();

            address.Add(IPAddress.Parse(addr.IpAddress.address));
            mask.Add(IPAddress.Parse(addr.IpMask.address));
			while(addr.Next != 0)
			{
				addr = (IPHelper.IP_ADDR_STRING)Marshal.PtrToStructure((IntPtr)addr.Next,typeof(IPHelper.IP_ADDR_STRING));
                address.Add(IPAddress.Parse(addr.IpAddress.address));
                mask.Add(IPAddress.Parse(addr.IpMask.address));
			}
		}

		private List<string> GetIpGateways(IPHelper.IP_ADDR_STRING addr)
		{
			
			List<string> result = new List<string>();
			result.Add(addr.IpAddress.address);
			while(addr.Next != 0)
			{
				addr = (IPHelper.IP_ADDR_STRING)Marshal.PtrToStructure((IntPtr)addr.Next,typeof(IPHelper.IP_ADDR_STRING));
				result.Add( addr.IpAddress.address );
			}
			return result;
		}

		private string FromPcapName( string pcapName )
		{
			System.Text.RegularExpressions.Regex regex =
				new System.Text.RegularExpressions.Regex("{.*}");
			System.Text.RegularExpressions.Match match = 
				regex.Match( pcapName );

			if(match.Success)
			{
				return match.Value;
			}
			return pcapName;
		}
		
		internal static bool IsNetworkDevice( string deviceName )
		{
			System.Text.RegularExpressions.Regex regex =
				new System.Text.RegularExpressions.Regex("{.*}");
			System.Text.RegularExpressions.Match match = 
				regex.Match( deviceName );
			return match.Success;
		}
	}
}
