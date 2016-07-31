using System;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class PositiveTimeValueAggregator : TimeValueAggregator
    {
        public override bool CanPlot(TimeSpan timespan)
        {
            return timespan >= TimeSpan.Zero;
        }
    }
}
