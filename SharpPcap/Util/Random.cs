using System;
using System.Security.Cryptography;
namespace SharpPcap.Util
{
    public class Rand
    {
        RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();

        public static Rand CreateInstance()
        {
            Rand r = new Rand();
            return r;           
        }
        private static Rand singleton = CreateInstance();

        public static Rand Instance
        {
            get
            {
                return singleton;
            }
        }

        /// <summary>
        /// Returns the given number of seed bytes generated for the first running of a new instance 
        /// of the random number generator.
        /// </summary>
        /// <param name="numberOfBytes">Number of seed bytes to generate.</param>
        /// <returns>Seed bytes generated</returns>
        public static byte[] GetSeed(int numberOfBytes)
        {
            RNGCryptoServiceProvider generatedSeed = new RNGCryptoServiceProvider();
            byte[] seeds = new byte[numberOfBytes];
            generatedSeed.GetBytes(seeds);
            return seeds;
        }

        public byte[] GetBytes(byte[] bytes)
        {
            generator.GetBytes(bytes);
            return bytes;
        }

        public byte[] GetBytes(int size)
        {
            byte[] bytes = new byte[size];
            generator.GetBytes(bytes);
            return bytes;
        }

        public double GetDouble()
        {
#if false
            byte[] bytes = GetBytes(8);
            double d = System.BitConverter.ToDouble(bytes, 0);
            return d;
#else
            return (new Random(GetInt())).NextDouble();
#endif
        }

        public long GetLong()
        {
            byte[] bytes = GetBytes(8);
            long i = System.BitConverter.ToInt32(bytes, 0);
            return i;
        }

        public long GetLong(long min, long max)
        {
            double _min = min;
            double _max = max;
            double dif = (_max - _min + 1);
            double final = ((GetDouble() * dif) + _min);
            return (long)final;
        }

        public long GetLong(long max)
        {
            return GetLong(0, max);
        }

        public int GetInt()
        {
            byte[] bytes = GetBytes(4);
            int i = System.BitConverter.ToInt32(bytes, 0);
            return i;
        }

        public int GetInt(int min, int max)
        {
            return (int)GetLong(min, max);
        }

        public int GetInt(int max)
        {
            return (int)GetLong(0, max);
        }
    }
}
