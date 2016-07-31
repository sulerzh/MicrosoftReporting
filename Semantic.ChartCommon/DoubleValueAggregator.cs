using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class DoubleValueAggregator : ValueAggregator
    {
        public override bool CanPlot(object value)
        {
            double x;
            return this.TryConvert(value, out x);
        }

        protected virtual bool CanPlot(double x)
        {
            if (!x.IsNaN())
                return !double.IsInfinity(x);
            return false;
        }

        protected virtual bool TryConvert(object value, out double x)
        {
            if (ValueHelper.TryConvert(value, true, out x))
                return this.CanPlot(x);
            return false;
        }

        public override IComparable GetValue(object value)
        {
            double x;
            if (this.TryConvert(value, out x))
                return x;
            return null;
        }

        public override Range<IComparable> GetRange(IEnumerable<object> values)
        {
            double num1 = double.MaxValue;
            double num2 = double.MinValue;
            foreach (object obj in values)
            {
                double x;
                if (this.TryConvert(obj, out x))
                {
                    if (x < num1)
                        num1 = x;
                    if (x > num2)
                        num2 = x;
                }
            }
            if (num1 != double.MaxValue)
                return new Range<IComparable>(num1, num2);
            return new Range<IComparable>();
        }

        public override Range<IComparable> GetSumRange(IEnumerable<object> values)
        {
            double num = 0.0;
            foreach (object obj in values)
            {
                double x;
                if (this.TryConvert(obj, out x))
                    num += x;
            }
            return new Range<IComparable>(0.0, num);
        }
    }
}
