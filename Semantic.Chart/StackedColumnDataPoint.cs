using System;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class StackedColumnDataPoint : XYDataPoint
    {
        internal const string YValuePercentPropertyName = "YValuePercent";
        private double _yValuePercent;

        internal double YValuePercent
        {
            get
            {
                return this._yValuePercent;
            }
            set
            {
                if (this._yValuePercent == value)
                    return;
                double num = this._yValuePercent;
                this._yValuePercent = value;
                this.OnValueChanged("YValuePercent", num, value);
            }
        }

        protected override object PrimaryValue
        {
            get
            {
                StackedColumnSeries stackedColumnSeries = this.Series as StackedColumnSeries;
                if (stackedColumnSeries == null || !stackedColumnSeries.ActualIsHundredPercent)
                    return base.PrimaryValue;
                if (this.YValuePercent == 0.0)
                    return null;
                return this.YValuePercent;
            }
        }

        public StackedColumnDataPoint()
        {
        }

        public StackedColumnDataPoint(IComparable xValue, IComparable yValue)
          : base(xValue, yValue)
        {
        }

        protected override void UpdateActualPropertiesFromDataPoint(string propertyName)
        {
            base.UpdateActualPropertiesFromDataPoint(propertyName);
            if (!(propertyName == "YValuePercent"))
                return;
            StackedColumnSeries stackedColumnSeries = this.Series as StackedColumnSeries;
            if (stackedColumnSeries == null || !stackedColumnSeries.ActualIsHundredPercent)
                return;
            this.UpdateActualLabelContent();
        }
    }
}
