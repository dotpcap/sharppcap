//// $Id: IPPacketTest.cs,v 1.1.1.1 2007-07-03 10:15:17 tamirgal Exp $

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
	
//    public class IPPacketTest:TestCase
//    {
		
//        // yes, I realize that as IP packets, these are not SYN-ACK and 
//        // PSH-ACKs, but I use the same shell for testing TCPPacket
//        private IPPacket _synAck;
//        private IPPacket _pshAck;
//        // and bad is always bad
//        private IPPacket _baddie;
		
//        public IPPacketTest(System.String testName):base(testName)
//        {
//        }
		
//        [STAThread]
//        public static void  Main(System.String[] args)
//        {
//            //junit.swingui.TestRunner.main (new String[] {IPPacketTest.class.getName ()});
//            junit.textui.TestRunner.run(suite());
//        }
		
//        public static Test suite()
//        {
//            TestSuite suite = new TestSuite(typeof(IPPacketTest));
//            return suite;
//        }
		
//        private static byte[] SYN_ACK_PACKET = new byte[]{(byte) (0x00), (byte) (0x10), (byte) (0x7b), (byte) (0x38), (byte) (0x46), (byte) (0x33), (byte) (0x08), (byte) (0x00), (byte) (0x20), (byte) SupportClass.Identity(0x89), (byte) SupportClass.Identity(0xa5), (byte) SupportClass.Identity(0x9f), (byte) (0x08), (byte) (0x00), (byte) (0x45), (byte) (0x00), (byte) (0x00), (byte) (0x2c), (byte) SupportClass.Identity(0x93), (byte) SupportClass.Identity(0x83), (byte) (0x40), (byte) (0x00), (byte) SupportClass.Identity(0xff), (byte) (0x06), (byte) (0x6c), (byte) (0x38), (byte) SupportClass.Identity(0xac), (byte) (0x10), (byte) (0x70), (byte) (0x32), (byte) SupportClass.Identity(0x87), (byte) (0x0d), (byte) SupportClass.Identity(0xd8), (byte) SupportClass.Identity(0xbf), (byte) (0x00), (byte) (0x19), (byte) (0x50), (byte) (0x49), (byte) (0x78), (byte) SupportClass.Identity(0xbe), (byte) SupportClass.Identity(0xe0), (byte) SupportClass.Identity(0xa7), (byte) SupportClass.Identity(0x9f), (byte) (0x3a), (byte) SupportClass.Identity(0xb4), (byte) (0x03), (byte) (0x60), (byte) (0x12), (byte) (0x22), (byte) (0x38), (byte) SupportClass.Identity(0xfc), (byte) SupportClass.Identity(0xc7), (byte) (0x00), (byte) (0x00), (byte) (0x02), (byte) (0x04), (byte) (0x05), (byte) SupportClass.Identity(0xb4), (byte) (0x70), (byte) (0x6c)};
//        private static byte[] PSH_ACK_PACKET = new byte[]{(byte) (0x08), (byte) (0x00), (byte) (0x20), (byte) SupportClass.Identity(0x89), (byte) SupportClass.Identity(0xa5), (byte) SupportClass.Identity(0x9f), (byte) (0x00), (byte) (0x10), (byte) (0x7b), (byte) (0x38), (byte) (0x46), (byte) (0x33), (byte) (0x08), (byte) (0x00), (byte) (0x45), (byte) (0x00), (byte) (0x00), (byte) (0x3e), (byte) SupportClass.Identity(0x87), (byte) (0x08), (byte) (0x40), (byte) (0x00), (byte) (0x3f), (byte) (0x06), (byte) (0x38), (byte) SupportClass.Identity(0xa2), (byte) SupportClass.Identity(0x87), (byte) (0x0d), (byte) SupportClass.Identity(0xd8), (byte) SupportClass.Identity(0xbf), (byte) SupportClass.Identity(0xac), (byte) (0x10), (byte) (0x70), (byte) (0x32), (byte) (0x50), (byte) (0x49), (byte) (0x00), (byte) (0x19), (byte) SupportClass.Identity(0x9f), (byte) (0x3a), (byte) SupportClass.Identity(0xb4), (byte) (0x03), (byte) (0x78), (byte) SupportClass.Identity(0xbe), (byte) SupportClass.Identity(0xe0), (byte) SupportClass.Identity(0xf8), (byte) (0x50), (byte) (0x18), (byte) (0x7d), (byte) (0x78), (byte) SupportClass.Identity(0x86), (byte) SupportClass.Identity(0xf0), (byte) (0x00), (byte) (0x00), (byte) (0x45), (byte) (0x48), (byte) (0x4c), (byte) (0x4f), (byte) (0x20), (byte) (0x61), (byte) (0x6c), (byte) (0x70), (byte) (0x68), (byte) (0x61), (byte) (0x2e), (byte) (0x61), (byte) (0x70), (byte) (0x70), (byte) (0x6c), (byte) (0x65), (byte) (0x2e), (byte) (0x65), (byte) (0x64), (byte) (0x75), (byte) (0x0d), (byte) (0x0a)};
		
