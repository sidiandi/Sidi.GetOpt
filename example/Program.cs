using Sidi.GetOpt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 414

namespace example
{
    [Description("Basic arithmetic operations")]
    class Calculate
    {
        [Description("add")]
        public void Add(double a, double b)
        {
            Console.WriteLine(a + b);
        }

        [Description("multiply")]
        public void Multiply(double a, double b)
        {
            Console.WriteLine(a * b);
        }
    }


    // Decorate a class with the Description attribute to add a usage message
    [Description(@"Demonstrator for the Sidi.GetOpt library. See https://github.com/sidiandi/Sidi.GetOpt.")]
    class Example
    {
        static void Main(string[] args)
        {
            // Add this line to Main to start command line parsing
            GetOpt.Run(new Example(), args);
        }

        // Decorate fields with the Description attribute to turn them into options.
        [Description("Increase verbosity")]
        bool Verbose = false;

        // Decorate properties with the Description attribute to turn them into options.
        [Description("Wait time in seconds")]
        Double Time { get; set; }

        // Decorate methods with the Description attribute to turn them into commands.
        [Description("Wait for 1 second")]
        public void Wait()
        {
            Console.WriteLine("Wait for {0} seonds", Time);
        }

        // Decorate properties or fields with the Command attribute to turn them into sub-commands.
        [Command]
        Calculate Calculate = new Calculate();
    }
}
