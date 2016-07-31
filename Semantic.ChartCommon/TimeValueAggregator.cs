using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class TimeValueAggregator : ValueAggregator
    {
        public override bool CanPlot(object value)
        {
            TimeSpan x;
            return this.TryConvert(value, out x);
        }

        public virtual bool CanPlot(TimeSpan timespan)
        {
            return true;
        }

        protected virtual bool TryConvert(object value, out TimeSpan x)
        {
            x = new TimeSpan();
            if (!(value is TimeSpan))
                return false;
            x = (TimeSpan)value;
            return this.CanPlot(x);
        }

        public override IComparable GetValue(object value)
        {
            TimeSpan x;
            if (this.TryConvert(value, out x))
                return x;
            return null;
        }

        public override Range<IComparable> GetRange(IEnumerable<object> values)
        {
            TimeSpan timeSpan1 = TimeSpan.MaxValue;
            TimeSpan timeSpan2 = TimeSpan.MinValue;
            foreach (object obj in values)
            {
                TimeSpan x;
                if (this.TryConvert(obj, out x))
                {
                    if (x < timeSpan1)
                        timeSpan1 = x;
                    if (x > timeSpan2)
                        timeSpan2 = x;
                }
            }
            if (timeSpan1 != TimeSpan.MaxValue)
                return new Range<IComparable>(timeSpan1, timeSpan2);
            return new Range<IComparable>();
        }

        public override Range<IComparable> GetSumRange(IEnumerable<object> values)
        {
            long ticks1 = 0;
            foreach (object obj in values)
            {
                TimeSpan x;
                if (this.TryConvert(obj, out x))
                {
                    long ticks2 = x.Ticks;
                    ticks1 += ticks2;
                }
            }
            return new Range<IComparable>(new TimeSpan(0L), new TimeSpan(ticks1));
        }
    }
}
