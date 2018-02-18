using System.IO;

namespace Sidi.GetOpt
{
    internal class CommandSourceCommand : ICommand
    {
        private ICommandSource commandSource;

        public CommandSourceCommand(ICommandSource commandSource)
        {
            this.commandSource = commandSource;
        }

        public string Name => throw new System.NotImplementedException();

        public string Description => throw new System.NotImplementedException();

        public string ArgumentSyntax => throw new System.NotImplementedException();

        public int Invoke(Args args)
        {
            throw new System.NotImplementedException();
        }

        public void PrintUsage(TextWriter w)
        {
            throw new System.NotImplementedException();
        }
    }
}