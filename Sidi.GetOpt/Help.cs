﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    class HelpApplication
    {
        public HelpApplication(Func<GetOpt> getGetOpt)
        {
            this.getGetOpt = getGetOpt;
        }

        Func<GetOpt> getGetOpt;

        GetOpt GetOpt => getGetOpt();

        [Description("Show this help message.")]
        public bool Help
        {
            set
            {
                ShowHelp(Console.Out);
            }
        }

        [Description("Show version information.")]
        public bool Version
        {
            set
            {
                ShowVersion(Console.Out);
            }
        }

        public Func<GetOpt> GetGetOpt { get; }

        public void ShowVersion(TextWriter w)
        {
            w.WriteLine("Version");
        }

        public void ShowHelp(TextWriter w)
        {
            if (GetOpt.commandSource.Commands.Count() > 1)
            {
                ShowCommandStyleHelp(w);
            }
            else
            {
                ShowOptionStyleHelp(w);
            }
        }

        private void ShowOptionStyleHelp(TextWriter w)
        {
            var g = GetOpt;
            var ea = Assembly.GetEntryAssembly();
            var exeName = g.Invocation;

            w.WriteLine(@"

Usage: " + g.Invocation + @" " + g.commandSource.Commands.Single().Method.GetArgumentSyntax() + @" [option]...

Options:
" + String.Join("\r\n", GetOpt.commandSource.Options.Select(Extensions.GetOptionSyntax)) + @"

");
        }

    private void ShowCommandStyleHelp(TextWriter w)
        {
            var g = GetOpt;
            var ea = Assembly.GetEntryAssembly();
            var exeName = g.Invocation;

            w.WriteLine(@"

Usage: " + g.Invocation + @" <command>

where <command> is one of:");
            w.Wrap(String.Join(", ", g.commandSource.Commands.Select(_ => _.Name)), 4, 72);

            w.WriteLine(@"

" + exeName + @" help <command>  help for command <command>
" + exeName + @" help            overview

");
        }
    }
}
