using System;
using System.ComponentModel;

namespace Sidi.GetOpt.Test
{
    public class HelloWorld
    {
        [Description("Greets all names.")] 
        public void Greet(params string[] name)
        {
            foreach (var i in name)
            {
                LastGreeted = i;
                if (Cordiality > 0)
                {
                    Console.WriteLine("Hello, my dear {0}", i);
                }
                else
                {
                    Console.WriteLine("Hello, {0}", i);
                }
            }
        }

        public string LastGreeted { get; private set; }

        [Description("Greet cordially")] 
        public bool Cordially { set { ++Cordiality; } }

        public int Cordiality { get; set; }

        [Description("A option")]
        public bool Alpha { get; set; }

        [Description("B option")]
        public bool Bravo { get; set; }

        [Description("Output file")]
        public string Output { get; set; }
    }
}
