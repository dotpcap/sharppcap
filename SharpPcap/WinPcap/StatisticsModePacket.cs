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
 * Copyright 2005 Tamir Gal <tamir@tamirgal.com>
 * Copyright 2008-2009 Chris Morgan <chmorgan@gmail.com>
 */

using System;

namespace SharpPcap.WinPcap
{
    /// <summary>
    /// Holds network statistics entry from winpcap when in statistics mode
    /// See http://www.winpcap.org/docs/docs_41b5/html/group__wpcap__tut9.html
    /// </summary>
    public class StatisticsModePacket
    {
        /// <summary>
        /// This holds time value
        /// </summary>
        public PosixTimeval Timeval
        {
            get;
            set;
        }

        /// <summary>
        /// This holds byte received and packets received
        /// </summary>
        private byte[]  m_pktData;

        internal StatisticsModePacket(RawCapture p)
        {
            this.Timeval = p.Timeval;
            this.m_pktData  = p.Data;
        }

        /// <summary>
        /// Number of packets received since last sample
        /// </summary>
        public Int64 RecievedPackets
        {
            get
            {
                return BitConverter.ToInt64(m_pktData, 0);
            }
        }

        /// <summary>
        /// Number of bytes received since last sample
        /// </summary>
        public Int64 RecievedBytes
        {
            get
            {
                return BitConverter.ToInt64(m_pktData, 8);
            }
        }
    }
}
