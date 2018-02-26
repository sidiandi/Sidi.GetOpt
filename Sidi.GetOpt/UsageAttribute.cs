using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field|AttributeTargets.Method|AttributeTargets.Property)]
    public class UsageAttribute : Attribute
    {
        public UsageAttribute(string usage)
        {
            Usage = usage ?? throw new ArgumentNullException(nameof(usage));
        }

        public string Usage { get; }
    }
}
