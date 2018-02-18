using System;

namespace Sidi.GetOpt
{
    internal class ParseError : Exception
    {
        private readonly Args args;

        internal ParseError(Args args, string message)
            :base(String.Format("{0}\r\nArguments:\r\n{1}", message, args))
        {
            this.args = args;
        }
    }
}
