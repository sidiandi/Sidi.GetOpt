using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class Command
    {
        static IEnumerable<ICommand> GetCommands(ICommand parent, IObjectProvider containingObject, MemberInfo m, IEnumerable<IOption> inheritedOptions)
        {
            if (m.GetCustomAttribute<ModuleAttribute>() != null)
            {
                return GetCommands(parent, ObjectProvider.ResolveNow(containingObject, m), inheritedOptions);
            }

            return ObjectCommand.Create(parent, m, containingObject).ToEnumerable();
        }

        public static IEnumerable<ICommand> GetCommands(ICommand parent, IObjectProvider objectProvider, IEnumerable<IOption> inheritedOptions)
        {
            var commandObjects = objectProvider.Type.GetMembers(
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.Static)
                .Where(_ => _.MemberType == System.Reflection.MemberTypes.Property || _.MemberType == System.Reflection.MemberTypes.Field)
                .SelectMany(member => GetCommands(parent, objectProvider, member, inheritedOptions))
                .Where(_ => _ != null)
                .ToList();

            return objectProvider.Type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .SelectMany(_ => MethodCommand.Create(parent, objectProvider, _, inheritedOptions).ToEnumerable())
                .Concat(commandObjects)
                .ToList();

        }
    }
}
