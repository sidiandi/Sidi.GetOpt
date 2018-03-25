using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sidi.GetOpt
{
    public interface Maybe<T>
    {
        T Value { get; }
        bool HasValue { get; }

        Maybe<Y> Select<Y>(Func<T, Y> func);
    }

    public class Nothing<T> : Maybe<T>
    {
        public T Value => throw new NotImplementedException();

        public bool HasValue => false;

        public Maybe<Y> Select<Y>(Func<T, Y> func)
        {
            return new Nothing<Y>();
        }
    }

    public class Just<T> : Maybe<T>
    {
        private readonly T value;

        public Just(T value)
        {
            this.value = value;
        }

        public T Value => value;

        public bool HasValue => true;

        public Maybe<Y> Select<Y>(Func<T, Y> func)
        {
            return new Just<Y>(func(Value));
        }
    }

    public static class MaybeExtensions
    {
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                return new Nothing<T>();
            }
            else
            {
                return new Just<T>(value);
            }
        }

        public static Maybe<T> SingleMaybe<T>(this IEnumerable<T> sequence)
        {
            return sequence.SingleOrDefault().ToMaybe();
        }

        public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
        {
            if (maybe.HasValue)
            {
                return new[] { maybe.Value };
            }
            else
            {
                return Enumerable.Empty<T>();
            }
        }

        public static T Or<T>(this Maybe<T> maybe, T defaultValue)
        {
            return maybe.HasValue ? maybe.Value : defaultValue;
        }
    }

}
