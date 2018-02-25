using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class ArgumentFile
    {
        public static IEnumerable<string> Read(TextReader r)
        {
            return ReadImpl(r).ToList();
        }


        static IEnumerable<string> ReadImpl(TextReader r)
        {
            for (; ; )
            {
                if (!ConsumeWhitespace(r)) break;
                if (!ReadArgument(r, out var a)) break;
                yield return a;
            }
        }

        static bool ConsumeWhitespace(TextReader r)
        {
            for (; ;)
            {
                var next = r.Peek();
                if (next == -1) return false;

                var c = (char)next;
                if (Char.IsWhiteSpace(c))
                {
                    r.Read();
                }
                else if (c == '#')
                {
                    r.ReadLine();
                }
                else
                {
                    return true;
                }
            }
        }

        static bool ReadArgument(TextReader r, out string a)
        {
            a = null;
            var next = r.Peek();
            if (next == -1) return false;

            var c = (char)next;
            if (c == '"')
            {
                return ReadQuotedArgument(r, out a);
            }

            var sw = new StringWriter();
            for (; ; )
            {
                if (Char.IsWhiteSpace(c))
                {
                    break;
                }

                sw.Write(c);
                r.Read();
                next = r.Peek();
                if (next == -1) return false;
                c = (char)next;
            }

            a = sw.ToString();

            if (a.StartsWith("<<"))
            {
                var endMarker = a;
                return ReadMultilineArgument(r, endMarker, out a);
            }
            return true;
        }

        private static bool ReadMultilineArgument(TextReader r, string endMarker, out string a)
        {
            a = null;
            // new line
            r.ReadLine();
            // Read characters until the string ends with endMarker
            var sw = new StringWriter();
            for (;!sw.ToString().EndsWith(endMarker); )
            {
                var next = r.Peek();
                if (next == -1) return false;
                var c = (char)next;
                r.Read();
                sw.Write(c);
            }
            a = sw.ToString();
            a = a.Substring(0, a.Length - endMarker.Length);
            return true;
        }

        private static bool ReadQuotedArgument(TextReader r, out string a)
        {
            a = null;
            bool escaped = false;
            var next = r.Read();
            if (next == -1) return false;
            var c = (char)next;
            if (c != '"') return false;

            var sw = new StringWriter();
            for (; ; )
            {
                next = r.Peek();
                if (next == -1) return false;
                c = (char)next;

                if (escaped)
                {
                    escaped = false;
                }
                else
                {
                    if (c == '"')
                    {
                        r.Read();
                        break;
                    }
                    if (c == '\\')
                    {
                        escaped = true;
                        r.Read();
                        continue;
                    }
                }

                sw.Write(c);
                r.Read();
            }

            a = sw.ToString();
            return true;
        }
    }
}
