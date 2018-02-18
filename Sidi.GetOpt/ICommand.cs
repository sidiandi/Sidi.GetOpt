using System.IO;

namespace Sidi.GetOpt
{
    internal interface ICommand
    {
        int Invoke(Args args);
        string Name { get; }
        void PrintUsage(TextWriter w);
        string Description { get; }
        string ArgumentSyntax { get; }
    }
}
