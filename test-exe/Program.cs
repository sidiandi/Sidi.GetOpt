using Sidi.GetOpt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 414

namespace example
{
    [Description("Basic arithmetic operations")]
    class Calculate
    {
        [Description("Add a and b and print the result.")]
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
        static int Main(string[] args)
        {
            // Add this line to Main to start command line parsing
            return GetOpt.Run(new Example(), args);
        }

        // Decorate fields with the Description attribute to turn them into options.
        [Description("Increase verbosity")]
        bool Verbose = false;

        // Decorate properties with the Description attribute to turn them into options.
        [Description("A number option")]
        Double Number { get; set; }

        // Decorate methods with the Description attribute to turn them into commands.
        [Description("Wait")]
        public void Wait(int seconds)
        {
            if (Verbose)
            {
                Console.WriteLine("Waiting for {0} seonds", seconds);
            }
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
        }

        // Decorate properties or fields with the Command attribute to turn them into sub-commands.
        [Command]
        Calculate Calculate = new Calculate();
    }
}
