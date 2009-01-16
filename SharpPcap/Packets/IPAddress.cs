// $Id: IPAddress.cs,v 1.1.1.1 2007-07-03 10:15:17 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
//UPGRADE_TODO: The type 'Tamir.IPLib.Packets.Util.ArrayHelper' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using ArrayHelper = Tamir.IPLib.Packets.Util.ArrayHelper;
namespace Tamir.IPLib.Packets
{
	
	
	/// <summary> IP address.
	/// <p>
	/// This class doesn't store IP addresses. There's a java class for that,
	/// and it is too big and cumbersome for our purposes.
	/// <p>
	/// This class contains a utility method for extracting an IP address 
	/// from a big-endian byte array.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:17 $ </lastModifiedAt>
	public class IPAddress
	{
		/// <summary> Convert an IP address stored in an int to its string representation.</summary>
		private static System.String toString(int address)
		{
			System.Text.StringBuilder sa = new System.Text.StringBuilder();
			for (int i = 0; i < WIDTH; i++)
			{
				sa.Append(0xff & address >> 24);
				address <<= 8;
				if (i != WIDTH - 1)
					sa.Append('.');
			}
			return sa.ToString();
		}
		
		/// <summary> Extract a string describing an IP address from an array of bytes.
		/// 
		/// </summary>
		/// <param name="offset">the offset of the address data.
		/// </param>
		/// <param name="bytes">an array of bytes containing the IP address.
		/// </param>
		/// <returns> a string of the form "255.255.255.255"
		/// </returns>
		public static System.String extract(int offset, byte[] bytes)
		{
			return toString(ArrayHelper.extractInteger(bytes, offset, WIDTH));
			/*
			StringBuffer sa = new StringBuffer();
			for(int i=offset; i<offset + WIDTH; i++) {
			sa.append(0xff & bytes[i]);
			if(i != offset + WIDTH - 1)
			sa.append('.');
			}
			return sa.toString();
			*/
		}
		
		public static void  insert(byte[] bytes, System.String ip, int pos)
		{
			long val = ipToLong(ip);
			ArrayHelper.insertLong(bytes, val, pos, 4);
		}
		
		//From: http://www.ip2location.com/README-IP-COUNTRY.htm
		/// <param name="dottedIP">
		/// </param>
		/// <returns>
		/// </returns>
		public static long ipToLong(System.String dottedIP)
		{
			int i;
			System.String[] arrDec;
			double num = 0;
			if ((System.Object) dottedIP == (System.Object) "")
			{
				return 0;
			}
			else
			{
				arrDec = dottedIP.Split(new char[] { '.' });
				for (i = arrDec.Length - 1; i >= 0; i--)
				{
					num += ((System.Int64.Parse(arrDec[i]) % 256) * System.Math.Pow(256, (3 - i)));
				}
				//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
				return (long) num;
			}
		}
		
		/// <summary> Generate a random IP number between 0.0.0.0 and 255.255.255.255.</summary>
		public static int random()
		{
			// cast to long before int to preserve all 32-bits of precision
			// (otherwise, highest bit is lost for based on sign)
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			return (int) (0xffffffffL * SupportClass.Random.NextDouble());
		}
		
		/// <summary> Generate a random IP address.</summary>
		/// <param name="network">the network number. i.e. 0x0a000000.
		/// </param>
		/// <param name="mask">the network mask. i.e. 0xffffff00.
		/// </param>
		/// <returns> a random IP address on the specified network.
		/// </returns>
		public static int random(int network, int mask)
		{
			// the bits that get randomized are the inverse of the mask
			int rbits = ~ mask;
			
			//UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
			int random = network + (int) (rbits * SupportClass.Random.NextDouble()) + 1;
			
			return random;
		}
		
		
		///// <summary> Unit test.</summary>
		//[STAThread]
		//public static void  Main(System.String[] args)
		//{
		//    for (int i = 0; i < 10; i++)
		//    {
		//        // 10.0.0.16/255.255.255.240
		//        int r = random(0x0a000010, unchecked((int) 0xfffffff0));
		//        System.Console.Error.WriteLine(System.Convert.ToString(r, 16) + " " + toString(r));
		//    }
		//}
		
		
		/// <summary> The width in bytes of an IP address.</summary>
		public const int WIDTH = 4;
	}
}