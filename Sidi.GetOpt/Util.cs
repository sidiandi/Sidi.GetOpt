using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class Util
    {
        static internal string CSharpIdentifierToLongOption(string csharpIdentifier)
        {
            var o = new StringWriter();
            int i = 0;
            for (; i < csharpIdentifier.Length && i < 1; ++i)
            {
                o.Write(char.ToLower(csharpIdentifier[i]));
            }
            for (; i < csharpIdentifier.Length; ++i)
            {
                if (char.IsLower(csharpIdentifier[i - 1]) && char.IsUpper(csharpIdentifier[i]))
                {
                    o.Write("-");
                }
                o.Write(char.ToLower(csharpIdentifier[i]));
            }
            return o.ToString();
        }

        internal static object ParseValue(object application, Type type, string value)
        {
            // type string? do not parse.
            if (type.Equals(typeof(string)))
            {
                return value;
            }

            if (application is ICommandSource)
            {
                // pass on to commandSource
                var r = ((ICommandSource)application).ParseValue(type, value);
                if (r != null)
                {
                    return r;
                }
            }

            // has application a Parse{Type} method?
            {
                var parser = type.GetParser(application.GetType(), "Parse" + type.Name);
                if (parser != null)
                {
                    return parser.Invoke(application, new[] { value });
                }
            }

            // has the type a Parse(string) method?
            {
                var parser = type.GetParser(type, "Parse");
                if (parser != null)
                {
                    return parser.Invoke(application, new[] { value });
                }
            }

            // has the type a ctor(string) ?
            {
                var parser = type.GetConstructor(new[] { typeof(string) });
                if (parser != null)
                {
                    return parser.Invoke(application, new[] { value });
                }
            }

            throw new GetOpt.ParseError(null, "Cannot parse value");
        }
    }
}
