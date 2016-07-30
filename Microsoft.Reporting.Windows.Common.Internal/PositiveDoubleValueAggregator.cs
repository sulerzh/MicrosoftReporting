namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class PositiveDoubleValueAggregator : DoubleValueAggregator
    {
        protected override bool CanPlot(double x)
        {
            if (base.CanPlot(x))
                return x >= 0.0;
            return false;
        }
    }
}
