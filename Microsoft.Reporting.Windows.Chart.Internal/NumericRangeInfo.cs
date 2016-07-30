using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Diagnostics;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [DebuggerDisplay("[{Minimum}-{Maximum}] HasMin={HasMin} HasMax={HasMax} HasDataRange={HasDataRange}")]
    internal struct NumericRangeInfo
    {
        private const double DefaultMax = 10.0;
        public bool HasDataRange;
        public bool HasMin;
        public bool HasMax;
        public DoubleR10 Minimum;
        public DoubleR10 Maximum;
        public DoubleR10? ForsedSingleStop;

        public DoubleR10 Size
        {
            get
            {
                return this.Maximum - this.Minimum;
            }
        }

        public NumericRangeInfo(DoubleR10 minimum, DoubleR10 maximum)
        {
            this.HasDataRange = false;
            this.HasMin = true;
            this.HasMax = true;
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.ForsedSingleStop = new DoubleR10?();
            this.FixDirection();
        }

        public static NumericRangeInfo Calculate(Range<double>? dataRange, double? minimum, double? maximum, bool includeZero, double? interval = null)
        {
            NumericRangeInfo numericRangeInfo = new NumericRangeInfo();
            numericRangeInfo.HasDataRange = dataRange.HasValue && dataRange.Value.HasData;
            numericRangeInfo.HasMin = minimum.HasValue;
            numericRangeInfo.HasMax = maximum.HasValue;
            double num1 = numericRangeInfo.HasDataRange ? dataRange.Value.Minimum : double.MaxValue;
            double num2 = numericRangeInfo.HasDataRange ? dataRange.Value.Maximum : double.MinValue;
            double num3 = numericRangeInfo.HasMin ? minimum.Value : double.MaxValue;
            double num4 = numericRangeInfo.HasMax ? maximum.Value : double.MinValue;
            if (numericRangeInfo.HasMin && numericRangeInfo.HasMax)
            {
                numericRangeInfo.Minimum = num3;
                numericRangeInfo.Maximum = num4;
            }
            else if (numericRangeInfo.HasMin)
            {
                numericRangeInfo.Minimum = numericRangeInfo.Maximum = num3;
                if (num3 <= num2)
                    numericRangeInfo.Maximum = num2;
            }
            else if (numericRangeInfo.HasMax)
            {
                numericRangeInfo.Minimum = numericRangeInfo.Maximum = num4;
                if (num4 >= num1)
                    numericRangeInfo.Minimum = num1;
            }
            else if (numericRangeInfo.HasDataRange)
            {
                numericRangeInfo.Minimum = num1;
                numericRangeInfo.Maximum = num2;
            }
            else
                numericRangeInfo.Minimum = numericRangeInfo.Maximum = 0;
            if (includeZero)
                numericRangeInfo.IncludeZero();
            numericRangeInfo.FixIfEmpty(interval, includeZero);
            numericRangeInfo.FixDirection();
            if (includeZero)
            {
                if (numericRangeInfo.Minimum == 0)
                    numericRangeInfo.HasMin = true;
                if (numericRangeInfo.Maximum == 0)
                    numericRangeInfo.HasMax = true;
            }
            return numericRangeInfo;
        }

        private void IncludeZero()
        {
            if (this.HasMin && this.Minimum > 0 || this.HasMax && this.Maximum < 0)
                return;
            if (this.Minimum > 0)
                this.Minimum = 0;
            if (!(this.Maximum < 0))
                return;
            this.Maximum = 0;
        }

        private void FixDirection()
        {
            if (!(this.Minimum > this.Maximum))
                return;
            DoubleR10 doubleR10 = this.Minimum;
            this.Minimum = this.Maximum;
            this.Maximum = doubleR10;
        }

        private void FixIfEmpty(double? interval, bool includeZero)
        {
            if (!(this.Minimum == this.Maximum))
                return;
            if (this.HasMin && this.HasMax)
            {
                if (interval.HasValue)
                    this.Maximum = this.Minimum + interval.Value;
                else if (this.Maximum < 0)
                    this.Maximum = 0;
                else if (this.Minimum > 0)
                    this.Minimum = 0;
                else
                    this.Maximum = 10.0;
            }
            else if (this.HasMin)
            {
                if (interval.HasValue)
                    this.Maximum = this.Minimum + interval.Value;
                else if (this.Minimum > 0)
                    this.Maximum = this.Minimum * 2;
                else if (this.Minimum < 0)
                    this.Maximum = 0;
                else
                    this.Maximum = 10.0;
            }
            else if (this.HasMax)
            {
                if (interval.HasValue)
                    this.Minimum = this.Maximum - interval.Value;
                else if (this.Maximum > 0)
                    this.Minimum = 0;
                else if (this.Maximum < 0)
                    this.Minimum = this.Maximum * 2;
                else
                    this.Minimum = -10.0;
            }
            else if (this.HasDataRange)
            {
                if (interval.HasValue)
                    this.Maximum = this.Minimum + interval.Value;
                else if (this.Maximum < 0)
                {
                    if (includeZero || this.Maximum.IsSpecial())
                        this.Maximum = 0;
                    else
                        this.FixEmptySingleDataValue();
                }
                else if (this.Minimum > 0)
                {
                    if (includeZero || this.Minimum.IsSpecial())
                        this.Minimum = 0;
                    else
                        this.FixEmptySingleDataValue();
                }
                else
                {
                    this.Minimum = 0;
                    this.Maximum = 10.0;
                    this.HasDataRange = false;
                }
            }
            else
            {
                this.Minimum = 0;
                if (interval.HasValue)
                    this.Maximum = interval.Value;
                else
                    this.Maximum = 10.0;
            }
        }

        private void FixEmptySingleDataValue()
        {
            DoubleR10 doubleR10_1 = this.Maximum;
            doubleR10_1.Normalize();
            int num = 0;
            if (doubleR10_1.E >= 0 && doubleR10_1.Log10() < 4)
            {
                this.ForsedSingleStop = new DoubleR10?(doubleR10_1);
                num = 3;
            }
            DoubleR10 doubleR10_2 = DoubleR10.Pow10(Math.Max(doubleR10_1.E, doubleR10_1.Log10() - num)) / 2;
            this.Minimum = doubleR10_1 + doubleR10_2;
            this.Maximum = doubleR10_1 - doubleR10_2;
        }

        internal void ShrinkByStepQuick(NumericRangeInfo range, DoubleR10 step)
        {
            this.Minimum = this.Minimum.IncrementByStep(range.Minimum, step);
            this.Maximum = this.Maximum.DecrementByStep(range.Maximum, step);
        }

        internal void ShrinkByStep(NumericRangeInfo range, DoubleR10 step, bool useZeroRefPoint)
        {
            if (this.Minimum == 0 && this.Maximum == 0)
                this.Maximum = 1;
            else if (this.Minimum < 0 && this.Maximum <= 0)
            {
                this.Maximum = this.Maximum.DecrementByStep(range.Maximum, step);
                this.Minimum = this.Maximum.DecrementByStep(range.Minimum, step);
                if (!(this.Minimum == this.Maximum))
                    return;
                this.Minimum = this.Minimum - step;
            }
            else if (this.Minimum >= 0 && this.Maximum > 0 || !useZeroRefPoint)
            {
                this.Minimum = this.Minimum.IncrementByStep(range.Minimum, step);
                this.Maximum = this.Minimum.IncrementByStep(range.Maximum, step);
                if (!(this.Minimum == this.Maximum))
                    return;
                this.Maximum = this.Maximum + step;
            }
            else
            {
                this.Minimum = 0;
                this.Maximum = 0;
                this.Minimum = this.Minimum.DecrementByStep(range.Minimum, step);
                if (this.Minimum == this.Maximum)
                    this.Minimum = this.Minimum - step;
                this.Maximum = this.Maximum.IncrementByStep(range.Maximum, step);
                if (!(this.Minimum == this.Maximum))
                    return;
                this.Maximum = this.Maximum + step;
            }
        }
    }
}
