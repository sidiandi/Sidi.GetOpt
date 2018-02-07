using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace Sidi.GetOpt
{
    public class CommandSource : ICommandSource
    {
        private readonly object application;

        class Command : ICommand
        {
            private readonly Func<GetOpt> getGetOpt;
            private readonly object application;
            private readonly MethodInfo method;
            private readonly string usage;

            public string Name => Util.CSharpIdentifierToLongOption(method.Name);

            GetOpt getOpt => getGetOpt();

            public MethodInfo Method => this.method;

            public static ICommand Create(Func<GetOpt> getGetOpt, object application, MethodInfo method)
            {
                if (application == null)
                {
                    throw new ArgumentNullException(nameof(application));
                }

                if (method == null)
                {
                    throw new ArgumentNullException(nameof(method));
                }

                var description = method.GetCustomAttribute<DescriptionAttribute>();
                if (description == null) return null;

                return new Command(getGetOpt, application, method, description.Description);
            }

            class CommandHelp
            {
                private readonly Command command;

                public CommandHelp(Command command)
                {
                    this.command = command ?? throw new ArgumentNullException(nameof(command));
                }

                [Description("Show help for command.")]
                public bool Help
                {
                    set
                    {
                        command.PrintUsage(Console.Out);
                    }
                }
            }

            public void Invoke(Args args)
            {
                GetOpt getOpt = null;
                getOpt = new GetOpt(
                    new CompositeCommandSource(new[]
                    {
                        new CommandSource(() => getOpt, new CommandHelp(this)),
                        this.getGetOpt().commandSource
                    }));

                var parameters = this.method.GetParameters()
                    .Select(_ => getOpt.GetParameter(_.ParameterType, args))
                    .ToArray();
                this.method.Invoke(this.application, parameters);
            }

            public void PrintUsage(TextWriter w)
            {
                var arguments = method.GetArgumentSyntax();
                var options = String.Join("\r\n", this.getOpt.commandSource.Options.Select(Extensions.GetOptionSyntax));
                w.WriteLine(@"" + getOpt.Invocation + @" " + this.Name + @" " + arguments + @" [options]

" + this.method.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>().Description + @"

Options:
" + options + @"
");
            }

            Command(Func<GetOpt> getGetOpt, object application, MethodInfo method, string usage)
            {
                this.getGetOpt = getGetOpt ?? throw new ArgumentNullException(nameof(getGetOpt));
                this.application = application ?? throw new ArgumentNullException(nameof(application));
                this.method = method ?? throw new ArgumentNullException(nameof(method));
                this.usage = usage ?? throw new ArgumentNullException(nameof(usage));
            }
        }

        class CommandObject : ICommand
        {
            public static ICommand Create(Func<GetOpt> getParent, object application, MemberInfo member)
            {
                if (getParent == null)
                {
                    throw new ArgumentNullException(nameof(getParent));
                }

                if (application == null)
                {
                    throw new ArgumentNullException(nameof(application));
                }

                if (member == null)
                {
                    throw new ArgumentNullException(nameof(member));
                }

                var command = member.GetCustomAttribute<CommandAttribute>();
                if (command == null)
                {
                    return null;
                }

                return new CommandObject(getParent, application, member);
            }

            CommandObject(Func<GetOpt> getParent, object application, MemberInfo member)
            {
                Name = Util.CSharpIdentifierToLongOption(member.Name);
                getInstance = member.GetGetter(application);
                this.getParent = getParent ?? throw new ArgumentNullException(nameof(getParent));
            }

            Func<object> getInstance;
            private readonly Func<GetOpt> getParent;

            public string Name { get; }

            public MethodInfo Method => throw new NotImplementedException();

            public void Invoke(Args args)
            {
                GetOpt getOpt = null;

                var commandSource = (ICommandSource)new CommandSource(() => getOpt, getInstance());

                if (commandSource.Commands.Count() > 1)
                {
                    commandSource = commandSource.Concat(new CommandSource(() => getOpt, new HelpCommands(() => getOpt)));
                }
                commandSource = commandSource.Concat(new CommandSource(() => getOpt, new HelpApplication(() => getOpt)));

                getOpt = new GetOpt(commandSource);
                getOpt.Invocation = String.Join(" ", getParent().Invocation, Name);

                getOpt.Run(args);
            }
        }

        class Option
        {
            public static IOption Create(object application, MemberInfo member)
            {
                if (application == null)
                {
                    throw new ArgumentNullException(nameof(application));
                }

                if (member == null)
                {
                    throw new ArgumentNullException(nameof(member));
                }

                if (!(member is FieldInfo || member is PropertyInfo))
                {
                    return null;
                }

                var description = member.GetCustomAttribute<DescriptionAttribute>();
                if (description == null) return null;

                if (member is FieldInfo)
                {
                    return (IOption) new FieldOption(application, (FieldInfo) member, description.Description);
                }
                else if (member is PropertyInfo)
                {
                    return (IOption) new PropertyOption(application, (PropertyInfo)member, description.Description);
                }
                throw new NotSupportedException(String.Format("{0} of type {1} is not supported.", member, member.GetType()));
            }

            class FieldOption : IOption
            {
                private readonly object application;
                private readonly FieldInfo field;
                private readonly string usage;

                public FieldOption(object application, FieldInfo field, string usage)
                {
                    this.application = application ?? throw new ArgumentNullException(nameof(application));
                    this.field = field ?? throw new ArgumentNullException(nameof(field));
                    this.usage = usage ?? throw new ArgumentNullException(nameof(usage));
                    Name = Util.CSharpIdentifierToLongOption(field.Name);
                }

                public Type Type => field.FieldType;

                public string Name { get; }

                public object Description => field.GetUsage();

                public void Set(string value)
                {
                    field.SetValue(this.application, Util.ParseValue(application, this.Type, value));
                }
            }

            class PropertyOption : IOption
            {
                private readonly object application;
                private readonly PropertyInfo property;
                private readonly string usage;

                public PropertyOption(object application, PropertyInfo property, string usage)
                {
                    this.application = application ?? throw new ArgumentNullException(nameof(application));
                    this.property = property ?? throw new ArgumentNullException(nameof(property));
                    this.usage = usage ?? throw new ArgumentNullException(nameof(usage));
                    this.Name = Util.CSharpIdentifierToLongOption(property.Name);
                }

                public Type Type => property.PropertyType;

                public string Name { get; }

                public object Description => property.GetUsage();

                public void Set(string value)
                {
                    property.SetValue(this.application, Util.ParseValue(application, this.Type, value));
                }
            }
        }

        public CommandSource(Func<GetOpt> getGetOpt, object application)
        {
            this.application = application ?? throw new ArgumentNullException(nameof(application));

            var type = this.application.GetType();

            var commandObjects = type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => CommandObject.Create(getGetOpt, this.application, _))
                .Where(_ => _ != null)
                .ToList();

            Commands = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => Command.Create(getGetOpt, this.application, _))
                .Where(_ => _ != null)
                .Concat(commandObjects)
                .ToList();

            Options = type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => Option.Create(this.application, _))
                .Where(_ => _ != null)
                .ToList();
        }

        public IEnumerable<ICommand> Commands { get; }
        public IEnumerable<IOption> Options { get; }

        public object ParseValue(Type type, string value)
        {
            // has application a Parse{Type} method?
            {
                var parser = type.GetParser(this.application.GetType(), "Parse" + type.Name);
                if (parser != null)
                {
                    return parser.Invoke(application, new[] { value });
                }
            }
            return null;
        }
    }
}
