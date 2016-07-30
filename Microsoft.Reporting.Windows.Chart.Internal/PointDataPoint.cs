using System;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class PointDataPoint : XYDataPoint
    {
        public static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(PointLabelPosition), typeof(PointDataPoint), new PropertyMetadata(PointLabelPosition.Auto, new PropertyChangedCallback(PointDataPoint.OnLabelPositionChanged)));
        internal const string LabelPositionPropertyName = "LabelPosition";

        public PointLabelPosition LabelPosition
        {
            get
            {
                return (PointLabelPosition)this.GetValue(PointDataPoint.LabelPositionProperty);
            }
            set
            {
                this.SetValue(PointDataPoint.LabelPositionProperty, value);
            }
        }

        public PointDataPoint()
        {
        }

        public PointDataPoint(IComparable xValue, IComparable yValue)
          : base(xValue, yValue)
        {
        }

        private static void OnLabelPositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            PointLabelPosition newValue = (PointLabelPosition)e.NewValue;
            PointLabelPosition oldValue = (PointLabelPosition)e.OldValue;
            ((PointDataPoint)o).OnLabelPositionChanged(oldValue, newValue);
        }

        protected virtual void OnLabelPositionChanged(PointLabelPosition oldValue, PointLabelPosition newValue)
        {
            this.OnValueChanged("LabelPosition", oldValue, newValue);
        }
    }
}