//        public virtual void  setUp()
//        {
//            // get link layer length
//            int linkLayerLen = LinkLayer.getLinkLayerLength(LinkLayers_Fields.EN10MB);
//            // create syn-ack packet
//            _synAck = new IPPacket(linkLayerLen, SYN_ACK_PACKET);
//            // create psh-ack packet
//            _pshAck = new IPPacket(linkLayerLen, PSH_ACK_PACKET);
//            // create packet with random garbage
//            byte[] badBytes = new byte[SYN_ACK_PACKET.Length];
//            (new System.Random()).NextBytes(SupportClass.ToByteArray(badBytes));
//            _baddie = new IPPacket(linkLayerLen, badBytes);
//        }
		
//        public virtual void  tearDown()
//        {
//        }
		
//        public virtual void  testSynAckPacketHeaderLengths()
//        {
//            assertEquals(20, _synAck.IPHeaderLength);
//            assertEquals(20, _synAck.IPHeader.Length);
//            assertEquals(20, _synAck.HeaderLength);
//            assertEquals(20, _synAck.Header.Length);
//        }
		
//        public virtual void  testPshAckPacketHeaderLengths()
//        {
//            assertEquals(20, _pshAck.IPHeaderLength);
//            assertEquals(20, _pshAck.IPHeader.Length);
//            assertEquals(20, _pshAck.HeaderLength);
//            assertEquals(20, _pshAck.Header.Length);
//        }
		
//        public virtual void  testSynAckPacketDataLengths()
//        {
//            assertEquals(24, _synAck.IPData.Length);
//            assertEquals(24, _synAck.getData().Length);
//        }
		
//        public virtual void  testPshAckPacketDataLengths()
//        {
//            assertEquals(42, _pshAck.IPData.Length);
//            assertEquals(42, _pshAck.getData().Length);
//        }
		
//        public virtual void  testSynAckPacketAddresses()
//        {
//            assertEquals("172.16.112.50", _synAck.SourceAddress);
//            assertEquals("135.13.216.191", _synAck.DestinationAddress);
//            assertEquals(2886758450L, _synAck.getSourceAddressAsLong());
//            assertEquals(2265831615L, _synAck.getDestinationAddressAsLong());
//            byte[] srcAdd = _synAck.SourceAddresbytes;
//            assertTrue("Source address as byte array does not match, bytes are: " + noNeg(srcAdd[0]) + "." + noNeg(srcAdd[1]) + "." + noNeg(srcAdd[2]) + "." + noNeg(srcAdd[3]), ((srcAdd[0] == (byte) SupportClass.Identity(172)) && (srcAdd[1] == (byte) 16) && (srcAdd[2] == (byte) 112) && (srcAdd[3] == (byte) 50)));
//            byte[] dstAdd = _synAck.DestinationAddresbytes;
//            assertTrue("Dest address as byte array does not match, bytes are: " + noNeg(dstAdd[0]) + "." + noNeg(dstAdd[1]) + "." + noNeg(dstAdd[2]) + "." + noNeg(dstAdd[3]), ((dstAdd[0] == (byte) SupportClass.Identity(135)) && (dstAdd[1] == (byte) 13) && (dstAdd[2] == (byte) SupportClass.Identity(216)) && (dstAdd[3] == (byte) SupportClass.Identity(191))));
//        }
		
//        public virtual void  testPshAckPacketAddresses()
//        {
//            assertEquals("135.13.216.191", _pshAck.SourceAddress);
//            assertEquals("172.16.112.50", _pshAck.DestinationAddress);
//        }
		
