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
            Assert.Multiple(() =>
            {
                Assert.That(p1 < p2, Is.True, "p1 < p2");
                Assert.That(p2 < p1, Is.False, "p2 < p1");
                Assert.That(p2 < p3, Is.True, "p2 < p3");
                Assert.That(p1 < p3, Is.True, "p1 < p3");
                Assert.That(p1 < p5, Is.False, "p1 < p5");

                Assert.That(p2 > p1, Is.True, "p2 > p1");
                Assert.That(p3 > p2, Is.True, "p3 > p2");
                Assert.That(p3 > p1, Is.True, "p3 > p1");

                Assert.That(p1 != p2, Is.True, "p1 != p2");

                Assert.That(p1 == p4, Is.True, "p1 == p4");

                Assert.That(p1 <= p2, Is.True, "p1 <= p2");
                Assert.That(p1 <= p3, Is.True, "p1 <= p3");
                Assert.That(p2 <= p1, Is.False, "p2 <= p1");
                Assert.That(p2 >= p1, Is.True, "p2 >= p1");

                Assert.That(p1.CompareTo(p4), Is.EqualTo(0));
                Assert.That(p1.CompareTo(p2), Is.EqualTo(-1));
                Assert.That(p2.CompareTo(p1), Is.EqualTo(1));

                Assert.That(p1.Equals(p4), Is.EqualTo(true));
                Assert.That(p1.Equals(p2), Is.EqualTo(false));
            });
        }

        // Test string formatting output
        [Test]
        public void ToStringTest()
        {
            var p1 = new PosixTimeval(123, 12345);

            Assert.That(p1.ToString(), Is.EqualTo("123.012345s"));
        }

        [Test]
        public void HashTest()
        {
            Assert.That(p2.GetHashCode(), Is.Not.EqualTo(p1.GetHashCode()));
            Assert.That(p4.GetHashCode(), Is.EqualTo(p1.GetHashCode()));
        }

        [Test]
        public void DateTimeConversion()
        {
            var now = DateTime.Now;
            var pX = new PosixTimeval(now);
            Assert.That(now.ToUniversalTime().Ticks, Is.EqualTo(pX.Date.Ticks).Within(TimeSpan.TicksPerMillisecond * 1.0));
        }

        [Test]
        public void EmptyConstructor()
        {
            var pX = new PosixTimeval();
            Assert.That(DateTime.UtcNow.Ticks, Is.EqualTo(pX.Date.Ticks).Within(TimeSpan.TicksPerMillisecond * 1.0));
        }
    }
}
