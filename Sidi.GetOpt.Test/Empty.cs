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

    class ParameterlessMethod
    {
        [Usage("To test if a parameterless method is called")]
        public void Main()
        {
            MainWasCalled = true;
        }

        public bool MainWasCalled { get; private set; }
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
