using System;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class XYDataPoint : DataPoint
    {
        public static readonly DependencyProperty XValueProperty = DependencyProperty.Register("XValue", typeof(object), typeof(XYDataPoint), new PropertyMetadata(new PropertyChangedCallback(XYDataPoint.OnXValueChanged)));
        public static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof(object), typeof(XYDataPoint), new PropertyMetadata(new PropertyChangedCallback(XYDataPoint.OnYValueChanged)));
        internal const string XValuePropertyName = "XValue";
        internal const string YValuePropertyName = "YValue";
        internal const string XValueInScaleUnitsPropertyName = "XValueInScaleUnits";
        internal const string YValueInScaleUnitsPropertyName = "YValueInScaleUnits";
        internal const string XValueInScaleUnitsWithoutAnimationPropertyName = "XValueInScaleUnitsWithoutAnimation";
        internal const string YValueInScaleUnitsWithoutAnimationPropertyName = "YValueInScaleUnitsWithoutAnimation";
        private double _xValueInScaleUnits;
        private double _yValueInScaleUnits;
        private double _xValueInScaleUnitsWithoutAnimation;
        private double _yValueInScaleUnitsWithoutAnimation;

        public object XValue
        {
            get
            {
                return this.GetValue(XYDataPoint.XValueProperty);
            }
            set
            {
                this.SetValue(XYDataPoint.XValueProperty, value);
            }
        }

        public object YValue
        {
            get
            {
                return this.GetValue(XYDataPoint.YValueProperty);
            }
            set
            {
                this.SetValue(XYDataPoint.YValueProperty, value);
            }
        }

        public double XValueInScaleUnits
        {
            get
            {
                return this._xValueInScaleUnits;
            }
            set
            {
                if (this._xValueInScaleUnits == value)
                    return;
                double num = this._xValueInScaleUnits;
                this._xValueInScaleUnits = value;
                this.OnValueChanged("XValueInScaleUnits", num, value);
            }
        }

        public double YValueInScaleUnits
        {
            get
            {
                return this._yValueInScaleUnits;
            }
            set
            {
                if (this._yValueInScaleUnits == value)
                    return;
                double num = this._yValueInScaleUnits;
                this._yValueInScaleUnits = value;
                this.OnValueChanged("YValueInScaleUnits", num, value);
            }
        }

        public double XValueInScaleUnitsWithoutAnimation
        {
            get
            {
                return this._xValueInScaleUnitsWithoutAnimation;
            }
            set
            {
                if (this._xValueInScaleUnitsWithoutAnimation == value)
                    return;
                double num = this._xValueInScaleUnitsWithoutAnimation;
                this._xValueInScaleUnitsWithoutAnimation = value;
                this.OnValueChanged("XValueInScaleUnitsWithoutAnimation", num, value);
            }
        }

        public double YValueInScaleUnitsWithoutAnimation
        {
            get
            {
                return this._yValueInScaleUnitsWithoutAnimation;
            }
            set
            {
                if (this._yValueInScaleUnitsWithoutAnimation == value)
                    return;
                double num = this._yValueInScaleUnitsWithoutAnimation;
                this._yValueInScaleUnitsWithoutAnimation = value;
                this.OnValueChanged("YValueInScaleUnitsWithoutAnimation", num, value);
            }
        }

        protected override object PrimaryValue
        {
            get
            {
                return this.YValue;
            }
        }

        public override bool ActualIsEmpty
        {
            get
            {
                if (this.PrimaryValue != null)
                    return this.IsEmpty;
                return true;
            }
        }

        public XYDataPoint()
        {
        }

        public XYDataPoint(IComparable xValue, IComparable yValue)
        {
            this.XValue = xValue;
            this.YValue = yValue;
        }

        private static void OnXValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((XYDataPoint)o).OnXValueChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnXValueChanged(object oldValue, object newValue)
        {
            this.OnValueChanged("XValue", oldValue, newValue);
        }

        private static void OnYValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((XYDataPoint)o).OnYValueChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnYValueChanged(object oldValue, object newValue)
        {
            this.OnValueChanged("YValue", oldValue, newValue);
            if (!this.ShowValueAsLabel)
                return;
            this.UpdateActualLabelContent();
        }

        internal override void UpdateBinding()
        {
            base.UpdateBinding();
            if (this.Series == null || this.Series.ItemsBinder != null)
                return;
            this.SetDataPointBinding(XYDataPoint.XValueProperty, ((XYSeries)this.Series).XValueBinding);
            this.SetDataPointBinding(XYDataPoint.YValueProperty, ((XYSeries)this.Series).YValueBinding);
        }
    }
}
