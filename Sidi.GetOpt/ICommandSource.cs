using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    public interface ICommand
    {
        string Name { get; }
        MethodInfo Method { get; }

        void Invoke(Args args);
    }

    public interface IOption
    {
        Type Type { get; }
        string Name { get; }
        object Description { get; }

        void Set(string value);
    }

    public interface ICommandSource
    {
        IEnumerable<ICommand> Commands { get; }
        IEnumerable<IOption> Options { get; }

        object ParseValue(Type type, string value);
    }

    public static class ICommandSourceExtensions
    {
        public static ICommandSource Concat(this ICommandSource commandSource, params ICommandSource[] sources)
        {
            return new CompositeCommandSource(new[] { commandSource }.Concat(sources));
        }
    }
}
