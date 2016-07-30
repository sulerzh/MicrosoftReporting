using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [DebuggerDisplay("[{Minimum}-{Maximum}] Count={Count} Interval={Interval} Offset={IntervalOffset}")]
    internal class NumericSequence : IEnumerable<DoubleR10>, IEnumerable
    {
        private static readonly DoubleR10[] DefaultGridSteps = new DoubleR10[3] { 0.1, 0.2, 0.5 };
        private static readonly DoubleR10 MaxGoodLookingDouble = new DoubleR10(1L, 308);
        private const int DefaultMinPower = -2147483648;
        private const double DefaultMax = 10.0;
        private const int MinSequenceCount = 1;
        private const int MaxSequenceCount = 1000;
        private DoubleR10 _min;
        private DoubleR10 _max;
        private IList<DoubleR10> _sequence;
        private DoubleR10 _interval;
        private DoubleR10 _intervalOffset;
        private bool _canExtendMin;
        private bool _canExtendMax;
        private DoubleR10 _maxAllowedMargin;

        public DoubleR10 Minimum
        {
            get
            {
                return this._min;
            }
            protected set
            {
                this._min = value;
            }
        }

        public DoubleR10 Maximum
        {
            get
            {
                return this._max;
            }
            protected set
            {
                this._max = value;
            }
        }

        public IList<DoubleR10> Sequence
        {
            get
            {
                return this._sequence;
            }
            set
            {
                this._sequence = value;
            }
        }

        public DoubleR10 Size
        {
            get
            {
                return this._max - this._min;
            }
        }

        public int Count
        {
            get
            {
                return this._sequence.Count;
            }
        }

        public DoubleR10 Interval
        {
            get
            {
                return this._interval;
            }
            protected set
            {
                this._interval = value;
            }
        }

        public DoubleR10 IntervalOffset
        {
            get
            {
                return this._intervalOffset;
            }
            protected set
            {
                this._intervalOffset = value;
            }
        }

        public DoubleR10 this[int index]
        {
            get
            {
                return this._sequence[index];
            }
            set
            {
                this._sequence[index] = value;
            }
        }

        public NumericSequence()
        {
            this._sequence = new List<DoubleR10>();
        }

        protected void Load(IEnumerable<DoubleR10> ticks)
        {
            this._sequence = new List<DoubleR10>();
            DoubleR10 doubleR10_1 = 0;
            DoubleR10 doubleR10_2 = 0;
            bool flag = false;
            int num = 0;
            foreach (DoubleR10 tick in ticks)
            {
                if (num == 0 || tick != doubleR10_1)
                {
                    DoubleR10 doubleR10_3 = tick - doubleR10_1;
                    if (num > 1 && doubleR10_3 != doubleR10_2)
                        flag = true;
                    this._sequence.Add(tick);
                    doubleR10_1 = tick;
                    doubleR10_2 = doubleR10_3;
                    ++num;
                }
            }
            this._min = this._sequence[0];
            this._max = this._sequence[this._sequence.Count - 1];
            if (flag)
                return;
            this._interval = doubleR10_2;
        }

        public virtual void ExtendToCover(DoubleR10 min, DoubleR10 max)
        {
            if (this._interval == 0 || this._interval == DoubleR10.NaN)
                return;
            DoubleR10 doubleR10 = this._sequence[0];
            while (min < doubleR10 && this._sequence.Count < 1000)
            {
                doubleR10 -= this._interval;
                if (!doubleR10.IsDoubleInfinity())
                {
                    this._sequence.Insert(0, doubleR10);
                    this._min = doubleR10;
                }
            }
            doubleR10 = this._sequence[this._sequence.Count - 1];
            while (max > doubleR10 && this._sequence.Count < 1000)
            {
                doubleR10 += this._interval;
                if (!doubleR10.IsDoubleInfinity())
                {
                    this._sequence.Add(doubleR10);
                    this._max = doubleR10;
                }
            }
            this.TrimMinMax(min, max);
        }

        private void TrimMinMax(DoubleR10 min, DoubleR10 max)
        {
            DoubleR10 doubleR10_1 = (min - this._min) / this._interval;
            DoubleR10 doubleR10_2 = (this._max - max) / this._interval;
            DoubleR10 doubleR10_3 = 0.001;
            if (!this._canExtendMin || doubleR10_1 > this._maxAllowedMargin && doubleR10_1 > doubleR10_3)
                this._min = min;
            if (this._canExtendMax && (!(doubleR10_2 > this._maxAllowedMargin) || !(doubleR10_2 > doubleR10_3)))
                return;
            this._max = max;
        }

        public void MoveToCover(DoubleR10 min, DoubleR10 max)
        {
            DateTime now = DateTime.Now;
            if (this._interval == 0 || this._interval == DoubleR10.NaN)
                return;
            this._min = this._sequence[0];
            long num = DoubleR10.CountSteps(this._min - min, this._interval);
            this._min = !(min < this._min) ? this._min + this._interval * num : this._min - this._interval * (num + 1L);
            this._sequence.Clear();
            this._max = this._min;
            if (min == this._min)
                this._sequence.Add(this._min);
            while (max > this._max && this._sequence.Count < 1000)
            {
                this._max = this._max + this._interval;
                this._sequence.Add(this._max);
            }
        }

        public IEnumerator<DoubleR10> GetEnumerator()
        {
            return this._sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._sequence.GetEnumerator();
        }

        public static NumericSequence Calculate(NumericRangeInfo range, double? interval = null, double? intervalOffset = null, int maxCount = 10, int minPower = -2147483648, DoubleR10[] steps = null, bool useZeroRefPoint = true, double maxAllowedMargin = 1.0)
        {
            NumericSequence numericSequence = new NumericSequence();
            if (steps == null)
                steps = NumericSequence.DefaultGridSteps;
            long num1 = 0;
            numericSequence._maxAllowedMargin = maxAllowedMargin;
            if (range.ForsedSingleStop.HasValue)
            {
                DoubleR10 doubleR10 = range.ForsedSingleStop.Value;
                numericSequence._interval = range.Maximum - range.Minimum;
                numericSequence._intervalOffset = doubleR10;
                numericSequence._min = range.Minimum;
                numericSequence._max = range.Maximum;
                numericSequence._sequence.Add(doubleR10);
                numericSequence.ExtendToCover(range.Minimum, range.Maximum);
                return numericSequence;
            }
            numericSequence._interval = interval.HasValue ? Math.Abs(interval.Value) : DoubleR10.Zero;
            numericSequence._intervalOffset = intervalOffset.HasValue ? intervalOffset.Value : DoubleR10.MinValue;
            numericSequence._min = DoubleR10.Zero;
            numericSequence._max = DoubleR10.Zero;
            maxCount = Math.Max(maxCount, 1);
            maxCount = Math.Min(maxCount, 1000);
            numericSequence._canExtendMin = maxAllowedMargin > 0.0 && range.HasDataRange && !range.HasMin && range.Minimum != 0;
            numericSequence._canExtendMax = maxAllowedMargin > 0.0 && range.HasDataRange && !range.HasMax && range.Maximum != 0;
            if (interval.HasValue && numericSequence._interval != 0 && intervalOffset.HasValue)
            {
                num1 = (long)(range.Size / numericSequence._interval);
                numericSequence._min = range.Minimum;
                numericSequence._max = range.Minimum + num1 * numericSequence._interval;
            }
            else
            {
                int num2 = !(numericSequence._interval != 0) ? (!(range.Size != 0) ? range.Maximum.Log10() : range.Size.Log10()) : numericSequence._interval.Log10() + 1;
                int num3 = steps[0].Log10();
                int num4 = num2 - num3 - 1 - (maxCount - (DoubleR10)1).Log10();
                if (minPower != int.MinValue)
                    num4 = Math.Max(num4, minPower - num3);
                if (numericSequence._interval != 0)
                {
                    NumericRangeInfo numericRangeInfo = new NumericRangeInfo(DoubleR10.Floor(range.Minimum, num4), DoubleR10.Ceiling(range.Maximum, num4));
                    numericRangeInfo.ShrinkByStep(range, numericSequence._interval, useZeroRefPoint);
                    numericSequence._min = numericRangeInfo.Minimum;
                    numericSequence._max = numericRangeInfo.Maximum;
                    num1 = DoubleR10.CountSteps(numericRangeInfo.Size, numericSequence._interval);
                }
                else
                {
                    for (int index1 = 0; index1 < 3; ++index1)
                    {
                        int exp = num4 + index1;
                        DoubleR10 doubleR10 = DoubleR10.Pow10(exp);
                        DoubleR10 minimum = DoubleR10.Floor(range.Minimum, exp);
                        DoubleR10 maximum = DoubleR10.Ceiling(range.Maximum, exp);
                        for (int index2 = 0; index2 < steps.Length; ++index2)
                        {
                            DoubleR10 step = steps[index2] * doubleR10;
                            step.Normalize();
                            NumericRangeInfo roundRange = new NumericRangeInfo(minimum, maximum);
                            if (steps == NumericSequence.DefaultGridSteps)
                                roundRange.ShrinkByStepQuick(range, step);
                            else
                                roundRange.ShrinkByStep(range, step, useZeroRefPoint);
                            if (numericSequence._canExtendMin && range.Minimum == roundRange.Minimum && maxAllowedMargin >= 1.0)
                                roundRange.Minimum -= step;
                            if (numericSequence._canExtendMax && range.Maximum == roundRange.Maximum && maxAllowedMargin >= 1.0)
                                roundRange.Maximum += step;
                            num1 = NumericSequence.GetCount(range, roundRange, step, numericSequence._canExtendMin, numericSequence._canExtendMin, maxAllowedMargin, maxCount);
                            if (num1 <= maxCount || index1 == 2 && index2 == steps.Length - 1)
                            {
                                numericSequence._interval = step;
                                numericSequence._min = roundRange.Minimum;
                                numericSequence._max = roundRange.Maximum;
                                break;
                            }
                        }
                        if (numericSequence._interval != 0)
                            break;
                    }
                }
            }
            if (num1 > maxCount * 32)
            {
                num1 = maxCount * 32;
                numericSequence._interval = numericSequence.Size / num1;
            }
            if (intervalOffset.HasValue)
            {
                numericSequence._intervalOffset = numericSequence._intervalOffset % numericSequence._interval;
                DoubleR10 doubleR10 = range.Minimum + numericSequence._intervalOffset - numericSequence._min;
                numericSequence._min = numericSequence._min + doubleR10;
                numericSequence._max = numericSequence._max + doubleR10;
            }
            else
                numericSequence._intervalOffset = numericSequence._min - range.Minimum;
            numericSequence.FixDoubleOverflow(range);
            numericSequence._sequence = new List<DoubleR10>();
            DoubleR10 doubleR10_1 = numericSequence._min;
            numericSequence._sequence.Add(doubleR10_1);
            for (int index = 0; index < num1; ++index)
            {
                doubleR10_1 = doubleR10_1.Add(ref numericSequence._interval);
                if (!doubleR10_1.IsDoubleInfinity())
                    numericSequence._sequence.Add(doubleR10_1);
            }
            numericSequence.ExtendToCover(range.Minimum, range.Maximum);
            return numericSequence;
        }

        private void FixDoubleOverflow(NumericRangeInfo range)
        {
            if (this._interval > NumericSequence.MaxGoodLookingDouble)
            {
                this._interval = NumericSequence.MaxGoodLookingDouble;
                if (this._min.IsDoubleInfinity())
                    this._min = -NumericSequence.MaxGoodLookingDouble;
            }
            else if (this._min.IsDoubleInfinity())
                this._min = this._min + this._interval;
            if (!this._max.IsDoubleInfinity())
                return;
            this._max = range.Maximum;
        }

        public static NumericSequence CalculateUnits(int min, int max, int? interval, int? intervalOffset, int[] steps, int maxCount)
        {
            maxCount = Math.Max(maxCount, 1);
            maxCount = Math.Min(maxCount, 1000);
            if (min == max)
                max = min + 1;
            int num1 = max - min;
            int num2 = 0;
            int num3 = 0;
            if (interval.HasValue)
            {
                int? nullable1 = interval;
                if ((nullable1.GetValueOrDefault() != 0 ? 0 : (nullable1.HasValue ? 1 : 0)) == 0)
                {
                    int num4 = num1;
                    int? nullable2 = interval;
                    int? nullable3 = nullable2.HasValue ? new int?(num4 / nullable2.GetValueOrDefault()) : new int?();
                    if ((nullable3.GetValueOrDefault() <= 1000 ? 0 : (nullable3.HasValue ? 1 : 0)) == 0)
                    {
                        num3 = interval.Value;
                        num2 = num1 / num3;
                        goto label_12;
                    }
                }
            }
            for (int index = 0; index < steps.Length; ++index)
            {
                num3 = steps[index];
                int num4 = num1 / num3;
                if (num1 % num3 != 0)
                    ++num4;
                if (num4 <= maxCount)
                    break;
            }
            label_12:
            int num5 = -min;
            if (intervalOffset.HasValue)
                num5 += intervalOffset.Value;
            int num6 = num5 % num3;
            NumericSequence numericSequence = new NumericSequence();
            int num7 = min + num6;
            while (true)
            {
                numericSequence._sequence.Add(num7);
                if (num7 < max)
                    num7 += num3;
                else
                    break;
            }
            numericSequence.Interval = num3;
            numericSequence.IntervalOffset = num6;
            numericSequence.Minimum = numericSequence._sequence[0];
            numericSequence.Maximum = numericSequence._sequence[numericSequence._sequence.Count - 1];
            numericSequence.ExtendToCover(min, max);
            return numericSequence;
        }

        private static long GetCount(NumericRangeInfo range, NumericRangeInfo roundRange, DoubleR10 step, bool canExtendMin, bool canExtendMax, double maxAllowedMargin, long maxCount)
        {
            long num = DoubleR10.CountSteps(roundRange.Size, step);
            if (maxCount < num && num < maxCount + 3L)
                num = NumericSequence.GetExactCount(range, roundRange, step, canExtendMin, canExtendMax, maxAllowedMargin);
            return num;
        }

        private static long GetExactCount(NumericRangeInfo range, NumericRangeInfo roundRange, DoubleR10 step, bool canExtendMin, bool canExtendMax, double maxAllowedMargin)
        {
            long num = 0;
            DoubleR10 min1 = NumericSequence.GetMin(range.Minimum, roundRange.Minimum, step, canExtendMin, maxAllowedMargin);
            DoubleR10 min2 = NumericSequence.GetMin(range.Maximum, roundRange.Maximum, step, canExtendMax, maxAllowedMargin);
            DoubleR10 doubleR10 = roundRange.Minimum;
            while (doubleR10 < roundRange.Maximum)
            {
                if (min1 <= doubleR10 && doubleR10 <= min2)
                    ++num;
                doubleR10 += step;
            }
            return num - 1L;
        }

        private static DoubleR10 GetMin(DoubleR10 min, DoubleR10 roundedMin, DoubleR10 interval, bool canExtendMin, DoubleR10 maxMargin)
        {
            if (!canExtendMin || (min - roundedMin) / interval > maxMargin)
                return min;
            return roundedMin;
        }

        private static DoubleR10 GetMax(DoubleR10 max, DoubleR10 roundedMax, DoubleR10 interval, bool canExtendMax, DoubleR10 maxMargin)
        {
            if (!canExtendMax || (roundedMax - max) / interval > maxMargin)
                return max;
            return roundedMax;
        }
    }
}
