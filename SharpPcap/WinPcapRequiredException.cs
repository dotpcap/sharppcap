/*
This file is part of SharpPcap.

SharpPcap is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

SharpPcap is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with SharpPcap.  If not, see <http://www.gnu.org/licenses/>.
*/
/* 
 * Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;

namespace SharpPcap
{
    /// <summary>
    /// Exception thrown when a WinPcap extension method is called from
    /// a non-Windows platform
    /// </summary>
    public class WinPcapRequiredException : PcapException
    {
        /// <summary>
        /// string constructor
        /// </summary>
        /// <param name="msg">
        /// A <see cref="System.String"/>
        /// </param>
        public WinPcapRequiredException(string msg) : base(msg)
        {
        }
    }
}
