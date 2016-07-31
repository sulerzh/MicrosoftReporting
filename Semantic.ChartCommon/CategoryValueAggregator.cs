using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class CategoryValueAggregator : ValueAggregator
    {
        public override bool CanPlot(object value)
        {
            return value != null;
        }

        public override IComparable GetValue(object value)
        {
            return (IComparable)value;
        }

        public override Range<IComparable> GetRange(IEnumerable<object> values)
        {
            int num = values.FastCount();
            if (num > 0)
                return new Range<IComparable>(0, num - 1);
            return new Range<IComparable>();
        }

        public override Range<IComparable> GetSumRange(IEnumerable<object> values)
        {
            return new Range<IComparable>(0, values.FastCount());
        }
    }
}
