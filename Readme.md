# Sidi.GetOpt

Library for easy implementation of [getopt](https://www.gnu.org/software/libc/manual/html_node/Argument-Syntax.html#Argument-Syntax) compliant command line interfaces.

## Usage

Install package via nuget
````
nuget install-package Sidi.GetOpt
````

Decorate your classes, fields, properties, and methods with the [Usage](Sidi.GetOpt/Usage.cs) attribute.

See a full example [here](example/Program.cs)

## Todo
- [ ] built-in value parsers for TimeSpan, DateTime, TextReader
- [ ] extensible value parsers
- [x] Modules
- [x] version info
- [x] parameter files (-@arguments.txt)
- [x] Command Objects
- [x] help
- [x] abbreviations of commands and options
