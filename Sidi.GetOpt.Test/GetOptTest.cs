using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Reflection;
using System.IO;

namespace Sidi.GetOpt.Test
{
    [TestFixture]
    public class GetOptTest
    {
        [Test]
        public void Run()
        {
            var e = GetOpt.Run(new HelloWorld(), new[] { "Donald", "Dagobert" });
            Assert.AreEqual(0, e);
        }

        [Test]
        public void BasicOptions()
        {
            var a = new HelloWorld();
            var e = GetOpt.Run(a, new[] { "--cordially" , "--cordially" });
            Assert.AreEqual(0, e);
            Assert.AreEqual(2, a.Cordiality);
        }

        [Test]
        public void BasicShortOptions()
        {
            var a = new HelloWorld();
            var e = GetOpt.Run(a, new[] { "-c", "-c" });
            Assert.AreEqual(0, e);
            Assert.AreEqual(2, a.Cordiality);
        }

        [Test]
        public void BasicCombinedShortOptions()
        {
            var a = new HelloWorld();
            var e = GetOpt.Run(a, new[] { "-ccc" });
            Assert.AreEqual(0, e);
            Assert.AreEqual(3, a.Cordiality);
        }

        [Test]
        public void BasicCommand()
        {
            var calc = new Calculator();
            var e = GetOpt.Run(calc, new[] { "add", "1", "1" });
            Assert.AreEqual(0, e);
            Assert.AreEqual(2.0, calc.Result);

            e = GetOpt.Run(calc, new[] { "sum", "1", "2", "3" });
            Assert.AreEqual(0, e);
            Assert.AreEqual(6.0, calc.Result);
        }

        [Test]
        public void OptionsAfterArguments()
        {
            var hw = new HelloWorld();
            using (var c = new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(hw, new[] { "Donald", "--cordially" });
                Assert.AreEqual(0, e);
                StringAssert.Contains("dear Donald", c.output.ToString());
            }
        }

        [Test]
        public void TooManyParameters()
        {
            var calc = new Calculator();
            using (new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(calc, new[] { "add", "1", "1", "1" });
                Assert.AreNotEqual(0, e);
            }
        }

        [Test]
        public void InvalidOption()
        {
            var calc = new Calculator();
            using (new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(calc, new[] { "--adfasdfasdf" });
                Assert.AreNotEqual(0, e);
            }
        }

        [Test]
        public void ShowHelp()
        {
            var calc = new Calculator();
            using (var c = new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(calc, new[] { "--help" });
                Assert.AreEqual(0, e);
                Assert.IsTrue(c.output.ToString().StartsWith("Usage:"));
            }
        }

        [Test]
        public void ShowHelpIfNoArgumentsInMultiCommandMode()
        {
            var calc = new Calculator();
            using (var c = new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(calc, new string[] { });
                Assert.AreEqual(0, e);
                Assert.IsTrue(c.output.ToString().StartsWith("Usage:"));
            }
        }

        [Test]
        public void ShowVersion()
        {
            var calc = new Calculator();
            var e = GetOpt.Run(calc, new[] { "--version" });
            Assert.AreEqual(0, e);
        }

        [Test]
        public void ShowHelpOptionStyle()
        {
            var h = new HelloWorld();
            var e = GetOpt.Run(h, new[] { "--help" });
            Assert.AreEqual(0, e);
        }

        [Test]
        public void ShowHelpForCommand()
        {
            var cmds = new Commands();
            using (var c = new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(cmds, new string[] { });
            }

            using (var c = new CaptureConsoleOutput())
            {
                var e = GetOpt.Run(cmds, new[] { "calculator", "sum", "--help" });
                StringAssert.Contains("Usage: Sidi.GetOpt.Test calculator sum [option]... [a: Double]...", c.output.ToString());
                StringAssert.Contains("--version : Show version information", c.output.ToString());
                Assert.AreEqual(0, e);
                /*
                Assert.AreEqual(@"Usage: Sidi.GetOpt.Test calculator sum [option]... [a: Double]...

Add numbers

Options:
-h|--help : Show this help message
--print : Print results
"
                , c.output.ToString());
                */
            }
        }

        [Test]
        public void CommandClass()
        {
            var commands = new Commands();
            var e = GetOpt.Run(commands, new[] { "--help" });
            Assert.AreEqual(0, e);

            Assert.AreEqual(0, GetOpt.Run(commands, new[] { "calculator", "--help" }));

        }

        [Test]
        public void OptionsInSubcommandsAreConsidered()
        {
            var commands = new Commands();
            using (var c = new CaptureConsoleOutput())
            {
                Assert.AreEqual(0, GetOpt.Run(commands, new[] { "calculator", "--print", "sum", "1", "2", "3" }));
                Assert.AreEqual(true, commands.Calculator.Print);
                Assert.AreEqual(6.0, commands.Calculator.Result);
            }
        }

