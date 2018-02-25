using System.IO;

namespace Sidi.GetOpt
{
    internal interface ICommand
    {
        int Invoke(Args args);
        string Name { get; }
        ICommand Parent { get; }
        void PrintUsage(TextWriter w);
        string Description { get; }
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
