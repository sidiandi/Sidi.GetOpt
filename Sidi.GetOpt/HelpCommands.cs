using System;
using System.ComponentModel;
using System.IO;

namespace Sidi.GetOpt
{
    internal class HelpCommands
    {
        private Func<GetOpt> getGetOpt;

        public HelpCommands(Func<GetOpt> getGetOpt)
        {
            this.getGetOpt = getGetOpt;
        }

        GetOpt GetOpt => getGetOpt();

        [Description("Help on command")]
        public void Help(string command)
        {
            if (command == null)
            {
                new HelpApplication(getGetOpt).ShowHelp(Console.Out);
            }
            else
            {
                var c = GetOpt.FindCommand(command);
                if (c == null)
                {
                    throw new GetOpt.ParseError(null, String.Format("{0} is not the name of a command.", command));
                }
                c.Invoke(new Args(new[] { "--help" }));
            }
        }
    }
}