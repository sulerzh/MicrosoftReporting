using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class TickMark : MarkerControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(AxisElementPosition), typeof(TickMark), new PropertyMetadata(AxisElementPosition.Outside, new PropertyChangedCallback(TickMark.OnPositionPropertyChanged)));
        internal const string PositionPropertyName = "Position";

        public AxisElementPosition Position
        {
            get
            {
                return (AxisElementPosition)this.GetValue(TickMark.PositionProperty);
            }
            set
            {
                this.SetValue(TickMark.PositionProperty, value);
            }
        }

        public event EventHandler PositionChanged;

        public TickMark()
        {
            this.DefaultStyleKey = typeof(TickMark);
        }

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TickMark)d).OnPositionPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnPositionPropertyChanged(object oldValue, object newValue)
        {
            if (this.PositionChanged == null)
                return;
            this.PositionChanged(this, EventArgs.Empty);
        }
    }
}
