using System;
using System.IO;

namespace Sidi.GetOpt.Test
{
    internal class CaptureConsoleOutput : IDisposable
    {
        public CaptureConsoleOutput()
        {
            originalOutput = Console.Out;
            Console.SetOut(output);

            originalError = Console.Error;
            Console.SetError(error);
        }

        TextWriter originalOutput;
        TextWriter originalError;

        public TextWriter output = new StringWriter();
        public TextWriter error = new StringWriter();

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            Console.SetError(originalError);
            Console.WriteLine("Console.Out");
            Console.WriteLine(output);
            Console.WriteLine("Console.Error");
            Console.WriteLine(error);
        }
    }
}