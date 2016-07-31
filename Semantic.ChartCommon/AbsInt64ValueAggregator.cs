using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class AbsInt64ValueAggregator : Int64ValueAggregator
    {
        protected override bool TryConvert(object value, out long x)
        {
            if (!base.TryConvert(value, out x))
                return false;
            x = Math.Abs(x);
            return true;
        }
    }
}
