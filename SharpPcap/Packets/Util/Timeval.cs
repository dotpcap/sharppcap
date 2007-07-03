// $Id: Timeval.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace Tamir.IPLib.Packets.Util
{
	
	
	/// <summary> POSIX.4 timeval for Java.
	/// <p>
	/// Container for java equivalent of c's struct timeval.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	[Serializable]
	public class Timeval
	{
		/// <summary> Convert this timeval to a java Date.</summary>
		virtual public System.DateTime Date
		{
			get
			{
				//UPGRADE_TODO: Constructor 'java.util.Date.Date' was converted to 'System.DateTime.DateTime' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilDateDate_long'"
				return new System.DateTime(seconds * 1000 + microseconds / 1000);
			}
			
		}
		virtual public long Seconds
		{
			get
			{
				return seconds;
			}
			
		}
		virtual public int MicroSeconds
		{
			get
			{
				return microseconds;
			}
			
		}
		public Timeval(long seconds, int microseconds)
		{
			this.seconds = seconds;
			this.microseconds = microseconds;
		}
		
		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(seconds);
			sb.Append('.');
			sb.Append(microseconds);
			sb.Append('s');
			
			return sb.ToString();
		}
		
		internal long seconds;
		internal int microseconds;
	}
}