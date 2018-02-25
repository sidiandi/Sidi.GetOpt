using System;
using System.Collections.Generic;

namespace Sidi.GetOpt
{
    internal interface IOption
    {
        Type Type { get; }
        string Name { get; }
        string Description { get; }
        void Set(string value);
        IEnumerable<string> Aliases { get; }
    }
}
