using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    class VersionApplication
    {
        [Usage("Show version information.")]
        public bool Version
        {
            set
            {
                ShowVersion(Console.Out);
            }
        }
        public void ShowVersion(TextWriter w)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            w.WriteLine("Version {0}", entryAssembly.GetName().Version);
        }
    }
}
