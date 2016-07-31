namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class NegativeInt64ValueAggregator : Int64ValueAggregator
    {
        protected override bool CanPlot(long x)
        {
            if (base.CanPlot(x))
                return x <= 0L;
            return false;
        }
    }
}
