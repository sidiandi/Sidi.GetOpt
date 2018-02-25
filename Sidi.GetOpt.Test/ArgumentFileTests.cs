using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt.Test
{
    [TestFixture]
    public class ArgumentFileTests
    {
        [Test]
        public void Read()
        {
            var r = new System.IO.StringReader(@"
# this is a comment
       argument1 argument2

argument 3
""argument 4""

""Quoted argument with a \"" character""

<<EndOfText
This
is
a
multiline
string<<EndOfText

");
            var a = ArgumentFile.Read(r).ToArray();
            Assert.AreEqual("argument1", a[0]);
            Assert.AreEqual("argument2", a[1]);
            Assert.AreEqual("argument", a[2]);
            Assert.AreEqual("3", a[3]);
            Assert.AreEqual("argument 4", a[4]);
            Assert.AreEqual("Quoted argument with a \" character", a[5]);
            Assert.AreEqual(@"This
is
a
multiline
string"
                , a[6]);

        }
    }
}
