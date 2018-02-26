using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class VersionOption
    {
        private readonly Assembly assembly;

        public VersionOption(Assembly assembly)
        {
            this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        [Usage("Show version information")]
        public bool Version
        {
            set
            {
                var n = assembly.GetName();
                Console.WriteLine("{0} {1}", n.Name, n.Version);
            }
        }

    }
}
