///// <summary>************************************************************************
///// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
///// Distributed under the Mozilla Public License                            *
///// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
///// *************************************************************************
///// </summary>
//using System;
//using HexHelper = SharpPcap.Packets.Util.HexHelper;
//using Timeval = SharpPcap.Packets.Util.Timeval;
//namespace SharpPcap.Packets
//{
//    /// <summary> A captured packet containing raw data.
//    /// <p>
//    /// Encapsulation for data captured on a network device by PacketCapture's
//    /// raw capture interface.
//    /// 
//    /// </summary>
//    [Serializable]
//    public class RawPacket
//    {
//        /// <summary> Fetch the timeval containing the time the packet arrived on the 
//        /// device where it was captured.
//        /// </summary>
//        virtual public Timeval Timeval
//        {
//            get
//            {
//                return timeval;
//            }
            
//        }
//        /// <summary> Fetch the raw packet data.</summary>
//        virtual public byte[] Data
//        {
//            get
//            {
//                return bytes;
//            }
            
//        }
//        /// <summary> Fetch the number of bytes dropped (if any) when the packet 
//        /// was captured.
//        /// <p>
//        /// Bytes are dropped when the snapshot length (a ceiling on the number of 
//        /// bytes per packet to capture) is smaller than the actual number of bytes
//        /// in the packet on the wire. In other words, when caplen exceeds snaplen,
//        /// bytes are dropped and droplen will be nonzero. Otherwise, all the 
//        /// packet bytes were captured and droplen is zero.
//        /// </summary>
//        virtual public int Droplen
//        {
//            get
//            {
//                return droplen;
//            }
            
//        }
//        /// <summary> Create a new raw packet.
//        /// 
//        /// </summary>
//        /// <param name="timeval">the time the packet arrived on the device where it was 
//        /// captured.
//        /// </param>
//        /// <param name="bytes">the raw packet data, including headers.
//        /// </param>
//        /// <param name="droplen">the number of bytes dropped (if any) when the packet 
//        /// was captured.
//        /// </param>
//        public RawPacket(Timeval timeval, byte[] bytes, int droplen)
//        {
//            this.timeval = timeval;
//            this.bytes = bytes;
//            this.droplen = droplen;
//        }
        
//        /// <summary> Convert this packet to a readable string.</summary>
//        public override System.String ToString()
//        {
//            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
//            int length = bytes.Length;
//            buffer.Append("[RawPacket: ");
//            buffer.Append("l = " + length + " of " + (length + droplen) + ", ");
//            buffer.Append("t = " + timeval + ", ");
//            buffer.Append("d = ");
//            buffer.Append(HexHelper.toString(bytes));
//            buffer.Append(']');
            
//            return buffer.ToString();
//        }
        
        
//        private Timeval timeval;
//        private byte[] bytes;
//        private int droplen;
        
//        private System.String _rcsid = "$Id: RawPacket.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $";
//    }
//}
