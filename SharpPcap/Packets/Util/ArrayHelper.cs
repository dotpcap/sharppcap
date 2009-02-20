/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets.Util
{
    /// <summary> Utility functions for populating and manipulating arrays.
    /// </summary>
    public class ArrayHelper
    {
        /// <summary> Join two arrays.</summary>
        public static byte[] join(byte[] a, byte[] b)
        {
            byte[] bytes = new byte[a.Length + b.Length];
            
            Array.Copy(a, 0, bytes, 0, a.Length);
            Array.Copy(b, 0, bytes, a.Length, b.Length);
            
            return bytes;
        }
        
        public static byte[] copy(byte[] from)
        {
            return copy(from, 0);
        }
        
        public static byte[] copy(byte[] from, int start)
        {
            return copy(from, start, from.Length);
        }
        
        public static byte[] copy(byte[] from, int start, int len)
        {
            byte[] to = new byte[len];
            Array.Copy(from, start, to, 0, len);
            return to;
        }

        //FIXME: this routine is broken in the sense that it performs
        //       implicit byte order conversion
        /// <summary> Extract a long from a byte array.
        /// 
        /// </summary>
        /// <param name="bytes">an array.
        /// </param>
        /// <param name="pos">the starting position where the integer is stored.
        /// </param>
        /// <param name="cnt">the number of bytes which contain the integer.
        /// </param>
        /// <returns> the long, or <b>0</b> if the index/length to use 
        /// would cause an ArrayOutOfBoundsException
        /// </returns>
        public static long extractLong(byte[] bytes, int pos, int cnt)
        {
            // commented out because it seems like it might mask a fundamental 
            // problem, if the caller is referencing positions out of bounds and 
            // silently getting back '0'.
            //   if((pos + cnt) > bytes.length) return 0;
            long value_Renamed = 0;
            for (int i = 0; i < cnt; i++)
                value_Renamed |= ((bytes[pos + cnt - i - 1] & 0xffL) << 8 * i);
            
            return value_Renamed;
        }
        
        /// <summary> Extract an integer from a byte array.
        /// 
        /// </summary>
        /// <param name="bytes">an array.
        /// </param>
        /// <param name="pos">the starting position where the integer is stored.
        /// </param>
        /// <param name="cnt">the number of bytes which contain the integer.
        /// </param>
        /// <returns> the integer, or <b>0</b> if the index/length to use 
        /// would cause an ArrayOutOfBoundsException
        /// </returns>
        public static int extractInteger(byte[] bytes, int pos, int cnt)
        {
            // commented out because it seems like it might mask a fundamental 
            // problem, if the caller is referencing positions out of bounds and 
            // silently getting back '0'.
            // if((pos + cnt) > bytes.length) return 0;
            int value_Renamed = 0;
            for (int i = 0; i < cnt; i++)
                value_Renamed |= ((bytes[pos + cnt - i - 1] & 0xff) << 8 * i);
            
            return value_Renamed;
        }
        
        /// <summary> Insert data contained in a long integer into an array.
        /// 
        /// </summary>
        /// <param name="bytes">an array.
        /// </param>
        /// <param name="value">the long to insert into the array.
        /// </param>
        /// <param name="pos">the starting position into which the long is inserted.
        /// </param>
        /// <param name="cnt">the number of bytes to insert.
        /// </param>
        public static void  insertLong(byte[] bytes, long value_Renamed, int pos, int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                bytes[pos + cnt - i - 1] = (byte) (value_Renamed & 0xff);
                value_Renamed >>= 8;
            }
        }
        
        public static void  insertInt32(byte[] bytes, int value_Renamed, int pos)
        {
            insertLong(bytes, value_Renamed, pos, 4);
        }
        
        public static void  insertInt16(byte[] bytes, int value_Renamed, int pos)
        {
            insertLong(bytes, value_Renamed, pos, 2);
        }
        
        /// <summary> Convert a long integer into an array of bytes.
        /// 
        /// </summary>
        /// <param name="value">the long to convert.
        /// </param>
        /// <param name="cnt">the number of bytes to convert.
        /// </param>
        public static byte[] toBytes(long value_Renamed, int cnt)
        {
            byte[] bytes = new byte[cnt];
            for (int i = 0; i < cnt; i++)
            {
                bytes[cnt - i - 1] = (byte) (value_Renamed & 0xff);
                value_Renamed >>= 8;
            }
            
            return bytes;
        }
        
        /// <summary> Convert a long integer into an array of bytes, little endian format. 
        /// (i.e. this does the same thing as toBytes() but returns an array 
        /// in reverse order from the array returned in toBytes().
        /// </summary>
        /// <param name="value">the long to convert.
        /// </param>
        /// <param name="cnt">the number of bytes to convert.
        /// </param>
        public static byte[] toBytesLittleEndian(long value_Renamed, int cnt)
        {
            byte[] bytes = new byte[cnt];
            for (int i = 0; i < cnt; i++)
            {
                bytes[i] = (byte) (value_Renamed & 0xff);
                value_Renamed >>= 8;
            }
            
            return bytes;
        }
        
        public static void  fillBytes(byte[] byteArray, long value_Renamed, int cnt, int index)
        {
            
            for (int i = 0; i < cnt; i++)
            {
                byteArray[cnt - i - 1 + index] = (byte) (value_Renamed & 0xff);
                value_Renamed >>= 8;
            }
        }
        
        public static void  fillBytesLittleEndian(byte[] byteArray, long value_Renamed, int cnt, int index)
        {
            for (int i = 0; i < cnt; i++)
            {
                byteArray[index + i] = (byte) (value_Renamed & 0xff);
                value_Renamed >>= 8;
            }
        }
        
        public static bool equals(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;
            
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }
            return true;
        }
    }
}