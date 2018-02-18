using System;

namespace Sidi.GetOpt
{
    internal interface IOption
    {
        Type Type { get; }
        string Name { get; }
        object Description { get; }

        void Set(string value);
    }
}
