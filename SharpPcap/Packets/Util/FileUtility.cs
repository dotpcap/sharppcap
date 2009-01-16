// $Id: FileUtility.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2004, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets.Util
{
	
	
	/// <summary> Writes data in tcpdump format
	/// 
	/// </summary>
	/// <author>  Joyce Lin
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	/// <summary> 
	/// </summary>
	public class FileUtility
	{
		public static System.String readFile(System.String filename)
		{
			System.String readString = "";
			//System.String tmp;
			
			System.IO.FileInfo f = new System.IO.FileInfo(filename);
			char[] readIn = new char[(int) ((long) SupportClass.FileLength(f))];
			
			//UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
			//UPGRADE_WARNING: At least one expression was used more than once in the target code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1181'"
			//UPGRADE_TODO: Constructor 'java.io.FileReader.FileReader' was converted to 'System.IO.StreamReader' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
			System.IO.StreamReader in_Renamed = new System.IO.StreamReader(new System.IO.StreamReader(f.FullName, System.Text.Encoding.Default).BaseStream, new System.IO.StreamReader(f.FullName, System.Text.Encoding.Default).CurrentEncoding);
			
			//UPGRADE_TODO: Method 'java.io.Reader.read' was converted to 'System.IO.StreamReader.Read' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioReaderread_char[]'"
			in_Renamed.Read((System.Char[]) readIn, 0, readIn.Length);
			readString = new System.String(readIn);
			
			in_Renamed.Close();
			
			return readString;
		}
		
		public static void  writeFile(System.String str, System.String filename, bool append)
		{
			
			int length = str.Length;
			//UPGRADE_TODO: Class 'java.io.FileWriter' was converted to 'System.IO.StreamWriter' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileWriter'"
			//UPGRADE_TODO: Constructor 'java.io.FileWriter.FileWriter' was converted to 'System.IO.StreamWriter' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileWriterFileWriter_javalangString_boolean'"
			System.IO.StreamWriter out_Renamed = new System.IO.StreamWriter(filename, append, System.Text.Encoding.Default);
			out_Renamed.Write(str.ToCharArray(), 0, length);
			out_Renamed.Close();
		}
		
		public static void  writeFile(byte[] bytes, System.String filename, bool append)
		{
			
			//UPGRADE_TODO: Constructor 'java.io.FileOutputStream.FileOutputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileOutputStreamFileOutputStream_javalangString_boolean'"
			System.IO.FileStream out_Renamed = SupportClass.GetFileStream(filename, append);
			out_Renamed.Write(bytes, 0, bytes.Length);
			out_Renamed.Close();
		}
		
		public static void  writeFile(byte[][] bytes, System.String filename, bool append)
		{
			
			writeFile(bytes[0], filename, append);
			for (int i = 1; i < bytes.Length; i++)
				writeFile(bytes[i], filename, true);
		}
		
		public static void  writeFile(byte[][] bytes, int beginIndex, int endIndex, System.String filename, bool append)
		{
			writeFile(bytes[beginIndex], filename, append);
			for (int i = beginIndex + 1; i <= endIndex; i++)
				writeFile(bytes[i], filename, true);
		}
		
		
		internal const System.String _rcsid = "$Id: FileUtility.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $";
	}
}