using System;
using System.ComponentModel;

namespace Sidi.GetOpt.Test
{
    public class HelloWorld
    {
        [Description("Greet")]
        public void Greet(params string[] name)
        {
            foreach (var i in name)
            {
                Console.WriteLine("Hello, {0}", name);
            }
        }

        [Description("Greet cordially")]
        public bool Cordially { set { ++Cordiality; } }

        public int Cordiality { get; set; }
    }
}
