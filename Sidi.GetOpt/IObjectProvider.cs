using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    /// <summary>
    /// interface for lazy object creation where you first can query the expected object type and much later ask for the instance.
    /// </summary>
    interface IObjectProvider
    {
        Type Type { get; }
        object Instance { get; }
    }
}
