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

        class Starter
        {
            [Module]
            object application;

            [Module]
            VersionOption version;

            [Module]
            OptionArgumentFile argumentFile;

            public Starter(object application, Args args)
            {
                this.application = application ?? throw new ArgumentNullException(nameof(application));
                this.version = new VersionOption(application.GetType().Assembly);
                this.argumentFile = new OptionArgumentFile(args);
            }
        }

        public static int Run(object application, string[] args)
        {
            try
            {
                var a = new Args(args);
                var entryAssembly = Assembly.GetCallingAssembly();
                var programName = application.GetType().Assembly.GetName().Name;

                /*
                var rootCommand = new ObjectCommand(null, programName);
                var commandSource = new ObjectCommandSource(rootCommand, ObjectProvider.Create(application)).Concat(
                    new ObjectCommandSource(rootCommand, ObjectProvider.Create(new VersionOption(entryAssembly))), 
                    new ObjectCommandSource(rootCommand, ObjectProvider.Create(new OptionArgumentFile(a)))
                    );
                rootCommand.CommandSource = commandSource;
                */
                var rootCommand = ObjectCommand.Create(programName, ObjectProvider.Create(new Starter(application, a)));

                try
                {
                    return rootCommand.Invoke(a);
                }
                catch (Exception e)
                {
                    throw ParseError.ToParseError(a, e);
                }
            }
            catch (ParseError ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// if file equals "-", this methods opens Console.In. Otherwise, the file is opened.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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
