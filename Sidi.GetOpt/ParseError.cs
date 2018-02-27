using System;
using System.Reflection;

namespace Sidi.GetOpt
{
    internal class ParseError : Exception
    {
        private readonly Args args;

        internal ParseError(Args args, string message, Exception innerException = null)
            :base(String.Format("{0}\r\n\r\nArguments:\r\n{1}", message, args), innerException)
        {
            this.args = args;
        }

        internal static ParseError ToParseError(Args a, Exception e)
        {
            if (e is ParseError)
            {
                return (ParseError)e;
            }
            var message = e.Message;
            if (e is TargetInvocationException)
            {
                message = e.InnerException.Message;
            }
            return new ParseError(a, message, e);
        }
    }
}
