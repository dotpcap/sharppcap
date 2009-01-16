//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//

using System;

namespace SharpPcap.Util
{
	/// <summary>
	/// Contains conversion support elements such as classes, interfaces and static methods.
	/// </summary>
	/// <author>Tamir Gal</author>
	/// <version>  $Revision: 1.3 $ </version>
	/// <lastModifiedBy>  $Author: tamirgal $ </lastModifiedBy>
	/// <lastModifiedAt>  $Date: 2007-07-16 08:49:15 $ </lastModifiedAt>
	public class SupportClass
	{
		/// <summary>
		/// Performs an unsigned bitwise right shift with the specified number
		/// </summary>
		/// <param name="number">Number to operate on</param>
		/// <param name="bits">Ammount of bits to shift</param>
		/// <returns>The resulting number from the shift operation</returns>
		public static int URShift(int number, int bits)
		{
			if ( number >= 0)
				return number >> bits;
			else
				return (number >> bits) + (2 << ~bits);
		}

		/// <summary>
		/// Performs an unsigned bitwise right shift with the specified number
		/// </summary>
		/// <param name="number">Number to operate on</param>
		/// <param name="bits">Ammount of bits to shift</param>
		/// <returns>The resulting number from the shift operation</returns>
		public static int URShift(int number, long bits)
		{
			return URShift(number, (int)bits);
		}

		/// <summary>
		/// Performs an unsigned bitwise right shift with the specified number
		/// </summary>
		/// <param name="number">Number to operate on</param>
		/// <param name="bits">Ammount of bits to shift</param>
		/// <returns>The resulting number from the shift operation</returns>
		public static long URShift(long number, int bits)
		{
			if ( number >= 0)
				return number >> bits;
			else
				return (number >> bits) + (2L << ~bits);
		}

		/// <summary>
		/// Performs an unsigned bitwise right shift with the specified number
		/// </summary>
		/// <param name="number">Number to operate on</param>
		/// <param name="bits">Ammount of bits to shift</param>
		/// <returns>The resulting number from the shift operation</returns>
		public static long URShift(long number, long bits)
		{
			return URShift(number, (int)bits);
		}
		/// <summary>
		/// Writes the exception stack trace to the received stream
		/// </summary>
		/// <param name="throwable">Exception to obtain information from</param>
		/// <param name="stream">Output sream used to write to</param>
		public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}

		/*******************************/
		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static long Identity(long literal)
		{
			return literal;
		}

		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static ulong Identity(ulong literal)
		{
			return literal;
		}

		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static float Identity(float literal)
		{
			return literal;
		}

		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static double Identity(double literal)
		{
			return literal;
		}

		/*******************************/
		/// <summary>
		/// This class uses a cryptographic Random Number Generator to provide support for
		/// strong pseudo-random number generation.
		/// </summary>
		[Serializable]
			public class SecureRandomSupport : System.Runtime.Serialization.ISerializable
		{
			private System.Security.Cryptography.RNGCryptoServiceProvider generator;

			//Serialization
			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
			}

			protected SecureRandomSupport(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				this.generator = new System.Security.Cryptography.RNGCryptoServiceProvider();
			}

			/// <summary>
			/// Initializes a new instance of the random number generator.
			/// </summary>
			public SecureRandomSupport()
			{
				this.generator = new System.Security.Cryptography.RNGCryptoServiceProvider();
			}

			/// <summary>
			/// Initializes a new instance of the random number generator with the given seed.
			/// </summary>
			/// <param name="seed">The initial seed for the generator</param>
			public SecureRandomSupport(byte[] seed)
			{
				this.generator = new System.Security.Cryptography.RNGCryptoServiceProvider(seed);
			}

			/// <summary>
			/// Returns an array of bytes with a sequence of cryptographically strong random values.
			/// </summary>
			/// <param name="randomnumbersarray">The array of bytes to fill.</param>
			public sbyte[] NextBytes(byte[] randomnumbersarray)
			{
				this.generator.GetBytes(randomnumbersarray);
				return ToSByteArray(randomnumbersarray);
			}

