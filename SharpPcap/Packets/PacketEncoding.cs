/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
using System;
namespace SharpPcap.Packets
{
    /// <summary> Packet encoding.
    /// <p>
    /// Contains utility methods for decoding generic packets.
    /// 
    /// </summary>
    public class PacketEncoding
    {       
        // create an empty array to return whenever we need to return an, er,
        // empty array. this should be okay, because how can you mutate a 
        // 0-length array?
        //UPGRADE_NOTE: Final was removed from the declaration of 'EMPTY_BYTE_ARRAY '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
        private static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];
        
        /// <summary> Extract a header from a packet.
        /// 
        /// </summary>
        /// <param name="offset">the offset in bytes to the start of the embedded header.
        /// </param>
        /// <param name="headerLen">the length of the header embedded in the packet.
        /// </param>
        /// <param name="bytes">the packet data, including the embedded header and data.
        /// </param>
        /// <returns> the extracted header data.
        /// </returns>
        public static byte[] extractHeader(int offset, int headerLen, byte[] bytes)
        {
            // null in = null out ?
            if (bytes == null)
                return null;
            // negative in, empty array out
            if ((offset < 0) || (headerLen < 0))
                return EMPTY_BYTE_ARRAY;
            
            // verify that requested length is in the bounds of the array
            int useLen = (headerLen <= (bytes.Length - offset)?headerLen:(bytes.Length - offset));
            // verify that requested offset is also in the bounds
            if (useLen <= 0)
                return EMPTY_BYTE_ARRAY;
            
            byte[] header = new byte[useLen];
            Array.Copy(bytes, offset, header, 0, useLen);
            return header;
        }
        
        /// <summary> Extract data from a packet.
        /// 
        /// </summary>
        /// <param name="offset">the offset in bytes to the start of the embedded header.
        /// </param>
        /// <param name="headerLen">the length of the header embedded in the packet.
        /// </param>
        /// <param name="bytes">the packet data, including the embedded header and data.
        /// </param>
        /// <returns> the extracted packet data.
        /// </returns>
        public static byte[] extractData(int offset, int headerLen, byte[] bytes)
        {
            // null in = null out ?
            if (bytes == null)
                return null;
            // negative in, empty array out
            if ((offset < 0) || (headerLen < 0))
                return EMPTY_BYTE_ARRAY;
            
            int dataLength = bytes.Length - headerLen - offset;
            
            // check that requested datalength is valid.
            // (it may not be if packet values are invalid.)
            if (dataLength <= 0)
                return EMPTY_BYTE_ARRAY;
            
            // valid length, go for it dude
            byte[] data = new byte[dataLength];
            Array.Copy(bytes, offset + headerLen, data, 0, dataLength);
            return data;
        }
        
        /// <summary> Extract data from a packet.
        /// 
        /// </summary>
        /// <param name="offset">the offset in bytes to the start of the embedded header.
        /// </param>
        /// <param name="headerLen">the length of the header embedded in the packet.
        /// </param>
        /// <param name="bytes">the packet data, including the embedded header and data.
        /// </param>
        /// <returns> the extracted packet data.
        /// </returns>
        public static byte[] extractData(int offset, int headerLen, byte[] bytes, int dataLength)
        {
            // null in = null out ?
            if (bytes == null)
                return null;
            // negative in, empty array out. request for no-data in, empty array out
            if ((offset < 0) || (headerLen < 0) || (dataLength <= 0) || ((offset + headerLen) > bytes.Length))
                return EMPTY_BYTE_ARRAY;
            
            //-- make sure dataLength + offset + headerLen <= bytes.length
            if ((dataLength + offset + headerLen) > bytes.Length)
            {
                //-- adjust dataLength
                dataLength = bytes.Length - headerLen - offset;
            }
            
            //-- valid length, go for it dude
            byte[] data = new byte[dataLength];
            Array.Copy(bytes, offset + headerLen, data, 0, dataLength);
            return data;
        }
    }
}
