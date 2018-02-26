using System.Collections.Generic;
using System.IO;

namespace Sidi.GetOpt
{
    internal interface ICommand
    {
        int Invoke(Args args);
        string Name { get; }
        ICommand Parent { get; }
        IEnumerable<IOption> Options { get; }
        void PrintUsage(TextWriter w);
        string Usage { get; }
        string ArgumentSyntax { get; }
    }

    internal static class ICommandExtensions
    {
        public static string GetInvocation(this ICommand c)
        {
            if (c.Parent == null)
            {
                return c.Name;
            }
            else
            {
                return c.Parent.GetInvocation() + " " + c.Name;
            }
        }
    }
}
