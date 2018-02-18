using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class ObjectCommandSource : ICommandSource
    {

        public ObjectCommandSource(Type type, Func<object> getInstance)
        {
            Options = Option.GetOptions(type, getInstance).ToList();
            Commands = Command.GetCommands(type, getInstance, Options).ToList();
        }

        public IEnumerable<ICommand> Commands { get; }

        public IEnumerable<IOption> Options { get; }

        public string Description => throw new NotImplementedException();

        public object ParseValue(Type type, string value)
        {
            throw new NotImplementedException();
        }
    }
}
