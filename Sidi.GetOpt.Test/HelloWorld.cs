using System;

namespace Sidi.GetOpt.Test
{
    public class HelloWorld
    {
        [Usage("Greets all names.")] 
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

        [Usage("Greet cordially")] 
        public bool Cordially { set { ++Cordiality; } }

        public int Cordiality { get; set; }

        [Usage("A option")]
        public bool Alpha { get; set; }

        [Usage("B option")]
        public bool Bravo { get; set; }

        [Usage("D option")]
        public bool Delta { get; set; }

        [Usage("Output file")]
        public string Output { get; set; }
    }
}
