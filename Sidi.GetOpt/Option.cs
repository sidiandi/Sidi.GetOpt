using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sidi.GetOpt
{
    abstract class Option : IOption
    {
        public abstract Type Type { get; }

        public abstract IEnumerable<string> Aliases { get;  }

        public string Name { get; protected set; }
        public abstract string Usage { get; }

        static IEnumerable<IOption> noOptions = Enumerable.Empty<IOption>();

        public static IEnumerable<IOption> GetOptions(IObjectProvider container, MemberInfo member)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (!(member is FieldInfo || member is PropertyInfo))
            {
                return noOptions;
            }

            if (member.GetCustomAttribute<ModuleAttribute>() != null)
            {
                return Option.GetOptions(ObjectProvider.ResolveNow(container, member));
            }

            var usage = member.GetUsage();
            if (usage == null) return noOptions;

            var command = member.GetCustomAttribute<CommandAttribute>();
            if (command != null) return noOptions;

            if (member is FieldInfo)
            {
                return new[] { (IOption)new FieldOption(container, (FieldInfo)member) };
            }
            else if (member is PropertyInfo)
            {
                return new[] { (IOption)new PropertyOption(container, (PropertyInfo)member) };
            }
            throw new NotSupportedException(String.Format("{0} of type {1} is not supported.", member, member.GetType()));
        }

        public static IEnumerable<IOption> GetOptions(IObjectProvider getInstance)
        {
            var members = getInstance.Type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);

            return members.SelectMany(_ => Option.GetOptions(getInstance, _))
                .Where(_ => _ != null)
                .ToList();
        }

        public abstract void Set(string value);

        bool NeedsValue => !this.Type.Equals(typeof(bool));

        public override string ToString()
        {
            if (NeedsValue)
            {
                return String.Format(
                    Aliases.Where(_ => _.Length == 1).Select(_ => String.Format("-{0}{1}", _, this.Type.Name))
                    .Concat(Aliases.Where(_ => _.Length > 1).Select(_ => String.Format("--{0}={1}", _, this.Type.GetNameForUsage())))
                    .JoinNonEmpty("|")
                    + " : " + this.Usage);
            }
            else
            {
                return String.Format(
                    Aliases.Where(_ => _.Length == 1).Select(_ => String.Format("-{0}", _, this.Type.Name))
                    .Concat(Aliases.Where(_ => _.Length > 1).Select(_ => String.Format("--{0}", _, this.Type.Name)))
                    .JoinNonEmpty("|")
                    + " : " + this.Usage);
            }
        }

        class FieldOption : Option
        {
            private readonly IObjectProvider getInstance;
            private readonly FieldInfo field;

            public FieldOption(IObjectProvider getInstance, FieldInfo field)
            {
                this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
                this.field = field ?? throw new ArgumentNullException(nameof(field));
                Name = Util.CSharpIdentifierToLongOption(field.Name);
            }

            public override Type Type => field.FieldType;

            public override IEnumerable<string> Aliases => new[] { Name }.Concat(this.field.GetCustomAttributes<AliasAttribute>().Select(_ => _.Alias));

            public override string Usage => field.GetUsage();

            public override void Set(string value)
            {
                field.SetValue(this.getInstance.Instance, Util.ParseValue(getInstance.Instance, this.Type, value));
            }

            public override string ToString()
            {
                return String.Format("--{0} : {1}", this.Name, this.Usage);
            }
        }

        class PropertyOption : Option
        {
            private readonly IObjectProvider getInstance;
            private readonly PropertyInfo property;

            public PropertyOption(IObjectProvider getInstance, PropertyInfo property)
            {
                this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
                this.property = property ?? throw new ArgumentNullException(nameof(property));
                this.Name = Util.CSharpIdentifierToLongOption(property.Name);
            }

            public override Type Type => property.PropertyType;

            public override IEnumerable<string> Aliases => new[] { Name }.Concat(this.property.GetCustomAttributes<AliasAttribute>().Select(_ => _.Alias));

            public override string Usage => property.GetUsage();

            public override void Set(string value)
            {
                try
                {
                    property.SetValue(getInstance.Instance, Util.ParseValue(getInstance.Instance, this.Type, value));
                }
                catch (System.Reflection.TargetException ex)
                {
                    throw new Exception(String.Format("cannot set option {0} to {1}", this.Name, value), ex);
                }
            }
        }
    }
}
