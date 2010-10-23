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

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// The mode used when opening a device
    /// </summary>
    [Flags]
    public enum OpenFlags : short
    {
        /// <summary>
        /// Defines if the adapter has to go in promiscuous mode. 
        /// </summary>
        Promiscuous = 1,

        /// <summary>
        /// Defines if the data trasfer (in case of a remote capture)
        /// has to be done with UDP protocol. 
        /// </summary>
        DataTransferUdp = 2, 

        /// <summary>
        /// Defines if the remote probe will capture its own generated traffic. 
        /// </summary>
        NoCaptureRemote = 4,

        /// <summary>
        /// Defines if the local adapter will capture its own generated traffic. 
        /// </summary>
        NoCaptureLocal = 8,

        /// <summary>
        /// This flag configures the adapter for maximum responsiveness. 
        /// </summary>
        MaxResponsiveness = 16
    }
}