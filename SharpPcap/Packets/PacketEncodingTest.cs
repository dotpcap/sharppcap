//// $Id: PacketEncodingTest.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

///// <summary>************************************************************************
///// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
///// Distributed under the Mozilla Public License                            *
///// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
///// *************************************************************************
///// </summary>
//using System;
////UPGRADE_TODO: The package 'junit.framework' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
//using junit.framework;
//namespace SharpPcap.Packets
//{
	
//    /// <summary> </summary>
//    /// <author>  jguthrie
//    /// </author>
//    public class PacketEncodingTest:TestCase
//    {
//        internal byte[] testBytes = new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
//        internal int headerLen = 3;
//        internal byte[] goodHeader0 = new byte[]{1, 2, 3};
//        internal byte[] goodData0 = new byte[]{4, 5, 6, 7, 8, 9, 10};
//        internal byte[] goodSizedData0 = new byte[]{4, 5, 6, 7};
//        internal byte[] goodHeader2 = new byte[]{3, 4, 5};
//        internal byte[] goodData2 = new byte[]{6, 7, 8, 9, 10};
//        internal byte[] goodSizedData2 = new byte[]{6, 7, 8, 9};
		
//        public PacketEncodingTest(System.String testName):base(testName)
//        {
//        }
		
//        [STAThread]
//        public static void  Main(System.String[] args)
//        {
//            junit.textui.TestRunner.run(suite());
//        }
		
//        public static Test suite()
//        {
//            TestSuite suite = new TestSuite(typeof(PacketEncodingTest));
			
//            return suite;
//        }
		
//        private bool sameBytes(byte[] b1, byte[] b2)
//        {
//            if ((b1 == null) || (b2 == null))
//                return false; // nulls are bad
//            if (b1.Length != b2.Length)
//                return false; // different lengths are bad
//            for (int i = 0; i < b1.Length; i++)
//            {
//                if (b1[i] != b2[i])
//                    return false; // different values are bad
//            }
//            return true; // nothing bad, so that's good
//        }
		
//        private System.String bytesAsString(byte[] bytes)
//        {
//            if (bytes == null)
//                return "null";
//            System.Text.StringBuilder buf = new System.Text.StringBuilder("[");
//            System.String sep = "";
//            for (int i = 0; i < bytes.Length; i++)
//            {
//                buf.Append(sep);
//                sep = ",";
//                buf.Append((byte) bytes[i]);
//            }
//            buf.Append("]");
//            return buf.ToString();
//        }
		
//        /// <summary>Test of extractHeader method with header start at position 0 </summary>
//        public virtual void  testExtractHeaderAtPosition0()
//        {
//            byte[] header = PacketEncoding.extractHeader(0, headerLen, testBytes);
//            assertTrue("byte array mismatch, " + bytesAsString(header) + ", should be " + bytesAsString(goodHeader0), sameBytes(header, goodHeader0));
//        }
		
//        /// <summary>Test of extractHeader method with header start at position 2 </summary>
//        public virtual void  testExtractHeaderAtPosition2()
//        {
//            byte[] header = PacketEncoding.extractHeader(2, headerLen, testBytes);
//            assertTrue("byte array mismatch, " + bytesAsString(header) + ", should be " + bytesAsString(goodHeader2), sameBytes(header, goodHeader2));
//        }
		
//        /// <summary>Test of extractHeader method with offset out of bounds.
//        /// Should return 0 length byte array 
//        /// </summary>
//        public virtual void  testExtractHeaderTooLargeOffset()
//        {
//            byte[] header = PacketEncoding.extractHeader(20, headerLen, testBytes);
//            assertTrue("bad parameter did not return 0-length array", (header.Length == 0));
//        }
		
//        /// <summary>Test of extractHeader method with offset out of bounds.
//        /// Should return whole byte array 
//        /// </summary>
//        public virtual void  testExtractHeaderTooLargeLength()
//        {
//            byte[] header = PacketEncoding.extractHeader(0, 20, testBytes);
//            assertTrue("byte array mismatch, " + bytesAsString(header) + ", should be " + bytesAsString(testBytes), sameBytes(header, testBytes));
//        }
		
//        /// <summary>Test of extractHeader method with negative offset. </summary>
//        public virtual void  testExtractHeaderNegativeOffset()
//        {
//            byte[] header = PacketEncoding.extractHeader(- 20, headerLen, testBytes);
//            assertTrue("negative parameter did not return 0-length array", (header.Length == 0));
//        }
		
//        /// <summary>Test of extractHeader method with negative header length.
//        /// Should return 0 length byte array 
//        /// </summary>
//        public virtual void  testExtractHeaderNegativeLength()
//        {
//            byte[] header = PacketEncoding.extractHeader(0, - 20, testBytes);
//            assertTrue("negative parameter did not return 0-length array", (header.Length == 0));
//        }
		
//        /// <summary>Test of extractHeader method with null input array.
//        /// Should return null byte array 
//        /// </summary>
//        public virtual void  testExtractHeaderNullArray()
//        {
//            byte[] header = PacketEncoding.extractHeader(0, 20, null);
//            assertNull("null in did not return null", header);
//        }
		
