// $Id: IPVersions.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace Tamir.IPLib.Packets
{
	
	
	/// <summary> Code constants for internet protocol versions.
	/// 
	/// </summary>
	/// <author>  Patrick Charles and Jonas Lehmann
	/// </author>
	/// <version>  $Revision: 1.1.1.1 $
	/// </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
	public struct IPVersions_Fields{
		/// <summary> Internet protocol version 4.</summary>
		public readonly static int IPV4 = 4;
		/// <summary> Internet protocol version 6.</summary>
		public readonly static int IPV6 = 6;
	}
	public interface IPVersions
	{
		//UPGRADE_NOTE: Members of interface 'IPVersions' were extracted into structure 'IPVersions_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
		
	}
}