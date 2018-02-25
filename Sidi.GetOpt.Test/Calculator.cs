using System;
using System.ComponentModel;
using System.Linq;

namespace Sidi.GetOpt.Test
{
    [Description("Basic calculations")]
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

        [Description("Add two numbers")]
        public void Add(double a, double b)
        {
            Result = a + b;
        }

        [Description("Add numbers")]
        public void Sum(double[] a)
        {
            Result = a.Sum();
        }

        [Description("Print results")]
        public bool Print { get; set; }
    }
}
