using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class AbsTimeValueAggregator : TimeValueAggregator
    {
        protected override bool TryConvert(object value, out TimeSpan x)
        {
            if (!base.TryConvert(value, out x))
                return false;
            if (x < TimeSpan.Zero)
                x = -x;
            return true;
        }
    }
}
