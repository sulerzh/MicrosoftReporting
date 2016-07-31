using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class DateTimeScale : Scale<DateTime, int, DateTimeScaleUnit>
    {
        public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register("FirstDayOfWeek", typeof(DayOfWeek), typeof(DateTimeScale), new PropertyMetadata(CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek, new PropertyChangedCallback(DateTimeScale.OnFirstDayOfWeekChanged)));
        private const string FirstDayOfWeekPropertyName = "FirstDayOfWeek";
        private DateTimeSequence _majorSequence;
        private DateTimeSequence _minorSequence;
        private LabelDefinition _yearLabelDefinition;
        private LabelDefinition _monthLabelDefinition;
        private LabelDefinition _weekLabelDefinition;
        private LabelDefinition _dayLabelDefinition;
        private LabelDefinition _hourLabelDefinition;
        private LabelDefinition _minuteLabelDefinition;
        private LabelDefinition _secondLabelDefinition;
        private LabelDefinition _millisecondLabelDefinition;

        [TypeConverter(typeof(NullableConverter<DateTime>))]
        public DateTime? Maximum
        {
            get
            {
                return (DateTime?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MaximumProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MaximumProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<DateTime>))]
        public DateTime? Minimum
        {
            get
            {
                return (DateTime?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinimumProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinimumProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<DateTime>))]
        public DateTime? ViewMaximum
        {
            get
            {
                return (DateTime?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.ViewMaximumProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.ViewMaximumProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<DateTime>))]
        public DateTime? ViewMinimum
        {
            get
            {
                return (DateTime?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.ViewMinimumProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.ViewMinimumProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<int>))]
        public int? MajorInterval
        {
            get
            {
                return (int?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MajorIntervalProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MajorIntervalProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<int>))]
        public int? MajorIntervalOffset
        {
            get
            {
                return (int?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MajorIntervalOffsetProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MajorIntervalOffsetProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<DateTimeScaleUnit>))]
        public DateTimeScaleUnit? MajorIntervalUnit
        {
            get
            {
                return (DateTimeScaleUnit?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MajorIntervalUnitProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MajorIntervalUnitProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<int>))]
        public int? MinorInterval
        {
            get
            {
                return (int?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinorIntervalProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinorIntervalProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<int>))]
        public int? MinorIntervalOffset
        {
            get
            {
                return (int?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinorIntervalOffsetProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinorIntervalOffsetProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<DateTimeScaleUnit>))]
        public DateTimeScaleUnit? MinorIntervalUnit
        {
            get
            {
                return (DateTimeScaleUnit?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinorIntervalUnitProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.MinorIntervalUnitProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<DateTime>))]
        public DateTime? CrossingPosition
        {
            get
            {
                return (DateTime?)this.GetValue(Scale<DateTime, int, DateTimeScaleUnit>.CrossingPositionProperty);
            }
            set
            {
                this.SetValue(Scale<DateTime, int, DateTimeScaleUnit>.CrossingPositionProperty, value);
            }
        }

        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                return (DayOfWeek)this.GetValue(DateTimeScale.FirstDayOfWeekProperty);
            }
            set
            {
                this.SetValue(DateTimeScale.FirstDayOfWeekProperty, value);
            }
        }

        public override int PreferredMaxCount
        {
            get
            {
                return 12;
            }
        }

        internal override object SampleLabelContent
        {
            get
            {
                this.RecalculateIfEmpty();
                return this.GetLabel(this._majorSequence.Unit).SampleContent;
            }
        }

        internal override object DefaultLabelContent
        {
            get
            {
                return new DateTime(2011, 3, 28, 17, 47, 53);
            }
        }

        public LabelDefinition YearLabelDefinition
        {
            get
            {
                return this._yearLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._yearLabelDefinition, value);
            }
        }

        public LabelDefinition MonthLabelDefinition
        {
            get
            {
                return this._monthLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._monthLabelDefinition, value);
            }
        }

        public LabelDefinition WeekLabelDefinition
        {
            get
            {
                return this._weekLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._weekLabelDefinition, value);
            }
        }

        public LabelDefinition DayLabelDefinition
        {
            get
            {
                return this._dayLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._dayLabelDefinition, value);
            }
        }

        public LabelDefinition HourLabelDefinition
        {
            get
            {
                return this._hourLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._hourLabelDefinition, value);
            }
        }

        public LabelDefinition MinuteLabelDefinition
        {
            get
            {
                return this._minuteLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._minuteLabelDefinition, value);
            }
        }

        public LabelDefinition SecondLabelDefinition
        {
            get
            {
                return this._secondLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._secondLabelDefinition, value);
            }
        }

        public LabelDefinition MillisecondLabelDefinition
        {
            get
            {
                return this._millisecondLabelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._millisecondLabelDefinition, value);
            }
        }

        protected override Range<DateTime> DefaultRange
        {
            get
            {
                return new Range<DateTime>(new DateTime(2000, 1, 1), new DateTime(2020, 1, 1));
            }
        }

        internal override int ActualMajorCount
        {
            get
            {
                if (this._majorSequence == null)
                    return 0;
                return this._majorSequence.Count;
            }
        }

        public DateTimeScale()
        {
            this.MaxCount = 10;
            this.YearLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_YearLabelFormat
            };
            this.MonthLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_MonthLabelFormat
            };
            this.WeekLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_WeekLabelFormat
            };
            this.DayLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_DayLabelFormat
            };
            this.HourLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_HourLabelFormat
            };
            this.MinuteLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_MinuteLabelFormat
            };
            this.SecondLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_SecondLabelFormat
            };
            this.MillisecondLabelDefinition = new LabelDefinition()
            {
                Format = Properties.Resources.DateTimeScale_MillisecondLabelFormat
            };
            this.MinorTickmarkDefinition.Visibility = Visibility.Visible;
        }

        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DateTimeScale)d).OnFirstDayOfWeekChanged((DayOfWeek)e.OldValue, (DayOfWeek)e.NewValue);
        }

        protected virtual void OnFirstDayOfWeekChanged(DayOfWeek oldValue, DayOfWeek newValue)
        {
            this.Invalidate();
        }

        public override bool CanProject(DataValueType valueType)
        {
            if (valueType != DataValueType.Date && valueType != DataValueType.DateTime && (valueType != DataValueType.Time && valueType != DataValueType.DateTimeOffset))
                return valueType == DataValueType.Auto;
            return true;
        }

        protected override DateTime ConvertToPositionType(object value)
        {
            return ValueHelper.ToDateTime(value);
        }

        public override void Recalculate()
        {
            Range<DateTime> range = DateTimeSequence.CalculateRange(this.ViewMinimum, this.ViewMaximum, this.Minimum, this.Maximum, RangeHelper.Add<DateTime>(this.ActualDataRange, this.CrossingPosition), this.DefaultRange);
            TimeSpan size = range.Maximum.Subtract(range.Minimum);
            this.ActualMajorIntervalUnit = DateTimeScale.IsNullOrRelative(this.MajorIntervalUnit) ? DateTimeSequence.GetMajorUnit(size, this.MaxCount) : this.MajorIntervalUnit.Value;
            this.ActualMinorIntervalUnit = DateTimeScale.IsNullOrRelative(this.MinorIntervalUnit) ? DateTimeSequence.GetMinorUnit(size, this.MaxCount) : this.MinorIntervalUnit.Value;
            int maxCount1 = this.MaxCount;
            switch (this.ActualMajorIntervalUnit)
            {
                case DateTimeScaleUnit.Years:
                    this._majorSequence = DateTimeSequence.CalculateYears(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1);
                    break;
                case DateTimeScaleUnit.Months:
                    this._majorSequence = DateTimeSequence.CalculateMonths(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1);
                    break;
                case DateTimeScaleUnit.Weeks:
                    this._majorSequence = DateTimeSequence.CalculateWeeks(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1, this.FirstDayOfWeek);
                    break;
                case DateTimeScaleUnit.Days:
                    this._majorSequence = DateTimeSequence.CalculateDays(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1);
                    break;
                case DateTimeScaleUnit.Hours:
                    this._majorSequence = DateTimeSequence.CalculateHours(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1);
                    break;
                case DateTimeScaleUnit.Minutes:
                    this._majorSequence = DateTimeSequence.CalculateMinutes(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1);
                    break;
                case DateTimeScaleUnit.Seconds:
                    this._majorSequence = DateTimeSequence.CalculateSeconds(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1);
                    break;
                case DateTimeScaleUnit.Milliseconds:
                    this._majorSequence = DateTimeSequence.CalculateMilliseconds(range, this.MajorInterval, this.MajorIntervalOffset, maxCount1);
                    break;
                default:
                    throw new NotImplementedException();
            }
            int maxCount2 = this._majorSequence.Count * DateTimeSequence.GetMinorCountPerMajorInterval(this.ActualMajorIntervalUnit);
            switch (this.ActualMinorIntervalUnit)
            {
                case DateTimeScaleUnit.Years:
                    this._minorSequence = DateTimeSequence.CalculateYears(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2);
                    break;
                case DateTimeScaleUnit.Months:
                    this._minorSequence = DateTimeSequence.CalculateMonths(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2);
                    break;
                case DateTimeScaleUnit.Weeks:
                    this._minorSequence = DateTimeSequence.CalculateWeeks(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2, this.FirstDayOfWeek);
                    break;
                case DateTimeScaleUnit.Days:
                    this._minorSequence = DateTimeSequence.CalculateDays(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2);
                    break;
                case DateTimeScaleUnit.Hours:
                    this._minorSequence = DateTimeSequence.CalculateHours(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2);
                    break;
                case DateTimeScaleUnit.Minutes:
                    this._minorSequence = DateTimeSequence.CalculateMinutes(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2);
                    break;
                case DateTimeScaleUnit.Seconds:
                    this._minorSequence = DateTimeSequence.CalculateSeconds(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2);
                    break;
                case DateTimeScaleUnit.Milliseconds:
                    this._minorSequence = DateTimeSequence.CalculateMilliseconds(range, this.MinorInterval, this.MinorIntervalOffset, maxCount2);
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (this.ActualDataRange.HasValue && !this.Minimum.HasValue)
                range = range.ExtendTo(this._minorSequence.Minimum);
            if (this.ActualDataRange.HasValue && !this.Maximum.HasValue)
                range = range.ExtendTo(this._minorSequence.Maximum);
            this.CalculateActual(range);
            this.CalculateSampleContent();
            base.Recalculate();
        }

        private void CalculateActual(Range<DateTime> range)
        {
            this.ActualMajorInterval = (int)this._majorSequence.Interval;
            this.ActualMajorIntervalOffset = (int)this._majorSequence.IntervalOffset;
            this.ActualMinorInterval = (int)this._minorSequence.Interval;
            this.ActualMinorIntervalOffset = (int)this._minorSequence.IntervalOffset;
            if (!this.IsZooming)
            {
                if (this.Minimum.HasValue)
                    this.ActualMinimum = this.Minimum.Value;
                else if (this.ActualDataRange.HasValue && this.ActualDataRange.Value.HasData && this.ActualDataRange.Value.Minimum < range.Minimum)
                    this.ActualMinimum = this.ActualDataRange.Value.Minimum;
                else
                    this.ActualMinimum = range.Minimum;
                if (this.Maximum.HasValue)
                    this.ActualMaximum = this.Maximum.Value;
                else if (this.ActualDataRange.HasValue && this.ActualDataRange.Value.HasData && this.ActualDataRange.Value.Maximum > range.Maximum)
                    this.ActualMaximum = this.ActualDataRange.Value.Maximum;
                else
                    this.ActualMaximum = range.Maximum;
            }
            this.ActualViewMinimum = this.ViewMinimum.HasValue ? this.ViewMinimum.Value : this.ActualMinimum;
            this.ActualViewMaximum = this.ViewMaximum.HasValue ? this.ViewMaximum.Value : this.ActualMaximum;
        }

        private void CalculateSampleContent()
        {
            string str1 = null;
            LabelDefinition label = this.GetLabel(this.ActualMajorIntervalUnit);
            DateTimeSequence dateTimeSequence = this._majorSequence;
            for (int index = 0; index < dateTimeSequence.Count && index < 50; ++index)
            {
                string str2 = label.GetContent(dateTimeSequence[index]) as string;
                if (!string.IsNullOrEmpty(str2) && (str1 == null || str2.Length > str1.Length))
                    str1 = str2;
            }
            if (string.IsNullOrEmpty(str1))
                label.SampleContent = label.GetContent(DateTime.Now);
            else
                label.SampleContent = str1;
        }

        internal override void RecalculateIfEmpty()
        {
            if (this._majorSequence != null && this._minorSequence != null)
                return;
            this.Recalculate();
        }

        public override double Project(DateTime value)
        {
            this.RecalculateIfEmpty();
            return new Range<long>(this.ActualViewMinimum.Ticks, this.ActualViewMaximum.Ticks).Project(value.Ticks, Scale.PercentRange);
        }

        public override double ConvertToPercent(object value)
        {
            if (value == null)
                return double.NaN;
            this.RecalculateIfEmpty();
            Range<long> fromRange = new Range<long>(this.ActualMinimum.Ticks, this.ActualMaximum.Ticks);
            long ticks = this.ConvertToPositionType(value).Ticks;
            if (ticks < fromRange.Minimum || ticks > fromRange.Maximum)
                return double.NaN;
            return fromRange.Project(ticks, Scale.PercentRange);
        }

        public override IEnumerable<double> ProjectValues(IEnumerable values)
        {
            this.RecalculateIfEmpty();
            if (values != null)
            {
                Range<double> fromRange = new Range<double>(this.ActualViewMinimum.Ticks, this.ActualViewMaximum.Ticks);
                foreach (object obj in values)
                {
                    DateTime time = this.ConvertToPositionType(obj);
                    double x = time.Ticks;
                    yield return fromRange.Project(x, Scale.PercentRange);
                }
            }
        }

        private IEnumerable<ScalePosition> Project(ScaleElementDefinition element, DateTimeSequence sequence)
        {
            this.RecalculateIfEmpty();
            foreach (DateTime dateTime in sequence)
                yield return new ScalePosition(dateTime, this.Project(dateTime));
        }

        public override IEnumerable<ScaleElementDefinition> ProjectElements()
        {
            this.MinorTickmarkDefinition.Positions = this.Project(this.MinorTickmarkDefinition, this._minorSequence);
            this.MajorTickmarkDefinition.Positions = this.Project(this.MajorTickmarkDefinition, this._majorSequence);
            yield return this.MinorTickmarkDefinition;
            yield return this.MajorTickmarkDefinition;
            ScaleElementDefinition majorLabel = this.GetLabel(this._majorSequence.Unit);
            majorLabel.Positions = this.Project(majorLabel, this._majorSequence);
            yield return majorLabel;
        }

        public override IEnumerable<ScalePosition> ProjectMajorIntervals()
        {
            return this.Project(this.MajorTickmarkDefinition, this._majorSequence);
        }

        private LabelDefinition GetLabel(DateTimeScaleUnit unit)
        {
            switch (unit)
            {
                case DateTimeScaleUnit.Years:
                    return this.YearLabelDefinition;
                case DateTimeScaleUnit.Months:
                    return this.MonthLabelDefinition;
                case DateTimeScaleUnit.Weeks:
                    return this.WeekLabelDefinition;
                case DateTimeScaleUnit.Days:
                    return this.DayLabelDefinition;
                case DateTimeScaleUnit.Hours:
                    return this.HourLabelDefinition;
                case DateTimeScaleUnit.Minutes:
                    return this.MinuteLabelDefinition;
                case DateTimeScaleUnit.Seconds:
                    return this.SecondLabelDefinition;
                case DateTimeScaleUnit.Milliseconds:
                    return this.MillisecondLabelDefinition;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void ScrollToValue(DateTime position)
        {
            long ticks1 = this.ActualViewMaximum.Ticks;
            long ticks2 = this.ActualViewMinimum.Ticks;
            long num = ticks1 - ticks2;
            long ticks3 = position.Ticks;
            long min1 = ticks3;
            long max1 = ticks3 + num;
            RangeHelper.BoxRangeInsideAnother(ref min1, ref max1, this.ActualMinimum.Ticks, this.ActualMaximum.Ticks);
            if (min1 == ticks2)
                return;
            DateTime min2 = new DateTime(min1);
            DateTime max2 = new DateTime(max1);
            this._majorSequence.MoveToCover(min2, max2);
            this._minorSequence.MoveToCover(min2, max2);
            this.BeginInit();
            this.ViewMinimum = new DateTime?(this.ActualViewMinimum = min2);
            this.ViewMaximum = new DateTime?(this.ActualViewMaximum = max2);
            this.IsScrolling = true;
            this.EndInit();
        }

        protected override void ResetView()
        {
            this.BeginInit();
            base.ResetView();
            this.ViewMinimum = new DateTime?();
            this.ViewMaximum = new DateTime?();
            this.EndInit();
        }

        public override void ScrollToPercent(double position)
        {
            Range<long> targetRange = new Range<long>(this.ActualMinimum.Ticks, this.ActualMaximum.Ticks);
            this.ScrollToValue(new DateTime(Scale.PercentRange.Project(position, targetRange)));
        }

        public override void ZoomToPercent(double viewMinimum, double viewMaximum)
        {
            double minSize = 1.0 / this.ActualZoomRange.Maximum;
            double maxSize = 1.0 / this.ActualZoomRange.Minimum;
            RangeHelper.BoxRangeInsideAnother(ref viewMinimum, ref viewMaximum, 0.0, 1.0, minSize, maxSize, 0.01);
            Range<long> targetRange = new Range<long>(this.ActualMinimum.Ticks, this.ActualMaximum.Ticks);
            this.ZoomToValue(new DateTime(Scale.PercentRange.Project(viewMinimum, targetRange)), new DateTime(Scale.PercentRange.Project(viewMaximum, targetRange)));
        }

        public override void ZoomBy(double centerValue, double ratio)
        {
            this.BoxZoomRatio(ref ratio, this.ConvertActualViewToPercent());
            Range<long> targetRange = new Range<long>(this.ActualViewMinimum.Ticks, this.ActualViewMaximum.Ticks);
            long num1 = Scale.PercentRange.Project(centerValue, targetRange);
            Decimal num2 = (Decimal)ratio;
            this.ZoomToValue(new DateTime((long)Math.Round(num1 - (num1 - targetRange.Minimum) / num2)), new DateTime((long)Math.Round(num1 + (targetRange.Maximum - num1) / num2)));
        }

        internal override void BoxViewRange(ref DateTime viewMinimum, ref DateTime viewMaximum)
        {
            long ticks1 = viewMinimum.Ticks;
            long ticks2 = viewMaximum.Ticks;
            RangeHelper.BoxRangeInsideAnother(ref ticks1, ref ticks2, this.ActualMinimum.Ticks, this.ActualMaximum.Ticks);
            viewMinimum = new DateTime(ticks1);
            viewMaximum = new DateTime(ticks2);
        }

        private static bool IsNullOrRelative(DateTimeScaleUnit? unit)
        {
            if (unit.HasValue && unit.Value != DateTimeScaleUnit.MajorInterval)
                return unit.Value == DateTimeScaleUnit.MinorInterval;
            return true;
        }

        internal class DateTimeScaleFactory : Scale.ScaleFactory
        {
            public override Scale Create(DataValueType valueType)
            {
                DateTimeScale dateTimeScale = new DateTimeScale();
                dateTimeScale.ValueType = valueType;
                return dateTimeScale;
            }
        }
    }
}
