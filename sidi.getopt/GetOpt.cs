using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                var command = ObjectCommand.Create(Util.CSharpIdentifierToLongOption(application.GetType().Name), application.GetType(), () => application);
                return command.Invoke(new Args(args));
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
    }
}
