/*
Copyright (c) 2006 Tamir Gal, http://www.tamirgal.com, All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

    1. Redistributions of source code must retain the above copyright notice,
        this list of conditions and the following disclaimer.

    2. Redistributions in binary form must reproduce the above copyright 
        notice, this list of conditions and the following disclaimer in 
        the documentation and/or other materials provided with the distribution.

    3. The names of the authors may not be used to endorse or promote products
        derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Net;
using System.Text.RegularExpressions;

namespace SharpPcap.Util
{
    public class IPUtil
    {
        /// <param name="ipAndMaskBits">
        /// </param>
        /// <returns>
        /// </returns>
        public static System.String ExtractIp(System.String ipRange)
        {
            if (IsRangeWithMaskBits(ipRange))
            {
                Match m = Regex.Match(ipRange, REGEX_IP_AND_MASK_BITS);
                if (m.Success)
                {
                    return m.Groups[1].Value;
                }
                throw new System.Exception("IpUtil.ExtractIp(): bad IP format: " + ipRange);
            }
            else if (IsRangeWithDottedMask(ipRange))
            {
                return ipRange.Split(new char[]{' '})[0];
            }
            else
            {
                throw new System.Exception("IpUtil.ExtractIp(): bad IP format: " + ipRange);
            }
        }
        
        /// <param name="ipAndMaskBits">
        /// </param>
        /// <returns>
        /// </returns>
        public static int ExtractMaskBits(System.String ipRange)
        {
            if (IsRangeWithMaskBits(ipRange))
            {
                Match m = Regex.Match(ipRange, REGEX_IP_AND_MASK_BITS);
                if (m.Success)
                {
                    return int.Parse( m.Groups[2].Value );
                }
                throw new System.Exception("IpUtil.ExtractMaskBits(): bad IP format: " + ipRange);
            }
            else if (IsRangeWithDottedMask(ipRange))
            {
                System.String mask = ipRange.Split(new char[]{' '})[1];
                return MaskToBits(mask);
            }
            else
            {
                throw new System.Exception("IpUtil.ExtractMaskBits(): bad IP format: " + ipRange);
            }
        }
        
        /// <param name="dottedIP">
        /// </param>
        /// <param name="maskLength">
        /// </param>
        /// <returns>
        /// </returns>
        public static System.String ApplyMask(System.String dottedIP, int maskLength)
        {
            long mask = IpToLong(MaskToString(maskLength));
            long ip = IpToLong(dottedIP);
            ip &= mask;
            return IpToString(ip);
        }
        
        //From: http://www.ip2location.com/README-IP-COUNTRY.htm
        /// <param name="dottedIP">
        /// </param>
        /// <returns>
        /// </returns>
        public static long IpToLong(System.String dottedIP)
        {
            int i;
            System.String[] arrDec;
            double num = 0;
            if (dottedIP.Equals(""))
            {
                return 0;
            }
            else
            {
                arrDec = dottedIP.Split(new char[]{'.'});
                for (i = arrDec.Length - 1; i >= 0; i--)
                {
                    num += ((System.Int64.Parse(arrDec[i]) % 256) * System.Math.Pow(256, (3 - i)));
                }
                //UPGRADE_WARNING: Data types in Visual C# might be different.  Verify the accuracy of narrowing conversions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1042'"
                return (long) num;
            }
        }
        
        /// <param name="ip">
        /// </param>
        /// <returns>
        /// </returns>
        public static System.String IpToString(long ip)
        {           
            byte[] bytes = IpToBytes(ip);
            return (IpToString(bytes));
        }
        
        public static byte[] IpToBytes(System.String ip)
        {
            return IpToBytes(IpToLong(ip));
        }
        
        public static byte[] IpToBytes(long ip)
        {
            byte[] tmp = new byte[4];
            long val = ip;
            tmp[0] = (byte) (val >> 24);
            tmp[1] = (byte) (val >> 16);
            tmp[2] = (byte) (val >> 8);
            tmp[3] = (byte) (val);
            return tmp;
        }
        
        public static System.String IpToString(byte[] ip)
        {
            return (((int) ip[0] & 0x000000ff) + "." + ((int) ip[1] & 0x000000ff) + "." + ((int) ip[2] & 0x000000ff) + "." + ((int) ip[3] & 0x000000ff));
        }
        
        /// <param name="numBits">
        /// </param>
        /// <returns>
        /// </returns>
        public static System.String MaskToString(int numBits)
        {
            /*
            * Based on:
            * http://javaalmanac.com/egs/java.util/Bits2Array.html
            */
            int[] bytes = new int[4];
            for (int i = 0; i < 32; i++)
            {
                if (i < numBits)
                {
                    bytes[bytes.Length - i / 8 - 1] |= 1 << (7 - i % 8); //the "1<<(7-i%8)" statement reverses the bits within the byte
                }
            }
            //java uses big-endian:
            return bytes[3] + "." + bytes[2] + "." + bytes[1] + "." + bytes[0];
        }
        
        public static long MaskToLong(int bits)
        {
            return IpToLong(MaskToString(bits));
        }
        
        public static long MaskToLong(System.String dottedMask)
        {
            return IpToLong(dottedMask);
        }
        
        public static int MaskToBits(System.String mask)
        {
            return MaskToBits(IpToBytes(mask));
        }
        
        public static int MaskToBits(long mask)
        {
            return MaskToBits(IpToBytes(mask));
        }
        
        public static int MaskToBits(byte[] mask)
        {
            /*
            * Based on:
            * http://javaalmanac.com/egs/java.util/Bits2Array.html
            */
            int bits = 0;
            byte[] bytes = mask;
            for (int i = 0; i < bytes.Length * 8; i++)
            {
                if ((bytes[bytes.Length - i / 8 - 1] & (1 << (i % 8))) > 0)
                {
                    bits++;
                }
            }
            return bits;
        }
        
        public static bool IsRange(System.String ipRange)
        {
            return IsRangeWithMaskBits(ipRange) || IsRangeWithDottedMask(ipRange);
        }
        
        public static bool IsRangeWithMaskBits(System.String range)
        {
            return Regex.Match(range, REGEX_IP_AND_MASK_BITS).Success;
        }
        
        public static bool IsRangeWithDottedMask(System.String range)
        {
            return (range.IndexOf(" ")!=-1 && IsIP(range.Split(new char[]{' '})[0]) && IsIP(range.Split(new char[]{' '})[1]));
        }
        
        public static bool IsIP(System.String ip)
        {
            return Regex.Match(ip, REGEX_IP_ADDR).Success;
        }

        public static string REGEX_IP_AND_MASK_BITS = "(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})/(\\d{1,2})";
        public static string REGEX_IP_ADDR = "^(\\d{1,3})\\.(\\d{1,3})\\.(\\d{1,3})\\.(\\d{1,3})$";

        public static short Ntoh(short val)
        {
            return IPAddress.NetworkToHostOrder(val);
        }

        public static int Ntoh(int val)
        {
            return IPAddress.NetworkToHostOrder(val);
        }

        public static long Ntoh(long val)
        {
            return IPAddress.NetworkToHostOrder(val);
        }

        public static short Hton(short val)
        {
            return IPAddress.HostToNetworkOrder(val);
        }

        public static int Hton(int val)
        {
            return IPAddress.HostToNetworkOrder(val);
        }

        public static long Hton(long val)
        {
            return IPAddress.HostToNetworkOrder(val);
        }

    }
}
