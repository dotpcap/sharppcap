/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
using ArrayHelper = SharpPcap.Packets.Util.ArrayHelper;
using HexHelper = SharpPcap.Packets.Util.HexHelper;
namespace SharpPcap.Packets
{
    /// <summary> MAC address.
    /// <p>
    /// This class doesn't yet store MAC addresses. Only a utility method
    /// to extract a MAC address from a big-endian byte array is implemented.
    /// 
    /// </summary>
    public class MACAddress
    {
        /// <summary> Extract a MAC address from an array of bytes.</summary>
        /// <param name="offset">the offset of the address data from the start of the 
        /// packet.
        /// </param>
        /// <param name="bytes">an array of bytes containing at least one MAC address.
        /// </param>
        public static System.String extract(int offset, byte[] bytes)
        {
            System.Text.StringBuilder sa = new System.Text.StringBuilder();
            for (int i = offset; i < offset + WIDTH; i++)
            {
                sa.Append(HexHelper.toString(bytes[i]));
                if (i != offset + WIDTH - 1)
                    sa.Append(':');
            }
            return sa.ToString();
        }

        public static System.String extract(long mac)
        {
            byte[] bytes = new byte[6];
            ArrayHelper.insertLong(bytes, mac, 0, 6);
            return extract(0, bytes);
        }

        public static void insert(System.String mac, byte[] bytes, int offset)
        {
            mac = mac.Replace(":", "").Replace("-","");
            long l = System.Convert.ToInt64(mac, 16);
            ArrayHelper.insertLong(bytes, l, offset, 6);
        }

        public static void insert(System.String mac, int offset, byte[] bytes)
        {
            mac = mac.Replace(":", "").Replace("-","");
            long l = System.Convert.ToInt64(mac, 16);
            ArrayHelper.insertLong(bytes, l, offset, 6);
        }

        /// <summary> Generate a random MAC address.</summary>
        public static long random()
        {
            System.Random rand = new System.Random();
            rand.NextDouble();
            return (long)(0xffffffffffffL * rand.NextDouble());
        }

        /// <summary> The width in bytes of a MAC address.</summary>
        public const int WIDTH = 6;
    }
}
