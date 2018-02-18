using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 414

namespace example 
{
    [Description("Demonstrator for Sidi.GetOpt")]
    class Program
    {
        static void Main(string[] args)
        {
            Sidi.GetOpt.GetOpt.Run(new Program(), args);
        }

        [Description("Increase verbosity")]
        bool Verbose = false;

        [Description("Wait time in seconds")]
        Double Time = 0.0;

        [Description("Wait for 1 second")]
        public void Wait()
        {
            Console.WriteLine("Wait for {0} seonds", Time);
        }
    }
}
