# Sidi.GetOpt

Library for easy implementation of [getopt](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html#Argument-Syntax) compliant command line interfaces.

## Usage

Install package via nuget
````
nuget install-package Sidi.GetOpt
````

Decorate your classes, fields, properties, and methods with the Usage attribute.
````C#
// Decorate a class with the Usage attribute to add a usage message
[Usage(@"Demonstrator for the Sidi.GetOpt library. See https://github.com/sidiandi/Sidi.GetOpt.")]
class Example
{
    static int Main(string[] args)
    {
        // Add this line to Main to start command line parsing
        return GetOpt.Run(new Example(), args);
    }

    // Decorate fields with the Usage attribute to turn them into options.
    [Usage("Increase verbosity")]
    bool Verbose = false;

    // Decorate properties with the Usage attribute to turn them into options.
    [Usage("A number option")]
    Double Number { get; set; }

    // Decorate methods with the Usage attribute to turn them into commands.
    [Usage("Wait")]
    public void Wait(int seconds)
    {
        if (Verbose)
        {
            Console.WriteLine("Waiting for {0} seonds", seconds);
        }
        Thread.Sleep(TimeSpan.FromSeconds(seconds));
    }

    // Decorate properties or fields with the Command attribute to turn them into sub-commands.
    [Command]
    Calculate Calculate = new Calculate();
}
````

Result:
````
C:\>example --help
Usage: example [option]... <command>

Demonstrator for the Sidi.GetOpt library. See https://github.com/sidiandi/Sidi.GetOpt.

Commands:
wait : Wait for 1 second
calculate : Basic arithmetic operations

Options:
--verbose : Increase verbosity
--time : Wait time in seconds
--help : Show this help message
````

## Todo
- built-in value parsers for TimeSpan, DateTime, TextReader
- extensible value parsers

## Done
- version info
- parameter files
- Command Objects
- help
- abbreviations of commands and options
