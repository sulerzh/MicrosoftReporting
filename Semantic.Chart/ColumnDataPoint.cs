using System;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class ColumnDataPoint : XYDataPoint
    {
        public static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(ColumnLabelPosition), typeof(ColumnDataPoint), new PropertyMetadata(ColumnLabelPosition.OutsideEnd, new PropertyChangedCallback(ColumnDataPoint.OnLabelPositionChanged)));
        internal const string LabelPositionPropertyName = "LabelPosition";

        public ColumnLabelPosition LabelPosition
        {
            get
            {
                return (ColumnLabelPosition)this.GetValue(ColumnDataPoint.LabelPositionProperty);
            }
            set
            {
                this.SetValue(ColumnDataPoint.LabelPositionProperty, value);
            }
        }

        public ColumnDataPoint()
        {
        }

        public ColumnDataPoint(IComparable xValue, IComparable yValue)
          : base(xValue, yValue)
        {
        }

        private static void OnLabelPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ColumnLabelPosition newValue = (ColumnLabelPosition)e.NewValue;
            ColumnLabelPosition oldValue = (ColumnLabelPosition)e.OldValue;
            ((ColumnDataPoint)o).OnLabelPositionChanged(oldValue, newValue);
        }

        protected virtual void OnLabelPositionChanged(ColumnLabelPosition oldValue, ColumnLabelPosition newValue)
        {
            this.OnValueChanged("LabelPosition", oldValue, newValue);
        }
    }
}
