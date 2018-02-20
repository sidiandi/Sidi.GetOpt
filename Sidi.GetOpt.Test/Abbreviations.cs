using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt.Test
{
    [TestFixture]
    public class Abbreviations
    {
        [Test]
        public void AbbreviateCommands()
        {
            var c = new Commands();
            using (var o = new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(c, new[] { "c", "s", "1", "1",  "--pr", "-p"});
                Assert.AreEqual(e, 0);
                Assert.AreEqual(2.0, c.Calculator.Result);
            }
        }

        [Test]
        public void NoSuchLongOption()
        {
            var c = new Commands();
            using (var o = new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(c, new[] { "c", "s", "1", "1", "--printer" });
                Assert.AreEqual(e, -1);
            }
        }
    }
}
