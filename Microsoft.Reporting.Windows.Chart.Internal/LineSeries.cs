using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(LineDataPoint))]
    public class LineSeries : XYSeries
    {
        internal override SeriesPresenter CreateSeriesPresenter()
        {
            return new LineSeriesPresenter((XYSeries)this);
        }

        internal override DataPoint CreateDataPoint()
        {
            return new LineDataPoint();
        }
    }
}
