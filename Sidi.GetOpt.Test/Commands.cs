
namespace Sidi.GetOpt.Test
{
    internal class Commands
    {
        public Commands()
        {
            Calculator = new Calculator();
            Hello = new HelloWorld();
        }

        [Command, Usage("Greet people")]
        public HelloWorld Hello;

        [Command, Usage("Basic calculations")]
        public Calculator Calculator;
    }
}