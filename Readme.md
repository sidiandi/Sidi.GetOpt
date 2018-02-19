# Sidi.GetOpt

Library for easy implementation of [getopt](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html#Argument-Syntax) compliant command line interfaces.

## Usage

Install package via nuget
````
nuget install-package Sidi.GetOpt
````

Decorate your classes, fields, properties, and methods with the [Description]() attribute.
````
// Decorate a class with the Description attribute to add a usage message
[Description(@"Demonstrator for the Sidi.GetOpt library. See https://github.com/sidiandi/Sidi.GetOpt.")]
class Example
{
	static void Main(string[] args)
	{
		// Add this line to Main to start command line parsing
		GetOpt.Run(new Example(), args);
	}

	// Decorate fields with the Description attribute to turn them into options.
	[Description("Increase verbosity")]
	bool Verbose = false;

	// Decorate properties with the Description attribute to turn them into options.
	[Description("Wait time in seconds")]
	Double Time { get; set; }

	// Decorate methods with the Description attribute to turn them into commands.
	[Description("Wait a little bit")]
	public void Wait()
	{
		Console.WriteLine("Wait for {0} seconds", Time);
	}

	// Decorate properties or fields with the Command attribute to turn them into sub-commands.
	[Command]
	Calculate Calculate = new Calculate();
}
````


## Todo
- built-in value parsers for TimeSpan, DateTime, TextReader
- extensible value parsers

## Done
- Command Objects
- help
