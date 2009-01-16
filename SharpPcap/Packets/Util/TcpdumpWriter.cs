//// $Id: TcpdumpWriter.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

///// <summary>************************************************************************
///// Copyright (C) 2004, Patrick Charles and Jonas Lehmann                   *
///// Distributed under the Mozilla Public License                            *
///// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
///// *************************************************************************
///// </summary>
//using System;
////UPGRADE_TODO: The type 'SharpPcap.Packets.RawPacket' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using RawPacket = SharpPcap.Packets.RawPacket;
//namespace SharpPcap.Packets.Util
//{
	
	
//    /// <summary> Writes data in tcpdump format
//    /// 
//    /// </summary>
//    /// <author>  Joyce Lin
//    /// </author>
//    /// <version>  $Revision: 1.1.1.1 $
//    /// </version>
//    /// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
//    /// <lastModifiedAt>  $Date: 2007-07-03 10:15:18 $ </lastModifiedAt>
//    /// <summary> 
//    /// </summary>
//    public class TcpdumpWriter
//    {
//        public const int BIG_ENDIAN = 0;
//        public const int LITTLE_ENDIAN = 1;
//        public const int MAJOR_VERSION = 0x0002;
//        public const int MINOR_VERSION = 0x0004;
//        public const int CODE_ETHERNET = 0x00000001;
		
//        /// <summary> A Tcpdump file header is 24 bytes:
//        /// 4 bytes: Tcpdump signature "magic number" 0xA1B2C3D4
//        /// 2 bytes: Major version number, currently 2
//        /// 2 bytes: Minor version number, currently 4
//        /// 4 bytes: Time Zone offset, currently unused (set to zero)
//        /// 4 bytes: Time stamp accuracy, currently unused (set to zero)
//        /// 4 bytes: Snapshot length
//        /// 4 bytes: Link-layer type, Ethernet for now
//        /// </summary>
//        public static void  writeHeader(System.String filename, int endian, long snaplen)
//        {
//            //byte[][] headerArray= new byte[6][];
			
//            byte[] headerArray = new byte[24];
			
//            if (endian == LITTLE_ENDIAN)
//            {
//                /*
//                headerArray[0] = ArrayHelper.toBytesLittleEndian(0xA1B2C3D4, 4);
//                headerArray[1] = ArrayHelper.toBytesLittleEndian(MAJOR_VERSION, 2);
//                headerArray[2] = ArrayHelper.toBytesLittleEndian(MINOR_VERSION, 2);
//                headerArray[3] = ArrayHelper.toBytes(0,8);
//                headerArray[4] = ArrayHelper.toBytesLittleEndian(snaplen, 4);
//                headerArray[5] = ArrayHelper.toBytesLittleEndian(CODE_ETHERNET, 4);
//                */
//                ArrayHelper.fillBytesLittleEndian(headerArray, 0xA1B2C3D4, 4, 0);
//                ArrayHelper.fillBytesLittleEndian(headerArray, MAJOR_VERSION, 2, 4);
//                ArrayHelper.fillBytesLittleEndian(headerArray, MINOR_VERSION, 2, 6);
//                ArrayHelper.fillBytesLittleEndian(headerArray, 0, 8, 8);
//                ArrayHelper.fillBytesLittleEndian(headerArray, snaplen, 4, 16);
//                ArrayHelper.fillBytesLittleEndian(headerArray, CODE_ETHERNET, 4, 20);
//            }
//            else
//            {
//                /*
//                headerArray[0] = ArrayHelper.toBytes(0xA1B2C3D4, 4);
//                headerArray[1] = ArrayHelper.toBytes(MAJOR_VERSION, 2);
//                headerArray[2] = ArrayHelper.toBytes(MINOR_VERSION, 2);
//                headerArray[2] = ArrayHelper.toBytes(0,8);
//                headerArray[3] = ArrayHelper.toBytes(snaplen, 4);
//                headerArray[4] = ArrayHelper.toBytes(CODE_ETHERNET, 4);
//                */
//            }
			
//            FileUtility.writeFile(headerArray, filename, false);
//        }
		
//        /// <summary> A Tcpdump packet has the following format:
//        /// seconds [4 bytes]
//        /// microseconds [4 bytes]
//        /// captured length [4 bytes]
//        /// original packet length [4 bytes]
//        /// packet data [variable length]
//        /// </summary>
//        public static void  appendPacket(System.String filename, RawPacket rawPacket, int endian)
//        {
//            byte[][] packetArray = new byte[5][];
//            byte[] data = rawPacket.getData();
			
//            if (endian == LITTLE_ENDIAN)
//            {
//                packetArray[0] = ArrayHelper.toBytesLittleEndian(rawPacket.getTimeval().getSeconds(), 4);
//                packetArray[1] = ArrayHelper.toBytesLittleEndian((long) rawPacket.getTimeval().getMicroSeconds(), 4);
//                packetArray[2] = ArrayHelper.toBytesLittleEndian((long) data.Length, 4);
//                packetArray[3] = ArrayHelper.toBytesLittleEndian((long) data.Length + rawPacket.getDroplen(), 4);
//            }
//            else
//            {
//                packetArray[0] = ArrayHelper.toBytes(rawPacket.getTimeval().getSeconds(), 4);
//                packetArray[1] = ArrayHelper.toBytes((long) rawPacket.getTimeval().getMicroSeconds(), 4);
//                packetArray[2] = ArrayHelper.toBytes((long) data.Length, 4);
//                packetArray[3] = ArrayHelper.toBytes((long) data.Length + rawPacket.getDroplen(), 4);
//            }
			
//            packetArray[4] = data;
			
//            FileUtility.writeFile(packetArray, filename, true);
//        }
		
		
//        internal const System.String _rcsid = "$Id: TcpdumpWriter.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $";
//    }
//}