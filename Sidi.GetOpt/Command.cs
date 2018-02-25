using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal class Command
    {
        public static IEnumerable<ICommand> GetCommands(ICommand parent, IObjectProvider objectProvider, IEnumerable<IOption> inheritedOptions)
        {
            var commandObjects = objectProvider.Type.GetMembers(
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.Static)
                .Where(_ => _.MemberType == System.Reflection.MemberTypes.Property || _.MemberType == System.Reflection.MemberTypes.Field)
                .Select(_ => ObjectCommand.Create(parent, _, objectProvider))
                .Where(_ => _ != null)
                .ToList();

            return objectProvider.Type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => MethodCommand.Create(parent, objectProvider, _, inheritedOptions))
                .Where(_ => _ != null)
                .Concat(commandObjects)
                .ToList();

        }
    }
}
