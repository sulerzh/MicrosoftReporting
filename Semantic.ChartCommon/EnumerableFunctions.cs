using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class EnumerableFunctions
    {
        public static int FastCount(this IEnumerable that)
        {
            IList list = that as IList;
            if (list != null)
                return list.Count;
            return that.Cast<object>().Count<object>();
        }

        public static bool IsEmpty(this IEnumerable that)
        {
            return !that.GetEnumerator().MoveNext();
        }

        public static T MinOrNull<T>(this IEnumerable<T> that, Func<T, IComparable> projectionFunction) where T : class
        {
            T obj1 = default(T);
            if (!that.Any<T>())
                return obj1;
            T obj2 = that.First<T>();
            IComparable comparable1 = projectionFunction(obj2);
            foreach (T obj3 in that.Skip<T>(1))
            {
                IComparable comparable2 = projectionFunction(obj3);
                if (comparable1.CompareTo(comparable2) > 0)
                {
                    comparable1 = comparable2;
                    obj2 = obj3;
                }
            }
            return obj2;
        }

        public static double SumOrDefault(this IEnumerable<double> that)
        {
            if (!that.Any<double>())
                return 0.0;
            return that.Sum();
        }

        public static T MaxOrNull<T>(this IEnumerable<T> that, Func<T, IComparable> projectionFunction) where T : class
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

        public static IEnumerable<T> Iterate<T>(T value, Func<T, T> nextFunction)
        {
            yield return value;
            while (true)
            {
                value = nextFunction(value);
                yield return value;
            }
        }

        public static int IndexOf(this IEnumerable that, object value)
        {
            int num = 0;
            foreach (object objB in that)
            {
                if (object.ReferenceEquals(value, objB) || value.Equals(objB))
                    return num;
                ++num;
            }
            return -1;
        }

        public static int FindIndexOf<T>(this IEnumerable<T> that, Func<T, bool> action)
        {
            int num = 0;
            foreach (T obj in that)
            {
                if (action(obj))
                    return num;
                ++num;
            }
            return -1;
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> that, Action<T, int> action)
        {
            int num = 0;
            foreach (T obj in that)
            {
                action(obj, num);
                ++num;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> that, Action<T> action)
        {
            foreach (T obj in that)
                action(obj);
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

        public static IEnumerable<TSource> DistinctOfSorted<TSource>(this IEnumerable<TSource> source)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    TSource last = enumerator.Current;
                    yield return last;
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.Equals(last))
                        {
                            last = enumerator.Current;
                            yield return last;
                        }
                    }
                }
            }
        }

        public static T FastElementAt<T>(this IEnumerable that, int index)
        {
            IList<T> objList = that as IList<T>;
            if (objList != null)
                return objList[index];
            IList list = that as IList;
            if (list != null)
                return (T)list[index];
            return that.Cast<T>().ElementAt<T>(index);
        }

        public static bool IsSameAs<T>(this IList<T> one, IList<T> another)
        {
            if (one == another)
                return true;
            if (one == null || another == null || one.Count != another.Count)
                return false;
            for (int index = 0; index < one.Count; ++index)
            {
                if (!object.Equals(one[index], another[index]))
                    return false;
            }
            return true;
        }

        public static bool IsSameAs<T>(this ISet<T> one, ISet<T> another)
        {
            if (one == another)
                return true;
            if (one == null || another == null || one.Count != another.Count)
                return false;
            foreach (T obj in one)
            {
                if (!another.Contains(obj))
                    return false;
            }
            return true;
        }
    }
}
