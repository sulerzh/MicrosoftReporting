using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class StackedColumnSeries : ColumnSeries
    {
        public static readonly DependencyProperty GroupingKeyProperty = DependencyProperty.Register("GroupingKey", typeof(object), typeof(StackedColumnSeries), new PropertyMetadata(null, new PropertyChangedCallback(StackedColumnSeries.OnGroupingKeyPropertyChanged)));
        public static readonly DependencyProperty IsHundredPercentProperty = DependencyProperty.Register("IsHundredPercent", typeof(bool), typeof(StackedColumnSeries), new PropertyMetadata(false, new PropertyChangedCallback(StackedColumnSeries.OnIsHundredPercentPropertyChanged)));
        internal const string GroupingKeyPropertyName = "GroupingKey";
        internal const string IsHundredPercentPropertyName = "IsHundredPercent";
        internal const string ActualIsHundredPercentPropertyName = "ActualIsHundredPercent";
        private bool _actualIsHundredPercent;

        public object GroupingKey
        {
            get
            {
                return this.GetValue(StackedColumnSeries.GroupingKeyProperty);
            }
            set
            {
                this.SetValue(StackedColumnSeries.GroupingKeyProperty, value);
            }
        }

        public bool IsHundredPercent
        {
            get
            {
                return (bool)this.GetValue(StackedColumnSeries.IsHundredPercentProperty);
            }
            set
            {
                this.SetValue(StackedColumnSeries.IsHundredPercentProperty, value);
            }
        }

        public bool ActualIsHundredPercent
        {
            get
            {
                return this._actualIsHundredPercent;
            }
            private set
            {
                if (value == this._actualIsHundredPercent)
                    return;
                this._actualIsHundredPercent = value;
                if (this.ChartArea != null && this.YAxis != null && this.YAxis.Scale != null)
                {
                    this.UpdateActualYDataRange();
                    this.SeriesPresenter.InvalidateSeries();
                }
                this.OnPropertyChanged("ActualIsHundredPercent");
            }
        }

        internal override SeriesPresenter CreateSeriesPresenter()
        {
            return new StackedColumnSeriesPresenter(this);
        }

        private static void OnGroupingKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StackedColumnSeries)d).OnGroupingKeyPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnGroupingKeyPropertyChanged(object oldValue, object newValue)
        {
            if (this.SeriesPresenter != null)
            {
                this.UpdateRelatedSeries();
                this.UpdateActualYDataRange();
                this.SeriesPresenter.InvalidateSeries();
            }
            this.OnPropertyChanged("GroupingKey");
        }

        private static void OnIsHundredPercentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StackedColumnSeries)d).OnIsHundredPercentPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsHundredPercentPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsHundredPercent");
        }

        protected override Range<IComparable> GetActualYDataRange()
        {
            StackedColumnSeriesPresenter columnSeriesPresenter = this.SeriesPresenter as StackedColumnSeriesPresenter;
            List<XYSeries> counterpartDataSeries = columnSeriesPresenter.GetCounterpartDataSeries(true);
            List<IComparable> that = new List<IComparable>();
            if (counterpartDataSeries.Count == 1 && !this.ActualIsHundredPercent)
                return base.GetActualYDataRange();
            foreach (XYDataPoint dataPoint in this.DataPoints)
            {
                if (dataPoint.YValue != null)
                {
                    object xValue = ValueHelper.ConvertValue(dataPoint.XValue, this.ActualXValueType);
                    Range<IComparable> range = columnSeriesPresenter.GetDataPointYValueRange(xValue, counterpartDataSeries);
                    if (range.HasData)
                    {
                        range = columnSeriesPresenter.ConvertToPercentRange(range);
                        that.Add(range.Minimum);
                        that.Add(range.Maximum);
                    }
                }
            }
            return that.GetRange<IComparable>();
        }

        internal override DataPoint CreateDataPoint()
        {
            return new StackedColumnDataPoint();
        }

        protected override DataValueType GetYValueType(IEnumerable<DataPoint> dataPoints)
        {
            DataValueType dataValueType = base.GetYValueType(dataPoints);
            if (dataValueType == DataValueType.Integer && this.IsHundredPercent)
                dataValueType = DataValueType.Float;
            return dataValueType;
        }

        private void UpdateActualIsHundredPercent()
        {
            this.ActualIsHundredPercent = this.IsHundredPercent && (this.ActualYValueType == DataValueType.Integer || this.ActualYValueType == DataValueType.Float);
        }

        protected override void OnPropertyChanged(string name)
        {
            switch (name)
            {
                case "IsHundredPercent":
                    this.UpdateActualYValueType();
                    this.UpdateActualIsHundredPercent();
                    break;
                case "ActualYValueType":
                    this.UpdateActualIsHundredPercent();
                    break;
            }
            base.OnPropertyChanged(name);
        }

        internal override void UpdateRelatedSeries()
        {
            base.UpdateRelatedSeries();
            ((StackedColumnSeriesPresenter)this.SeriesPresenter).InvalidateCounterpartSeriesAndDataPoints();
        }
    }
}
