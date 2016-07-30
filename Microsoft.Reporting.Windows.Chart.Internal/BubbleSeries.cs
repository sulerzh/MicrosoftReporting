using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(PointDataPoint))]
    public class BubbleSeries : PointSeries
    {
        public static readonly DependencyProperty SizeValueTypeProperty = DependencyProperty.Register("SizeValueType", typeof(DataValueType), typeof(BubbleSeries), new PropertyMetadata(DataValueType.Auto, new PropertyChangedCallback(BubbleSeries.OnSizeValueTypeChanged)));
        public static readonly DependencyProperty SizeDataRangeProperty = DependencyProperty.Register("SizeDataRange", typeof(Range<IComparable>?), typeof(BubbleSeries), new PropertyMetadata(null, new PropertyChangedCallback(BubbleSeries.OnSizeDataRangePropertyChanged)));
        private bool _isSizeValueUsed = true;
        private Range<double> _bubbleMarkerSizeRange = new Range<double>(200.0, 3000.0);
        private Range<IComparable> _actualSizeDataRange = new Range<IComparable>();
        internal const string SizeValueTypePropertyName = "SizeValueType";
        internal const string IsSizeValueUsedPropertyName = "IsSizeValueUsed";
        internal const string ActualSizeValueTypePropertyName = "ActualSizeValueType";
        internal const string BubbleMarkerSizeRangePropertyName = "BubbleMarkerSizeRange";
        internal const string BubbleMarkerSizeRangeUnitTypePropertyName = "BubbleMarkerSizeRangeUnitType";
        internal const string SizeDataRangePropertyName = "SizeDataRange";
        internal const string ActualSizeDataRangePropertyName = "ActualSizeDataRange";
        private Binding _sizeValueBinding;
        private DataValueType _actualSizeValueType;
        private BubbleSizeRangeUnitType _bubbleMarkerSizeRangeUnitType;

        public Binding SizeValueBinding
        {
            get
            {
                return this._sizeValueBinding;
            }
            set
            {
                if (value == this._sizeValueBinding)
                    return;
                this._sizeValueBinding = value;
                this.DataPoints.ForEachWithIndex<DataPoint>((item, index) => item.UpdateBinding());
            }
        }

        public string SizeValuePath
        {
            get
            {
                if (this.SizeValueBinding == null)
                    return null;
                return this.SizeValueBinding.Path.Path;
            }
            set
            {
                if (value == null)
                    this.SizeValueBinding = null;
                else
                    this.SizeValueBinding = new Binding(value);
            }
        }

        public DataValueType SizeValueType
        {
            get
            {
                return (DataValueType)this.GetValue(BubbleSeries.SizeValueTypeProperty);
            }
            set
            {
                this.SetValue(BubbleSeries.SizeValueTypeProperty, value);
            }
        }

        public bool IsSizeValueUsed
        {
            get
            {
                return this._isSizeValueUsed;
            }
            set
            {
                if (this._isSizeValueUsed == value)
                    return;
                this._isSizeValueUsed = value;
                this.OnPropertyChanged("IsSizeValueUsed");
            }
        }

        public DataValueType ActualSizeValueType
        {
            get
            {
                DataValueType dataValueType = this._actualSizeValueType;
                if (this._actualSizeValueType == DataValueType.Auto)
                {
                    dataValueType = this.GetSizeValueType();
                    if (dataValueType != DataValueType.Auto)
                    {
                        this._actualSizeValueType = dataValueType;
                        this.OnPropertyChanged("ActualSizeValueType");
                    }
                    else
                        dataValueType = DataValueType.Float;
                }
                return dataValueType;
            }
            protected set
            {
                DataValueType dataValueType = value != DataValueType.Auto ? value : this.GetSizeValueType();
                if (this._actualSizeValueType == dataValueType)
                    return;
                this._actualSizeValueType = dataValueType;
                if (this._actualSizeValueType == DataValueType.Auto)
                    return;
                this.OnPropertyChanged("ActualSizeValueType");
            }
        }

        public Range<double> BubbleMarkerSizeRange
        {
            get
            {
                return this._bubbleMarkerSizeRange;
            }
            set
            {
                if (!(this._bubbleMarkerSizeRange != value))
                    return;
                this._bubbleMarkerSizeRange = value;
                this.OnPropertyChanged("BubbleMarkerSizeRange");
            }
        }

        public BubbleSizeRangeUnitType BubbleMarkerSizeRangeUnitType
        {
            get
            {
                return this._bubbleMarkerSizeRangeUnitType;
            }
            set
            {
                if (this._bubbleMarkerSizeRangeUnitType == value)
                    return;
                this._bubbleMarkerSizeRangeUnitType = value;
                this.OnPropertyChanged("BubbleMarkerSizeRangeUnitType");
            }
        }

        public Range<IComparable>? SizeDataRange
        {
            get
            {
                return (Range<IComparable>?)this.GetValue(BubbleSeries.SizeDataRangeProperty);
            }
            set
            {
                this.SetValue(BubbleSeries.SizeDataRangeProperty, value);
            }
        }

        public Range<IComparable> ActualSizeDataRange
        {
            get
            {
                return this._actualSizeDataRange;
            }
            private set
            {
                if (!(this._actualSizeDataRange != value))
                    return;
                this._actualSizeDataRange = value;
                this.OnActualSizeDataRangeChanged();
            }
        }

        internal override ScaleDefaults XScaleDefaults
        {
            get
            {
                return new ScaleDefaults(AutoBool.False, 1.0);
            }
        }

        internal override ScaleDefaults YScaleDefaults
        {
            get
            {
                return new ScaleDefaults(AutoBool.False, 1.0);
            }
        }

        internal override SeriesPresenter CreateSeriesPresenter()
        {
            return new BubbleSeriesPresenter((XYSeries)this);
        }

        internal override DataPoint CreateDataPoint()
        {
            return new BubbleDataPoint();
        }

        private static void OnSizeValueTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleSeries)o).ActualSizeValueType = DataValueType.Auto;
        }

        private DataValueType GetSizeValueType()
        {
            if (this.SizeValueType == DataValueType.Auto)
            {
                BubbleDataPoint bubbleDataPoint = this.DataPoints.OfType<BubbleDataPoint>().Where<BubbleDataPoint>(item => item.SizeValue != null).FirstOrDefault<BubbleDataPoint>();
                if (bubbleDataPoint != null)
                    return ValueHelper.GetDataValueType(bubbleDataPoint.SizeValue);
            }
            return this.SizeValueType;
        }

        private static void OnSizeDataRangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as BubbleSeries).OnSizeDataRangeChanged(e.OldValue, e.NewValue);
        }

        private void OnSizeDataRangeChanged(object oldValue, object newValue)
        {
            Range<IComparable>? nullable = newValue as Range<IComparable>?;
            if (!nullable.HasValue)
                nullable = new Range<IComparable>?(this.GetSizeDataRange());
            this.ActualSizeDataRange = this.CalculateActualSizeDataRange(nullable.Value);
        }

        protected virtual void OnActualSizeDataRangeChanged()
        {
            if (this.ChartArea != null && this.IsSizeValueUsed)
                this.UpdateAllDataPointsSize();
            this.OnPropertyChanged("ActualSizeDataRange");
        }

        private Range<IComparable> CalculateActualSizeDataRange(Range<IComparable> dataRange)
        {
            if (dataRange.HasData)
            {
                IComparable zeroValue = ValueHelper.GetZeroValue(dataRange.Minimum);
                if (zeroValue != null && !dataRange.Contains(zeroValue))
                    dataRange = dataRange.ExtendTo(zeroValue);
            }
            return dataRange;
        }

        internal void UpdateAllDataPointsSize()
        {
            foreach (BubbleDataPoint dataPoint in this.DataPoints)
            {
                if (dataPoint.IsSizeValueUsed)
                {
                    dataPoint.SizeValueInScaleUnitsWithoutAnimation = dataPoint.SizeValue == null ? 0.0 : ((BubbleSeriesPresenter)this.SeriesPresenter).ProjectSizeToPixels(dataPoint, ValueHelper.ToDouble(dataPoint.SizeValue));
                    string storyboardKey = DependencyPropertyAnimationHelper.GetStoryboardKey("SizeValueInScaleUnits");
                    StoryboardInfo storyboardInfo = null;
                    if (dataPoint.Storyboards.TryGetValue(storyboardKey, out storyboardInfo) && storyboardInfo.Storyboard.Children.Count > 0)
                    {
                        DoubleAnimation doubleAnimation = storyboardInfo.Storyboard.Children[0] as DoubleAnimation;
                        if (doubleAnimation != null)
                        {
                            doubleAnimation.To = new double?(dataPoint.SizeValueInScaleUnitsWithoutAnimation);
                            continue;
                        }
                    }
                    dataPoint.SizeValueInScaleUnits = dataPoint.SizeValueInScaleUnitsWithoutAnimation;
                }
            }
        }

        internal override void UpdateActualDataRange()
        {
            base.UpdateActualDataRange();
            this.UpdateSizeDataRange();
        }

        internal void UpdateSizeDataRange()
        {
            this.ComputeAndSetSizeRangeOnAllBubbleSeries();
        }

        internal override void UpdateVisibility()
        {
        }

        public void ComputeAndSetSizeRangeOnAllBubbleSeries()
        {
            double num1 = double.MaxValue;
            double num2 = double.MinValue;
            XYChartArea xyChartArea = (XYChartArea)this.ChartArea;
            if (xyChartArea == null)
                return;
            foreach (Series series in xyChartArea.Series)
            {
                BubbleSeries bubbleSeries = series as BubbleSeries;
                if (bubbleSeries != null)
                {
                    Range<IComparable> sizeDataRange = bubbleSeries.GetSizeDataRange();
                    if (sizeDataRange.HasData)
                    {
                        if (ValueHelper.IsNumeric(sizeDataRange.Minimum) && Convert.ToDouble(sizeDataRange.Minimum, CultureInfo.InvariantCulture) < num1)
                            num1 = Convert.ToDouble(sizeDataRange.Minimum, CultureInfo.InvariantCulture);
                        if (ValueHelper.IsNumeric(sizeDataRange.Maximum) && Convert.ToDouble(sizeDataRange.Maximum, CultureInfo.InvariantCulture) > num2)
                            num2 = Convert.ToDouble(sizeDataRange.Maximum, CultureInfo.InvariantCulture);
                    }
                }
            }
            if (num1 == double.MaxValue && num2 == double.MinValue)
                return;
            Range<IComparable> dataRange = new Range<IComparable>(num1, num2);
            foreach (Series series in xyChartArea.Series)
            {
                BubbleSeries bubbleSeries = series as BubbleSeries;
                if (bubbleSeries != null && !bubbleSeries.SizeDataRange.HasValue)
                    bubbleSeries.ActualSizeDataRange = this.CalculateActualSizeDataRange(dataRange);
            }
        }

        private Range<IComparable> GetSizeDataRange()
        {
            IEnumerable<object> sizeValues = this.GetSizeValues(this.DataPointsByXValue);
            DataValueType valueType = this.ActualSizeValueType;
            if (valueType == DataValueType.Auto)
                valueType = sizeValues.GetDataValueType();
            return ValueAggregator.GetAggregator(valueType).GetRange(sizeValues);
        }

        private IEnumerable<object> GetSizeValues(IEnumerable<DataPoint> dataPoints)
        {
            foreach (BubbleDataPoint dataPoint in dataPoints)
                yield return dataPoint.SizeValue;
        }
    }
}
