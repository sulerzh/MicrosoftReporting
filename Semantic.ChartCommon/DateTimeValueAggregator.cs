using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class DateTimeValueAggregator : ValueAggregator
    {
        public override bool CanPlot(object value)
        {
            DateTime dateTimeValue;
            return ValueHelper.TryConvert(value, true, out dateTimeValue);
        }

        public override IComparable GetValue(object value)
        {
            DateTime dateTimeValue;
            if (ValueHelper.TryConvert(value, true, out dateTimeValue))
                return dateTimeValue;
            return null;
        }

        public override Range<IComparable> GetRange(IEnumerable<object> values)
        {
            DateTime dateTime1 = DateTime.MaxValue;
            DateTime dateTime2 = DateTime.MinValue;
            foreach (object obj in values)
            {
                DateTime dateTimeValue;
                if (ValueHelper.TryConvert(obj, true, out dateTimeValue))
                {
                    if (dateTimeValue < dateTime1)
                        dateTime1 = dateTimeValue;
                    if (dateTimeValue > dateTime2)
                        dateTime2 = dateTimeValue;
                }
            }
            if (dateTime1 != DateTime.MaxValue)
                return new Range<IComparable>(dateTime1, dateTime2);
            return new Range<IComparable>();
        }

        public override Range<IComparable> GetSumRange(IEnumerable<object> values)
        {
            throw new NotSupportedException();
        }
    }
}
