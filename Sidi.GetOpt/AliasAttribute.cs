using System;

namespace Sidi.GetOpt
{
    /// <summary>
    /// Add an alias name to an option or command
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    internal class AliasAttribute : Attribute
    {
        public AliasAttribute(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                throw new ArgumentException("message", nameof(alias));
            }

            Alias = alias;
        }

        public string Alias { get; }
    }
}