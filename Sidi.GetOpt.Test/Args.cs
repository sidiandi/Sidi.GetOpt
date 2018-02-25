using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sidi.GetOpt.Test
{
    [TestFixture]
    public class ArgsTests
    {
        [Test]
        public void Insert()
        {
            var a = new Args(Enumerable.Range(0, 10).Select(_ => _.ToString()));
            a.Insert(Enumerable.Range(100, 10).Select(_ => _.ToString()));
            a.MoveNext();
            Assert.AreEqual("100", a.Current);
            Console.WriteLine(a);
        }

        [Test]
        public void Insert2()
        {
            var a = new Args(Enumerable.Range(0, 10).Select(_ => _.ToString()));
            a.MoveNext();
            Assert.AreEqual("0", a.Current);
            Console.WriteLine(a);
            a.Insert(Enumerable.Range(100, 10).Select(_ => _.ToString()));
            Assert.AreEqual("0", a.Current);
            a.MoveNext();
            Console.WriteLine(a);
            Assert.AreEqual("100", a.Current);
        }
    }
}