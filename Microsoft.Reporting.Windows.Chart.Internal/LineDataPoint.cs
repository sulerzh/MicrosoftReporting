using System;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class LineDataPoint : PointDataPoint
    {
        public LineDataPoint()
        {
        }

        public LineDataPoint(IComparable xValue, IComparable yValue)
          : base(xValue, yValue)
        {
        }
    }
}
