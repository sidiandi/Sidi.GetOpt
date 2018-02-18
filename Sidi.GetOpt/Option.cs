﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Sidi.GetOpt
{
    abstract class Option : IOption
    {
        public abstract Type Type { get; }
        public string Name { get; protected set; }
        public abstract string Description { get; }

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
                return (IOption)new FieldOption(getInstance, (FieldInfo)member);
            }
            else if (member is PropertyInfo)
            {
                return (IOption)new PropertyOption(getInstance, (PropertyInfo)member);
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

        public abstract void Set(string value);

        bool NeedsValue => !this.Type.Equals(typeof(bool));

        public override string ToString()
        {
            if (NeedsValue)
            {
                return String.Format("--{0}={2} : {1}", this.Name, this.Description, this.Type.Name);
            }
            else
            {
                return String.Format("--{0} : {1}", this.Name, this.Description);
            }
        }

        class FieldOption : Option
        {
            private readonly Func<object> getInstance;
            private readonly FieldInfo field;

            public FieldOption(Func<object> getInstance, FieldInfo field)
            {
                this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
                this.field = field ?? throw new ArgumentNullException(nameof(field));
                Name = Util.CSharpIdentifierToLongOption(field.Name);
            }

            public override Type Type => field.FieldType;

            public override string Description => field.GetUsage();

            public override void Set(string value)
            {
                field.SetValue(this.getInstance(), Util.ParseValue(getInstance(), this.Type, value));
            }

            public override string ToString()
            {
                return String.Format("--{0} : {1}", this.Name, this.Description);
            }
        }

        class PropertyOption : Option
        {
            private readonly Func<object> getInstance;
            private readonly PropertyInfo property;

            public PropertyOption(Func<object> getInstance, PropertyInfo property)
            {
                this.getInstance = getInstance ?? throw new ArgumentNullException(nameof(getInstance));
                this.property = property ?? throw new ArgumentNullException(nameof(property));
                this.Name = Util.CSharpIdentifierToLongOption(property.Name);
            }

            public override Type Type => property.PropertyType;

            public override string Description => property.GetUsage();

            public override void Set(string value)
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
