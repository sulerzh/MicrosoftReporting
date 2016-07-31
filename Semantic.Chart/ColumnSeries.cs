using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class ColumnSeries : XYSeries
    {
        public static readonly DependencyProperty PointGapRelativeWidthProperty = DependencyProperty.Register("PointGapRelativeWidth", typeof(double), typeof(ColumnSeries), new PropertyMetadata(0.2, new PropertyChangedCallback(ColumnSeries.OnPointGapRelativeWidthChanged)));
        public static readonly DependencyProperty PointWidthProperty = DependencyProperty.Register("PointWidth", typeof(double?), typeof(ColumnSeries), new PropertyMetadata(new PropertyChangedCallback(ColumnSeries.OnPointWidthChanged)));
        public static readonly DependencyProperty PointMaximumWidthProperty = DependencyProperty.Register("PointMaximumWidth", typeof(double?), typeof(ColumnSeries), new PropertyMetadata(new PropertyChangedCallback(ColumnSeries.OnPointMaximumWidthChanged)));
        public static readonly DependencyProperty PointMinimumWidthProperty = DependencyProperty.Register("PointMinimumWidth", typeof(double?), typeof(ColumnSeries), new PropertyMetadata(new PropertyChangedCallback(ColumnSeries.OnPointMinimumWidthChanged)));
        internal const string ClusterGroupKeyPropertyName = "ClusterGroupKey";
        private object _clusterGroupKey;

        public object ClusterGroupKey
        {
            get
            {
                return this._clusterGroupKey;
            }
            set
            {
                if (this._clusterGroupKey == value)
                    return;
                this._clusterGroupKey = value;
                this.OnClusterGroupKeyChanged();
            }
        }

        public double PointGapRelativeWidth
        {
            get
            {
                return (double)this.GetValue(ColumnSeries.PointGapRelativeWidthProperty);
            }
            set
            {
                this.SetValue(ColumnSeries.PointGapRelativeWidthProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<double>))]
        public double? PointWidth
        {
            get
            {
                return this.GetValue(ColumnSeries.PointWidthProperty) as double?;
            }
            set
            {
                this.SetValue(ColumnSeries.PointWidthProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<double>))]
        public double? PointMaximumWidth
        {
            get
            {
                return this.GetValue(ColumnSeries.PointMaximumWidthProperty) as double?;
            }
            set
            {
                this.SetValue(ColumnSeries.PointMaximumWidthProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<double>))]
        public double? PointMinimumWidth
        {
            get
            {
                return this.GetValue(ColumnSeries.PointMinimumWidthProperty) as double?;
            }
            set
            {
                this.SetValue(ColumnSeries.PointMinimumWidthProperty, value);
            }
        }

        internal override int ClusterKey
        {
            get
            {
                return new Tuple<string, Axis>("__CalculatePointWidth__", this.XAxis).GetHashCode();
            }
        }

        internal override ScaleDefaults YScaleDefaults
        {
            get
            {
                return new ScaleDefaults(AutoBool.True, 0.95);
            }
        }

        internal override SeriesPresenter CreateSeriesPresenter()
        {
            return new ColumnSeriesPresenter((XYSeries)this);
        }

        private void OnClusterGroupKeyChanged()
        {
            this.OnPropertyChanged("ClusterGroupKey");
            if (this.SeriesPresenter == null)
                return;
            this.SeriesPresenter.InvalidateSeries();
        }

        private static void OnPointGapRelativeWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColumnSeries columnSeries = (ColumnSeries)o;
            if (columnSeries.CallNestLevelFromRoot == 0)
            {
                double d = (double)e.NewValue;
                double num = (double)e.OldValue;
                if (d < 0.0 || d > 1.0 || double.IsNaN(d))
                {
                    ++columnSeries.CallNestLevelFromRoot;
                    columnSeries.PointGapRelativeWidth = num;
                    --columnSeries.CallNestLevelFromRoot;
                    throw new ArgumentException(Properties.Resources.Series_point_gap_width_must_be_in_range_from_0_to_1);
                }
            }
            if (columnSeries.SeriesPresenter == null)
                return;
            columnSeries.SeriesPresenter.InvalidateSeries();
        }

        private static void OnPointWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColumnSeries columnSeries = (ColumnSeries)o;
            if (columnSeries.CallNestLevelFromRoot == 0)
            {
                double? nullable1 = (double?)e.NewValue;
                double? nullable2 = (double?)e.OldValue;
                if (nullable1.HasValue && (nullable1.Value < 0.0 || double.IsNaN(nullable1.Value)))
                {
                    ++columnSeries.CallNestLevelFromRoot;
                    columnSeries.PointWidth = nullable2;
                    --columnSeries.CallNestLevelFromRoot;
                    throw new ArgumentException(Properties.Resources.Series_point_width_must_be_a_positive_number);
                }
            }
            if (columnSeries.SeriesPresenter == null)
                return;
            columnSeries.SeriesPresenter.InvalidateSeries();
        }

        private static void OnPointMaximumWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColumnSeries columnSeries = (ColumnSeries)o;
            if (columnSeries.CallNestLevelFromRoot == 0)
            {
                double? nullable1 = (double?)e.NewValue;
                double? nullable2 = (double?)e.OldValue;
                if (nullable1.HasValue && (nullable1.Value < 0.0 || double.IsNaN(nullable1.Value)))
                {
                    ++columnSeries.CallNestLevelFromRoot;
                    columnSeries.PointMaximumWidth = nullable2;
                    --columnSeries.CallNestLevelFromRoot;
                    throw new ArgumentException(Properties.Resources.Series_point_width_must_be_a_positive_number);
                }
            }
            if (columnSeries.SeriesPresenter == null)
                return;
            columnSeries.SeriesPresenter.InvalidateSeries();
        }

        private static void OnPointMinimumWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColumnSeries columnSeries = (ColumnSeries)o;
            if (columnSeries.CallNestLevelFromRoot == 0)
            {
                double? nullable1 = (double?)e.NewValue;
                double? nullable2 = (double?)e.OldValue;
                if (nullable1.HasValue && (nullable1.Value < 0.0 || double.IsNaN(nullable1.Value)))
                {
                    ++columnSeries.CallNestLevelFromRoot;
                    columnSeries.PointMinimumWidth = nullable2;
                    --columnSeries.CallNestLevelFromRoot;
                    throw new ArgumentException(Properties.Resources.Series_point_width_must_be_a_positive_number);
                }
            }
            if (columnSeries.SeriesPresenter == null)
                return;
            columnSeries.SeriesPresenter.InvalidateSeries();
        }

        internal override DataPoint CreateDataPoint()
        {
            return new ColumnDataPoint();
        }
    }
}
