// $Id: PropertyHelper.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets.Util
{
	
	
	/// <summary> Property helper utility methods.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	public class PropertyHelper
	{
		/// <summary> Read the specified float property.
		/// <p>
		/// Throws an exception if the property value isn't a floating-point number.
		/// 
		/// </summary>
		/// <param name="key">the name of the property
		/// </param>
		/// <returns> the float value of the property
		/// </returns>
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
		public static float getFloatProperty(System.Collections.Specialized.NameValueCollection properties, System.Object key)
		{
			System.String string_Renamed = (System.String) properties[(System.String) key];
			
			if (string_Renamed == null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				System.Console.Error.WriteLine("WARN: couldn't find float value under '" + key + "'");
			}
			
			return System.Single.Parse((System.String) properties[(System.String) key]);
		}
		
		/// <summary> Read the specified integer property.
		/// <p>
		/// Throws an exception if the property value isn't an integer.
		/// 
		/// </summary>
		/// <param name="key">the name of the property
		/// </param>
		/// <returns> the integer value of the property
		/// </returns>
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
		public static int getIntProperty(System.Collections.Specialized.NameValueCollection properties, System.Object key)
		{
			System.String string_Renamed = (System.String) properties[(System.String) key];
			
			if (string_Renamed == null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				System.Console.Error.WriteLine("WARN: couldn't find integer value under '" + key + "'");
			}
			
			return System.Int32.Parse((System.String) properties[(System.String) key]);
		}
		
		/// <summary> Convert a space delimited color tuple string to a color.
		/// <p>
		/// Converts a string value like "255 255 0" to a color constant,
		/// in this case, yellow.
		/// 
		/// </summary>
		/// <param name="key">the name of the property
		/// </param>
		/// <returns> a Color object equivalent to the provided string contents. 
		/// Returns white if the string is null or can't be converted.
		/// </returns>
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
		public static System.Drawing.Color getColorProperty(System.Collections.Specialized.NameValueCollection properties, System.Object key)
		{
			System.String string_Renamed = (System.String) properties[(System.String) key];
			
			if (string_Renamed == null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				System.Console.Error.WriteLine("WARN: couldn't find color tuplet under '" + key + "'");
				return System.Drawing.Color.White;
			}
			
			SupportClass.Tokenizer st = new SupportClass.Tokenizer(string_Renamed, " ");
			System.Drawing.Color c;
			try
			{
				c = System.Drawing.Color.FromArgb(System.Int32.Parse(st.NextToken()), System.Int32.Parse(st.NextToken()), System.Int32.Parse(st.NextToken()));
			}
			catch (System.ArgumentOutOfRangeException)
			{
				c = System.Drawing.Color.White;
				System.Console.Error.WriteLine("WARN: invalid color spec '" + string_Renamed + "' in property file");
			}
			
			return c;
		}
		
		/// <summary> Convert a dot-delimited IP address to an integer.
		/// <p>
		/// Converts a string value like "10.0.0.5" to an integer.
		/// 
		/// </summary>
		/// <param name="key">the name of the property
		/// </param>
		/// <returns> the integer value of the specified IP number.
		/// returns zero if the IP number is not valid.
		/// </returns>
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
		public static int getIpProperty(System.Collections.Specialized.NameValueCollection properties, System.Object key)
		{
			System.String string_Renamed = (System.String) properties[(System.String) key];
			
			if (string_Renamed == null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				System.Console.Error.WriteLine("WARN: couldn't find IP value under '" + key + "'");
				return 0;
			}
			
			SupportClass.Tokenizer st = new SupportClass.Tokenizer(string_Renamed, ".");
			int address;
			try
			{
				address = System.Int32.Parse(st.NextToken()) << 24 | System.Int32.Parse(st.NextToken()) << 16 | System.Int32.Parse(st.NextToken()) << 8 | System.Int32.Parse(st.NextToken());
			}
			catch (System.ArgumentOutOfRangeException)
			{
				address = 0;
				System.Console.Error.WriteLine("WARN: invalid color spec '" + string_Renamed + "' in property file");
			}
			
			return address;
		}
		
		/// <summary> Read the specified boolean property.
		/// Converts a property value like "true" or "1" to its boolean value. 
		/// <p>
		/// Returns false if the property doesn't exist or can't be converted to a 
		/// boolean.
		/// 
		/// </summary>
		/// <param name="key">the name of the property
		/// </param>
		/// <returns> the property value
		/// </returns>
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
		public static bool getBooleanProperty(System.Collections.Specialized.NameValueCollection properties, System.Object key)
		{
			System.String string_Renamed = (System.String) properties[(System.String) key];
			
			if (string_Renamed == null)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				System.Console.Error.WriteLine("WARN: couldn't find boolean value under '" + key + "'");
				return false;
			}
			
			if (string_Renamed.ToLower().Equals("true") || string_Renamed.ToLower().Equals("on") || string_Renamed.ToLower().Equals("yes") || string_Renamed.ToLower().Equals("1"))
				return true;
			else
				return false;
		}
		
		/// <summary> Refresh property settings from disk.</summary>
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
		public static System.Collections.Specialized.NameValueCollection refresh(System.String name, System.IO.FileStream fis)
		{
			System.Console.Error.WriteLine("INFO: loading properties from " + name);
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
			//UPGRADE_TODO: Format of property file may need to be changed. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1089'"
			System.Collections.Specialized.NameValueCollection properties = new System.Collections.Specialized.NameValueCollection();
			//UPGRADE_TODO: Method 'java.util.Properties.load' was converted to 'System.Collections.Specialized.NameValueCollection' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilPropertiesload_javaioInputStream'"
			properties = new System.Collections.Specialized.NameValueCollection(System.Configuration.ConfigurationSettings.AppSettings);
			
			return properties;
		}
		
		/// <summary> Load the specified properties file from one of the specified set of 
		/// paths.
		/// 
		/// </summary>
		/// <param name="paths">an array of strings containing target paths.
		/// </param>
		/// <param name="fileName">the name of the property file.
		/// </param>
		/// <returns> a populated set of properties loaded from the first file 
		/// found in the set of supplied paths. If no property file is found, 
		/// returns null.
		/// </returns>
		//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
		public static System.Collections.Specialized.NameValueCollection load(System.String[] paths, System.String fileName)
		{
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.Properties' and 'System.Collections.Specialized.NameValueCollection' may cause compilation errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1186'"
			System.Collections.Specialized.NameValueCollection properties = null;
			System.IO.FileInfo propertiesFile = null;
			try
			{
				System.String path = null;
				for (int i = 0; i < paths.Length; i++)
				{
					path = paths[i] + System.IO.Path.DirectorySeparatorChar.ToString() + fileName;
					propertiesFile = new System.IO.FileInfo(path);
					bool tmpBool;
					if (System.IO.File.Exists(propertiesFile.FullName))
						tmpBool = true;
					else
						tmpBool = System.IO.Directory.Exists(propertiesFile.FullName);
					if (tmpBool)
						break;
				}
				bool tmpBool2;
				if (System.IO.File.Exists(propertiesFile.FullName))
					tmpBool2 = true;
				else
					tmpBool2 = System.IO.Directory.Exists(propertiesFile.FullName);
				if (!tmpBool2)
				{
					System.Console.Error.WriteLine("FATAL: could not find file '" + fileName + "' in default search paths: ");
					for (int i = 0; i < paths.Length; i++)
					{
						System.Console.Error.Write("'" + paths[i] + "'");
						if (i < paths.Length - 1)
							System.Console.Error.Write(", ");
					}
					System.Console.Error.WriteLine();
					System.Environment.Exit(1);
				}
				
				//UPGRADE_TODO: Constructor 'java.io.FileInputStream.FileInputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileInputStreamFileInputStream_javaioFile'"
				properties = refresh(propertiesFile.FullName, new System.IO.FileStream(propertiesFile.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read));
			}
			catch
			{
				System.Console.Error.WriteLine("ERROR: couldn't load properties from '" + propertiesFile + "'");
			}
			
			return properties;
		}
	}
}