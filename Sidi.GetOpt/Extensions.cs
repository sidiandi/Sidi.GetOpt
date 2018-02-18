﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    internal static class Extensions
    {
        public static string JoinNonEmpty(this IEnumerable<string> items, string separator)
        {
            return String.Join(separator, items.Where(_ => _ != null).Select(_ => _.Trim()).Where(_ => !String.IsNullOrEmpty(_)));
        }

        public static string GetDescription(this Type type)
        {
            var da = type.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
            if (da == null) return null;
            return da.Description;
        }

        public static string GetDescription(this MemberInfo member)
        {
            var da = member.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
            if (da == null) return null;
            return da.Description;
        }

        public static bool TryRemovePrefix(this string text, string prefix, out string textWithoutPrefix)
        {
            if (text.StartsWith(prefix))
            {
                textWithoutPrefix = text.Substring(prefix.Length);
                return true;
            }
            else
            {
                textWithoutPrefix = null;
                return false;
            }
        }

        public static void Wrap(this TextWriter w, string text, int startColumn = 0, int width = 80)
        {
            var indentText = new String(' ', startColumn);
                var words = Regex.Split(text, @"\s+");
                int c = 0;
                int i = 0;

                for (; i < words.Length;)
                {
                    w.Write(indentText);
                    for (; ; )
                    {
                        if ((i < words.Length) && ((c + words[i].Length) < width))
                        {
                            w.Write(words[i]);
                            w.Write(' ');
                            c += words[i].Length;
                            ++i;
                        }
                        else
                        {
                            w.WriteLine();
                            c = 0;
                            break;
                        }
                    }
                }
            }

        public static MethodInfo GetParser(this Type valueType, Type applicationType, string parserName)
        {
            return applicationType.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                .Where(_ => _.Name.Equals(parserName) && _.TakesASingleString()).FirstOrDefault();
        }

        public static bool TakesASingleString(this MethodInfo m)
        {
            var p = m.GetParameters();
            return p.Length == 1 && p[0].ParameterType.Equals(typeof(string));
        }

        public static bool TryRemovePrefix(this string text, IEnumerable<string> prefixCandidates, out string textWithoutPrefix)
        {
            foreach (var prefix in prefixCandidates)
            {
                if (text.TryRemovePrefix(prefix, out textWithoutPrefix))
                {
                    return true;
                }
            }
            textWithoutPrefix = null;
            return false;
        }

        public static Array ToArray(this IEnumerable<object> elements, Type elementType)
        {
            var elementList = elements.ToList();
            var a = Array.CreateInstance(elementType, elementList.Count);
            for (int i=0; i<elementList.Count; ++i)
            {
                a.SetValue(elementList[i], i);
            }
            return a;
        }

        public static string GetUsage(this MemberInfo p)
        {
            var a = p.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
            return a == null ? null : a.Description;
        }

        public static Type GetValueType(this MemberInfo member)
        {
            if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).PropertyType;
            }
            else if (member is FieldInfo)
            {
                return ((FieldInfo)member).FieldType;
            }
            else
            {
                return null;
            }
        }

        public static Func<object> GetGetter(this MemberInfo member, object containingObject)
        {
            if (member is PropertyInfo)
            {
                var property = (PropertyInfo)member;
                return new Func<object>(() => property.GetValue(containingObject));
            }
            else if (member is FieldInfo)
            {
                var field = (FieldInfo)member;
                return new Func<object>(() => field.GetValue(containingObject));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static Func<object> GetGetter(this MemberInfo member, Func<object> containingObjectGetter)
        {
            var cog = containingObjectGetter;
            var m = member;
            return () => m.GetGetter(cog())();
        }

        static string GetArgumentSyntax(this ParameterInfo _)
        {
            if (_.ParameterType.IsArray)
            {
                return String.Format("[{0}: {1}]...", _.Name, _.ParameterType.GetElementType().Name);
            }
            else
            {
                return String.Format("[{0}: {1}]", _.Name, _.ParameterType.Name);
            }
        }

        public static string GetArgumentSyntax(this MethodInfo m)
        {
            return String.Join(" ", m.GetParameters().Select(GetArgumentSyntax));
        }

        public static string GetSummary(this IOption option)
        {
            if (option.Type.Equals(typeof(bool)))
            {
                return String.Format("--{0} : {2}", option.Name, option.Type, option.Description);
            }
            else
            {
                return String.Format("--{0}={1} : {2}", option.Name, option.Type, option.Description);
            }
        }

        public static string GetSummary(this ICommand command)
        {
            return String.Format("{0} : {1}", command.Name, command.Description);
        }
    }
}
