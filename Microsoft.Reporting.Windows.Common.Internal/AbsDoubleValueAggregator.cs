using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class AbsDoubleValueAggregator : DoubleValueAggregator
    {
        protected override bool TryConvert(object value, out double x)
        {
            if (!base.TryConvert(value, out x))
                return false;
            x = Math.Abs(x);
            return true;
        }
    }
}
