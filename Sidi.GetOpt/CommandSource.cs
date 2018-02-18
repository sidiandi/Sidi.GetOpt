using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class CommandSource : ICommandSource
    {
        private readonly object application;

        public CommandSource(Func<GetOpt> getGetOpt, object application)
        {
            this.application = application ?? throw new ArgumentNullException(nameof(application));

            var type = this.application.GetType();

            var commandObjects = type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => ObjectCommand.Create(getGetOpt, this.application, _))
                .Where(_ => _ != null)
                .ToList();

            Commands = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => MethodCommand.Create(getGetOpt, this.application, _))
                .Where(_ => _ != null)
                .Concat(commandObjects)
                .ToList();

            Options = type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => Option.Create(this.application, _))
                .Where(_ => _ != null)
                .ToList();
        }

        public IEnumerable<ICommand> Commands { get; }
        public IEnumerable<IOption> Options { get; }

        public string Description => this.application.GetType().GetDescription();

        public object ParseValue(Type type, string value)
        {
            // has application a Parse{Type} method?
            {
                var parser = type.GetParser(this.application.GetType(), "Parse" + type.Name);
                if (parser != null)
                {
                    return parser.Invoke(application, new[] { value });
                }
            }
            return null;
        }
    }
}
