using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sidi.GetOpt
{
    internal class Args
    {
        string[] args;
        public int i = -1;
        public IList<object> parameters = new List<object>();

        internal string[] LongOptionPrefix { get; set; }
        internal string[] ShortOptionPrefix { get; set; }

        public Args(IEnumerable<string> args)
        {
            this.args = (args ?? throw new System.ArgumentNullException(nameof(args))).ToArray();
            LongOptionPrefix = new[] { "--" };
            ShortOptionPrefix = new[] { "-" };
        }

        public void Insert(IEnumerable<string> toInsert)
        {
            args = args.Take(i+1).Concat(toInsert).Concat(args.Skip(i+1)).ToArray();
        }

        public bool MoveNext()
        {
            ++i;
            return !IsEnd;
        }

        public bool IsEnd => i >= args.Length;

        public string Current => args[i];

        public IEnumerable<string> Rest => args.Skip(i);

        public string Next => this.args[i + 1];

        public bool HasNext => (i + 1) < args.Length;

        public bool TreatAsParameters { set; get; }

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

        public bool MovePrevious()
        {
            if (i >= 0)
            {
                --i;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}