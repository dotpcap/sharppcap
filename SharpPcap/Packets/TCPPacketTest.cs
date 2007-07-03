//// $Id: TCPPacketTest.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

///// <summary>************************************************************************
///// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
///// Distributed under the Mozilla Public License                            *
///// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
///// *************************************************************************
///// </summary>
//using System;
////UPGRADE_TODO: The package 'junit.framework' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using junit.framework;
//namespace Tamir.IPLib.Packets
//{
	
//    public class TCPPacketTest:TestCase
//    {
		
//        // yes, I realize that as TCP packets, these are not SYN-ACK and 
//        // PSH-ACKs, but I use the same shell for testing TCPPacket
//        private TCPPacket _synAck;
//        private TCPPacket _pshAck;
//        // and bad is always bad
//        private TCPPacket _baddie;
		
//        public TCPPacketTest(System.String testName):base(testName)
//        {
//        }
		
//        [STAThread]
//        public static void  Main(System.String[] args)
//        {
//            //junit.swingui.TestRunner.main (new String[] {TCPPacketTest.class.getName ()});
//            junit.textui.TestRunner.run(suite());
//        }
		
//        public static Test suite()
//        {
//            TestSuite suite = new TestSuite(typeof(TCPPacketTest));
//            return suite;
//        }
		
//        private static byte[] SYN_ACK_PACKET = new byte[]{(byte) (0x00), (byte) (0x10), (byte) (0x7b), (byte) (0x38), (byte) (0x46), (byte) (0x33), (byte) (0x08), (byte) (0x00), (byte) (0x20), (byte) SupportClass.Identity(0x89), (byte) SupportClass.Identity(0xa5), (byte) SupportClass.Identity(0x9f), (byte) (0x08), (byte) (0x00), (byte) (0x45), (byte) (0x00), (byte) (0x00), (byte) (0x2c), (byte) SupportClass.Identity(0x93), (byte) SupportClass.Identity(0x83), (byte) (0x40), (byte) (0x00), (byte) SupportClass.Identity(0xff), (byte) (0x06), (byte) (0x6c), (byte) (0x38), (byte) SupportClass.Identity(0xac), (byte) (0x10), (byte) (0x70), (byte) (0x32), (byte) SupportClass.Identity(0x87), (byte) (0x0d), (byte) SupportClass.Identity(0xd8), (byte) SupportClass.Identity(0xbf), (byte) (0x00), (byte) (0x19), (byte) (0x50), (byte) (0x49), (byte) (0x78), (byte) SupportClass.Identity(0xbe), (byte) SupportClass.Identity(0xe0), (byte) SupportClass.Identity(0xa7), (byte) SupportClass.Identity(0x9f), (byte) (0x3a), (byte) SupportClass.Identity(0xb4), (byte) (0x03), (byte) (0x60), (byte) (0x12), (byte) (0x22), (byte) (0x38), (byte) SupportClass.Identity(0xfc), (byte) SupportClass.Identity(0xc7), (byte) (0x00), (byte) (0x00), (byte) (0x02), (byte) (0x04), (byte) (0x05), (byte) SupportClass.Identity(0xb4), (byte) (0x70), (byte) (0x6c)};
//        private static byte[] PSH_ACK_PACKET = new byte[]{(byte) (0x08), (byte) (0x00), (byte) (0x20), (byte) SupportClass.Identity(0x89), (byte) SupportClass.Identity(0xa5), (byte) SupportClass.Identity(0x9f), (byte) (0x00), (byte) (0x10), (byte) (0x7b), (byte) (0x38), (byte) (0x46), (byte) (0x33), (byte) (0x08), (byte) (0x00), (byte) (0x45), (byte) (0x00), (byte) (0x00), (byte) (0x3e), (byte) SupportClass.Identity(0x87), (byte) (0x08), (byte) (0x40), (byte) (0x00), (byte) (0x3f), (byte) (0x06), (byte) (0x38), (byte) SupportClass.Identity(0xa2), (byte) SupportClass.Identity(0x87), (byte) (0x0d), (byte) SupportClass.Identity(0xd8), (byte) SupportClass.Identity(0xbf), (byte) SupportClass.Identity(0xac), (byte) (0x10), (byte) (0x70), (byte) (0x32), (byte) (0x50), (byte) (0x49), (byte) (0x00), (byte) (0x19), (byte) SupportClass.Identity(0x9f), (byte) (0x3a), (byte) SupportClass.Identity(0xb4), (byte) (0x03), (byte) (0x78), (byte) SupportClass.Identity(0xbe), (byte) SupportClass.Identity(0xe0), (byte) SupportClass.Identity(0xf8), (byte) (0x50), (byte) (0x18), (byte) (0x7d), (byte) (0x78), (byte) SupportClass.Identity(0x86), (byte) SupportClass.Identity(0xf0), (byte) (0x00), (byte) (0x00), (byte) (0x45), (byte) (0x48), (byte) (0x4c), (byte) (0x4f), (byte) (0x20), (byte) (0x61), (byte) (0x6c), (byte) (0x70), (byte) (0x68), (byte) (0x61), (byte) (0x2e), (byte) (0x61), (byte) (0x70), (byte) (0x70), (byte) (0x6c), (byte) (0x65), (byte) (0x2e), (byte) (0x65), (byte) (0x64), (byte) (0x75), (byte) (0x0d), (byte) (0x0a)};
		
