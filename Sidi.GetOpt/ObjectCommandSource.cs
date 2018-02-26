using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class ObjectCommandSource : ICommandSource
    {
        private readonly IObjectProvider getInstance;

        public ObjectCommandSource(ICommand parent, IObjectProvider getInstance)
        {
            if (getInstance == null)
            {
                throw new ArgumentNullException(nameof(getInstance));
            }

            Options = Option.GetOptions(getInstance);
            Commands = Command.GetCommands(parent, getInstance, Options).ToList();
            this.getInstance = getInstance;
        }

        class NoCommand
        {
            [Usage("")]
            public void Nothing() { }
        }

        public IEnumerable<ICommand> Commands { get; }

        public IEnumerable<IOption> Options { get; }

        public string Usage => getInstance.GetUsage();

        public object ParseValue(Type type, string value)
        {
            throw new NotImplementedException();
        }
    }
}
