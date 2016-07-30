using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Iterate<T>(T value, Func<T, T> next)
        {
            while (true)
            {
                yield return value;
                value = next(value);
            }
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> that, T value)
        {
            if (that == null)
                throw new ArgumentNullException("that");
            yield return value;
            foreach (T obj in that)
                yield return obj;
        }

        public static T MaxOrNull<T>(this IEnumerable<T> that, Func<T, IComparable> projectionFunction) where T : struct
        {
            T obj1 = default(T);
            if (!that.Any<T>())
                return obj1;
            T obj2 = that.First<T>();
            IComparable comparable1 = projectionFunction(obj2);
            foreach (T obj3 in that.Skip<T>(1))
            {
                IComparable comparable2 = projectionFunction(obj3);
                if (comparable1.CompareTo(comparable2) < 0)
                {
                    comparable1 = comparable2;
                    obj2 = obj3;
                }
            }
            return obj2;
        }

        public static T? MaxOrNullable<T>(this IEnumerable<T> that) where T : struct, IComparable
        {
            if (!that.Any<T>())
                return new T?();
            return new T?(that.Max<T>());
        }

        public static T? MinOrNullable<T>(this IEnumerable<T> that) where T : struct, IComparable
        {
            if (!that.Any<T>())
                return new T?();
            return new T?(that.Min<T>());
        }

        public static IEnumerable<int> Range(int from, int to, int by)
        {
            if (by <= 0)
                throw new ArgumentOutOfRangeException("by", "Parameter by is expected to be larger than 0");
            if (from < to)
            {
                int i = from;
                while (i <= to)
                {
                    yield return i;
                    i += by;
                }
            }
            else
            {
                int i = from;
                while (i >= to)
                {
                    yield return i;
                    i -= by;
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                return;
            foreach (T obj in source)
                action(obj);
        }
    }
}
