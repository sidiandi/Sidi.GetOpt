using System;

namespace Sidi.GetOpt
{
    internal interface IOption
    {
        Type Type { get; }
        string Name { get; }
        string Description { get; }

        void Set(string value);
    }
}
