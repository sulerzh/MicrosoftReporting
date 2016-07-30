using System;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class BubbleDataPoint : PointDataPoint
    {
        public static readonly DependencyProperty SizeValueProperty = DependencyProperty.Register("SizeValue", typeof(object), typeof(BubbleDataPoint), new PropertyMetadata(new PropertyChangedCallback(BubbleDataPoint.OnSizeValueChanged)));
        private bool _isSizeValueUsed = true;
        internal const string SizeValuePropertyName = "SizeValue";
        internal const string SizeValueInScaleUnitsPropertyName = "SizeValueInScaleUnits";
        internal const string SizeValueInScaleUnitsWithoutAnimationPropertyName = "SizeValueInScaleUnitsWithoutAnimation";
        private double _sizeValueInScaleUnits;
        private double _sizeValueInScaleUnitsWithoutAnimation;

        public object SizeValue
        {
            get
            {
                return this.GetValue(BubbleDataPoint.SizeValueProperty);
            }
            set
            {
                this.SetValue(BubbleDataPoint.SizeValueProperty, value);
            }
        }

        internal double SizeValueInScaleUnits
        {
            get
            {
                return this._sizeValueInScaleUnits;
            }
            set
            {
                if (this._sizeValueInScaleUnits == value)
                    return;
                double num = this._sizeValueInScaleUnits;
                this._sizeValueInScaleUnits = value;
                this.OnValueChanged("SizeValueInScaleUnits", num, value);
            }
        }

        internal double SizeValueInScaleUnitsWithoutAnimation
        {
            get
            {
                return this._sizeValueInScaleUnitsWithoutAnimation;
            }
            set
            {
                if (this._sizeValueInScaleUnitsWithoutAnimation == value)
                    return;
                double num = this._sizeValueInScaleUnitsWithoutAnimation;
                this._sizeValueInScaleUnitsWithoutAnimation = value;
                this.OnValueChanged("SizeValueInScaleUnitsWithoutAnimation", num, value);
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
                bool flag = this._isSizeValueUsed;
                this._isSizeValueUsed = value;
                this.OnValueChanged("IsSizeValueUsed", flag, this._isSizeValueUsed);
            }
        }

        public BubbleDataPoint()
        {
        }

        public BubbleDataPoint(IComparable xValue, IComparable yValue, IComparable sizeValue)
          : base(xValue, yValue)
        {
            this.SizeValue = sizeValue;
        }

        private static void OnSizeValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((BubbleDataPoint)o).OnSizeValueChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnSizeValueChanged(object oldValue, object newValue)
        {
            this.OnValueChanged("SizeValue", oldValue, newValue);
        }

        internal override void UpdateBinding()
        {
            base.UpdateBinding();
            if (this.Series == null || this.Series.ItemsBinder != null)
                return;
            this.SetDataPointBinding(BubbleDataPoint.SizeValueProperty, ((BubbleSeries)this.Series).SizeValueBinding);
        }
    }
}