//        private int noNeg(byte b)
//        {
//            return 0 | (b & 0xff);
//        }
		
		
//        public virtual void  testSynAckPacketHeaderValues()
//        {
//            assertEquals(Tamir.IPLib.Packets.IPProtocols_Fields.TCP, _synAck.getIPProtocol());
//            assertEquals(Tamir.IPLib.Packets.IPProtocols_Fields.TCP, _synAck.getIPProtocol());
//            assertEquals("IP Checksum mismatch, should be 0x6c38, but is " + System.Convert.ToString(_synAck.IPChecksum, 16), 0x6c38, _synAck.IPChecksum);
//            assertEquals("(IP) Checksum mismatch, should be 0x6c38, but is " + System.Convert.ToString(_synAck.getChecksum(), 16), 0x6c38, _synAck.getChecksum());
//            IPPacket.TestProbe probe = new Tamir.IPLib.Packets.IPPacket.TestProbe(_synAck);
//            assertTrue("Computed IP checksum mismatch, should be " + System.Convert.ToString(_synAck.IPChecksum, 16) + ", but is " + System.Convert.ToString(probe.ComputedSenderIPChecksum, 16) + ", (" + System.Convert.ToString(probe.ComputedReceiverIPChecksum, 16) + ")", _synAck.ValidChecksum);
//            assertEquals("Version mismatch, should be " + Tamir.IPLib.Packets.IPVersions_Fields.IPV4 + ", but is " + _synAck.Version, Tamir.IPLib.Packets.IPVersions_Fields.IPV4, _synAck.Version);
//            assertEquals("TOS incorrect, should be 0, but is " + _synAck.getTypeOfService(), 0, _synAck.getTypeOfService());
//            assertEquals("Length incorrect, should be 44, but is " + _synAck.getLength(), 44, _synAck.getLength());
//            assertEquals("ID incorrect, should be 0x9383, but is " + _synAck.Id, 0x9383, _synAck.Id);
//            assertEquals("Fragment flags incorrect, should be 0, but are " + _synAck.getFragmentFlags(), 2, _synAck.getFragmentFlags());
//            assertEquals("Fragment offset incorrect, should be 0, but is " + _synAck.FragmentOffset, 0, _synAck.FragmentOffset);
//            assertEquals("Time-to-live incorrect, should be 255, but is " + _synAck.getTimeToLive(), 255, _synAck.getTimeToLive());
//        }
		
//        public virtual void  testPshAckPacketHeaderValues()
//        {
//            assertEquals(Tamir.IPLib.Packets.IPProtocols_Fields.TCP, _pshAck.getIPProtocol());
//            assertEquals(Tamir.IPLib.Packets.IPProtocols_Fields.TCP, _pshAck.getIPProtocol());
//            assertEquals("IP Checksum mismatch, should be 0x38a2, but is " + System.Convert.ToString(_pshAck.IPChecksum, 16), 0x38a2, _pshAck.IPChecksum);
//            assertEquals("(IP) Checksum mismatch, should be 0x38a2, but is " + System.Convert.ToString(_pshAck.getChecksum(), 16), 0x38a2, _pshAck.getChecksum());
//            IPPacket.TestProbe probe = new Tamir.IPLib.Packets.IPPacket.TestProbe(_pshAck);
//            assertTrue("Computed IP checksum mismatch, should be " + System.Convert.ToString(_pshAck.IPChecksum, 16) + ", but is " + System.Convert.ToString(probe.ComputedSenderIPChecksum, 16) + ", (" + System.Convert.ToString(probe.ComputedReceiverIPChecksum, 16) + ")", _pshAck.ValidChecksum);
//            assertEquals("Version mismatch, should be " + Tamir.IPLib.Packets.IPVersions_Fields.IPV4 + ", but is " + _pshAck.Version, Tamir.IPLib.Packets.IPVersions_Fields.IPV4, _pshAck.Version);
//            assertEquals("TOS incorrect, should be 0, but is " + _pshAck.getTypeOfService(), 0, _pshAck.getTypeOfService());
//            assertEquals("Length incorrect, should be 62, but is " + _pshAck.getLength(), 62, _pshAck.getLength());
//            assertEquals("ID incorrect, should be 0x8708, but is " + _pshAck.Id, 0x8708, _pshAck.Id);
//            assertEquals("Fragment flags incorrect, should be 0, but are " + _pshAck.getFragmentFlags(), 2, _pshAck.getFragmentFlags());
//            assertEquals("Fragment offset incorrect, should be 0, but is " + _pshAck.FragmentOffset, 0, _pshAck.FragmentOffset);
//            assertEquals("Time-to-live incorrect, should be 63, but is " + _pshAck.getTimeToLive(), 63, _pshAck.getTimeToLive());
//        }
		
//        public virtual void  testBadPacketHeaderLengths()
//        {
//            // really just make sure this doesn't crash the thing
//            assertTrue("Bad read of IP header for random data", (_baddie.IPHeader.Length >= 0));
//            assertTrue("Bad read of IP header for random data", (_baddie.Header.Length >= 0));
//        }
		
//        public virtual void  testBadPacketDataLengths()
//        {
//            // really just make sure this doesn't crash the thing
//            assertTrue("Bad read of IP data (payload) for random data", (_baddie.IPData.Length >= 0));
//            assertTrue("Bad read of IP data (payload) for random data", (_baddie.getData().Length >= 0));
//        }
//    }
//}