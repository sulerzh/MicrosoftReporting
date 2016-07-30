using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class DefaultValueAggregator : ValueAggregator
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
            return Range<IComparable>.Empty;
        }

        public override Range<IComparable> GetSumRange(IEnumerable<object> values)
        {
            return Range<IComparable>.Empty;
        }
    }
}
