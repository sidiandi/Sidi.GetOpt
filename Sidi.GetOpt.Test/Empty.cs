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

    class ParameterlessMethodApplication
    {
        [Usage("To test if a parameterless method is called")]
        public void Main()
        {
            MainWasCalled = true;
        }

        [Usage("To test if options are processed")]
        public bool SomeOption { get; set; }

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
