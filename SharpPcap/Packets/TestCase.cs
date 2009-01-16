using System;

namespace Tamir.IPLib.Packets
{
	/// <summary>
	/// Summary description for TestCase.
	/// </summary>
	public class TestCase
	{
		string name;
		public TestCase()
		{
		}

		public TestCase(string name)
		{
			this.name=name;
		}

		public void assertTrue(bool exp)
		{
			if(!exp)
				Console.WriteLine(exp);
		}

		public void assertTrue(string msg, bool exp)
		{
			if(!exp)
				Console.WriteLine(msg);
		}

		public void assertEquals(object o1, object o2)
		{
			if(!o1.Equals(o2))
			{
				Console.WriteLine("Not eqals");
			}
		}
		public void assertEquals(string msg, object o1, object o2)
		{
			if(!o1.Equals(o2))
			{
				Console.WriteLine(msg);
			}
		}
	}

	/// <summary>
	/// Summary description for TestCase.
	/// </summary>
	public class Test
	{
		public Test()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
