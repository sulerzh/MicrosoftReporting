using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class StackedColumnSeriesLabelPresenter : SeriesLabelPresenter
    {
        internal override bool IsDataPointVisibilityUsesXAxisOnly
        {
            get
            {
                return true;
            }
        }

        public StackedColumnSeriesLabelPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        internal override void OnUpdateView(DataPoint dataPoint)
        {
            base.OnUpdateView(dataPoint);
            if (dataPoint.View == null || dataPoint.View.LabelView == null)
                return;
            StackedColumnSeriesPresenter columnSeriesPresenter = this.SeriesPresenter as StackedColumnSeriesPresenter;
            ContentPositions validContentPositions = ContentPositions.InsideCenter;
            if (columnSeriesPresenter.IsStackTopSeries() && !columnSeriesPresenter.IsHundredPercent())
                validContentPositions |= ContentPositions.OutsideEnd;
            AnchorPanel.SetValidContentPositions(dataPoint.View.LabelView, validContentPositions);
            AnchorPanel.SetContentPosition(dataPoint.View.LabelView, ContentPositions.InsideCenter);
            AnchorPanel.SetAnchorMargin(dataPoint.View.LabelView, 0.0);
            ColumnSeriesLabelPresenter.SetLabelMaxMovingDistance((XYChartArea)this.SeriesPresenter.Series.ChartArea, dataPoint.View.LabelView);
        }

        internal override void AdjustDataPointLabelVisibilityRating(LabelVisibilityManager.DataPointRange range, Dictionary<XYDataPoint, double> dataPointRanks)
        {
            if (range.DataPoints.Count <= 0)
                return;
            XYDataPoint xyDataPoint = range.DataPoints[0] as XYDataPoint;
            foreach (XYDataPoint dataPoint in range.DataPoints)
            {
                if (ValueHelper.Compare(dataPoint.XValue as IComparable, xyDataPoint.XValue as IComparable) == 0)
                {
                    if (dataPointRanks.ContainsKey(dataPoint))
                        dataPointRanks[dataPoint] = 100.0;
                    else
                        dataPointRanks.Add(dataPoint, 100.0);
                }
            }
        }

        internal override void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            base.BindViewToDataPoint(dataPoint, view, valueName);
            StackedColumnSeries stackedColumnSeries = dataPoint.Series as StackedColumnSeries;
            StackedColumnDataPoint stackedColumnDataPoint = dataPoint as StackedColumnDataPoint;
            LabelControl labelControl = view as LabelControl;
            if (labelControl == null || stackedColumnDataPoint == null || (stackedColumnSeries == null || !stackedColumnSeries.ActualIsHundredPercent) || !(valueName == "ActualLabelContent") && valueName != null)
                return;
            double yvaluePercent = stackedColumnDataPoint.YValuePercent;
            if (Math.Abs(yvaluePercent) < 0.005)
                labelControl.Content = null;
            else
                labelControl.Content = yvaluePercent.ToString("P0", (IFormatProvider)CultureInfo.CurrentCulture);
        }
    }
}
