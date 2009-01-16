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
using System.Collections;


namespace SharpPcap
{
	/// <summary>
	/// Summary description for PcapDeviceList.
	/// </summary>
	/// <author>Tamir Gal</author>
	/// <version>  $Revision: 1.3 $ </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-16 08:49:14 $ </lastModifiedAt>
	public class PcapDeviceList : CollectionBase
	{
		public PcapDevice this[ int index ]  
		{
			get  
			{
				return( (PcapDevice) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		public int Add( PcapDevice value )  
		{
			return( List.Add( value ) );
		}

		public int IndexOf( PcapDevice value )  
		{
			return( List.IndexOf( value ) );
		}

		public void Insert( int index, PcapDevice value )  
		{
			List.Insert( index, value );
		}

		public void Remove( PcapDevice value )  
		{
			List.Remove( value );
		}

		public bool Contains( PcapDevice value )  
		{
			// If value is not of type PcapDevice, this will return false.
			return( List.Contains( value ) );
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
			if ( !(value is PcapDevice) )
				throw new ArgumentException( "value must be of type PcapDevice.", "value" );
		}

		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int i=0; i<this.Count; i++)
			{
				sb.Append("Device "+i+": ");
				sb.Append( this[i].ToString() );
				if(i!=Count-1)
					sb.Append("\n\n\n");
			}
			return sb.ToString();
		}

	}
}
