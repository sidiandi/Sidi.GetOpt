using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt.Test
{
    class Empty
    {
    }

    class OptionsApplication
    {
        [Usage("prints all arguments")]
        public void Main(params string[] argument)
        {
            this.arguments = argument; 
        }

        public string[] arguments;
    }
}
