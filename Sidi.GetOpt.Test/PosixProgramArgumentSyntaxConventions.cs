using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt.Test
{
    /// <summary>
    /// Tests if Sidi.GetOpt fulfills the POSIX Program Argument Syntax Conventions (https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html#Argument-Syntax)
    /// </summary>
    /* 25.1.1 Program Argument Syntax Conventions

    POSIX recommends these conventions for command line arguments.getopt(see Getopt) and argp_parse(see Argp) make it easy to implement them.

    Arguments are options if they begin with a hyphen delimiter (‘-’).
    Multiple options may follow a hyphen delimiter in a single token if the options do not take arguments.Thus, ‘-abc’ is equivalent to ‘-a -b -c’.
    Option names are single alphanumeric characters(as for isalnum; see Classification of Characters).
    Certain options require an argument.For example, the ‘-o’ command of the ld command requires an argument—an output file name.
    An option and its argument may or may not appear as separate tokens. (In other words, the whitespace separating them is optional.) Thus, ‘-o foo’ and ‘-ofoo’ are equivalent.
    Options typically precede other non-option arguments.


    The implementations of getopt and argp_parse in the GNU C Library normally make it appear as if all the option arguments were specified before all the non-option arguments for the purposes of parsing, even if the user of your program intermixed option and non-option arguments. They do this by reordering the elements of the argv array. This behavior is nonstandard; if you want to suppress it, define the _POSIX_OPTION_ORDER environment variable.See Standard Environment.
    The argument ‘--’ terminates all options; any following arguments are treated as non-option arguments, even if they begin with a hyphen.
    A token consisting of a single hyphen character is interpreted as an ordinary non-option argument.By convention, it is used to specify input from or output to the standard input and output streams.

    Options may be supplied in any order, or appear multiple times. The interpretation is left up to the particular application program.

    GNU adds long options to these conventions.Long options consist of ‘--’ followed by a name made of alphanumeric characters and dashes. Option names are typically one to three words long, with hyphens to separate words.Users can abbreviate the option names as long as the abbreviations are unique.

    To specify an argument for a long option, write ‘--name= value’. This syntax enables a long option to accept an argument that is itself optional.

    Eventually, GNU systems will provide completion for long option names in the shell. 
    */
    [TestFixture]
    public class PosixProgramArgumentSyntaxConventions
    {
        // Arguments are options if they begin with a hyphen delimiter (‘-’).
        [Test]
        public void ArgumentsBeginWithHyphen()
        {
            var hw = new HelloWorld();
            var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "-c", "c" });
            Assert.AreEqual(0, exitCode);
            Assert.AreEqual(1, hw.Cordiality);
        }

        // Multiple options may follow a hyphen delimiter in a single token if the options do not take arguments.Thus, ‘-abc’ is equivalent to ‘-a -b -c’.
        // Option names are single alphanumeric characters(as for isalnum; see Classification of Characters).
        [Test]
        public void Multiple_options_may_follow_a_hyphen_delimiter_in_a_single_token_if_the_options_do_not_take_arguments()
        {
            {
                var hw = new HelloWorld();
                var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "-abc" });
                Assert.AreEqual(0, exitCode);
                Assert.AreEqual(1, hw.Cordiality);
                Assert.AreEqual(true, hw.Alpha);
                Assert.AreEqual(true, hw.Bravo);
            }

            {
                var hw = new HelloWorld();
                var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "-a", "-b", "-c" });
                Assert.AreEqual(0, exitCode);
                Assert.AreEqual(1, hw.Cordiality);
                Assert.AreEqual(true, hw.Alpha);
                Assert.AreEqual(true, hw.Bravo);
            }
        }

        // Certain options require an argument.For example, the ‘-o’ command of the ld command requires an argument—an output file name.
        // An option and its argument may or may not appear as separate tokens. (In other words, the whitespace separating them is optional.) Thus, ‘-o foo’ and ‘-ofoo’ are equivalent. 
        public void Certain_options_require_an_argument()
        {
            {
                var hw = new HelloWorld();
                var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "-ooutput.txt" });
                Assert.AreEqual(0, exitCode);
                Assert.AreEqual("output.txt", hw.Output);
            }

            {
                var hw = new HelloWorld();
                var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "-o", "output.txt" });
                Assert.AreEqual(0, exitCode);
                Assert.AreEqual("output.txt", hw.Output);
            }
        }

        // The argument ‘--’ terminates all options; any following arguments are treated as non-option arguments, even if they begin with a hyphen. 
        [Test]
        public void The_argument_hyphen_hyphen_terminates_all_options()
        {
            var hw = new HelloWorld();
            var exitCode = Sidi.GetOpt.GetOpt.Run(hw, new[] { "--alpha", "--", "--bravo" });
            Assert.AreEqual(0, exitCode);
            Assert.AreEqual(true, hw.Alpha);
            Assert.AreEqual(false, hw.Bravo);
            Assert.AreEqual("--bravo", hw.LastGreeted);
        }
    }
}
