using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Sidi.GetOpt
{
    internal class OptionArgumentFile
    {
        private readonly Args args;

        public OptionArgumentFile(Args args)
        {
            this.args = args ?? throw new ArgumentNullException(nameof(args));
        }

        [Description("Add arguments from a file"), Alias("@")]
        public string ArgumentFile
        {
            set
            {
                using (var r = GetOpt.ReadInputFile(value))
                {
                    // read arguments and add them to args
                    args.Insert(Sidi.GetOpt.ArgumentFile.Read(r));
                }
            }
        }

        private static IEnumerable<string> ReadArguments(TextReader r)
        {
            throw new NotImplementedException();
        }
    }
}