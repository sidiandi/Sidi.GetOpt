using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    public partial class GetOpt
    {
        internal readonly ICommandSource commandSource;

        public static int Run(object application, string[] args)
        {
            try
            {
                var a = new Args(args);
                var entryAssembly = Assembly.GetCallingAssembly();
                var programName = application.GetType().Assembly.GetName().Name;
                var rootCommand = new ObjectCommand(null, programName);
                var commandSource = new ObjectCommandSource(rootCommand, ObjectProvider.Create(application)).Concat(
                    new ObjectCommandSource(rootCommand, ObjectProvider.Create(new VersionOption(entryAssembly))), 
                    new ObjectCommandSource(rootCommand, ObjectProvider.Create(new OptionArgumentFile(a)))
                    );
                rootCommand.CommandSource = commandSource;
                
                return rootCommand.AddHelp().Invoke(a);
            }
            catch (ParseError ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -2;
            }
        }

        public static TextReader ReadInputFile(string file)
        {
            if (string.Equals(file, "-"))
            {
                return Console.In;
            }

            return new StreamReader(file);
        }
    }
}
