using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sidi.GetOpt
{
    public class Args
    {
        public readonly string[] args;
        public int i = -1;

        public Args(string[] args)
        {
            this.args = args ?? throw new System.ArgumentNullException(nameof(args));
        }

        public bool MoveNext()
        {
            ++i;
            return !IsEnd;
        }

        public bool IsEnd => i >= args.Length;

        public string Current => args[i];

        public IEnumerable<string> Rest => args.Skip(i);

        public override string ToString()
        {
            using (var w = new StringWriter())
            {
                Dump(w);
                return w.ToString();
            }
        }

        public void Dump(TextWriter w)
        {
            for (int i=0; i<this.args.Length; ++i)
            {
                w.WriteLine("{2}{0}: {1}", i, args[i], i == this.i ? "=>" : "  ");
            }
        }
    }
}