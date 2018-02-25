﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

namespace Sidi.GetOpt
{
    class MethodCommand : ICommand
    {
        private readonly IObjectProvider getInstance;
        private readonly MethodInfo method;

        public string Name => Util.CSharpIdentifierToLongOption(method.Name);

        public MethodInfo Method => this.method;

        public string Description => this.method.GetDescription();

        public string ArgumentSyntax => this.method.GetArgumentSyntax();

        public static ICommand Create(ICommand parent, IObjectProvider containingObject, MethodInfo method, IEnumerable<IOption> inheritedOptions)
        {
            if (inheritedOptions == null)
            {
                throw new ArgumentNullException(nameof(inheritedOptions));
            }

            var description = method.GetCustomAttribute<DescriptionAttribute>();
            if (description == null) return null;

            var help = new HelpOption();
            var helpCommand = new ObjectCommandSource(null, ObjectProvider.Create(help));

            var c = new MethodCommand(parent, containingObject, method, helpCommand.Options.Concat(inheritedOptions));

            help.Command = c;
            return c;
        }

        public int Invoke(Args args)
        {
            for (;;)
            {
                if (!Parse(args))
                {
                    break;
                }
            }
            return result;
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

            args.MoveNext();

            var p = optionText.Split(new[] { '=' }, 2);
            var name = p.First();
            var valueText = p.Length == 2 ? p[1] : null;

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

        bool Parse(Args args)
        {
            /*
            if (OptionStop(args)) return true;
            if (LongOption(args)) return true;
            if (ShortOption(args)) return true;
            */

            return Parameter(args);
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

        bool Parameter(Args args)
        {
            if (args.parameters.Count >= parameterInfo.Length)
            {
                // execute method - enough parameters
                var instance = getInstance.Instance;
                var r = this.method.Invoke(instance, args.parameters.ToArray());
                result = r is int ? (int)r : 0;
                return false;
            }

            var expectedType = parameterInfo[args.parameters.Count].ParameterType;

            if (expectedType.IsArray)
            {
                return ArrayParameter(args);
            }

            ParseOptions(args);

            if (!args.HasNext)
            {
                return false;
            }

            args.MoveNext();

            args.parameters.Add(Util.ParseValue(getInstance.Instance, expectedType, args.Current));
            return true;
        }

        bool ArrayParameter(Args args)
        {
            var expectedType = parameterInfo[args.parameters.Count].ParameterType;
            var elementType = expectedType.GetElementType();

            var items = new List<object>();

            for (; ; )
            {
                ParseOptions(args);
                if (!args.HasNext) break;
                args.MoveNext();
                items.Add(Util.ParseValue(getInstance.Instance, elementType, args.Current));
            }

            args.parameters.Add(items.ToArray(elementType));
            return true;
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
                this.Description,
                OptionSynopsis
            }.JoinNonEmpty(endl + endl));
        }

        public override string ToString()
        {
            return String.Format("{0} : {1}", this.Name, this.Description);
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
