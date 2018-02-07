namespace Sidi.GetOpt.Test
{
    internal class Commands
    {
        public Commands()
        {
            Calculator = new Calculator();
            Hello = new HelloWorld();
        }

        [Command]
        public HelloWorld Hello;

        [Command]
        public Calculator Calculator;
    }
}