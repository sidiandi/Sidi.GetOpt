using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    class CompositeCommandSource : ICommandSource
    {
        private readonly IEnumerable<ICommandSource> commandSources;

        public CompositeCommandSource(IEnumerable<ICommandSource> commandSources)
        {
            this.commandSources = commandSources ?? throw new ArgumentNullException(nameof(commandSources));
        }

        public IEnumerable<ICommand> Commands => commandSources.SelectMany(_ => _.Commands);

        public IEnumerable<IOption> Options => commandSources.SelectMany(_ => _.Options);

        public string Usage => this.commandSources.First().Usage;

        public object ParseValue(Type type, string value)
        {
            return commandSources
                .Select(_ => _.ParseValue(type, value))
                .FirstOrDefault(_ => _ != null);
        }
    }
}
