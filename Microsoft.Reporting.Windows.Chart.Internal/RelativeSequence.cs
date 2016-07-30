using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class RelativeSequence : NumericSequence
    {
        private NumericSequence _baseSequence;

        public RelativeSequence(NumericSequence baseSequence, double relativeInterval, double relativeIntervalOffset)
        {
            this._baseSequence = baseSequence;
            this.Interval = Math.Abs(relativeInterval);
            if (this.Interval == 0 || this.Interval.IsSpecial())
                this.Interval = 0.5;
            this.IntervalOffset = relativeIntervalOffset % this.Interval;
            this.Load();
        }

        private void Load()
        {
            this.Sequence = new List<DoubleR10>();
            this.Minimum = this._baseSequence.Minimum;
            this.Maximum = this._baseSequence.Maximum;
            if (this.Interval < 1)
            {
                DoubleR10 doubleR10_1 = DoubleR10.NaN;
                foreach (DoubleR10 doubleR10_2 in this._baseSequence)
                {
                    if (doubleR10_1 != DoubleR10.NaN)
                    {
                        DoubleR10 doubleR10_3 = doubleR10_1 + this.IntervalOffset;
                        DoubleR10 doubleR10_4 = doubleR10_2;
                        DoubleR10 x2 = (doubleR10_4 - doubleR10_3) * this.Interval;
                        if (x2 != 0)
                        {
                            for (DoubleR10 doubleR10_5 = doubleR10_3; doubleR10_5 <= doubleR10_4; doubleR10_5 = doubleR10_5.Add(ref x2))
                            {
                                if (this.Sequence.Count == 0 || doubleR10_5 != doubleR10_1)
                                    this.Sequence.Add(doubleR10_5);
                            }
                        }
                    }
                    doubleR10_1 = doubleR10_2;
                }
            }
            else
            {
                DoubleR10 interval = this.Interval;
                for (DoubleR10 doubleR10_1 = this.IntervalOffset; doubleR10_1 <= this._baseSequence.Count - 1; doubleR10_1 = doubleR10_1.Add(ref interval))
                {
                    int index = (int)DoubleR10.Floor(doubleR10_1, 0);
                    if (index < this._baseSequence.Count - 1)
                    {
                        DoubleR10 doubleR10_2 = this._baseSequence[index];
                        DoubleR10 doubleR10_3 = this._baseSequence[index + 1];
                        DoubleR10 doubleR10_4 = doubleR10_3 - doubleR10_2;
                        if (doubleR10_4 == 0)
                            this.Sequence.Add(doubleR10_3);
                        else
                            this.Sequence.Add(doubleR10_2 + (doubleR10_1 - index) * doubleR10_4);
                    }
                    else
                        this.Sequence.Add(this._baseSequence[this._baseSequence.Count - 1]);
                }
            }
        }

        public override void ExtendToCover(DoubleR10 min, DoubleR10 max)
        {
            this._baseSequence.ExtendToCover(min, max);
            this.Load();
        }

        public static RelativeSequence Calculate(NumericSequence baseSequence, double? relativeInterval, double? relativeIntervalOffset, double defaultInterval)
        {
            double relativeInterval1 = relativeInterval.HasValue ? relativeInterval.Value : defaultInterval;
            if (relativeInterval1 == 0.0 || baseSequence.Size / relativeInterval1 > 5000 && relativeInterval1 < defaultInterval)
                relativeInterval1 = defaultInterval;
            double relativeIntervalOffset1 = relativeIntervalOffset.HasValue ? relativeIntervalOffset.Value : 0.0;
            return new RelativeSequence(baseSequence, relativeInterval1, relativeIntervalOffset1);
        }
    }
}
