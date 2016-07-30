using Microsoft.Reporting.Windows.Common.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public struct Range<T> where T : IComparable
    {
        private static Range<T> _empty = new Range<T>(false);
        private readonly bool _hasData;
        private readonly T _maximum;
        private readonly T _minimum;

        public bool HasData
        {
            get
            {
                return this._hasData;
            }
        }

        public static Range<T> Empty
        {
            get
            {
                return Range<T>._empty;
            }
        }

        public T Maximum
        {
            get
            {
                if (!this.HasData)
                    throw new InvalidOperationException(Resources.Range_get_Maximum_CannotReadTheMaximumOfAnEmptyRange);
                return this._maximum;
            }
        }

        public T Minimum
        {
            get
            {
                if (!this.HasData)
                    throw new InvalidOperationException(Resources.Range_get_Minimum_CannotReadTheMinimumOfAnEmptyRange);
                return this._minimum;
            }
        }

        private Range(bool hasData)
        {
            this._hasData = hasData;
            this._minimum = default(T);
            this._maximum = default(T);
        }

        public Range(T minimum, T maximum)
        {
            if (minimum == null)
                throw new ArgumentNullException("minimum");
            if (maximum == null)
                throw new ArgumentNullException("maximum");
            this._hasData = true;
            this._minimum = minimum;
            this._maximum = maximum;
            if (ValueHelper.Compare(minimum, maximum) <= 0)
                return;
            T obj = this._minimum;
            this._minimum = this._maximum;
            this._maximum = obj;
        }

        public static bool operator ==(Range<T> leftRange, Range<T> rightRange)
        {
            if (!leftRange.HasData)
                return !rightRange.HasData;
            if (!rightRange.HasData)
                return !leftRange.HasData;
            if (leftRange.Minimum.Equals(rightRange.Minimum))
                return leftRange.Maximum.Equals(rightRange.Maximum);
            return false;
        }

        public static bool operator !=(Range<T> leftRange, Range<T> rightRange)
        {
            return !(leftRange == rightRange);
        }

        public Range<T> Add(Range<T> range)
        {
            if (!this.HasData)
                return range;
            if (!range.HasData)
                return this;
            return new Range<T>(ValueHelper.Compare(this.Minimum, range.Minimum) == -1 ? this.Minimum : range.Minimum, ValueHelper.Compare(this.Maximum, range.Maximum) == 1 ? this.Maximum : range.Maximum);
        }

        public Range<T> Add(T value)
        {
            if (value == null)
                return this;
            if (!this.HasData)
                return new Range<T>(value, value);
            return new Range<T>(ValueHelper.Compare(this.Minimum, value) == -1 ? this.Minimum : value, ValueHelper.Compare(this.Maximum, value) == 1 ? this.Maximum : value);
        }

        public bool Equals(Range<T> range)
        {
            return this == range;
        }

        public override bool Equals(object obj)
        {
            return this == (Range<T>)obj;
        }

        public bool Contains(T value)
        {
            if (ValueHelper.Compare(this.Minimum, value) <= 0)
                return ValueHelper.Compare(value, this.Maximum) <= 0;
            return false;
        }

        public Range<T> ExtendTo(T value)
        {
            if (!this.HasData)
                return new Range<T>(value, value);
            if (ValueHelper.Compare(this.Minimum, value) > 0)
                return new Range<T>(value, this.Maximum);
            if (ValueHelper.Compare(this.Maximum, value) < 0)
                return new Range<T>(this.Minimum, value);
            return this;
        }

        public Range<T> Union(Range<T> other)
        {
            if (!other.HasData)
                return this;
            return this.ExtendTo(other.Minimum).ExtendTo(other.Maximum);
        }

        public bool IntersectsWith(Range<T> range)
        {
            if (!this.HasData || !range.HasData)
                return false;
            Func<Range<T>, Range<T>, bool> func = (leftRange, rightRange) =>
           {
               if (ValueHelper.Compare(rightRange.Minimum, leftRange.Maximum) <= 0 && ValueHelper.Compare(rightRange.Minimum, leftRange.Minimum) >= 0)
                   return true;
               if (ValueHelper.Compare(leftRange.Minimum, rightRange.Maximum) <= 0)
                   return ValueHelper.Compare(leftRange.Minimum, rightRange.Minimum) >= 0;
               return false;
           };
            if (!func(this, range))
                return func(range, this);
            return true;
        }

        public override int GetHashCode()
        {
            if (!this.HasData)
                return 0;

            unchecked
            {
                var key1 = EqualityComparer<T>.Default.GetHashCode(this.Minimum);
                var key2 = EqualityComparer<T>.Default.GetHashCode(this.Maximum);
                var num = 0x5374e861;
                num = (-1521134295 * num) + key1;
                num = (-1521134295 * num) + key2;
                return num;
            }
        }

        public override string ToString()
        {
            if (!this.HasData)
                return Resources.Range_ToString_NoData;
            return string.Format(CultureInfo.CurrentCulture, Resources.Range_ToString_Data, new object[2] { this.Minimum, this.Maximum });
        }
    }
}
