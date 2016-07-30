using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class LabelVisibilityManager
    {
        private List<LabelVisibilityManager.DataPointRange> _dataPointRanges = new List<LabelVisibilityManager.DataPointRange>();
        internal const double VisibilityRatingNone = 0.0;
        internal const double VisibilityRatingLow = 50.0;
        internal const double VisibilityRatingMedium = 100.0;
        internal const double VisibilityRatingHigh = 150.0;
        internal const int MinimumDataPointCountThreshold = 30;
        private XYChartArea _chartArea;
        private IEnumerable<double> _xScalePositions;
        private IEnumerable<double> _yScalePositions;

        private bool IsOnlyXAxisUsed
        {
            get
            {
                foreach (Series series in this._chartArea.Series)
                {
                    if (!series.SeriesPresenter.LabelPresenter.IsDataPointVisibilityUsesXAxisOnly)
                        return false;
                }
                return true;
            }
        }

        internal virtual double YScaleLabelDensity
        {
            get
            {
                double val1 = 0.0;
                foreach (Series series in this._chartArea.Series)
                {
                    double yscaleLabelDensity = series.SeriesPresenter.LabelPresenter.YScaleLabelDensity;
                    val1 = Math.Max(val1, yscaleLabelDensity);
                }
                if (val1 <= 0.0)
                    return double.NaN;
                return val1;
            }
        }

        internal virtual double XScaleLabelDensity
        {
            get
            {
                double val1 = 0.0;
                foreach (Series series in this._chartArea.Series)
                {
                    double xscaleLabelDensity = series.SeriesPresenter.LabelPresenter.XScaleLabelDensity;
                    val1 = Math.Max(val1, xscaleLabelDensity);
                }
                if (val1 <= 0.0)
                    return double.NaN;
                return val1;
            }
        }

        public LabelVisibilityManager(XYChartArea chartArea)
        {
            this._chartArea = chartArea;
        }

        public void InvalidateXIntervals()
        {
            this._xScalePositions = null;
        }

        public void InvalidateYIntervals()
        {
            this._yScalePositions = null;
        }

        private void RecalculateXIntervals()
        {
            if (this._chartArea == null || this._chartArea.Series.Count <= 0)
                return;
            if (double.IsNaN(this.XScaleLabelDensity))
            {
                XYSeries xySeries = this._chartArea.Series.FirstOrDefault<XYSeries>();
                if (xySeries != null)
                {
                    Scale scale = xySeries.XAxis.Scale;
                    if (scale != null)
                        this._xScalePositions = LabelVisibilityManager.GetScalePositions(scale);
                }
            }
            else
                this._xScalePositions = LabelVisibilityManager.IterateDoubles(0.0, 1.0, 1.0 / this.XScaleLabelDensity);
            this._dataPointRanges.Clear();
        }

        private void RecalculateYIntervals()
        {
            if (this._chartArea == null || this._chartArea.Series.Count <= 0 || this.IsOnlyXAxisUsed)
                return;
            if (double.IsNaN(this.YScaleLabelDensity))
            {
                XYSeries xySeries = this._chartArea.Series.FirstOrDefault<XYSeries>();
                if (xySeries != null)
                {
                    Scale scale = xySeries.YAxis.Scale;
                    if (scale != null)
                        this._yScalePositions = LabelVisibilityManager.GetScalePositions(scale);
                }
            }
            else
                this._yScalePositions = LabelVisibilityManager.IterateDoubles(0.0, 1.0, 1.0 / this.YScaleLabelDensity);
            this._dataPointRanges.Clear();
        }

        private static IEnumerable<double> IterateDoubles(double value, double toValue, double step)
        {
            yield return value;
            while (value < toValue)
            {
                value += step;
                yield return value;
            }
        }

        private static IEnumerable<double> GetScalePositions(Scale scale)
        {
            IEnumerable<ScalePosition> scalePositions = scale.ProjectMajorIntervals();
            int maxCount = scale.MaxCount + 1;
            if (scale.ActualMajorCount > maxCount)
                scalePositions = scalePositions.ToList<ScalePosition>().Sample<ScalePosition>(maxCount);
            foreach (ScalePosition scalePosition in scalePositions)
                yield return scalePosition.Position;
        }

        private void CreateDataPointRanges()
        {
            this._dataPointRanges.Clear();
            if (this._xScalePositions == null)
                return;
            double minimum1 = 0.0;
            foreach (double xScalePosition in this._xScalePositions)
            {
                if (xScalePosition > minimum1)
                {
                    Range<double> range = new Range<double>(minimum1, xScalePosition);
                    if (this._yScalePositions != null && !this.IsOnlyXAxisUsed)
                    {
                        double minimum2 = 0.0;
                        foreach (double yScalePosition in this._yScalePositions)
                        {
                            if (yScalePosition > minimum2)
                                this._dataPointRanges.Add(new LabelVisibilityManager.DataPointRange()
                                {
                                    ProjectedXRange = range,
                                    ProjectedYRange = new Range<double>(minimum2, yScalePosition)
                                });
                            minimum2 = yScalePosition;
                        }
                    }
                    else
                        this._dataPointRanges.Add(new LabelVisibilityManager.DataPointRange()
                        {
                            ProjectedXRange = range
                        });
                }
                minimum1 = xScalePosition;
            }
        }

        public virtual void UpdateDataPointLabelVisibility()
        {
            DateTime now = DateTime.Now;
            if (this._xScalePositions == null)
                this.RecalculateXIntervals();
            if (this._yScalePositions == null)
                this.RecalculateYIntervals();
            if (this._chartArea == null || this._chartArea.Series.Count <= 0)
                return;
            if (this._dataPointRanges.Count == 0)
                this.CreateDataPointRanges();
            List<XYDataPoint> xyDataPointList = new List<XYDataPoint>();
            Dictionary<Type, SeriesPresenter> dictionary = new Dictionary<Type, SeriesPresenter>();
            foreach (Series series in this._chartArea.Series)
            {
                if (!dictionary.ContainsKey(series.SeriesPresenter.GetType()))
                    dictionary.Add(series.SeriesPresenter.GetType(), series.SeriesPresenter);
                if (series.Visibility == Visibility.Visible && series.LabelVisibility == Visibility.Visible)
                {
                    foreach (XYDataPoint xyDataPoint in ((XYSeriesPresenter)series.SeriesPresenter).Series.DataPointsByXValue)
                    {
                        if (xyDataPoint.ActualLabelContent != null && xyDataPoint.XValueInScaleUnitsWithoutAnimation >= 0.0 && (xyDataPoint.XValueInScaleUnitsWithoutAnimation <= 1.0 && xyDataPoint.Visibility == Visibility.Visible))
                        {
                            if (this.IsOnlyXAxisUsed)
                                xyDataPointList.Add(xyDataPoint);
                            else if (xyDataPoint.YValueInScaleUnitsWithoutAnimation >= 0.0 && xyDataPoint.YValueInScaleUnitsWithoutAnimation <= 1.0)
                                xyDataPointList.Add(xyDataPoint);
                        }
                    }
                }
            }
            if (xyDataPointList.Count < 30)
            {
                foreach (DataPoint dataPoint in xyDataPointList)
                    dataPoint.ActualLabelVisibility = Visibility.Visible;
            }
            else
            {
                foreach (LabelVisibilityManager.DataPointRange dataPointRange in this._dataPointRanges)
                {
                    dataPointRange.DataPoints.Clear();
                    dataPointRange.DataPointProjectedYRange = Range<double>.Empty;
                    for (int index = 0; index < xyDataPointList.Count; ++index)
                    {
                        XYDataPoint xyDataPoint = xyDataPointList[index];
                        if (xyDataPoint.XValueInScaleUnitsWithoutAnimation >= dataPointRange.ProjectedXRange.Minimum && xyDataPoint.XValueInScaleUnitsWithoutAnimation < dataPointRange.ProjectedXRange.Maximum && (!dataPointRange.ProjectedYRange.HasData || xyDataPoint.YValueInScaleUnitsWithoutAnimation >= dataPointRange.ProjectedYRange.Minimum && xyDataPoint.YValueInScaleUnitsWithoutAnimation < dataPointRange.ProjectedYRange.Maximum))
                        {
                            dataPointRange.DataPoints.Add(xyDataPoint);
                            dataPointRange.DataPointProjectedYRange = dataPointRange.DataPointProjectedYRange.Add(xyDataPoint.YValueInScaleUnitsWithoutAnimation);
                            xyDataPointList.RemoveAt(index);
                            --index;
                        }
                    }
                }
                foreach (DataPoint dataPoint in xyDataPointList)
                    dataPoint.ActualLabelVisibility = Visibility.Collapsed;
                bool useMaximum = true;
                LabelVisibilityManager.DataPointRange prevRange = null;
                foreach (LabelVisibilityManager.DataPointRange dataPointRange in this._dataPointRanges)
                {
                    Dictionary<XYDataPoint, double> defaultRanking = this.GetDefaultRanking(dataPointRange, prevRange, ref useMaximum);
                    prevRange = dataPointRange;
                    foreach (SeriesPresenter seriesPresenter in dictionary.Values)
                        seriesPresenter.LabelPresenter.AdjustDataPointLabelVisibilityRating(dataPointRange, defaultRanking);
                    double num1 = -1.0;
                    foreach (double num2 in defaultRanking.Values)
                    {
                        if (num2 > num1)
                            num1 = num2;
                    }
                    foreach (XYDataPoint dataPoint in dataPointRange.DataPoints)
                    {
                        double num2;
                        if (defaultRanking.TryGetValue(dataPoint, out num2) && num2 == num1)
                            dataPoint.ActualLabelVisibility = Visibility.Visible;
                        else
                            dataPoint.ActualLabelVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private Dictionary<XYDataPoint, double> GetDefaultRanking(LabelVisibilityManager.DataPointRange range, LabelVisibilityManager.DataPointRange prevRange, ref bool useMaximum)
        {
            Dictionary<XYDataPoint, double> dictionary = new Dictionary<XYDataPoint, double>();
            if (!range.ProjectedYRange.HasData)
            {
                double num1 = 0.05;
                if (prevRange != null && prevRange.DataPointProjectedYRange.HasData && range.DataPointProjectedYRange.HasData)
                {
                    if (range.DataPointProjectedYRange.Maximum - prevRange.DataPointProjectedYRange.Maximum > num1 && range.DataPointProjectedYRange.Minimum - prevRange.DataPointProjectedYRange.Minimum > num1)
                        useMaximum = true;
                    else if (prevRange.DataPointProjectedYRange.Maximum - range.DataPointProjectedYRange.Maximum > num1 && prevRange.DataPointProjectedYRange.Minimum - range.DataPointProjectedYRange.Minimum > num1)
                        useMaximum = false;
                    else if (range.DataPointProjectedYRange.Maximum - prevRange.DataPointProjectedYRange.Maximum > num1 && prevRange.DataPointProjectedYRange.Minimum - range.DataPointProjectedYRange.Minimum > num1)
                    {
                        double num2 = Math.Abs(range.DataPointProjectedYRange.Maximum - prevRange.DataPointProjectedYRange.Maximum);
                        double num3 = Math.Abs(range.DataPointProjectedYRange.Minimum - prevRange.DataPointProjectedYRange.Minimum);
                        useMaximum = num2 > num3;
                    }
                    else
                        useMaximum = !useMaximum;
                }
            }
            XYDataPoint key = null;
            double d = double.NaN;
            foreach (XYDataPoint dataPoint in range.DataPoints)
            {
                if (double.IsNaN(d))
                {
                    key = dataPoint;
                    d = dataPoint.YValueInScaleUnitsWithoutAnimation;
                }
                else if (useMaximum && d < dataPoint.YValueInScaleUnitsWithoutAnimation)
                {
                    key = dataPoint;
                    d = dataPoint.YValueInScaleUnitsWithoutAnimation;
                }
                else if (!useMaximum && d >= dataPoint.YValueInScaleUnitsWithoutAnimation)
                {
                    key = dataPoint;
                    d = dataPoint.YValueInScaleUnitsWithoutAnimation;
                }
            }
            if (key != null)
                dictionary.Add(key, 50.0);
            if (range.ProjectedYRange.HasData)
                useMaximum = !useMaximum;
            return dictionary;
        }

        internal class DataPointRange
        {
            public Range<double> ProjectedXRange { get; set; }

            public Range<double> ProjectedYRange { get; set; }

            public Range<double> DataPointProjectedYRange { get; set; }

            public List<DataPoint> DataPoints { get; set; }

            public DataPointRange()
            {
                this.DataPoints = new List<DataPoint>();
            }
        }
    }
}
