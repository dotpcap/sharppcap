// Copyright 2010 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

using System;
using NUnit.Framework;
using SharpPcap;

namespace Test.Misc
{
    [TestFixture]
    public class PosixTimevalTest
    {
        static PosixTimeval p1 = new PosixTimeval(100, 50);
        static PosixTimeval p2 = new PosixTimeval(100, 100);
        static PosixTimeval p3 = new PosixTimeval(200, 20);

        static PosixTimeval p4 = new PosixTimeval(100, 50);

        static PosixTimeval p5 = new PosixTimeval(100, 20);

        // Test posix timeval comparison operators
        [Test]
        public void OperatorTest()
        {
            Assert.IsTrue(p1 < p2, "p1 < p2");
            Assert.IsFalse(p2 < p1, "p2 < p1");
            Assert.IsTrue(p2 < p3, "p2 < p3");
            Assert.IsTrue(p1 < p3, "p1 < p3");
            Assert.IsFalse(p1 < p5, "p1 < p5");

            Assert.IsTrue(p2 > p1, "p2 > p1");
            Assert.IsTrue(p3 > p2, "p3 > p2");
            Assert.IsTrue(p3 > p1, "p3 > p1");

            Assert.IsTrue(p1 != p2, "p1 != p2");

            Assert.IsTrue(p1 == p4, "p1 == p4");

            Assert.IsTrue(p1 <= p2, "p1 <= p2");
            Assert.IsTrue(p1 <= p3, "p1 <= p3");
            Assert.IsFalse(p2 <= p1, "p2 <= p1");
            Assert.IsTrue(p2 >= p1, "p2 >= p1");

            Assert.AreEqual(p1.CompareTo(p4), 0);
            Assert.AreEqual(p1.CompareTo(p2), -1);
            Assert.AreEqual(p2.CompareTo(p1), 1);

            Assert.AreEqual(p1.Equals(p4), true);
            Assert.AreEqual(p1.Equals(p2), false);
        }

        // Test string formatting output
        [Test]
        public void ToStringTest()
        {
            var p1 = new PosixTimeval(123, 12345);

            Assert.AreEqual("123.012345s", p1.ToString());
        }

        [Test]
        public void HashTest()
        {
            Assert.AreNotEqual(p1.GetHashCode(), p2.GetHashCode());
            Assert.AreEqual(p1.GetHashCode(), p4.GetHashCode());
        }

        [Test]
        public void DateTimeConversion()
        {
            var now = DateTime.Now;
            var pX = new PosixTimeval(now);
            Assert.AreEqual(pX.Date.Ticks, now.ToUniversalTime().Ticks, TimeSpan.TicksPerMillisecond * 1.0);
        }

        [Test]
        public void EmptyConstructor()
        {
            var pX = new PosixTimeval();
            Assert.AreEqual(pX.Date.Ticks, DateTime.UtcNow.Ticks, TimeSpan.TicksPerMillisecond * 1.0);
        }
    }
}
