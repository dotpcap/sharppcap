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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace SharpPcap.AirPcap
{
    public class AirPcapRadioHeader
    {
        public int Version { get; set; }
        public int Length { get; set; }
        public uint Present { get; set; }
        public byte[] TrailingHeaderBytes { get; set; }

        internal AirPcapRadioHeader(IntPtr data)
        {
            var radiotapHeader = (AirPcapUnmanagedStructures.ieee80211_radiotap_header)Marshal.PtrToStructure(data, typeof(AirPcapUnmanagedStructures.ieee80211_radiotap_header));

            this.Version = radiotapHeader.it_version;
            this.Length = radiotapHeader.it_len;
            this.Present = radiotapHeader.it_present;

            // copy the remaining header bytes so we can parse lazily
            TrailingHeaderBytes = new Byte[Length - Marshal.SizeOf(radiotapHeader)];
            var trailingPointer = new IntPtr(data.ToInt64() + Marshal.SizeOf(radiotapHeader));
            Marshal.Copy(trailingPointer, TrailingHeaderBytes, 0, TrailingHeaderBytes.Length);
        }

        public override string ToString()
        {
            return string.Format("Version {0}, Length {1}, Present 0x{2:x}",
                                 Version,
                                 Length,
                                 Present);
        }

        public List<RadioTapField> DecodeRadioTapFields()
        {
            var retval = new List<RadioTapField>();

            // make an array of the bitmask fields
            // the highest bit indicates whether other bitmask fields follow
            // the current field
            var bitmaskFields = new List<UInt32>();
            UInt32 bitmask = this.Present; // start with the first field
            bitmaskFields.Add(bitmask);
            var br = new BinaryReader(new MemoryStream(TrailingHeaderBytes));
            while((bitmask & (1 << 31)) == 1)
            {
                // retrieve the next field
                bitmask = br.ReadUInt32();
                bitmaskFields.Add(bitmask);
            }

            // now go through each of the bitmask fields looking at the least significant
            // bit first to retrieve each field
            int bitIndex = 0;
            foreach (var bmask in bitmaskFields)
            {
                int[] bmaskArray = new int[1];
                bmaskArray[0] = (int)bmask;
                var ba = new BitArray(bmaskArray);

                // look at all of the bits, note we don't want to consider the
                // highest bit since that indicates another bitfield that follows
                for(int x = 0; x < 31; x++)
                {
                    if (ba[x] == true)
                    {
                        retval.Add(RadioTapField.Parse(bitIndex, br));
                    }
                    bitIndex++;
                }
            }

            return retval;
        }
    }
}
