using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class BubbleSeriesPresenter : PointSeriesPresenter
    {
        private const double AreaOf300By300Chart = 90000.0;
        private const double ZIndexMultiplier = 50.0;
        private Panel _rootPanel;

        internal override Panel RootPanel
        {
            get
            {
                if (this._rootPanel == null)
                {
                    this._rootPanel = base.RootPanel;
                    this._rootPanel.SizeChanged += (s, e) =>
                   {
                       if (!this.BubbleSeries.IsSizeValueUsed || this.BubbleSeries.BubbleMarkerSizeRangeUnitType != BubbleSizeRangeUnitType.Relative)
                           return;
                       this.ChartArea.UpdateSession.BeginUpdates();
                       this.ChartArea.UpdateSession.ExecuteOnceBeforeUpdating((Action)(() => ((BubbleSeries)this.Series).UpdateAllDataPointsSize()), (object)new Tuple<string, Series>("__UpdateBubbleSizeInPixels__", (Series)this.Series));
                       this.ChartArea.UpdateSession.EndUpdates();
                   };
                }
                return this._rootPanel;
            }
        }

        private BubbleSeries BubbleSeries
        {
            get
            {
                return this.Series as BubbleSeries;
            }
        }

        private Range<double> BubblePixelAreaSizeRange
        {
            get
            {
                if (!this.BubbleSeries.IsSizeValueUsed || this.BubbleSeries.BubbleMarkerSizeRangeUnitType != BubbleSizeRangeUnitType.Relative)
                    return this.BubbleSeries.BubbleMarkerSizeRange;
                double num = 1.0;
                if (this.ChartArea != null && this.ChartArea.PlotAreaPanel != null && (this.ChartArea.PlotAreaPanel.ActualHeight > 0.0 && this.ChartArea.PlotAreaPanel.ActualWidth > 0.0))
                    num = Math.Pow(Math.Min(this.ChartArea.PlotAreaPanel.ActualHeight, this.ChartArea.PlotAreaPanel.ActualWidth), 2.0) / 90000.0;
                return new Range<double>(this.BubbleSeries.BubbleMarkerSizeRange.Minimum * num, this.BubbleSeries.BubbleMarkerSizeRange.Maximum * num);
            }
        }

        public BubbleSeriesPresenter(XYSeries series)
          : base(series)
        {
            this.IsRootPanelClipped = true;
        }

        internal override SeriesMarkerPresenter CreateMarkerPresenter()
        {
            return new BubbleSeriesMarkerPresenter((SeriesPresenter)this);
        }

        internal override SeriesLabelPresenter CreateLabelPresenter()
        {
            return new BubbleSeriesLabelPresenter((SeriesPresenter)this);
        }

        internal double ProjectSizeToPixels(DataPoint dataPoint, double value)
        {
            BubbleDataPoint bubbleDataPoint = dataPoint as BubbleDataPoint;
            if (!this.BubbleSeries.IsSizeValueUsed || bubbleDataPoint == null || !bubbleDataPoint.IsSizeValueUsed)
                return dataPoint.MarkerSize;
            if (!this.BubbleSeries.ActualSizeDataRange.HasData)
                return 0.0;
            Range<double> range = new Range<double>(ValueHelper.ToDouble(this.BubbleSeries.ActualSizeDataRange.Minimum), ValueHelper.ToDouble(this.BubbleSeries.ActualSizeDataRange.Maximum));
            double num = this.BubblePixelAreaSizeRange.Maximum;
            if (range.Size() != 0.0)
            {
                value = Math.Min(Math.Max(value, range.Minimum), range.Maximum);
                num = range.Project(value, this.BubblePixelAreaSizeRange);
            }
            if (this.BubbleSeries.BubbleMarkerSizeRangeUnitType == BubbleSizeRangeUnitType.Relative)
                num = 2.0 * Math.Sqrt(num / Math.PI);
            return num;
        }

        internal override bool CanGraph(XYDataPoint dataPointXY)
        {
            BubbleDataPoint bubbleDataPoint = dataPointXY as BubbleDataPoint;
            if (bubbleDataPoint != null && this.BubbleSeries.IsSizeValueUsed && bubbleDataPoint.IsSizeValueUsed)
            {
                double doubleValue = 0.0;
                if (!ValueHelper.TryConvert(bubbleDataPoint.SizeValue, true, out doubleValue) || !ValueHelper.CanGraph(doubleValue) || bubbleDataPoint.SizeValueInScaleUnits.LessOrEqualWithPrecision(0.0))
                    return false;
            }
            return base.CanGraph(dataPointXY);
        }

        protected override void OnSeriesDataPointValueChanged(DataPoint dataPoint, string valueName, object oldValue, object newValue)
        {
            BubbleDataPoint bubbleDataPoint = dataPoint as BubbleDataPoint;
            switch (valueName)
            {
                case "MarkerSize":
                    if (this.BubbleSeries.IsSizeValueUsed && bubbleDataPoint.IsSizeValueUsed)
                        break;
                    this.ChangeDataPointSizeValue((BubbleDataPoint)dataPoint, newValue);
                    break;
                case "SizeValue":
                    if (!this.BubbleSeries.IsSizeValueUsed || !bubbleDataPoint.IsSizeValueUsed)
                        break;
                    ((BubbleSeries)this.Series).UpdateSizeDataRange();
                    this.ChangeDataPointSizeValue((BubbleDataPoint)dataPoint, newValue);
                    break;
                case "SizeValueInScaleUnitsWithoutAnimation":
                    if (this.ChartArea == null || !this.ChartArea.IsTemplateApplied)
                        break;
                    this.ChartArea.UpdateSession.ExecuteOnceAfterUpdating(() => this.LabelVisibilityManager.UpdateDataPointLabelVisibility(), "LabelVisibilityManager_UpdateDataPointLabelVisibility", null);
                    break;
                case "SizeValueInScaleUnits":
                    if (this.ChartArea == null || !this.ChartArea.IsTemplateApplied)
                        break;
                    Tuple<Series, string> tuple = new Tuple<Series, string>(this.Series, "__UpdateDataPointVisibility__");
                    if (this.ChartArea.UpdateSession.IsUpdating)
                    {
                        this.ChartArea.UpdateSession.ExecuteOnceBeforeUpdating(() => this.UpdateDataPointVisibility(), tuple);
                        break;
                    }
                    this.ChartArea.UpdateSession.PostExecuteOnceOnUIThread(() => this.UpdateDataPointVisibility(), tuple);
                    break;
                case "IsSizeValueUsed":
                    this.OnSeriesModelPropertyChanged(valueName);
                    break;
                default:
                    base.OnSeriesDataPointValueChanged(dataPoint, valueName, oldValue, newValue);
                    break;
            }
        }

        protected override void OnSeriesModelPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "IsSizeValueUsed":
                case "BubbleMarkerSizeRange":
                case "BubbleMarkerSizeRangeUnitType":
                    Tuple<Series, string> tuple = new Tuple<Series, string>(this.Series, "__UpdateDataPointSizeValue__");
                    if (this.ChartArea.UpdateSession.IsUpdating)
                    {
                        this.ChartArea.UpdateSession.ExecuteOnceBeforeUpdating(() => this.UpdateAllSeriesPointSizes(), tuple);
                        break;
                    }
                    this.ChartArea.UpdateSession.PostExecuteOnceOnUIThread(() => this.UpdateAllSeriesPointSizes(), tuple);
                    break;
                default:
                    base.OnSeriesModelPropertyChanged(propertyName);
                    break;
            }
        }

        private void UpdateAllSeriesPointSizes()
        {
            foreach (BubbleDataPoint dataPoint in this.Series.DataPoints)
                this.ChangeDataPointSizeValue(dataPoint, dataPoint.SizeValue);
        }

        internal override void OnDataPointAdded(DataPoint dataPoint, bool useShowingAnimation)
        {
            BubbleDataPoint bubbleDataPoint = dataPoint as BubbleDataPoint;
            if (bubbleDataPoint != null)
            {
                bubbleDataPoint.SizeValueInScaleUnits = this.ProjectSizeToPixels(dataPoint, ValueHelper.ToDouble(bubbleDataPoint.SizeValue));
                bubbleDataPoint.SizeValueInScaleUnitsWithoutAnimation = bubbleDataPoint.SizeValueInScaleUnits;
            }
            base.OnDataPointAdded(dataPoint, useShowingAnimation);
        }

        private void ChangeDataPointSizeValue(BubbleDataPoint dataPoint, object newValue)
        {
            if (dataPoint == null)
                return;
            double valueInScaleUnits = dataPoint.SizeValueInScaleUnits;
            double pixels = this.ProjectSizeToPixels(dataPoint, ValueHelper.ToDouble(newValue));
            dataPoint.SizeValueInScaleUnitsWithoutAnimation = pixels;
            if (valueInScaleUnits == pixels)
                return;
            if (ValueHelper.CanGraph(valueInScaleUnits) && ValueHelper.CanGraph(pixels) && (this.IsSeriesAnimationEnabled && this.ChartArea != null))
            {
                DependencyPropertyAnimationHelper.BeginAnimation(this.ChartArea, "SizeValueInScaleUnits", valueInScaleUnits, pixels, (value1, value2) => dataPoint.SizeValueInScaleUnits = (double)value2, dataPoint.Storyboards, this.Series.ActualTransitionDuration, this.Series.ActualTransitionEasingFunction);
            }
            else
            {
                this.SkipToFillValueAnimation(dataPoint, "SizeValueInScaleUnits");
                dataPoint.SizeValueInScaleUnits = pixels;
            }
        }

        internal override void UpdateDataPointZIndex(DataPoint dataPoint)
        {
            FrameworkElement dataPointView = SeriesVisualStatePresenter.GetDataPointView(dataPoint);
            if (dataPointView == null)
                return;
            int val1 = dataPoint.ZIndex;
            if (val1 == 0)
                val1 = 16383 - (int)(dataPointView.Width * 50.0);
            if (dataPoint.IsSelected)
                val1 += 16383;
            int num = Math.Max(Math.Min(val1, 32766), -32766);
            Panel.SetZIndex(dataPointView, num);
        }
    }
}
