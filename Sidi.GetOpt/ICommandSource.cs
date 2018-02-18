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
        string Description { get; }

        object ParseValue(Type type, string value);
    }

    internal static class ICommandSourceExtensions
    {
        public static ICommandSource Concat(this ICommandSource commandSource, params ICommandSource[] sources)
        {
            return new CompositeCommandSource(new[] { commandSource }.Concat(sources));
        }

        public static IOption FindShortOption(this ICommandSource commandSource, string shortName)
        {
            var byShort = Util.DetermineShortOptions(commandSource.Options);
            return byShort[shortName.ToLower()];
        }

        public static IOption FindLongOption(this ICommandSource commandSource, string longName)
        {
            return commandSource.Options.FirstOrDefault(_ => string.Equals(longName, _.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static ICommand FindCommand(this ICommandSource commandSource, string commandName)
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
    }
}
