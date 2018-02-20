﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sidi.GetOpt
{
    class ObjectCommand : ICommand
    {
        public static ICommand Create(string name, Type type, Func<object> getInstance)
        {
            return new ObjectCommand(name, new ObjectCommandSource(type, getInstance)).AddHelp();
        }

        public static ICommand Create(MemberInfo member, Func<object> getInstance)
        {
            var getter = member.GetGetter(getInstance);

            var command = member.GetCustomAttribute<CommandAttribute>();
            if (command == null)
            {
                return null;
            }

            var name = Util.CSharpIdentifierToLongOption(member.Name);

            return new ObjectCommand(name, new ObjectCommandSource(member.GetValueType(), getter)).AddHelp();
        }

        ObjectCommand(string name, ICommandSource commandSource)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            CommandSource = commandSource;
        }

        public string Name { get; }

        public override string ToString()
        {
            return String.Format("{0} : {1}", this.Name, this.Description);
        }

        public ICommandSource CommandSource { get; set; }

        public string Description => CommandSource.Description;

        bool OptionStop(Args args)
        {
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

                var option = this.CommandSource.Options.FindShortOption(args, name);

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

        IEnumerable<IOption> options => CommandSource.Options;
        IEnumerable<ICommand> commands => CommandSource.Commands;

        bool LongOption(Args args)
        {
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

            var option = this.CommandSource.Options.FindLongOption(args);

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

        // parses next argument
        bool Parse(Args args)
        {
            if (OptionStop(args)) return true;
            if (LongOption(args)) return true;
            if (ShortOption(args)) return true;

            if (commands.Count() > 1)
            {
                return SelectCommand(args);
            }
            else
            {
                return ExecuteCommand(commands.FirstOrDefault(), args);
            }
        }

        bool SelectCommand(Args args)
        {
            if (!args.HasNext)
            {
                return false;
            }

            args.MoveNext();

            var name = args.Current;

            var command = CommandSource.FindCommand(args);

            return ExecuteCommand(command, args);
        }

        bool ExecuteCommand(ICommand command, Args args)
        {
            result = command.Invoke(args);
            return true;
        }

        int result;

        public int Invoke(Args args)
        {
            for (; args.HasNext;)
            {
                if (!Parse(args))
                {
                    break;
                }
            }
            return result;
        }

        const string endl = "\r\n";

        string CommandSynopsis
        {
            get

            {
                if (this.CommandSource.Commands.Any())
                {
                    return "Commands:" + endl + String.Join(endl, this.CommandSource.Commands) + endl;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        string OptionSynopsis
        {
            get

            {
                if (this.CommandSource.Options.Any())
                {
                    return "Options:" + endl + String.Join(endl, this.CommandSource.Options) + endl;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        bool MultiCommand => CommandSource.Commands.Count() > 1;
        ICommand SingleCommand
        {
            get
            {
                var c = CommandSource.Commands.SingleOrDefault();
                if (c == null)
                {
                    c = MethodCommand.Create(() => this, this.GetType().GetMethod("Nothing"), this.CommandSource.Options);
                }
                return c;
            }
        }

        [Description("")]
        public void Nothing()
        {

        }

        public string ArgumentSyntax => throw new NotImplementedException();

        public void PrintUsage(TextWriter w)
        {
            if (MultiCommand)
            {
                w.WriteLine(new[]
                {
                    @"Usage: " + this.Name + @" [option]... <command>",
                    this.Description,
                    CommandSynopsis,
                    OptionSynopsis
                }.JoinNonEmpty(endl + endl));
            }
            else
            {
                w.WriteLine(new[]
                {
                    @"Usage: " + this.Name + @" [option]... " + SingleCommand.ArgumentSyntax,
                    this.Description,
                    OptionSynopsis
                }.JoinNonEmpty(endl+endl));
        }
    }
    }
}
