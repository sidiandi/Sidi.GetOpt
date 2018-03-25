using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    class MethodCommand : ICommand
    {
        private readonly IObjectProvider getInstance;
        private readonly MethodInfo method;

        public string Name => Util.CSharpIdentifierToLongOption(method.Name);

        public MethodInfo Method => this.method;

        public string Usage => this.method.GetUsage().Or(String.Empty);

        public string ArgumentSyntax => this.method.GetArgumentSyntax();

        public static Maybe<ICommand> Create(ICommand parent, IObjectProvider containingObject, MethodInfo method, IEnumerable<IOption> inheritedOptions)
        {
            if (inheritedOptions == null)
            {
                throw new ArgumentNullException(nameof(inheritedOptions));
            }

            return method
                .GetUsage()
                .Select(usage =>
                {
                    var help = new HelpOption();
                    var helpCommand = new ObjectCommandSource(null, ObjectProvider.Create(help));

                    var c = new MethodCommand(parent, containingObject, method, helpCommand.Options.Concat(inheritedOptions));

                    help.Command = c;
                    return (ICommand) c;
                });
        }

        object ReadParameter(Args args, ParameterInfo parameterInfo)
        {
            ParseOptions(args);

            var expectedType = parameterInfo.ParameterType;

            if (expectedType.IsArray)
            {
                return ReadArrayParameter(args, parameterInfo);
            }

            if (args.HasNext)
            {
                var p = Util.ParseValue(getInstance.Instance, expectedType, args.Next);
                args.MoveNext();
                return p;
            }
            else
            {
                return null;
            }
        }

        public int Invoke(Args args)
        {
            var parameters = this.parameterInfo.Select(_ => ReadParameter(args, _)).ToArray();
            ParseOptions(args);
            if (args.HasNext)
            {
                throw new ParseError(args, String.Format("Too many parameters for {0}", this));
            }
            var instance = getInstance.Instance;
            return ConvertResultToInt(this.method.Invoke(instance, parameters.ToArray()));
        }

        public static int ConvertResultToInt(object r)
        {
            if (r is Task<int>)
            {
                r = ((Task<int>)r).Result;
            }
            else if (r is IAsyncResult)
            {
                var asyncResult = (IAsyncResult)r;
                asyncResult.AsyncWaitHandle.WaitOne();
            }
            var exitCode = r is int ? (int)r : 0;
            return exitCode;
        }

        bool OptionStop(Args args)
        {
            if (!args.HasNext)
            {
                return false;
            }

            if (args.TreatAsParameters)
            {
                return false;
            }

            if (args.LongOptionPrefix.Any(_ => string.Equals(_, args.Next)))
            {
                args.MoveNext();
                args.TreatAsParameters = true;
                return true;
            }
            return false;
        }

        bool ShortOption(Args args)
        {
            if (!args.HasNext)
            {
                return false;
            }

            if (args.TreatAsParameters)
            {
                return false;
            }

            if (!args.Next.TryRemovePrefix(args.ShortOptionPrefix, out var optionText))
            {
                return false;
            }

            args.MoveNext();

            for (; !String.IsNullOrEmpty(optionText);)
            {
                var name = optionText.Substring(0, 1);
                var valueText = optionText.Substring(1);

                var option = this.Options.FindShortOption(args, name);

                if (option.Type.Equals(typeof(bool)))
                {
                    // bool options do not have a value. value contains further short options
                    optionText = valueText;
                    valueText = true.ToString();
                }
                else
                {
                    if (String.IsNullOrEmpty(valueText))
                    {
                        args.MoveNext();
                        valueText = args.Current;
                    }
                }

                option.Set(valueText);
            }

            return true;
        }

        private readonly IEnumerable<IOption> inheritedOptions;
        ParameterInfo[] parameterInfo;
        int result;

        bool LongOption(Args args)
        {
            if (!args.HasNext)
            {
                return false;
            }

            if (args.TreatAsParameters)
            {
                return false;
            }

            if (!args.Next.TryRemovePrefix(args.LongOptionPrefix, out var optionText))
            {
                return false;
            }

            var p = optionText.Split(new[] { '=' }, 2);
            var name = p.First();
            var valueText = p.Length == 2 ? p[1] : null;

            args.MoveNext();

            var option = this.Options.FindLongOption(args);

            if (option.Type.Equals(typeof(bool)))
            {
                // bool options do not have a value. value contains further short options
                if (!String.IsNullOrEmpty(valueText))
                {
                    throw new ParseError(args, "boolean option cannot have a value.");
                }
                valueText = true.ToString();
            }
            else
            {
                if (String.IsNullOrEmpty(valueText))
                {
                    throw new ParseError(args, String.Format("This option requires a value. Specify with --{0}=value.", option.Name));
                }
                valueText = args.Current;
            }

            option.Set(valueText);
            return true;
        }

        bool ParseOptions(Args args)
        {
            for (; ; )
            {
                if (OptionStop(args)) continue;
                if (LongOption(args)) continue;
                if (ShortOption(args)) continue;
                break;
            }
            return true;
        }

        object ReadArrayParameter(Args args, ParameterInfo parameterInfo)
        {
            var elementType = parameterInfo.ParameterType.GetElementType();
            var items = new List<object>();

            for (; ; )
            {
                ParseOptions(args);
                if (!args.HasNext) break;
                args.MoveNext();
                items.Add(Util.ParseValue(getInstance.Instance, elementType, args.Current));
            }

            return items.ToArray(elementType);
        }

        const string endl = "\r\n";

        string OptionSynopsis
        {
            get

            {
                if (Options.Any())
                {
                    return "Options:" + endl + String.Join(endl, Options) + endl;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public ICommand Parent { get; }

        public IEnumerable<IOption> Options => inheritedOptions.Concat(this.Parent.Options)
            .DistinctBy(_ => _.Name);

        public void PrintUsage(TextWriter w)
        {
            w.WriteLine(new[]
            {
                @"Usage: " + this.GetInvocation() + @" [option]... " + this.method.GetArgumentSyntax(),
                this.Usage,
                OptionSynopsis
            }.JoinNonEmpty(endl + endl));
        }

        public override string ToString()
        {
            return String.Format("{0} : {1}", this.Name, this.Usage);
        }

        MethodCommand(ICommand parent, IObjectProvider getInstance, MethodInfo method, IEnumerable<IOption> inheritedOptions)
        {
            this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
            this.method = method ?? throw new ArgumentNullException(nameof(method));
            this.parameterInfo = method.GetParameters();
            this.inheritedOptions = inheritedOptions ?? Enumerable.Empty<IOption>();
            this.Parent = parent;
        }
    }
}
