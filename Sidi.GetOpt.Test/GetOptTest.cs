using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
        public void TooManyParameters()
        {
            var calc = new Calculator();
            var e = GetOpt.Run(calc, new[] { "add", "1", "1", "1"});
            Assert.AreNotEqual(0, e);
        }

        [Test]
        public void ShowHelp()
        {
            var calc = new Calculator();
            var e = GetOpt.Run(calc, new[] { "--help" });
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
            var calc = new Calculator();
            var e = GetOpt.Run(calc, new[] { "sum", "--help"});
            Assert.AreEqual(0, e);
        }

        [Test]
        public void CommandClass()
        {
            var commands = new Commands();
            var e = GetOpt.Run(commands, new[] { "--help" });
            Assert.AreEqual(0, e);

            Assert.AreEqual(0, GetOpt.Run(commands, new[] { "calculator", "--help" }));

            Assert.AreEqual(0, GetOpt.Run(commands, new[] { "calculator", "--print", "sum", "1", "2", "3" }));
            Assert.AreEqual(6.0, commands.Calculator.Result);
        }

        [Test]
        public void EmptyApplication()
        {
            var e = GetOpt.Run(new Empty(), new[] { "--help" });
            Assert.AreEqual(0, e);
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
    }
}
