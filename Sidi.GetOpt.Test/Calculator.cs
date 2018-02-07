using System.ComponentModel;
using System.Linq;

namespace Sidi.GetOpt.Test
{
    internal class Calculator
    {
        public Calculator() { }

        public double Result { get; private set; }

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
    }
}
