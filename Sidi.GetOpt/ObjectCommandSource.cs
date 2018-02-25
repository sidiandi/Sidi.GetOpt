using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class ObjectCommandSource : ICommandSource
    {
        private readonly Type type;

        public ObjectCommandSource(ICommand parent, Type type, Func<object> getInstance)
        {
            if (getInstance == null)
            {
                throw new ArgumentNullException(nameof(getInstance));
            }

            Options = Option.GetOptions(type, getInstance).ToList();
            Commands = Command.GetCommands(parent, type, getInstance, Options).ToList();
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public ObjectCommandSource(ICommand parent, object instance)
            : this(parent, instance.GetType(), () => instance)
        {
        }

        class NoCommand
        {
            [Description("")]
            public void Nothing() { }
        }

        public IEnumerable<ICommand> Commands { get; }

        public IEnumerable<IOption> Options { get; }

        public string Description => type.GetUsage();

        public object ParseValue(Type type, string value)
        {
            throw new NotImplementedException();
        }
    }
}
