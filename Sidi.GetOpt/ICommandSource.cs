using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{

    internal interface ICommandSource
    {
        IEnumerable<ICommand> Commands { get; }
        IEnumerable<IOption> Options { get; }
        string Usage { get; }

        object ParseValue(Type type, string value);
    }

    internal static class ICommandSourceExtensions
    {
        public static ICommandSource Concat(this ICommandSource commandSource, params ICommandSource[] sources)
        {
            return new CompositeCommandSource(new[] { commandSource }.Concat(sources));
        }

        static ICommand GetSingle(Args args, IEnumerable<ICommand> candidates)
        {
            var c = candidates.ToList();
            if (c.Count > 1)
            {
                throw new ParseError(args, String.Format("{0} is an ambiguous command. Could be: {1}", args.Current.Quote(), String.Join(", ", c.Select(_ => _.Name))));
            }
            else if (c.Count == 1)
            {
                return c.Single();
            }
            return null;
        }

        public static ICommand FindCommand(this ICommandSource commandSource, Args args)
        {
            var commandName = args.Current;

            if (String.IsNullOrEmpty(commandName))
            {
                return null;
            }

            var command = GetSingle(args, commandSource.Commands.Where(_ => _.Name.Equals(commandName)));
            if (command != null) return command;
            command = GetSingle(args, commandSource.Commands.Where(_ => commandName.IsAbbreviation(_.Name)));
            if (command != null) return command;
            throw new ParseError(args, String.Format("Invalid command: {0}", commandName));
        }

        static IOption GetSingle(Args args, IEnumerable<IOption> candidates)
        {
            var c = candidates.ToList();
            if (c.Count > 1)
            {
                throw new ParseError(args, String.Format("{0} is an ambiguous option. Could be: {1}", args.Current.Quote(), String.Join(", ", c.Select(_ => _.Name))));
            }
            else if (c.Count == 1)
            {
                return c.Single();
            }
            return null;
        }

        public static IOption FindLongOption(this IEnumerable<IOption> options, Args args)
        {
            if (!args.Current.TryRemovePrefix(args.LongOptionPrefix, out var optionText))
            {
                throw new ParseError(args, String.Format("{0} is not a valid option.", args.Current.Quote()));
            }
            var p = optionText.Split(new[] { '=' }, 2);
            var optionName = p[0];



            if (String.IsNullOrEmpty(optionName))
            {
                return null;
            }

            var option = GetSingle(args, options.Where(_ => _.Aliases.Any(alias => alias.Equals(optionName))));
            if (option != null) return option;
            option = GetSingle(args, options.Where(_ => _.Aliases.Any(alias => alias.StartsWith(optionName, StringComparison.InvariantCultureIgnoreCase))));
            if (option != null) return option;
            option = GetSingle(args, options.Where(_ => optionName.IsAbbreviation(_.Name)));
            if (option != null) return option;
            throw new ParseError(args, String.Format("Invalid option: {0}", optionName));
        }

        public static IOption FindShortOption(this IEnumerable<IOption> options, Args args, string optionName)
        {
            if (String.IsNullOrEmpty(optionName))
            {
                return null;
            }

            var option = GetSingle(args, options.Where(_ => _.Aliases.Any(alias => alias.Equals(optionName))));
            if (option != null) return option;
            option = GetSingle(args, options.Where(_ => optionName.IsAbbreviation(_.Name)));
            if (option != null) return option;
            throw new ParseError(args, String.Format("Invalid option: {0}", optionName));
        }
    }
}
