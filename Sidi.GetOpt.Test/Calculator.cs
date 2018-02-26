using System;
using System.Linq;

namespace Sidi.GetOpt.Test
{
    [Usage("Basic calculations")]
    internal class Calculator
    {
        public Calculator() { }

        public double Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
                if (Print)
                {
                    Console.WriteLine("Result: {0}", result);
                }
            }
        }
        double result = 0.0;

        [Usage("Add two numbers")]
        public void Add(double a, double b)
        {
            Result = a + b;
        }

        [Usage("Add numbers")]
        public void Sum(double[] a)
        {
            Result = a.Sum();
        }

        [Usage("Print results")]
        public bool Print { get; set; }
    }
}