        [Test]
        public void OptionsAfterArgumentsAreConsidered()
        {
            var commands = new Commands();
            using (var c = new CaptureConsoleOutput())
            {
                Assert.AreEqual(0, GetOpt.Run(commands, new[] { "calculator", "sum", "1", "2", "3", "--print" }));
                Assert.AreEqual(true, commands.Calculator.Print);
                Assert.AreEqual(6.0, commands.Calculator.Result);
            }
        }

        [Test]
        public void EmptyApplication()
        {
            var e = GetOpt.Run(new Empty(), new[] { "--help" });
            Assert.AreEqual(0, e);
        }

        [Test]
        public void ParameterlessCommand()
        {
            var p = new ParameterlessMethodApplication();
            var e = GetOpt.Run(p, new string[] { "--some-option" });
            Assert.AreEqual(0, e);
            Assert.IsTrue(p.SomeOption);
            Assert.IsTrue(p.MainWasCalled);
        }

        [Test]
        public void OptionStyleApplication()
        {
            var args = Enumerable.Range(0, 10).Select(_ => _.ToString()).ToArray();
            var a = new OptionsApplication();
            var e = GetOpt.Run(a, args);
            Assert.AreEqual(0, e);
            Assert.IsTrue(args.SequenceEqual(a.arguments), string.Join(",", a.arguments)); 
        }

        [Test]
        public void OptionStyleApplicationUsage()
        {
            var a = new OptionsApplication();
            var e = GetOpt.Run(a, new[] { "--help" });
            Assert.AreEqual(0, e);
        }

        [Test]
        public void CallAsyncMethodsCorrectly()
        {
            var hw = new TestAsyncApp();
            var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "TestAsyncWithIntResult" });
            Assert.IsTrue(hw.TestAsyncWasCalled);
            Assert.AreEqual(123, exitCode);
        }

        [Test]
        public void CallAsyncMethodsCorrectly2()
        {
            var hw = new TestAsyncApp();
            var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "TestAsyncWithStringResult" });
            Assert.IsTrue(hw.TestAsyncWasCalled);
            Assert.AreEqual(0, exitCode);
        }

        [Test]
        public void HandleExceptionsInAsyncMethodsCorrectly()
        {
            using (var c = new CaptureConsoleOutput())
            {
                var hw = new TestAsyncApp();
                var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "TestAsyncWithException" });
                Assert.IsTrue(hw.TestAsyncWasCalled);
                Assert.AreEqual(-1, exitCode);
                StringAssert.Contains(TestAsyncApp.anErrorOccured, c.error.ToString());
            }
        }

        [Test]
        public void CustomExceptionHandler()
        {
            using (var c = new CaptureConsoleOutput())
            {
                var hw = new TestAsyncApp();
                var go = new Sidi.GetOpt.GetOpt
                {
                    Application = hw,
                    Arguments = new[] { "TestAsyncWithException" },
                    OnException = (e) => -123
                };
                var exitCode = go.Run();
                Assert.IsTrue(hw.TestAsyncWasCalled);
                Assert.AreEqual(-123, exitCode);
            }
        }

        [Test]
        public void ParseValues()
        {
            using (var c = new CaptureConsoleOutput())
            {
                var a = new ValueParserTestApplication();
                Assert.AreEqual(0, Sidi.GetOpt.GetOpt.Run(a, new[] { "--birthday=2394-03-01" }));
                Assert.AreEqual(new DateTime(2394, 3, 1, 0, 0, 0), a.Birthday);

                Assert.AreEqual(0, Sidi.GetOpt.GetOpt.Run(a, new[] { "--duration=1.2:3:4.5" }));
                Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 500), a.Duration);

                Assert.AreEqual(0, Sidi.GetOpt.GetOpt.Run(a, new[] { "--duration=0:10" }));
                Assert.AreEqual(TimeSpan.FromMinutes(10), a.Duration);

                Assert.AreEqual(0, Sidi.GetOpt.GetOpt.Run(a, new[] { "--fruit=orange" }));
                Assert.AreEqual(ValueParserTestApplication.Fruits.Orange, a.Fruit);

                Assert.AreEqual(0, Sidi.GetOpt.GetOpt.Run(a, new[] { "--fruit=o" }));
                Assert.AreEqual(ValueParserTestApplication.Fruits.Orange, a.Fruit);
            }
        }

        [Test]
        public void EnumHelp()
        {
            using (var c = new CaptureConsoleOutput())
            {
                var a = new ValueParserTestApplication();
                Assert.AreEqual(0, Sidi.GetOpt.GetOpt.Run(a, new[] { "--help" }));
                foreach (var n in Enum.GetNames(typeof(ValueParserTestApplication.Fruits)))
                {
                    StringAssert.Contains(n, c.output.ToString());
                }
            }
        }
    }
}
