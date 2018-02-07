using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    public class GetOpt
    {
        internal readonly ICommandSource commandSource;

        public static int Run(object application, string[] args)
        {
            GetOpt getOpt = null;

            var commandSource = (ICommandSource)new CommandSource(() => getOpt, application);

            commandSource = commandSource.Concat(
                new CommandSource(() => getOpt, new HelpApplication(() => getOpt)),
                new CommandSource(() => getOpt, new VersionApplication())
                );

            getOpt = new GetOpt(commandSource);
            getOpt.Invocation = Process.GetCurrentProcess().ProcessName;

            return getOpt.Run(args);
        }

        public GetOpt(ICommandSource commandSource)
        {
            this.commandSource = commandSource ?? throw new ArgumentNullException(nameof(commandSource));
            LongOptionPrefix = new[] { "--" };
            ShortOptionPrefix = new[] { "-" };
            byShort = DetermineShortOptions(commandSource.Options);
        }

        static IDictionary<string, IOption> DetermineShortOptions(IEnumerable<IOption> options)
        {
            var d = new Dictionary<string, IOption>();
            foreach (var o in options.Reverse())
            {
                d[o.Name.Substring(0, 1).ToLower()] = o;
            }
            return d;
        }

        public string[] LongOptionPrefix { get; set; }
        public string[] ShortOptionPrefix { get; set; }

        IDictionary<string, IOption> byShort;

        public string Invocation { get; set; }

        bool HasPrefix(string text, string[] prefixes, out string reminder)
        {
            foreach (var i in prefixes)
            {
                if (text.StartsWith(i))
                {
                    reminder = text.Substring(i.Length);
                    return true;
                }
            }
            reminder = null;
            return false;
        }

        public object GetParameter(Type parameterType, Args a)
        {
            if (parameterType != null && parameterType.IsArray)
            {
                return GetArrayParameter(parameterType, a);
            }

            for (; ; )
            {
                if (!a.MoveNext())
                {
                    return null;
                }

                // long option?
                if (LongOptionPrefix.Any(a.Current.StartsWith))
                {
                    HandleLongOption(a);
                    continue;
                }

                // short option?
                if (ShortOptionPrefix.Any(a.Current.StartsWith))
                {
                    HandleShortOption(a);
                    continue;
                }

                if (parameterType == null)
                {
                    return null;
                }

                return Util.ParseValue(this.commandSource, parameterType, a.Current);
            }
        }

        public object GetArrayParameter(Type parameterType, Args a)
        {
            var parameters = new List<object>();
            var elementType = parameterType.GetElementType();
            for (; ; )
            {
                if (!a.MoveNext())
                {
                    return parameters.ToArray(elementType);
                }

                // long option?
                if (LongOptionPrefix.Any(a.Current.StartsWith))
                {
                    HandleLongOption(a);
                    continue;
                }

                // short option?
                if (ShortOptionPrefix.Any(a.Current.StartsWith))
                {
                    HandleShortOption(a);
                    continue;
                }

                parameters.Add(Util.ParseValue(this.commandSource, elementType, a.Current));
            }
        }

        IOption FindShortOption(string shortName)
        {
            // todo: error handling
            return byShort[shortName.ToLower()];
        }

        IOption FindLongOption(string longName)
        {
            return commandSource.Options.First(_ => string.Equals(longName, _.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        internal ICommand FindCommand(string commandName)
        {
            if (String.IsNullOrEmpty(commandName))
            {
                return null;
            }

            var candidates = commandSource.Commands.Where(_ => _.Name.Equals(commandName)).ToList();
            if (!candidates.Any())
            {
                candidates = commandSource.Commands.Where(_ => _.Name.StartsWith(commandName)).ToList();
            }
            if (candidates.Count > 1)
            {
                throw new ParseError(null, "Ambiguous command. Could be: " + String.Join(", ", candidates));
            }
            return candidates.FirstOrDefault();
        }

        IEnumerable<string> ParseOptions(Args a )
        {
            var rest = new List<string>();

            for (a.MoveNext();!a.IsEnd;a.MoveNext())
            {
                if (a.Current.Equals("--"))
                {
                    a.MoveNext();
                    rest.AddRange(a.Rest);
                    break;
                }
                if (HasPrefix(a.Current, ShortOptionPrefix, out var option))
                {
                    var o = FindLongOption(option);
                    string value = null;
                    if (o.Type.Equals(typeof(bool)))
                    {
                        value = true.ToString();
                    }
                    else
                    {
                        if (a.MoveNext())
                        {
                            value = a.Current;
                        }
                        else
                        {
                            // todo error handling
                        }
                    }
                    o.Set(value);
                }
                else if (HasPrefix(a.Current, ShortOptionPrefix, out option))
                {
                    if (option.Length > 1)
                    {
                        var value = option.Substring(1);
                        option = option.Substring(0, 1);
                        var o = FindShortOption(option);
                        o.Set(value);
                    }
                    else
                    {
                        var o = FindShortOption(option);
                        string value = null;
                        if (o.Type.Equals(typeof(bool)))
                        {
                            value = true.ToString();
                        }
                        else
                        {
                            if (a.MoveNext())
                            {
                                value = a.Current;
                            }
                            else
                            {
                                // todo error handling
                            }
                        }
                        o.Set(value);
                    }
                }
            }
            return rest;
        }

        public class ParseError : Exception
        {
            private readonly Args args;

            internal ParseError(Args args, string message)
                :base(String.Format("{0}\r\nArguments:\r\n{1}", message, args))
            {
                this.args = args;
            }
        }

        void HandleLongOption(Args a)
        {
            if (!a.Current.TryRemovePrefix(LongOptionPrefix, out var longName))
            {
                throw new ParseError(a, "Not a valid long option.");
            }

            var o = FindLongOption(longName);

            string value = null;
            if (o.Type.Equals(typeof(bool)))
            {
                value = true.ToString();
            }
            else
            {
                if (!a.MoveNext()) throw new ParseError(a, "Missing value for option.");
                value = a.Current;
            }
            o.Set(value);
        }

        void HandleShortOption(Args a)
        {
            if (!a.Current.TryRemovePrefix(ShortOptionPrefix, out var optionText))
            {
                throw new ParseError(a, "Not a valid long option.");
            }

            for (; !String.IsNullOrEmpty(optionText);)
            {
                var shortName = optionText.Substring(0, 1);
                var value = optionText.Substring(1);
                optionText = null;

                var o = FindShortOption(shortName);
                if (o.Type.Equals(typeof(bool)))
                {
                    if (!String.IsNullOrEmpty(value))
                    {
                        optionText = value;
                    }
                    value = true.ToString();
                }
                else
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        if (!a.MoveNext()) throw new ParseError(a, "Missing value for option.");
                        value = a.Current;
                    }
                }
                o.Set(value);
            }
        }

        void HandleCommand(Args a)
        {
            ICommand command = null;
            if (commandSource.Commands.Count() > 1)
            {
                command = FindCommand(a.Current);
                if (command == null)
                {
                    throw new ParseError(a, "Unknown command.");
                }
            }
            else
            {
                command = commandSource.Commands.Single();
            }
            command.Invoke(a);
        }

        void ParseNext(Args a)
        {
            Trace.TraceInformation("ParseNext:\r\n{0}", a);

            // long option?
            if (LongOptionPrefix.Any(a.Current.StartsWith))
            {
                HandleLongOption(a);
                return;
            }

            // short option?
            if (ShortOptionPrefix.Any(a.Current.StartsWith))
            {
                HandleShortOption(a);
                return;
            }

            HandleCommand(a);
        }

        public int Run(string[] args)
        {
            try
            {
                for (var a = new Args(args); a.MoveNext();)
                {
                    ParseNext(a);
                }

                return 0;
            }
            catch (GetOpt.ParseError e)
            {
                Console.Error.WriteLine("An error occured while parsing the command line arguments: {0}", e.Message);
                return -1;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return -1;
            }
        }

        public int Run(Args a)
        {
            try
            {
                for (; a.MoveNext();)
                {
                    ParseNext(a);
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return -1;
            }
        }
    }
}
