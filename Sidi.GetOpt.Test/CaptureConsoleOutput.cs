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

        TextWriter output = new StringWriter();
        TextWriter error = new StringWriter();

        public void Dispose()
        {
            Console.SetOut(originalOutput);
            Console.SetError(originalError);
            Console.WriteLine("Output:");
            Console.WriteLine(output);
            Console.WriteLine("Error:");
            Console.WriteLine(error);
        }
    }
}