//        /// <summary>Test of extractData method with header start at position 0 </summary>
//        public virtual void  testExtractDataAtPosition0()
//        {
//            byte[] data = PacketEncoding.extractData(0, headerLen, testBytes);
//            assertTrue("byte array mismatch, " + bytesAsString(data) + ", should be " + bytesAsString(goodData0), sameBytes(data, goodData0));
//        }
		
//        /// <summary>Test of extractData method with header start at position 2 </summary>
//        public virtual void  testExtractDataAtPosition2()
//        {
//            byte[] data = PacketEncoding.extractData(2, headerLen, testBytes);
//            assertTrue("byte array mismatch, " + bytesAsString(data) + ", should be " + bytesAsString(goodData2), sameBytes(data, goodData2));
//        }
		
//        /// <summary>Test of extractData method with offset out of bounds.
//        /// Should return 0 length byte array 
//        /// </summary>
//        public virtual void  testExtractDataTooLargeOffset()
//        {
//            byte[] data = PacketEncoding.extractData(20, headerLen, testBytes);
//            assertTrue("bad parameter did not return 0-length array", (data.Length == 0));
//        }
		
//        /// <summary>Test of extractData method with header length out of bounds.
//        /// Should return whole byte array 
//        /// </summary>
//        public virtual void  testExtractDataTooLargeLength()
//        {
//            byte[] data = PacketEncoding.extractData(0, 20, testBytes);
//            assertTrue("bad parameter did not return 0-length array", (data.Length == 0));
//        }
		
//        /// <summary>Test of extractData method with negative offset.
//        /// Should return 0 length byte array 
//        /// </summary>
//        public virtual void  testExtractDataNegativeOffset()
//        {
//            byte[] data = PacketEncoding.extractData(- 20, headerLen, testBytes);
//            assertTrue("negative parameter did not return 0-length array", (data.Length == 0));
//        }
		
//        /// <summary>Test of extractData method with negative header length.
//        /// Should return 0 length byte array 
//        /// </summary>
//        public virtual void  testExtractDataNegativeLength()
//        {
//            byte[] data = PacketEncoding.extractData(0, - 20, testBytes);
//            assertTrue("negative parameter did not return 0-length array", (data.Length == 0));
//        }
		
//        /// <summary>Test of extractData method with null input array.
//        /// Should return null byte array 
//        /// </summary>
//        public virtual void  testExtractDataNullArray()
//        {
//            byte[] data = PacketEncoding.extractData(0, 20, null);
//            assertNull("null in did not return null", data);
//        }
		
//        /// <summary>Test of extractData method with header start at position 0 </summary>
//        public virtual void  testExtractSizedDataAtPosition0()
//        {
//            byte[] data = PacketEncoding.extractData(0, headerLen, testBytes, 4);
//            assertTrue("byte array mismatch, " + bytesAsString(data) + ", should be " + bytesAsString(goodSizedData0), sameBytes(data, goodSizedData0));
//        }
		
//        /// <summary>Test of extractData method with header start at position 0 </summary>
//        public virtual void  testExtractSizedDataAtPosition2()
//        {
//            byte[] data = PacketEncoding.extractData(2, headerLen, testBytes, 4);
//            assertTrue("byte array mismatch, " + bytesAsString(data) + ", should be " + bytesAsString(goodSizedData2), sameBytes(data, goodSizedData2));
//        }
		
//        /// <summary>Test of extractData method with offset out of bounds.
//        /// Should return 0 length byte array 
//        /// </summary>
//        public virtual void  testExtractSizedDataTooLargeOffset()
//        {
//            byte[] data = PacketEncoding.extractData(20, headerLen, testBytes, 4);
//            assertTrue("bad parameter did not return 0-length array", (data.Length == 0));
//        }
		
//        /// <summary>Test of extractData method with header length out of bounds.
//        /// Should return whole byte array 
//        /// </summary>
//        public virtual void  testExtractSizedDataTooLargeLength()
//        {
//            byte[] data = PacketEncoding.extractData(0, 20, testBytes, 4);
//            assertTrue("bad parameter did not return 0-length array", (data.Length == 0));
//        }
		
//        /// <summary>Test of extractData method with negative size.
//        /// Should return 0 length byte array 
//        /// </summary>
//        public virtual void  testExtractSizedDataNegativeSize()
//        {
//            byte[] data = PacketEncoding.extractData(0, headerLen, testBytes, - 2);
//            assertTrue("bad parameter did not return 0-length array", (data.Length == 0));
//        }
		
//        /// <summary>Test of extractData method with header length out of bounds.
//        /// Should return whole byte array 
//        /// </summary>
//        public virtual void  testExtractSizedDataTooLargeSize()
//        {
//            byte[] data = PacketEncoding.extractData(0, headerLen, testBytes, 20);
//            assertTrue("byte array mismatch, " + bytesAsString(data) + ", should be " + bytesAsString(goodData0), sameBytes(data, goodData0));
//        }
		
//        /// <summary>Test of extractData method with null input array.
//        /// Should return null byte array 
//        /// </summary>
//        public virtual void  testExtractSizedDataNullArray()
//        {
//            byte[] data = PacketEncoding.extractData(0, 20, null, 4);
//            assertNull("null in did not return null", data);
//        }
//    }
//}