using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class RangeEnumerableExtensions
    {
        public static Range<T> GetRange<T>(this IEnumerable<T> that) where T : IComparable
        {
            if (!that.Any<T>())
                return new Range<T>();
            T minimum = that.First<T>();
            T maximum = minimum;
            foreach (T obj in that)
            {
                if (ValueHelper.Compare(minimum, obj) == 1)
                    minimum = obj;
                if (ValueHelper.Compare(maximum, obj) == -1)
                    maximum = obj;
            }
            if (minimum == null || maximum == null)
                return Range<T>.Empty;
            return new Range<T>(minimum, maximum);
        }

        public static Range<T> Sum<T>(this IEnumerable<Range<T>> that) where T : IComparable
        {
            if (!that.Any<Range<T>>())
                return new Range<T>();
            return that.Aggregate<Range<T>>((x, y) => x.Add(y));
        }
    }
}