//        public virtual void  setUp()
//        {
//            // get link layer length
//            int linkLayerLen = LinkLayer.getLinkLayerLength(Tamir.IPLib.Packets.LinkLayers_Fields.EN10MB);
//            // create syn-ack packet
//            _synAck = new TCPPacket(linkLayerLen, SYN_ACK_PACKET);
//            // create psh-ack packet
//            _pshAck = new TCPPacket(linkLayerLen, PSH_ACK_PACKET);
//            // create packet with random garbage
//            byte[] badBytes = new byte[SYN_ACK_PACKET.Length];
//            (new System.Random()).NextBytes(SupportClass.ToByteArray(badBytes));
//            _baddie = new TCPPacket(linkLayerLen, badBytes);
//        }
		
//        public virtual void  tearDown()
//        {
//        }
		
//        public virtual void  testSynAckPacketHeaderLengths()
//        {
//            assertEquals(24, _synAck.TCPHeaderLength);
//            assertEquals(24, _synAck.TCPHeader.Length);
//            assertEquals(24, _synAck.HeaderLength);
//            assertEquals(24, _synAck.Header.Length);
//        }
		
//        public virtual void  testPshAckPacketHeaderLengths()
//        {
//            assertEquals(20, _pshAck.TCPHeaderLength);
//            assertEquals(20, _pshAck.TCPHeader.Length);
//            assertEquals(20, _pshAck.HeaderLength);
//            assertEquals(20, _pshAck.Header.Length);
//        }
		
//        public virtual void  testSynAckPacketDataLengths()
//        {
//            assertEquals(0, _synAck.TCPData.Length);
//            assertEquals(0, _synAck.getData().Length);
//        }
		
//        public virtual void  testPshAckPacketDataLengths()
//        {
//            assertEquals(22, _pshAck.TCPData.Length);
//            assertEquals(22, _pshAck.getData().Length);
//        }
		
//        public virtual void  testSynAckPacketPorts()
//        {
//            assertEquals(25, _synAck.SourcePort);
//            assertEquals(20553, _synAck.DestinationPort);
//        }
		
//        public virtual void  testPshAckPacketAddresses()
//        {
//            assertEquals(20553, _pshAck.SourcePort);
//            assertEquals(25, _pshAck.DestinationPort);
//        }
		
//        public virtual void  testSynAckPacketHeaderValues()
//        {
//            assertEquals(2025775271L, _synAck.SequenceNumber);
//            assertEquals(2671424515L, _synAck.AcknowledgmentNumber);
//            assertEquals(8760, _synAck.WindowSize);
//            assertEquals(0xfcc7, _synAck.TCPChecksum);
//            assertEquals(0xfcc7, _synAck.getChecksum());
//            //   	assertTrue   ("Packet should checksum",_synAck.isValidChecksum ());
//            assertEquals(0, _synAck.getUrgentPointer());
//            assertTrue(!_synAck.Urg);
//            assertTrue(_synAck.Ack);
//            assertTrue(!_synAck.Psh);
//            assertTrue(!_synAck.Rst);
//            assertTrue(_synAck.Syn);
//            assertTrue(!_synAck.Fin);
//        }
		
//        public virtual void  testPshAckPacketHeaderValues()
//        {
//            assertEquals(2671424515L, _pshAck.SequenceNumber);
//            assertEquals(2025775352L, _pshAck.AcknowledgmentNumber);
//            assertEquals(32120, _pshAck.WindowSize);
//            assertEquals(0x86f0, _pshAck.TCPChecksum);
//            assertEquals(0x86f0, _pshAck.getChecksum());
//            //   	assertTrue   ("Packet should checksum",_pshAck.isValidChecksum ());
//            assertEquals(0, _pshAck.getUrgentPointer());
//            assertTrue(!_pshAck.Urg);
//            assertTrue(_pshAck.Ack);
//            assertTrue(_pshAck.Psh);
//            assertTrue(!_pshAck.Rst);
//            assertTrue(!_pshAck.Syn);
//            assertTrue(!_pshAck.Fin);
//        }
		
//        public virtual void  testBadPacketHeaderLengths()
//        {
//            // really just make sure this doesn't crash the thing
//            assertTrue("Bad read of TCP header for random data", (_baddie.TCPHeader.Length >= 0));
//            assertTrue("Bad read of TCP header for random data", (_baddie.Header.Length >= 0));
//        }
		
//        public virtual void  testBadPacketDataLengths()
//        {
//            // really just make sure this doesn't crash the thing
//            assertTrue("Bad read of TCP data (payload) for random data", (_baddie.TCPData.Length >= 0));
//            assertTrue("Bad read of TCP data (payload) for random data", (_baddie.getData().Length >= 0));
//        }
//    }
//}