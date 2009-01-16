// $Id: AsciiHelper.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace Tamir.IPLib.Packets.Util
{
	
	
	/// <summary> Functions for formatting and printing binary data as ascii.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	public class AsciiHelper
	{
		/// <summary> Returns a text representation of a byte array.
		/// Bytes in the array which don't convert to text in the range a..Z
		/// are dropped.
		/// 
		/// </summary>
		/// <param name="bytes">a byte array
		/// </param>
		/// <returns> a string containing the text equivalent of the bytes.
		/// </returns>
		public static System.String toText(byte[] bytes)
		{
			System.IO.StringWriter sw = new System.IO.StringWriter();
			
			int length = bytes.Length;
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
				{
					byte b = bytes[i];
					if (b > 64 && b < 91 || b > 96 && b < 123)
						sw.Write((char) b);
				}
			}
			return (sw.ToString());
		}
		
		/// <summary> Returns a text representation of a byte array.
		/// Bytes in the array which don't convert to printable ascii characters
		/// are dropped.
		/// 
		/// </summary>
		/// <param name="bytes">a byte array
		/// </param>
		/// <returns> a string containing the ascii equivalent of the bytes.
		/// </returns>
		public static System.String toString(byte[] bytes)
		{
			System.IO.StringWriter sw = new System.IO.StringWriter();
			
			int length = bytes.Length;
			if (length > 0)
			{
				for (int i = 0; i < length; i++)
				{
					byte b = bytes[i];
					if (b > 32 && b < 127)
						sw.Write((char) b);
				}
			}
			return (sw.ToString());
		}
		
		
		internal const System.String _rcsid = "$Id: AsciiHelper.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $";
	}
}