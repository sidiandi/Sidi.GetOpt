namespace Sidi.GetOpt.Test
{
    internal class Commands
    {
        public Commands()
        {
        }

        [Command]
        HelloWorld Hello => new HelloWorld();

        [Command]
        Calculator Calculator => new Calculator();
    }
}