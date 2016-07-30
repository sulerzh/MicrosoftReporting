using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class MathHelper
    {
        internal static double Distance(double x1, double y1, double x2, double y2)
        {
            double num1 = x2 - x1;
            double num2 = y2 - y1;
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }

        public static double GetMinimumDelta(IList<double> sortedValues)
        {
            double num1 = 0.0;
            if (sortedValues != null && sortedValues.Count > 1)
            {
                double num2 = 0.0;
                double precision = DoubleHelper.GetPrecision(new double[2] { sortedValues[0], sortedValues[sortedValues.Count - 1] });
                for (int index = 0; index < sortedValues.Count; ++index)
                {
                    double num3 = sortedValues[index];
                    if (index > 0)
                    {
                        double num4 = Math.Abs(num3 - num2);
                        if (num4.GreaterWithPrecision(0.0, precision) && (num4 < num1 || num1 == 0.0))
                            num1 = num4;
                    }
                    num2 = num3;
                }
            }
            return num1;
        }

        internal static double? Log(double? value, double logBase)
        {
            if (value.HasValue)
            {
                double a = value.Value;
                if (a > 0.0)
                    return new double?(Math.Log(a, logBase));
            }
            return new double?();
        }

        internal static Range<double> Log(Range<double> range, double logBase)
        {
            return new Range<double>(Math.Log(range.Minimum, logBase), Math.Log(range.Maximum, logBase));
        }

        internal static Range<double>? Log(Range<double>? range, double logBase)
        {
            if (range.HasValue)
            {
                Range<double> range1 = range.Value;
                if (range1.Minimum > 0.0 || range1.Maximum > 0.0)
                    return new Range<double>?(new Range<double>(range1.Minimum > 0.0 ? Math.Log(range1.Minimum, logBase) : 0.0, range1.Maximum > 0.0 ? Math.Log(range1.Maximum, logBase) : 10.0));
            }
            return new Range<double>?();
        }

        internal static double? Pow(double? value, double logBase)
        {
            if (!value.HasValue)
                return new double?();
            double y = value.Value;
            return new double?(Math.Pow(logBase, y));
        }

        internal static Range<double>? Pow(Range<double>? range, double logBase)
        {
            if (!range.HasValue)
                return new Range<double>?();
            Range<double> range1 = range.Value;
            return new Range<double>?(new Range<double>(Math.Pow(logBase, range1.Minimum), Math.Log(logBase, range1.Maximum)));
        }

        internal static IEnumerable<DoubleR10> Pow(IEnumerable<DoubleR10> values, double logBase)
        {
            foreach (DoubleR10 doubleR10 in values)
            {
                DoubleR10 powerX = Math.Pow(logBase, (double)doubleR10);
                yield return powerX;
            }
        }
    }
}
