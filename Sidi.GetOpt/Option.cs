using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Sidi.GetOpt
{
    class Option
    {
        public static IOption Create(Func<object> getInstance, MemberInfo member)
        {
            if (getInstance == null)
            {
                throw new ArgumentNullException(nameof(getInstance));
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

            var command = member.GetCustomAttribute<CommandAttribute>();
            if (command != null) return null;

            if (member is FieldInfo)
            {
                return (IOption)new FieldOption(getInstance, (FieldInfo)member, description.Description);
            }
            else if (member is PropertyInfo)
            {
                return (IOption)new PropertyOption(getInstance, (PropertyInfo)member, description.Description);
            }
            throw new NotSupportedException(String.Format("{0} of type {1} is not supported.", member, member.GetType()));
        }

        public static IEnumerable<IOption> GetOptions(Type type, Func<object> getInstance)
        {
            return type.GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
                .Select(_ => Option.Create(getInstance, _))
                .Where(_ => _ != null)
                .ToList();
        }

        class FieldOption : IOption
        {
            private readonly Func<object> getInstance;
            private readonly FieldInfo field;
            private readonly string usage;

            public FieldOption(Func<object> getInstance, FieldInfo field, string usage)
            {
                this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
                this.field = field ?? throw new ArgumentNullException(nameof(field));
                this.usage = usage ?? throw new ArgumentNullException(nameof(usage));
                Name = Util.CSharpIdentifierToLongOption(field.Name);
            }

            public Type Type => field.FieldType;

            public string Name { get; }

            public object Description => field.GetUsage();

            public void Set(string value)
            {
                field.SetValue(this.getInstance(), Util.ParseValue(getInstance(), this.Type, value));
            }
        }

        class PropertyOption : IOption
        {
            private readonly Func<object> getInstance;
            private readonly PropertyInfo property;
            private readonly string usage;

            public PropertyOption(Func<object> getInstance, PropertyInfo property, string usage)
            {
                this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
                this.property = property ?? throw new ArgumentNullException(nameof(property));
                this.usage = usage ?? throw new ArgumentNullException(nameof(usage));
                this.Name = Util.CSharpIdentifierToLongOption(property.Name);
            }

            public Type Type => property.PropertyType;

            public string Name { get; }

            public object Description => property.GetUsage();

            public void Set(string value)
            {
                try
                {
                    var instance = getInstance();
                    property.SetValue(instance, Util.ParseValue(instance, this.Type, value));
                }
                catch (System.Reflection.TargetException ex)
                {
                    throw new Exception(String.Format("cannot set option {0} to {1}", this.Name, value), ex);
                }
            }
        }
    }
}
