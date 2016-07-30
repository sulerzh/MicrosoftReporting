using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class DateTimeSequence : IEnumerable<DateTime>, IEnumerable
    {
        private List<DateTime> _sequence = new List<DateTime>();
        private DateTime _min = DateTime.MaxValue;
        private DateTime _max = DateTime.MinValue;
        private DateTimeScaleUnit _unit;
        private DoubleR10 _interval;
        private DoubleR10 _intervalOffset;

        public DateTime Minimum
        {
            get
            {
                return this._min;
            }
        }

        public DateTime Maximum
        {
            get
            {
                return this._max;
            }
        }

        public DateTimeScaleUnit Unit
        {
            get
            {
                return this._unit;
            }
        }

        public TimeSpan Size
        {
            get
            {
                return this._max.Subtract(this._min);
            }
        }

        public DoubleR10 Interval
        {
            get
            {
                return this._interval;
            }
            private set
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
            private set
            {
                this._intervalOffset = value;
            }
        }

        public int Count
        {
            get
            {
                return this._sequence.Count;
            }
        }

        public DateTime this[int index]
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

        public DateTimeSequence(DateTimeScaleUnit unit)
        {
            this._unit = unit;
            this._interval = 1;
        }

        public DateTimeSequence(IEnumerable<DateTime> ticks, DateTimeScaleUnit unit)
          : this(unit)
        {
            foreach (DateTime tick in ticks)
                this.Add(tick);
        }

        private void Add(DateTime date)
        {
            this._sequence.Add(date);
            if (date < this._min)
                this._min = date;
            if (!(date > this._max))
                return;
            this._max = date;
        }

        public DateTimeSequence RemoveDuplicates(DateTimeSequence other)
        {
            if (other == null || other.Count == 0)
                return this;
            DateTimeSequence dateTimeSequence = new DateTimeSequence(this._unit);
            int index1 = 0;
            for (int index2 = 0; index2 < this.Count; ++index2)
            {
                DateTime date = this[index2];
                bool flag = false;
                if (index1 < other.Count)
                {
                    DateTime dateTime = other[index1];
                    while (dateTime < date && index1 < other.Count - 1)
                        dateTime = other[++index1];
                    if (date == dateTime)
                        flag = true;
                }
                if (!flag)
                    dateTimeSequence.Add(date);
            }
            return dateTimeSequence;
        }

        public void ExtendToCover(DateTime min, DateTime max)
        {
            try
            {
                while (min < this._min)
                {
                    this._min = DateTimeSequence.AddInterval(this._min, -this._interval, this._unit);
                    this._sequence.Insert(0, this._min);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            try
            {
                while (max > this._max)
                {
                    this._max = DateTimeSequence.AddInterval(this._max, this._interval, this._unit);
                    this._sequence.Add(this._max);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
        }

        public void MoveToCover(DateTime min, DateTime max)
        {
            int num1 = (int)this._interval;
            switch (this.Unit)
            {
                case DateTimeScaleUnit.Years:
                    int num2 = (min.Year - this._min.Year) / num1;
                    if (num2 < 0)
                        --num2;
                    this._min = this._min.AddYears(num2 * num1);
                    break;
                case DateTimeScaleUnit.Months:
                    int num3 = ((min.Year - this._min.Year) * 12 + min.Month - this._min.Month) / num1;
                    if (num3 < 0)
                        --num3;
                    this._min = this._min.AddMonths(num3 * num1);
                    break;
                case DateTimeScaleUnit.Weeks:
                    this._min = this._min.AddDays(Math.Floor(min.Subtract(this._min).TotalDays / 7.0 / num1) * num1 * 7.0);
                    break;
                case DateTimeScaleUnit.Days:
                    this._min = this._min.AddDays(Math.Floor(min.Subtract(this._min).TotalDays / num1) * num1);
                    break;
                case DateTimeScaleUnit.Hours:
                    this._min = this._min.AddHours(Math.Floor(min.Subtract(this._min).TotalHours / num1) * num1);
                    break;
                case DateTimeScaleUnit.Minutes:
                    this._min = this._min.AddMinutes(Math.Floor(min.Subtract(this._min).TotalMinutes / num1) * num1);
                    break;
                case DateTimeScaleUnit.Seconds:
                    this._min = this._min.AddSeconds(Math.Floor(min.Subtract(this._min).TotalSeconds / num1) * num1);
                    break;
                case DateTimeScaleUnit.Milliseconds:
                    this._min = this._min.AddMilliseconds(Math.Floor(min.Subtract(this._min).TotalMilliseconds / num1) * num1);
                    break;
                default:
                    throw new NotImplementedException();
            }
            this._sequence.Clear();
            this._sequence.Add(this._min);
            this._max = this._min;
            try
            {
                while (max > this._max)
                {
                    this._max = DateTimeSequence.AddInterval(this._max, this._interval, this._unit);
                    this._sequence.Add(this._max);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
        }

        public static DateTimeSequence CalculateCenturies(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            NumericRangeInfo range1 = new NumericRangeInfo(range.Minimum.Year, range.Maximum.Year);
            int? nullable1 = interval;
            double? interval1 = nullable1.HasValue ? new double?(nullable1.GetValueOrDefault()) : new double?();
            int? nullable2 = intervalOffset;
            double? intervalOffset1 = nullable2.HasValue ? new double?(nullable2.GetValueOrDefault()) : new double?();
            int maxCount1 = maxCount;
            int minPower = 2;
            DoubleR10[] steps = new DoubleR10[3] { 1, 2, 5 };
            int num = 1;
            double maxAllowedMargin = 1.0;
            NumericSequence numericSequence = NumericSequence.Calculate(range1, interval1, intervalOffset1, maxCount1, minPower, steps, num != 0, maxAllowedMargin);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Years);
            try
            {
                foreach (DoubleR10 doubleR10 in numericSequence)
                {
                    int year = (int)doubleR10;
                    if (year > 0)
                        dateTimeSequence.Add(new DateTime(year, 1, 1));
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = numericSequence.Interval;
            dateTimeSequence.IntervalOffset = numericSequence.IntervalOffset;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateYears(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            NumericSequence units = NumericSequence.CalculateUnits(range.Minimum.Year, range.Maximum.Year, interval, intervalOffset, new int[10] { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000 }, maxCount);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Years);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    int year = (int)doubleR10;
                    if (year > 0 && year < 10000)
                        dateTimeSequence.Add(new DateTime(year, 1, 1));
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval;
            dateTimeSequence.IntervalOffset = units.IntervalOffset;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateMonths(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            DateTime dateTime = new DateTime(range.Minimum.Year, 1, 1);
            NumericSequence units = NumericSequence.CalculateUnits(range.Minimum.Month - 1, (range.Maximum.Year - dateTime.Year) * 12 + range.Maximum.Month - 1, interval, intervalOffset, new int[5] { 1, 2, 3, 6, 12 }, maxCount);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Months);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    int months = (int)doubleR10;
                    DateTime date = dateTime.AddMonths(months);
                    dateTimeSequence.Add(date);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval;
            dateTimeSequence.IntervalOffset = units.IntervalOffset;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateWeeks(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount, DayOfWeek firstDayOfWeek)
        {
            DateTime dateTime = new DateTime(range.Minimum.Year, range.Minimum.Month, range.Minimum.Day);
            while (dateTime.DayOfWeek != firstDayOfWeek)
                dateTime = dateTime.AddDays(-1.0);
            int num1 = (int)Math.Floor(range.Minimum.Subtract(dateTime).TotalDays);
            int num2 = (int)Math.Ceiling(range.Maximum.Subtract(dateTime).TotalDays);
            int min = num1;
            int max = num2;
            int? nullable = interval;
            int? interval1 = nullable.HasValue ? new int?(nullable.GetValueOrDefault() * 7) : new int?();
            int? intervalOffset1 = intervalOffset;
            int[] steps = new int[2] { 7, 14 };
            int maxCount1 = maxCount;
            NumericSequence units = NumericSequence.CalculateUnits(min, max, interval1, intervalOffset1, steps, maxCount1);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Weeks);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    int num3 = (int)doubleR10;
                    DateTime date = dateTime.AddDays(num3);
                    dateTimeSequence.Add(date);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval / 7;
            dateTimeSequence.IntervalOffset = units.IntervalOffset / 7;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateDays(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            DateTime dateTime = new DateTime(range.Minimum.Year, range.Minimum.Month, range.Minimum.Day);
            NumericSequence units = NumericSequence.CalculateUnits((int)Math.Floor(range.Minimum.Subtract(dateTime).TotalDays), (int)Math.Ceiling(range.Maximum.Subtract(dateTime).TotalDays), interval, intervalOffset, new int[4] { 1, 2, 7, 14 }, maxCount);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Days);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    int num = (int)doubleR10;
                    DateTime date = dateTime.AddDays(num);
                    dateTimeSequence.Add(date);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval;
            dateTimeSequence.IntervalOffset = units.IntervalOffset;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateHours(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            DateTime dateTime = new DateTime(range.Minimum.Year, range.Minimum.Month, range.Minimum.Day, 0, 0, 0);
            NumericSequence units = NumericSequence.CalculateUnits((int)Math.Floor(range.Minimum.Subtract(dateTime).TotalHours), (int)Math.Ceiling(range.Maximum.Subtract(dateTime).TotalHours), interval, intervalOffset, new int[6] { 1, 2, 3, 6, 12, 24 }, maxCount);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Hours);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    int num = (int)doubleR10;
                    DateTime date = dateTime.AddHours(num);
                    dateTimeSequence.Add(date);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval;
            dateTimeSequence.IntervalOffset = units.IntervalOffset;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateMinutes(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            DateTime dateTime = new DateTime(range.Minimum.Year, range.Minimum.Month, range.Minimum.Day, range.Minimum.Hour, 0, 0);
            NumericSequence units = NumericSequence.CalculateUnits((int)Math.Floor(range.Minimum.Subtract(dateTime).TotalMinutes), (int)Math.Ceiling(range.Maximum.Subtract(dateTime).TotalMinutes), interval, intervalOffset, new int[12] { 1, 2, 5, 10, 15, 30, 60, 120, 180, 360, 720, 1440 }, maxCount);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Minutes);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    double num = (double)doubleR10;
                    dateTimeSequence.Add(dateTime.AddMinutes(num));
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval;
            dateTimeSequence.IntervalOffset = units.IntervalOffset;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateSeconds(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            DateTime dateTime = new DateTime(range.Minimum.Year, range.Minimum.Month, range.Minimum.Day, range.Minimum.Hour, range.Minimum.Minute, 0);
            NumericSequence units = NumericSequence.CalculateUnits((int)Math.Floor(range.Minimum.Subtract(dateTime).TotalSeconds), (int)Math.Ceiling(range.Maximum.Subtract(dateTime).TotalSeconds), interval, intervalOffset, new int[12] { 1, 2, 5, 10, 15, 30, 60, 120, 180, 360, 720, 1440 }, maxCount);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Seconds);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    double num = (double)doubleR10;
                    dateTimeSequence.Add(dateTime.AddSeconds(num));
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval;
            dateTimeSequence.IntervalOffset = units.IntervalOffset;
            return dateTimeSequence;
        }

        public static DateTimeSequence CalculateMilliseconds(Range<DateTime> range, int? interval, int? intervalOffset, int maxCount)
        {
            DateTime dateTime = new DateTime(range.Minimum.Year, range.Minimum.Month, range.Minimum.Day, range.Minimum.Hour, range.Minimum.Minute, range.Minimum.Second);
            NumericSequence units = NumericSequence.CalculateUnits((int)Math.Floor(range.Minimum.Subtract(dateTime).TotalMilliseconds), (int)Math.Ceiling(range.Maximum.Subtract(dateTime).TotalMilliseconds), interval, intervalOffset, new int[10] { 1, 2, 5, 10, 20, 50, 100, 200, 500, 1000 }, maxCount);
            DateTimeSequence dateTimeSequence = new DateTimeSequence(DateTimeScaleUnit.Milliseconds);
            try
            {
                foreach (DoubleR10 doubleR10 in units)
                {
                    double num = (double)doubleR10;
                    dateTimeSequence.Add(dateTime.AddMilliseconds(num));
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
            }
            dateTimeSequence.Interval = units.Interval;
            dateTimeSequence.IntervalOffset = units.IntervalOffset;
            return dateTimeSequence;
        }

        private static DateTime AddInterval(DateTime val, DoubleR10 interval, DateTimeScaleUnit unit)
        {
            switch (unit)
            {
                case DateTimeScaleUnit.Years:
                    return val.AddYears((int)DoubleR10.Round(interval));
                case DateTimeScaleUnit.Months:
                    return val.AddMonths((int)DoubleR10.Round(interval));
                case DateTimeScaleUnit.Weeks:
                    return val.AddDays((double)(7 * interval));
                case DateTimeScaleUnit.Days:
                    return val.AddDays((double)interval);
                case DateTimeScaleUnit.Hours:
                    return val.AddHours((double)interval);
                case DateTimeScaleUnit.Minutes:
                    return val.AddMinutes((double)interval);
                case DateTimeScaleUnit.Seconds:
                    return val.AddSeconds((double)interval);
                case DateTimeScaleUnit.Milliseconds:
                    return val.AddMilliseconds((double)interval);
                default:
                    throw new NotImplementedException();
            }
        }

        internal static DateTimeScaleUnit GetMajorUnit(TimeSpan size, int maxCount)
        {
            maxCount = Math.Max(maxCount, 2);
            if (size.TotalDays > 356.0 && size.TotalDays >= 180 * maxCount)
                return DateTimeScaleUnit.Years;
            if (size.TotalDays > 60.0 && size.TotalDays > 7 * maxCount)
                return DateTimeScaleUnit.Months;
            if (size.TotalDays > 14.0 && size.TotalDays > 2 * maxCount)
                return DateTimeScaleUnit.Weeks;
            if (size.TotalDays > 2.0 && size.TotalHours > 12 * maxCount)
                return DateTimeScaleUnit.Days;
            if (size.TotalHours >= 24.0 && size.TotalHours >= maxCount)
                return DateTimeScaleUnit.Hours;
            if (size.TotalMinutes > 2.0 && size.TotalMinutes >= maxCount)
                return DateTimeScaleUnit.Minutes;
            return size.TotalSeconds > 2.0 && size.TotalSeconds >= 0.8 * maxCount ? DateTimeScaleUnit.Seconds : DateTimeScaleUnit.Milliseconds;
        }

        internal static DateTimeScaleUnit GetMinorUnit(TimeSpan size, int maxCount)
        {
            maxCount = Math.Max(maxCount, 2);
            if (size.TotalDays >= 365 * maxCount)
                return DateTimeScaleUnit.Years;
            if (size.TotalDays >= 30 * maxCount)
                return DateTimeScaleUnit.Months;
            if (size.TotalDays > 7 * maxCount)
                return DateTimeScaleUnit.Weeks;
            if (size.TotalDays >= maxCount)
                return DateTimeScaleUnit.Days;
            if (size.TotalHours >= maxCount)
                return DateTimeScaleUnit.Hours;
            if (size.TotalMinutes >= maxCount)
                return DateTimeScaleUnit.Minutes;
            return size.TotalSeconds >= maxCount ? DateTimeScaleUnit.Seconds : DateTimeScaleUnit.Milliseconds;
        }

        internal static int GetMinorCountPerMajorInterval(DateTimeScaleUnit unit)
        {
            switch (unit)
            {
                case DateTimeScaleUnit.Years:
                    return 5;
                case DateTimeScaleUnit.Months:
                    return 5;
                case DateTimeScaleUnit.Weeks:
                    return 7;
                case DateTimeScaleUnit.Days:
                    return 12;
                case DateTimeScaleUnit.Hours:
                    return 6;
                case DateTimeScaleUnit.Minutes:
                    return 6;
                case DateTimeScaleUnit.Seconds:
                    return 10;
                default:
                    return 10;
            }
        }

        internal static Range<DateTime> CalculateRange(DateTime? viewMin, DateTime? viewMax, DateTime? min, DateTime? max, Range<DateTime>? dataRange, Range<DateTime> defaultRange)
        {
            DateTime minimum = defaultRange.Minimum;
            if (viewMin.HasValue)
                minimum = viewMin.Value;
            else if (min.HasValue)
                minimum = min.Value;
            else if (dataRange.HasValue)
                minimum = dataRange.Value.Minimum;
            DateTime maximum = defaultRange.Maximum;
            if (viewMax.HasValue)
                maximum = viewMax.Value;
            else if (max.HasValue)
                maximum = max.Value;
            else if (dataRange.HasValue)
                maximum = dataRange.Value.Maximum;
            return new Range<DateTime>(minimum, maximum);
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            return this._sequence.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._sequence.GetEnumerator();
        }
    }
}