			/// <summary>
			/// Returns the given number of seed bytes generated for the first running of a new instance 
			/// of the random number generator.
			/// </summary>
			/// <param name="numberOfBytes">Number of seed bytes to generate.</param>
			/// <returns>Seed bytes generated</returns>
			public static byte[] GetSeed(int numberOfBytes)
			{
				System.Security.Cryptography.RNGCryptoServiceProvider generatedSeed = new System.Security.Cryptography.RNGCryptoServiceProvider();
				byte[] seeds = new byte[numberOfBytes];
				generatedSeed.GetBytes(seeds);
				return seeds;
			}

			/// <summary>
			/// Returns the given number of seed bytes generated for the first running of a new instance 
			/// of the random number generator.
			/// </summary>
			/// <param name="numberOfBytes">Number of seed bytes to generate.</param>
			/// <returns>Seed bytes generated.</returns>
			public byte[] GenerateSeed(int numberOfBytes)
			{
				System.Security.Cryptography.RNGCryptoServiceProvider generatedSeed = new System.Security.Cryptography.RNGCryptoServiceProvider();
				byte[] seeds = new byte[numberOfBytes];
				generatedSeed.GetBytes(seeds);
				return seeds;
			}

			/// <summary>
			/// Creates a new instance of the random number generator with the seed provided by the user.
			/// </summary>
			/// <param name="newSeed">Seed to create a new random number generator.</param>
			public void SetSeed(byte[] newSeed)
			{
				this.generator = new System.Security.Cryptography.RNGCryptoServiceProvider(newSeed);
			}

			/// <summary>
			/// Creates a new instance of the random number generator with the seed provided by the user.
			/// </summary>
			/// <param name="newSeed">Seed to create a new random number generator.</param>
			public void SetSeed(long newSeed)
			{
				byte[] bytes = new byte[8];
				for (int index = 7; index > 0; index--)
				{
					bytes[index] = (byte) (newSeed - (long) ((newSeed >> 8) << 8));
					newSeed  = (long) (newSeed >> 8);
				}
				SetSeed(bytes);
			}
		}


		/*******************************/
		/// <summary>
		/// Receives a byte array and returns it transformed in an sbyte array
		/// </summary>
		/// <param name="byteArray">Byte array to process</param>
		/// <returns>The transformed array</returns>
		public static sbyte[] ToSByteArray(byte[] byteArray)
		{
			sbyte[] sbyteArray = null;
			if (byteArray != null)
			{
				sbyteArray = new sbyte[byteArray.Length];
				for(int index=0; index < byteArray.Length; index++)
					sbyteArray[index] = (sbyte) byteArray[index];
			}
			return sbyteArray;
		}

		/*******************************/
		/// <summary>
		/// Converts an array of sbytes to an array of bytes
		/// </summary>
		/// <param name="sbyteArray">The array of sbytes to be converted</param>
		/// <returns>The new array of bytes</returns>
		public static byte[] ToByteArray(sbyte[] sbyteArray)
		{
			byte[] byteArray = null;

			if (sbyteArray != null)
			{
				byteArray = new byte[sbyteArray.Length];
				for(int index=0; index < sbyteArray.Length; index++)
					byteArray[index] = (byte) sbyteArray[index];
			}
			return byteArray;
		}

		/// <summary>
		/// Converts a string to an array of bytes
		/// </summary>
		/// <param name="sourceString">The string to be converted</param>
		/// <returns>The new array of bytes</returns>
		public static byte[] ToByteArray(System.String sourceString)
		{
			return System.Text.UTF8Encoding.UTF8.GetBytes(sourceString);
		}

		/// <summary>
		/// Converts a array of object-type instances to a byte-type array.
		/// </summary>
		/// <param name="tempObjectArray">Array to convert.</param>
		/// <returns>An array of byte type elements.</returns>
		public static byte[] ToByteArray(System.Object[] tempObjectArray)
		{
			byte[] byteArray = null;
			if (tempObjectArray != null)
			{
				byteArray = new byte[tempObjectArray.Length];
				for (int index = 0; index < tempObjectArray.Length; index++)
					byteArray[index] = (byte)tempObjectArray[index];
			}
			return byteArray;
		}

		/*******************************/
		//Provides access to a static System.Random class instance
		static public System.Random Random = new System.Random();

	}
}
