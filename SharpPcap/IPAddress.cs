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
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR,
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections;

namespace SharpPcap
{
	/// <summary>
	///Represent an IP Address and a Subnet Mask.
	///This struct hold two parameters:
	///address -  string of the IP Address
	///mask - a string of the subnet mask
	/// </summary>
	public struct IPAddress
	{
		private string _address, _mask;
		internal IPAddress(string address, string mask)
		{
			this._address = address;
			this._mask = mask;
		}

		/// <summary>
		/// IP Address
		/// </summary>
		public string Address
		{
			get
			{
				return _address;
			}
		}

		/// <summary>
		/// Subnet Mask
		/// </summary>
		public string Mask
		{
			get
			{
				return _mask;
			}
		}

		public override string ToString()
		{
			return (Address+"/"+Util.Convert.MaskStringToBits(Mask));
		}
	}

	/// <summary>
	/// Summary description for IPAddressList.
	/// </summary>
	public class IPAddressList : CollectionBase
	{
		public IPAddress this[ int index ]  
		{
			get  
			{
				return( (IPAddress) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		public int Add( IPAddress value )  
		{
			return( List.Add( value ) );
		}

		public int IndexOf( IPAddress value )  
		{
			return( List.IndexOf( value ) );
		}

		public void Insert( int index, IPAddress value )  
		{
			List.Insert( index, value );
		}

		public void Remove( IPAddress value )  
		{
			List.Remove( value );
		}

		public bool Contains( IPAddress value )  
		{
			// If value is not of type IPAddress, this will return false.
			return( List.Contains( value ) );
		}

		public bool ContainsIp( string Ip )
		{
			foreach (IPAddress item in this)
			{
				if (item.Address == Ip)
					return true;
			}
			return false;
		}

		//		protected override void OnInsert( int index, Object value )  
		//		{
		//			// Insert additional code to be run only when inserting values.
		//		}
		//
		//		protected override void OnRemove( int index, Object value )  
		//		{
		//			// Insert additional code to be run only when removing values.
		//		}
		//
		//		protected override void OnSet( int index, Object oldValue, Object newValue )  
		//		{
		//			// Insert additional code to be run only when setting values.
		//		}

		protected override void OnValidate( Object value )  
		{
			if ( value.GetType() != Type.GetType("SharpPcap.IPAddress") )
				throw new ArgumentException( "value must be of type IPAddress.", "value" );
		}
	}
}
