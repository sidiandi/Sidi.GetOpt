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
            return new GetOpt
            {
                Application = application,
                Arguments = args
            }.Run();
        }

        public int Run()
        {
            try
            {
                var a = new Args(this.Arguments);
                if (OnException != null)
                {
                    a.OnException = (e) =>
                    {
                        return this.OnException(e);
                    };
                }
                var entryAssembly = Assembly.GetCallingAssembly();
                var programName = this.Application.GetType().Assembly.GetName().Name;
                var rootCommand = ObjectCommand.Create(programName, ObjectProvider.Create(new Starter(this.Application, a)));
                return rootCommand.Invoke(a);
            }
            catch (ParseError ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        public string[] Arguments { set; get; }
        public Func<Exception, int> OnException { set; get; }
        public object Application { get; set; }

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
