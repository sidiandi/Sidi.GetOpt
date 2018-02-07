using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    class VersionApplication
    {
        [Description("Show version information.")]
        public bool Version
        {
            set
            {
                ShowVersion(Console.Out);
            }
        }
        public void ShowVersion(TextWriter w)
        {
            var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyVersionAttribute>().Version;
            w.WriteLine("Version {0}", version);
        }
    }
}
