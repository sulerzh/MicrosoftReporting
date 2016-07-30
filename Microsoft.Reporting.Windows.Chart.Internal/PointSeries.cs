using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(PointDataPoint))]
    public class PointSeries : XYSeries
    {
        internal override SeriesPresenter CreateSeriesPresenter()
        {
            return new PointSeriesPresenter((XYSeries)this);
        }

        internal override DataPoint CreateDataPoint()
        {
            return new PointDataPoint();
        }
    }
}
