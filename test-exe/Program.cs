using Sidi.GetOpt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable 414

namespace example
{
    [Usage("Basic arithmetic operations")]
    class Calculate
    {
        [Usage("Add a and b and print the result.")]
        public void Add(double a, double b)
        {
            Console.WriteLine(a + b);
        }

        [Usage("multiply")]
        public void Multiply(double a, double b)
        {
            Console.WriteLine(a * b);
        }
    }

    class Logging
    {
        [Usage("Increase verbosity")]
        bool Verbosity
        {
            set { ++LogLevel; }
        }

        [Usage("Log level. The higher, the more is logged.")]
        public int LogLevel { get; set; }

        [Usage("Log file path")]
        public string LogFile { set; get; }
    }

    // Decorate a class with the Usage attribute to add a usage message
    [Usage(@"Demonstrator for the Sidi.GetOpt library. See https://github.com/sidiandi/Sidi.GetOpt.")]
    class Example
    {
        static int Main(string[] args)
        {
            // Add this line to Main to start command line parsing
            return GetOpt.Run(new Example(), args);
        }

        // Decorate fields with the Module attribute to extend the current class with the options and commands of the module
        // This is a great way to add "standard" options to you application, e.g. logging 
        [Module]
        Logging logging = new Logging();

        // Decorate properties with the Usage attribute to turn them into options.
        [Usage("A number option")]
        Double Number { get; set; }

        // Decorate methods with the Usage attribute to turn them into commands.
        [Usage("Wait")]
        public void Wait(int seconds)
        {
            if (logging.LogLevel > 0)
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
