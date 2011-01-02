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
 * Copyright 2010-2011 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPcap.AirPcap
{
    /// <summary>
    /// Adapter description
    /// </summary>
    public class AirPcapDeviceDescription
    {
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Device description
        /// </summary>
        public string Description { get; set; }

        internal AirPcapDeviceDescription(AirPcapUnmanagedStructures.AirpcapDeviceDescription desc)
        {
            this.Name = desc.Name;
            this.Description = desc.Description;
        }

        /// <summary>
        /// ToString() override
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, Description: {1}",
                                 Name, Description);
        }
    }
}
