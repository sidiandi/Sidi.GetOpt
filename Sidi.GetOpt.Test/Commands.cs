using System.ComponentModel;

namespace Sidi.GetOpt.Test
{
    internal class Commands
    {
        public Commands()
        {
            Calculator = new Calculator();
            Hello = new HelloWorld();
        }

        [Command, Description("Greet people")]
        public HelloWorld Hello;

        [Command, Description("Basic calculations")]
        public Calculator Calculator;
    }
}