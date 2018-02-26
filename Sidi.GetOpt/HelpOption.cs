using System;

namespace Sidi.GetOpt
{
    internal class HelpOption
    {
        public HelpOption()
        {
        }

        [Usage("Show this help message"), Alias("h")]
        public bool Help { set { Command.PrintUsage(Console.Out);  } }

        public ICommand Command { get; set; }
    }

    internal static class HelpOptionExtensions
    {
        public static ICommand AddHelp(this ObjectCommand command)
        {
            var help = new HelpOption();
            command.CommandSource = command.CommandSource.Concat(new ObjectCommandSource(
                null, 
                ObjectProvider.Create(help.GetType(), () => help)
                ));
            help.Command = command;
            return command;
        }
    }
}