using Microsoft.Reporting.Windows.Common.Internal;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class PieSeriesLabelPresenter : PointSeriesLabelPresenter
    {
        public PieSeriesLabelPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        internal override ContentPositions GetAutomaticLabelPosition(DataPoint dataPoint)
        {
            return ContentPositions.None;
        }

        internal override void AdjustDataPointLabelVisibilityRating(LabelVisibilityManager.DataPointRange range, Dictionary<XYDataPoint, double> dataPointRanks)
        {
        }

        internal override void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            base.BindViewToDataPoint(dataPoint, view, valueName);
            LabelControl labelControl = view as LabelControl;
            if (labelControl == null)
                return;
            labelControl.Content = dataPoint.LabelContent;
        }
    }
}
