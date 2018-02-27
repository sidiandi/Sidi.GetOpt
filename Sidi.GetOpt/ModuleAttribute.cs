using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class ModuleAttribute : Attribute
    {
        internal static IEnumerable<IObjectProvider> Get(IObjectProvider container)
        {
            return container.Type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(m => (null == m.GetCustomAttribute<ModuleAttribute>()) ? null : m.GetGetter(container))
                .Where(_ => _ != null)
                .ToList();
        }
    }
}
