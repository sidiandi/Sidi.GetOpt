using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sidi.GetOpt.Test
{
    [TestFixture]
    public class OptionArgumentFileTest
    {
        [Test]
        public void ArgumentFile()
        {
            var hw = new HelloWorld();
            var argFile = Path.GetTempFileName();
            using (var w = new StreamWriter(argFile))
            {
                w.WriteLine(@"# test of argument files for Sidi.GetOpt

Name1

""GivenName Family Name""

<<eot
Name
with
line breaks<<eot

");
            }

            int e = 0;
            using (new CaptureConsoleOutput())
            {
                e = Sidi.GetOpt.GetOpt.Run(hw, new[] { "--argument-file=" + argFile });
            }
            Assert.AreEqual(0, e);
        }

        [Test]
        public void ArgumentFileNotFound()
        {
            var hw = new HelloWorld();
            var argFile = Path.GetRandomFileName();

            int e = 0;
            using (new CaptureConsoleOutput())
            {
                e = Sidi.GetOpt.GetOpt.Run(hw, new[] { "-@" + argFile });
            }
            Assert.AreEqual(-1, e);
        }

        [Test]
        public void ArgumentFileShortOption()
        {
            var hw = new HelloWorld();
            var argFile = Path.GetTempFileName();
            using (var w = new StreamWriter(argFile))
            {
                w.WriteLine(@"# test of argument files for Sidi.GetOpt

Name1

""GivenName Family Name""

<<eot
Name
with
line breaks<<eot

");
            }

            int e = 0;
            using (new CaptureConsoleOutput())
            {
                e = Sidi.GetOpt.GetOpt.Run(hw, new[] { "-@" + argFile });
            }
            Assert.AreEqual(0, e);
        }
    }
}