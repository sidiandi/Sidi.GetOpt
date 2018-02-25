using System;
using System.ComponentModel;

namespace Sidi.GetOpt
{
    internal class HelpOption
    {
        public HelpOption()
        {
        }

        [Description("Show this help message")]
        public bool Help { set { Command.PrintUsage(Console.Out);  } }

        public ICommand Command { get; set; }
    }

    internal static class HelpOptionExtensions
    {
        public static ICommand AddHelp(this ObjectCommand command)
        {
            var help = new HelpOption();
            command.CommandSource = command.CommandSource.Concat(new ObjectCommandSource(null, help.GetType(), () => help));
            help.Command = command;
            return command;
        }
    }
}