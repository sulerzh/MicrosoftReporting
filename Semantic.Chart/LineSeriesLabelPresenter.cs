using Microsoft.Reporting.Windows.Common.Internal;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class LineSeriesLabelPresenter : PointSeriesLabelPresenter
    {
        internal override bool IsDataPointVisibilityUsesXAxisOnly
        {
            get
            {
                return true;
            }
        }

        public LineSeriesLabelPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        internal override ContentPositions GetAutomaticLabelPosition(DataPoint dataPoint)
        {
            ContentPositions contentPositions1 = ContentPositions.TopCenter;
            ContentPositions contentPositions2 = ContentPositions.BottomCenter;
            if (this.SeriesPresenter.ChartArea != null)
            {
                if (this.SeriesPresenter.ChartArea.Orientation == Orientation.Vertical)
                {
                    contentPositions1 = ContentPositions.MiddleRight;
                    contentPositions2 = ContentPositions.MiddleLeft;
                }
                XYDataPoint xyDataPoint1 = dataPoint as XYDataPoint;
                int num = this.SeriesPresenter.Series.DataPoints.IndexOf(dataPoint);
                XYDataPoint xyDataPoint2 = num > 0 ? this.SeriesPresenter.Series.DataPoints[num - 1] as XYDataPoint : null;
                XYDataPoint xyDataPoint3 = num < this.SeriesPresenter.Series.DataPoints.Count - 1 ? this.SeriesPresenter.Series.DataPoints[num + 1] as XYDataPoint : null;
                if (xyDataPoint2 != null && xyDataPoint3 != null && (xyDataPoint1.YValueInScaleUnitsWithoutAnimation < xyDataPoint2.YValueInScaleUnitsWithoutAnimation && xyDataPoint1.YValueInScaleUnitsWithoutAnimation < xyDataPoint3.YValueInScaleUnitsWithoutAnimation) || xyDataPoint2 != null && xyDataPoint3 == null && xyDataPoint1.YValueInScaleUnitsWithoutAnimation < xyDataPoint2.YValueInScaleUnitsWithoutAnimation || xyDataPoint2 == null && xyDataPoint3 != null && xyDataPoint1.YValueInScaleUnitsWithoutAnimation < xyDataPoint3.YValueInScaleUnitsWithoutAnimation)
                    return contentPositions2;
            }
            return contentPositions1;
        }

        internal override void AdjustDataPointLabelVisibilityRating(LabelVisibilityManager.DataPointRange range, Dictionary<XYDataPoint, double> dataPointRanks)
        {
        }
    }
}
