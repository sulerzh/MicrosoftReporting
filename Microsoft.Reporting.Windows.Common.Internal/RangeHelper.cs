using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class RangeHelper
    {
        public static double Size(this Range<double> range)
        {
            return range.Maximum - range.Minimum;
        }

        public static long Size(this Range<long> range)
        {
            return range.Maximum - range.Minimum;
        }

        internal static Range<double> Expand(this Range<double> range, double increment)
        {
            if (range.Minimum <= range.Maximum)
                return new Range<double>(range.Minimum - increment, range.Maximum + increment);
            return new Range<double>(range.Minimum + increment, range.Minimum - increment);
        }

        public static double Project(this Range<double> fromRange, double value, Range<double> targetRange)
        {
            double d1 = fromRange.Size();
            double d2 = targetRange.Size();
            if (d1 == 0.0 || d2 == 0.0)
            {
                if (fromRange.Minimum <= value && value <= fromRange.Maximum)
                    return targetRange.Minimum;
                return double.NaN;
            }
            if (double.IsInfinity(d1) || double.IsInfinity(d2))
                return RangeHelper.ProjectOverflowingDoubles(fromRange, value, targetRange);
            double num = (value - fromRange.Minimum) / d1;
            return targetRange.Minimum + num * d2;
        }

        private static double ProjectOverflowingDoubles(Range<double> fromRange, double value, Range<double> toRange)
        {
            DoubleR10 doubleR10_1 = value;
            DoubleR10 doubleR10_2 = fromRange.Minimum;
            DoubleR10 doubleR10_3 = fromRange.Maximum - doubleR10_2;
            DoubleR10 doubleR10_4 = toRange.Minimum;
            DoubleR10 doubleR10_5 = toRange.Maximum - doubleR10_4;
            DoubleR10 doubleR10_6 = (doubleR10_1 - doubleR10_2) / doubleR10_3;
            return (double)(doubleR10_4 + doubleR10_6 * doubleR10_5);
        }

        public static long Project(this Range<double> fromRange, double value, Range<long> targetRange)
        {
            double num1 = fromRange.Size();
            long num2 = targetRange.Size();
            if (num1 == 0.0 || num2 == 0L)
                return targetRange.Minimum;
            double num3 = (value - fromRange.Minimum) / num1;
            return targetRange.Minimum + (long)(num3 * num2);
        }

        public static double Project(this Range<long> fromRange, long value, Range<double> targetRange)
        {
            long num1 = fromRange.Size();
            double num2 = targetRange.Size();
            if (num1 == 0L || num2 == 0.0)
                return targetRange.Minimum;
            DoubleR10 doubleR10 = (value - fromRange.Minimum) / (DoubleR10)num1;
            return targetRange.Minimum + (double)doubleR10 * num2;
        }

        internal static Range<double> Update(this Range<double> range, double? min, double? max)
        {
            if (min.HasValue && max.HasValue)
                return new Range<double>(min.Value, max.Value);
            if (min.HasValue)
                return new Range<double>(min.Value, range.Maximum);
            if (max.HasValue)
                return new Range<double>(range.Minimum, max.Value);
            return range;
        }

        internal static Range<double> FromDoubles(params double[] values)
        {
            if (values == null || values.Length == 0)
                return new Range<double>(double.NaN, double.NaN);
            double minimum = double.MaxValue;
            double maximum = double.MinValue;
            for (int index = 0; index < values.Length; ++index)
            {
                if (values[index] < minimum)
                    minimum = values[index];
                if (values[index] > maximum)
                    maximum = values[index];
            }
            return new Range<double>(minimum, maximum);
        }

        internal static void Normalize(ref double min, ref double max)
        {
            if (min <= max)
                return;
            double num = min;
            min = max;
            max = num;
        }

        internal static void Normalize(ref long min, ref long max)
        {
            if (min <= max)
                return;
            long num = min;
            min = max;
            max = num;
        }

        public static void BoxRangeInsideAnother(ref double min, ref double max, double outerMin, double outerMax)
        {
            RangeHelper.Normalize(ref min, ref max);
            RangeHelper.Normalize(ref outerMin, ref outerMax);
            double num1 = max - min;
            double num2 = outerMax - outerMin;
            if (num1 > num2)
            {
                min = outerMin;
                max = outerMax;
            }
            else if (min < outerMin)
            {
                min = outerMin;
                max = outerMin + num1;
            }
            else
            {
                if (max <= outerMax)
                    return;
                max = outerMax;
                min = outerMax - num1;
            }
        }

        public static void BoxRangeInsideAnother(ref long min, ref long max, long outerMin, long outerMax)
        {
            RangeHelper.Normalize(ref min, ref max);
            RangeHelper.Normalize(ref outerMin, ref outerMax);
            long num1 = max - min;
            long num2 = outerMax - outerMin;
            if (num1 > num2)
            {
                min = outerMin;
                max = outerMax;
            }
            else if (min < outerMin)
            {
                min = outerMin;
                max = outerMin + num1;
            }
            else
            {
                if (max <= outerMax)
                    return;
                max = outerMax;
                min = outerMax - num1;
            }
        }

        public static void BoxRangeInsideAnother(ref double min, ref double max, double outerMin, double outerMax, double minSize, double maxSize, double precision = 0.0)
        {
            RangeHelper.Normalize(ref min, ref max);
            RangeHelper.Normalize(ref outerMin, ref outerMax);
            double num1 = max - min;
            if (num1 < minSize)
                max = min + minSize;
            else if (num1 > maxSize)
                max = min + maxSize;
            double num2 = outerMax - outerMin;
            if (num1 > num2)
            {
                min = outerMin;
                max = outerMax;
            }
            else if (min < outerMin)
            {
                min = outerMin;
                max = outerMin + num1;
            }
            else if (max > outerMax)
            {
                max = outerMax;
                min = outerMax - num1;
            }
            if (precision <= 0.0)
                return;
            double num3 = num1 * precision;
            if (min - outerMin < num3)
                min = outerMin;
            if (outerMax - max >= num3)
                return;
            max = outerMax;
        }

        public static Range<T>? Add<T>(Range<T>? range, T? value) where T : struct, IComparable
        {
            if (!value.HasValue)
                return range;
            if (!range.HasValue)
                return new Range<T>?(new Range<T>(value.Value, value.Value));
            return new Range<T>?(range.Value.Add(value.Value));
        }
    }
}
