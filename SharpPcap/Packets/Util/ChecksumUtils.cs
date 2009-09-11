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
 * Copyright 2008-2009 Phillip Lemon <lucidcomms@gmail.com>
 */

using System;
using System.IO;

namespace SharpPcap
{
    /// <summary>
    /// Computes the one's sum on a byte array.
    /// Based TCP/IP Illustrated Vol. 2(1995) by Gary R. Wright and W. Richard
    /// Stevens. Page 236. And on http://www.cs.utk.edu/~cs594np/unp/checksum.html
    /// </summary>

    /*
    * taken from TCP/IP Illustrated Vol. 2(1995) by Gary R. Wright and W.
    * Richard Stevens. Page 236
    */
    public sealed class ChecksumUtils
    {
        private ChecksumUtils() { }

        /// <summary>
        /// Computes the one's complement sum on a byte array
        /// </summary>
        public static int OnesComplementSum(byte[] bytes)
        {
            //just complement the one's sum
            return OnesComplementSum(bytes, 0, bytes.Length);
        }

        /// <summary> 
        /// Computes the one's complement sum on a byte array
        /// </summary>
        public static int OnesComplementSum(byte[] bytes, int start, int len)
        {
            //just complement the one's sum
            return (~OnesSum(bytes, start, len)) & 0xFFFF;
        }

        public static int OnesSum(byte[] bytes)
        {
            return OnesSum(bytes, 0, bytes.Length);
        }

        // 16 bit sum of all values
        // http://en.wikipedia.org/wiki/Signed_number_representations#Ones.27_complement
        public static int OnesSum(byte[] bytes, int start, int len)
        {
            MemoryStream memStream = new MemoryStream(bytes, start, len);
            BinaryReader br = new BinaryReader(memStream);
            Int32 sum = 0;

            UInt16 val;

            while (memStream.Position < memStream.Length -1)
            {
                val = (UInt16)Util.IPUtil.Ntoh(br.ReadInt16());
                sum += val;
            }

            // if we have a remaining byte we should add it
            if (memStream.Position < len)
            {
                sum += br.ReadByte();
            }

            // fold the sum into 16 bits
            while((sum >> 16) != 0)
            {
                sum = (sum & 0xffff) + (sum >> 16);
            }

            return sum;
        }
    }
}
