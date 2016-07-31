using Microsoft.Reporting.Windows.Common.Internal;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "SmallBarStyle", StyleTargetType = typeof(BarControl))]
    [StyleTypedProperty(Property = "TinyBarStyle", StyleTargetType = typeof(BarControl))]
    public class BarControl : Control
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(RectOrientation), typeof(BarControl), new PropertyMetadata(RectOrientation.BottomTop, new PropertyChangedCallback(BarControl.OnOrientationPropertyChanged)));
        public static readonly DependencyProperty SmallBarSizeThresholdProperty = DependencyProperty.Register("SmallBarSizeThreshold", typeof(double), typeof(BarControl), new PropertyMetadata(10.0, new PropertyChangedCallback(BarControl.OnSmallBarSizeThresholdPropertyChanged)));
        public static readonly DependencyProperty SmallBarStyleProperty = DependencyProperty.Register("SmallBarStyle", typeof(Style), typeof(BarControl), new PropertyMetadata(new PropertyChangedCallback(BarControl.OnSmallBarStylePropertyChanged)));
        public static readonly DependencyProperty TinyBarSizeThresholdProperty = DependencyProperty.Register("TinyBarSizeThreshold", typeof(double), typeof(BarControl), new PropertyMetadata(5.0, new PropertyChangedCallback(BarControl.OnTinyBarSizeThresholdPropertyChanged)));
        public static readonly DependencyProperty TinyBarStyleProperty = DependencyProperty.Register("TinyBarStyle", typeof(Style), typeof(BarControl), new PropertyMetadata(new PropertyChangedCallback(BarControl.OnTinyBarStylePropertyChanged)));

        public RectOrientation Orientation
        {
            get
            {
                return (RectOrientation)this.GetValue(BarControl.OrientationProperty);
            }
            set
            {
                this.SetValue(BarControl.OrientationProperty, value);
            }
        }

        public double SmallBarSizeThreshold
        {
            get
            {
                return (double)this.GetValue(BarControl.SmallBarSizeThresholdProperty);
            }
            set
            {
                this.SetValue(BarControl.SmallBarSizeThresholdProperty, value);
            }
        }

        public Style SmallBarStyle
        {
            get
            {
                return this.GetValue(BarControl.SmallBarStyleProperty) as Style;
            }
            set
            {
                this.SetValue(BarControl.SmallBarStyleProperty, value);
            }
        }

        public double TinyBarSizeThreshold
        {
            get
            {
                return (double)this.GetValue(BarControl.TinyBarSizeThresholdProperty);
            }
            set
            {
                this.SetValue(BarControl.TinyBarSizeThresholdProperty, value);
            }
        }

        public Style TinyBarStyle
        {
            get
            {
                return this.GetValue(BarControl.TinyBarStyleProperty) as Style;
            }
            set
            {
                this.SetValue(BarControl.TinyBarStyleProperty, value);
            }
        }

        public BarControl()
        {
            this.DefaultStyleKey = typeof(BarControl);
            this.SizeChanged += (sender, args) => this.UpdateBarStyleBasedOnSize();
        }

        private void UpdateBarStyleBasedOnSize()
        {
            double num = this.Orientation == RectOrientation.TopBottom || this.Orientation == RectOrientation.BottomTop ? this.Width : this.Height;
            double barSizeThreshold1 = this.TinyBarSizeThreshold;
            double barSizeThreshold2 = this.SmallBarSizeThreshold;
            if (num < this.TinyBarSizeThreshold)
            {
                if (this.Style != this.TinyBarStyle)
                    this.Style = this.TinyBarStyle;
            }
            else if (num < this.SmallBarSizeThreshold)
            {
                if (this.Style != this.SmallBarStyle)
                    this.Style = this.SmallBarStyle;
            }
            else if (this.Style != null)
                this.ClearValue(FrameworkElement.StyleProperty);
            if (this.Style == null)
                return;
            this.TinyBarSizeThreshold = barSizeThreshold1;
            this.SmallBarSizeThreshold = barSizeThreshold2;
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarControl)d).UpdateBarStyleBasedOnSize();
        }

        private static void OnSmallBarSizeThresholdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarControl)d).UpdateBarStyleBasedOnSize();
        }

        private static void OnSmallBarStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                ((BarControl)d).SmallBarStyle = (Style)e.OldValue;
            ((BarControl)d).UpdateBarStyleBasedOnSize();
        }

        private static void OnTinyBarSizeThresholdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BarControl)d).UpdateBarStyleBasedOnSize();
        }

        private static void OnTinyBarStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                ((BarControl)d).TinyBarStyle = (Style)e.OldValue;
            ((BarControl)d).UpdateBarStyleBasedOnSize();
        }
    }
}
