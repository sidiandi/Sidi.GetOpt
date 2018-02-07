using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt.Test
{
    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void TryRemovePrefix()
        {
            var textWithPrefix = "--cordially";
            Assert.IsTrue(textWithPrefix.TryRemovePrefix("--", out var textWithoutPrefix));
            Assert.AreEqual("cordially", textWithoutPrefix);
        }
    }
}
