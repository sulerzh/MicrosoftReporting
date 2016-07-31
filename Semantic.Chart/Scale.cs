using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Globalization;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public abstract class Scale : FrameworkElement, INotifyPropertyChanged
    {
        internal static readonly Range<double> PercentRange = new Range<double>(0.0, 1.0);
        internal static readonly Range<double> DefaultZoomRange = new Range<double>(1.0, 100.0);
        internal static readonly Range<double> MaxZoomRange = new Range<double>(1.0, 1000000000.0);
        public static readonly DependencyProperty ZoomRangeProperty = DependencyProperty.Register("ZoomRange", typeof(Range<double>?), typeof(Scale), new PropertyMetadata(null, new PropertyChangedCallback(Scale.OnZoomRangeChanged)));
        private Range<double> _actualZoomRange = Scale.DefaultZoomRange;
        internal const string DefaultFormatString = "{0}";
        internal const double PercentPrecision = 1E-11;
        internal const double ViewPrecision = 0.01;
        private const string ZoomRangePropertyName = "ZoomRange";
        private int _initCount;
        private int _invalidateCount;
        private int _invalidateViewCount;
        private DataValueType _valueType;
        private ScaleDefaults _defaults;
        private LabelDefinition _labelDefinition;
        private TickmarkDefinition _majorTickmarkDefinition;
        private TickmarkDefinition _minorTickmarkDefinition;
        private GridlineDefinition _majorGridLineDefinition;
        private GridlineDefinition _minorGridLineDefinition;
        private static Dictionary<DataValueType, Scale.ScaleFactory> _factoryRegistry;

        internal bool IsInitializing
        {
            get
            {
                return this._initCount > 0;
            }
        }

        internal bool IsInvalidated
        {
            get
            {
                return this._invalidateCount > 0;
            }
        }

        internal bool IsScrolling { get; set; }

        internal bool IsZooming { get; set; }

        internal DataValueType ValueType
        {
            get
            {
                return this._valueType;
            }
            set
            {
                if (this._valueType == value)
                    return;
                this._valueType = value;
            }
        }

        internal ScaleDefaults Defaults
        {
            get
            {
                return this._defaults;
            }
            set
            {
                this._defaults = value;
            }
        }

        public int MaxCount { get; set; }

        public LabelDefinition LabelDefinition
        {
            get
            {
                return this._labelDefinition;
            }
            set
            {
                this.SetElement<LabelDefinition>(ref this._labelDefinition, value);
            }
        }

        public Range<double>? ZoomRange
        {
            get
            {
                return (Range<double>?)this.GetValue(Scale.ZoomRangeProperty);
            }
            set
            {
                this.SetValue(Scale.ZoomRangeProperty, value);
            }
        }

        public Range<double> ActualZoomRange
        {
            get
            {
                return this._actualZoomRange;
            }
            set
            {
                if (!(this._actualZoomRange != value))
                    return;
                this._actualZoomRange = value;
                this.InvalidateView();
            }
        }

        public abstract double ActualZoom { get; }

        public TickmarkDefinition MajorTickmarkDefinition
        {
            get
            {
                return this._majorTickmarkDefinition;
            }
            set
            {
                this.SetElement<TickmarkDefinition>(ref this._majorTickmarkDefinition, value);
            }
        }

        public TickmarkDefinition MinorTickmarkDefinition
        {
            get
            {
                return this._minorTickmarkDefinition;
            }
            set
            {
                this.SetElement<TickmarkDefinition>(ref this._minorTickmarkDefinition, value);
            }
        }

        public GridlineDefinition MajorGridLineDefinition
        {
            get
            {
                return this._majorGridLineDefinition;
            }
            set
            {
                this.SetElement<GridlineDefinition>(ref this._majorGridLineDefinition, value);
            }
        }

        public GridlineDefinition MinorGridLineDefinition
        {
            get
            {
                return this._minorGridLineDefinition;
            }
            set
            {
                this.SetElement<GridlineDefinition>(ref this._minorGridLineDefinition, value);
            }
        }

        public IList<ScaleElementDefinition> CustomElementDefinitions { get; private set; }

        public object ActualCrossingPosition { get; internal set; }

        public bool HasCustomCrossingPosition { get; internal set; }

        public abstract double ProjectedStartMargin { get; }

        public abstract double ProjectedEndMargin { get; }

        internal virtual object SampleLabelContent
        {
            get
            {
                return this.LabelDefinition.SampleContent;
            }
        }

        internal abstract object DefaultLabelContent { get; }

        public virtual int PreferredMaxCount
        {
            get
            {
                return 10;
            }
        }

        internal abstract int ActualMajorCount { get; }

        public abstract bool IsEmpty { get; }

        private static Dictionary<DataValueType, Scale.ScaleFactory> FactoryRegistry
        {
            get
            {
                if (Scale._factoryRegistry == null)
                {
                    Scale._factoryRegistry = new Dictionary<DataValueType, Scale.ScaleFactory>();
                    Scale.ScaleFactory scaleFactory1 = new CategoryScale.CategoryScaleFactory();
                    Scale.ScaleFactory scaleFactory2 = new NumericScale.NumericScaleFactory();
                    Scale.ScaleFactory scaleFactory3 = new DateTimeScale.DateTimeScaleFactory();
                    Scale._factoryRegistry.Add(DataValueType.Category, scaleFactory1);
                    Scale._factoryRegistry.Add(DataValueType.Date, scaleFactory3);
                    Scale._factoryRegistry.Add(DataValueType.DateTime, scaleFactory3);
                    Scale._factoryRegistry.Add(DataValueType.DateTimeOffset, scaleFactory3);
                    Scale._factoryRegistry.Add(DataValueType.Float, scaleFactory2);
                    Scale._factoryRegistry.Add(DataValueType.Integer, scaleFactory2);
                    Scale._factoryRegistry.Add(DataValueType.Time, scaleFactory3);
                    Scale._factoryRegistry.Add(DataValueType.TimeSpan, scaleFactory3);
                }
                return Scale._factoryRegistry;
            }
        }

        public event EventHandler Updated;

        public event EventHandler ElementChanged;

        public event EventHandler<ScaleViewChangedArgs> ViewChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected Scale()
        {
            ++this._initCount;
            this.IsScrolling = false;
            this.IsZooming = false;
            this.MaxCount = 10;
            LabelDefinition labelDefinition = new LabelDefinition();
            labelDefinition.Level = 0;
            labelDefinition.Group = ScaleElementGroup.Major;
            labelDefinition.Format = "{0}";
            this.LabelDefinition = labelDefinition;
            TickmarkDefinition tickmarkDefinition1 = new TickmarkDefinition();
            tickmarkDefinition1.Level = 0;
            tickmarkDefinition1.Group = ScaleElementGroup.Major;
            this.MajorTickmarkDefinition = tickmarkDefinition1;
            TickmarkDefinition tickmarkDefinition2 = new TickmarkDefinition();
            tickmarkDefinition2.Level = 0;
            tickmarkDefinition2.Group = ScaleElementGroup.Minor;
            tickmarkDefinition2.Visibility = Visibility.Collapsed;
            this.MinorTickmarkDefinition = tickmarkDefinition2;
            --this._initCount;
        }

        private static void OnZoomRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale)d).OnZoomRangeChanged((Range<double>?)e.OldValue, (Range<double>?)e.NewValue);
        }

        protected virtual void OnZoomRangeChanged(Range<double>? oldValue, Range<double>? newValue)
        {
            this.CalculateActualZoomRange();
        }

        internal void SetElement<T>(ref T element, T newValue) where T : ScaleElementDefinition
        {
            if (element == newValue)
                return;
            this.ResetScale(element);
            element = newValue;
            this.SetScale(element);
            this.OnElementChanged(element);
        }

        internal void ResetScale(ScaleElementDefinition element)
        {
            if (element == null)
                return;
            element.Scale = null;
            element.PropertyChanged -= new PropertyChangedEventHandler(this.ScaleElement_PropertyChanged);
        }

        internal void SetScale(ScaleElementDefinition element)
        {
            if (element == null)
                return;
            element.Scale = this;
            element.PropertyChanged += new PropertyChangedEventHandler(this.ScaleElement_PropertyChanged);
        }

        public abstract bool CanProject(DataValueType valueType);

        public abstract double ProjectDataValue(object value);

        public abstract IEnumerable<double> ProjectValues(IEnumerable values);

        public abstract IEnumerable<ScaleElementDefinition> ProjectElements();

        public virtual double ProjectClusterSize(IEnumerable values)
        {
            double val2 = 0.0;
            double num = this.ProjectMajorIntervalSize();
            List<double> list = this.ProjectValues(values).ToList<double>();
            if (list.Count > 1)
            {
                list.Sort();
                val2 = MathHelper.GetMinimumDelta(list);
            }
            if (val2 == 0.0 || val2 > num)
                val2 = num;
            return Math.Min(0.5, val2);
        }

        public virtual double ProjectMajorIntervalSize()
        {
            double num = double.MaxValue;
            foreach (ScalePosition projectMajorInterval in this.ProjectMajorIntervals())
            {
                double position = projectMajorInterval.Position;
                if (num != double.MaxValue)
                    return position - num;
                num = position;
            }
            return 0.0;
        }

        public abstract IEnumerable<ScalePosition> ProjectMajorIntervals();

        public abstract void Recalculate();

        internal abstract void RecalculateIfEmpty();

        public void Invalidate()
        {
            ++this._invalidateCount;
            this.Update();
        }

        internal void InvalidateView()
        {
            ++this._invalidateViewCount;
        }

        protected virtual void ResetView()
        {
            this.IsScrolling = false;
            this.IsZooming = false;
        }

        protected virtual bool NeedsRecalculation()
        {
            return !this.IsScrolling;
        }

        public virtual void Update()
        {
            if (this.IsInitializing || !this.IsInvalidated)
                return;
            ++this._initCount;
            DateTime now = DateTime.Now;
            if (this.NeedsRecalculation())
                this.Recalculate();
            this.OnUpdated();
            if (this._invalidateViewCount > 0)
                this.OnViewChanged();
            string name = this.GetType().Name;
            this._invalidateCount = 0;
            this._invalidateViewCount = 0;
            this._initCount = 0;
        }

        protected virtual void OnUpdated()
        {
            EventHandler eventHandler = this.Updated;
            if (eventHandler == null)
                return;
            eventHandler(this, EventArgs.Empty);
        }

        protected virtual void OnElementChanged(ScaleElementDefinition element)
        {
            EventHandler eventHandler = this.ElementChanged;
            if (eventHandler == null)
                return;
            eventHandler(element, EventArgs.Empty);
        }

        protected virtual void OnViewChanged(Range<IComparable> oldRange, Range<IComparable> newRange)
        {
            EventHandler<ScaleViewChangedArgs> eventHandler = this.ViewChanged;
            if (eventHandler == null)
                return;
            eventHandler(this, new ScaleViewChangedArgs()
            {
                OldRange = oldRange,
                NewRange = newRange
            });
        }

        protected abstract void OnViewChanged();

        public abstract void UpdateRange(IEnumerable<Range<IComparable>> ranges);

        internal abstract void UpdateRangeIfUndefined(IEnumerable<Range<IComparable>> ranges);

        internal virtual void UpdateValuesIfUndefined(IEnumerable<object> values)
        {
        }

        public virtual void UpdateValueType(DataValueType valueType)
        {
            if (!this.CanProject(valueType))
                throw new ArgumentException(Properties.Resources.Scale_DataValueTypeOutOfRange);
            if (this.ValueType == valueType)
                return;
            this.ValueType = valueType;
            this.Invalidate();
        }

        public void UpdateDefaults(ScaleDefaults defaults)
        {
            if (this.Defaults.Equals(defaults))
                return;
            this.Defaults = defaults;
            this.Invalidate();
        }

        public virtual bool TryChangeInterval(double ratio)
        {
            int actualMajorCount1 = this.ActualMajorCount;
            int num = Math.Max((int)Math.Round(this.MaxCount / ratio), 1);
            if (num == this.MaxCount)
                return false;
            this.MaxCount = num;
            this.Recalculate();
            int actualMajorCount2 = this.ActualMajorCount;
            if (actualMajorCount1 != 0 && actualMajorCount2 != actualMajorCount1)
                this.Invalidate();
            return true;
        }

        public abstract double ConvertToPercent(object value);

        public abstract Range<double> ConvertActualViewToPercent();

        public abstract void ScrollToPercent(double position);

        public abstract void ScrollBy(double offset);

        public abstract void ZoomToPercent(double viewMinimum, double viewMaximum);

        public void ZoomToPercent(double viewSize)
        {
            Range<double> percent = this.ConvertActualViewToPercent();
            double num = Math.Min(viewSize, 1.0);
            double viewMinimum = Math.Min(percent.Minimum, 1.0 - viewSize);
            double viewMaximum = viewMinimum + num;
            this.ZoomToPercent(viewMinimum, viewMaximum);
        }

        public abstract void ZoomBy(double centerValue, double delta);

        internal virtual void CalculateActualZoomRange()
        {
            Range<double> range = this.ZoomRange.HasValue ? this.ZoomRange.Value : Scale.DefaultZoomRange;
            double minimum = range.Minimum;
            double maximum = range.Maximum;
            double minPossibleZoom = this.GetMinPossibleZoom();
            double maxPossibleZoom = this.GetMaxPossibleZoom();
            RangeHelper.BoxRangeInsideAnother(ref minimum, ref maximum, minPossibleZoom, maxPossibleZoom);
            this.ActualZoomRange = new Range<double>(minimum, maximum);
        }

        protected virtual double GetMinPossibleZoom()
        {
            return 1.0;
        }

        protected virtual double GetMaxPossibleZoom()
        {
            return 1000000000.0;
        }

        public virtual bool TryChangeMaxCount(double maxMajorCount)
        {
            return this.TryChangeInterval(this.MaxCount / maxMajorCount);
        }

        private void ScaleElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnElementChanged(sender as ScaleElementDefinition);
        }

        public static Scale CreateScaleByType(DataValueType valueType)
        {
            return Scale.FactoryRegistry[valueType].Create(valueType);
        }

        public new void BeginInit()
        {
            ++this._initCount;
        }

        public new void EndInit()
        {
            if (this._initCount <= 0)
                return;
            --this._initCount;
            if (this._initCount != 0)
                return;
            this.Update();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        internal abstract class ScaleFactory
        {
            public abstract Scale Create(DataValueType valueType);
        }
    }

    public abstract class Scale<TPosition, TInterval, TScaleUnit> : Scale where TPosition : struct, IComparable, IComparable<TPosition> where TInterval : struct where TScaleUnit : struct
    {
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(TPosition?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMaximumPropertyChanged)));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(TPosition?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMinimumPropertyChanged)));
        public static readonly DependencyProperty ViewMaximumProperty = DependencyProperty.Register("ViewMaximum", typeof(TPosition?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnViewMaximumPropertyChanged)));
        public static readonly DependencyProperty ViewMinimumProperty = DependencyProperty.Register("ViewMinimum", typeof(TPosition?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnViewMinimumPropertyChanged)));
        public static readonly DependencyProperty MajorIntervalProperty = DependencyProperty.Register("MajorInterval", typeof(TInterval?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMajorIntervalPropertyChanged)));
        public static readonly DependencyProperty MajorIntervalUnitProperty = DependencyProperty.Register("MajorIntervalUnit", typeof(TScaleUnit?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMajorIntervalUnitPropertyChanged)));
        public static readonly DependencyProperty MajorIntervalOffsetProperty = DependencyProperty.Register("MajorIntervalOffset", typeof(TInterval?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMajorIntervalOffsetPropertyChanged)));
        public static readonly DependencyProperty MinorIntervalProperty = DependencyProperty.Register("MinorInterval", typeof(TInterval?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMinorIntervalPropertyChanged)));
        public static readonly DependencyProperty MinorIntervalUnitProperty = DependencyProperty.Register("MinorIntervalUnit", typeof(TScaleUnit?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMinorIntervalUnitPropertyChanged)));
        public static readonly DependencyProperty MinorIntervalOffsetProperty = DependencyProperty.Register("MinorIntervalOffset", typeof(TInterval?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMinorIntervalOffsetPropertyChanged)));
        public static readonly DependencyProperty CrossingPositionProperty = DependencyProperty.Register("CrossingPosition", typeof(TPosition?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnCrossingPositionPropertyChanged)));
        public static readonly DependencyProperty CrossingPositionModeProperty = DependencyProperty.Register("CrossingPositionMode", typeof(AxisCrossingPositionMode), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(AxisCrossingPositionMode.Auto, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnCrossingPositionModePropertyChanged)));
        public static readonly DependencyProperty DataRangeProperty = DependencyProperty.Register("DataRange", typeof(Range<TPosition>?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnDataRangeChanged)));
        public static readonly DependencyProperty IntervalAttachedProperty = DependencyProperty.RegisterAttached("Interval", typeof(TInterval?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnIntervalAttachedPropertyChanged)));
        public static readonly DependencyProperty IntervalOffsetAttachedProperty = DependencyProperty.RegisterAttached("IntervalOffset", typeof(TInterval?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnIntervalOffsetAttachedPropertyChanged)));
        public static readonly DependencyProperty IntervalUnitAttachedProperty = DependencyProperty.RegisterAttached("IntervalUnit", typeof(TScaleUnit?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnIntervalUnitAttachedPropertyChanged)));
        public static readonly DependencyProperty MaxCountAttachedProperty = DependencyProperty.RegisterAttached("MaxCount", typeof(int?), typeof(Scale<TPosition, int, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnMaxCountAttachedPropertyChanged)));
        public static readonly DependencyProperty ActualMinimumProperty = DependencyProperty.Register("ActualMinimum", typeof(TPosition), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnActualMinimumPropertyChanged)));
        public static readonly DependencyProperty ActualMaximumProperty = DependencyProperty.Register("ActualMaximum", typeof(TPosition), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnActualMaximumPropertyChanged)));
        public static readonly DependencyProperty ActualViewMinimumProperty = DependencyProperty.Register("ActualViewMinimum", typeof(TPosition), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnActualViewMinimumPropertyChanged)));
        public static readonly DependencyProperty ActualViewMaximumProperty = DependencyProperty.Register("ActualViewMaximum", typeof(TPosition), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnActualViewMaximumPropertyChanged)));
        public static readonly DependencyProperty ActualMajorIntervalProperty = DependencyProperty.Register("ActualMajorInterval", typeof(TInterval), typeof(Scale<TPosition, TInterval, TScaleUnit>), null);
        public static readonly DependencyProperty ActualMajorIntervalUnitProperty = DependencyProperty.Register("ActualMajorIntervalUnit", typeof(TScaleUnit), typeof(Scale<TPosition, TInterval, TScaleUnit>), null);
        public static readonly DependencyProperty ActualMajorIntervalOffsetProperty = DependencyProperty.Register("ActualMajorIntervalOffset", typeof(TInterval), typeof(Scale<TPosition, TInterval, TScaleUnit>), null);
        public static readonly DependencyProperty ActualMinorIntervalProperty = DependencyProperty.Register("ActualMinorInterval", typeof(TInterval), typeof(Scale<TPosition, TInterval, TScaleUnit>), null);
        public static readonly DependencyProperty ActualMinorIntervalUnitProperty = DependencyProperty.Register("ActualMinorIntervalUnit", typeof(TScaleUnit), typeof(Scale<TPosition, TInterval, TScaleUnit>), null);
        public static readonly DependencyProperty ActualMinorIntervalOffsetProperty = DependencyProperty.Register("ActualMinorIntervalOffset", typeof(TInterval), typeof(Scale<TPosition, TInterval, TScaleUnit>), null);
        public static readonly DependencyProperty ActualDataRangeProperty = DependencyProperty.Register("ActualDataRange", typeof(Range<TPosition>?), typeof(Scale<TPosition, TInterval, TScaleUnit>), new PropertyMetadata(null, new PropertyChangedCallback(Scale<TPosition, TInterval, TScaleUnit>.OnActualDataRangeChanged)));
        private TPosition? _previousViewMinimum;
        private TPosition? _previousViewMaximum;

        public AxisCrossingPositionMode CrossingPositionMode
        {
            get
            {
                return (AxisCrossingPositionMode)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.CrossingPositionModeProperty);
            }
            set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.CrossingPositionModeProperty, value);
            }
        }

        public Range<TPosition>? DataRange
        {
            get
            {
                return (Range<TPosition>?)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.DataRangeProperty);
            }
            set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.DataRangeProperty, value);
            }
        }

        public TPosition ActualMinimum
        {
            get
            {
                return (TPosition)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinimumProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinimumProperty, value);
            }
        }

        public TPosition ActualMaximum
        {
            get
            {
                return (TPosition)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMaximumProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMaximumProperty, value);
            }
        }

        public TPosition ActualViewMinimum
        {
            get
            {
                return (TPosition)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualViewMinimumProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualViewMinimumProperty, value);
            }
        }

        public TPosition ActualViewMaximum
        {
            get
            {
                return (TPosition)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualViewMaximumProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualViewMaximumProperty, value);
            }
        }

        public TInterval ActualMajorInterval
        {
            get
            {
                return (TInterval)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMajorIntervalProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMajorIntervalProperty, value);
            }
        }

        public TScaleUnit ActualMajorIntervalUnit
        {
            get
            {
                return (TScaleUnit)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMajorIntervalUnitProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMajorIntervalUnitProperty, value);
            }
        }

        public TInterval ActualMajorIntervalOffset
        {
            get
            {
                return (TInterval)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMajorIntervalOffsetProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMajorIntervalOffsetProperty, value);
            }
        }

        public TInterval ActualMinorInterval
        {
            get
            {
                return (TInterval)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinorIntervalProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinorIntervalProperty, value);
            }
        }

        public TScaleUnit ActualMinorIntervalUnit
        {
            get
            {
                return (TScaleUnit)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinorIntervalUnitProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinorIntervalUnitProperty, value);
            }
        }

        public TInterval ActualMinorIntervalOffset
        {
            get
            {
                return (TInterval)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinorIntervalOffsetProperty);
            }
            protected set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualMinorIntervalOffsetProperty, value);
            }
        }

        public Range<TPosition>? ActualDataRange
        {
            get
            {
                return (Range<TPosition>?)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualDataRangeProperty);
            }
            set
            {
                this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ActualDataRangeProperty, value);
            }
        }

        protected abstract Range<TPosition> DefaultRange { get; }

        public override double ProjectedStartMargin
        {
            get
            {
                if (this.ActualDataRange.HasValue && this.ActualDataRange.Value.HasData && this.ActualDataRange.Value.Minimum.CompareTo(this.ActualViewMinimum) > 0)
                    return this.Project(this.ActualDataRange.Value.Minimum);
                return 0.0;
            }
        }

        public override double ProjectedEndMargin
        {
            get
            {
                if (this.ActualDataRange.HasValue && this.ActualDataRange.Value.HasData && this.ActualDataRange.Value.Maximum.CompareTo(this.ActualViewMaximum) < 0)
                    return 1.0 - this.Project(this.ActualDataRange.Value.Maximum);
                return 0.0;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return this.ActualMaximum.CompareTo(this.ActualMinimum) <= 0;
            }
        }

        public override double ActualZoom
        {
            get
            {
                return 1.0 / this.ConvertActualViewToPercent().Size();
            }
        }

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMaximumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnMaximumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.IsScrolling = false;
            this.IsZooming = false;
            this.Invalidate();
        }

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMinimumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnMinimumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.IsScrolling = false;
            this.IsZooming = false;
            this.Invalidate();
        }

        private static void OnViewMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnViewMaximumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnViewMaximumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnViewMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnViewMinimumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnViewMinimumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnMajorIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMajorIntervalPropertyChanged((TInterval?)e.NewValue, (TInterval?)e.OldValue);
        }

        protected virtual void OnMajorIntervalPropertyChanged(TInterval? newValue, TInterval? oldValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnMajorIntervalUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMajorIntervalUnitPropertyChanged((TScaleUnit?)e.NewValue, (TScaleUnit?)e.OldValue);
        }

        protected virtual void OnMajorIntervalUnitPropertyChanged(TScaleUnit? newValue, TScaleUnit? oldValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnMajorIntervalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMajorIntervalOffsetPropertyChanged((TInterval?)e.NewValue, (TInterval?)e.OldValue);
        }

        protected virtual void OnMajorIntervalOffsetPropertyChanged(TInterval? newValue, TInterval? oldValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnMinorIntervalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMinorIntervalPropertyChanged((TInterval?)e.NewValue, (TInterval?)e.OldValue);
        }

        protected virtual void OnMinorIntervalPropertyChanged(TInterval? newValue, TInterval? oldValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnMinorIntervalUnitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMinorIntervalUnitPropertyChanged((TScaleUnit?)e.NewValue, (TScaleUnit?)e.OldValue);
        }

        protected virtual void OnMinorIntervalUnitPropertyChanged(TScaleUnit? newValue, TScaleUnit? oldValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnMinorIntervalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnMinorIntervalOffsetPropertyChanged((TInterval?)e.NewValue, (TInterval?)e.OldValue);
        }

        protected virtual void OnMinorIntervalOffsetPropertyChanged(TInterval? newValue, TInterval? oldValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnCrossingPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnCrossingPositionPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnCrossingPositionPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnCrossingPositionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnCrossingPositionModePropertyChanged((AxisCrossingPositionMode)e.OldValue, (AxisCrossingPositionMode)e.NewValue);
        }

        protected virtual void OnCrossingPositionModePropertyChanged(AxisCrossingPositionMode oldValue, AxisCrossingPositionMode newValue)
        {
            this.IsScrolling = false;
            this.Invalidate();
        }

        private static void OnDataRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range<TPosition>? oldValue = (Range<TPosition>?)e.OldValue;
            Range<TPosition>? newValue = (Range<TPosition>?)e.NewValue;
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnDataRangeChanged(oldValue, newValue);
        }

        protected virtual void OnDataRangeChanged(Range<TPosition>? oldValue, Range<TPosition>? newValue)
        {
            this.ActualDataRange = newValue;
        }

        public static void SetInterval(ScaleElementDefinition element, TInterval? value)
        {
            element.SetValue(Scale<TPosition, TInterval, TScaleUnit>.IntervalAttachedProperty, value);
        }

        public static TInterval? GetInterval(ScaleElementDefinition element)
        {
            return (TInterval?)element.GetValue(Scale<TPosition, TInterval, TScaleUnit>.IntervalAttachedProperty);
        }

        private static void OnIntervalAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleElementDefinition element = d as ScaleElementDefinition;
            Scale<TPosition, TInterval, TScaleUnit> scale = element.Scale as Scale<TPosition, TInterval, TScaleUnit>;
            TInterval? newValue = (TInterval?)e.NewValue;
            TInterval? oldValue = (TInterval?)e.OldValue;
            scale.OnIntervalAttachedPropertyChanged(element, newValue, oldValue);
        }

        protected virtual void OnIntervalAttachedPropertyChanged(ScaleElementDefinition element, TInterval? newValue, TInterval? oldValue)
        {
            this.OnElementChanged(element);
        }

        public static void SetIntervalOffset(ScaleElementDefinition element, TInterval? value)
        {
            element.SetValue(Scale<TPosition, TInterval, TScaleUnit>.IntervalOffsetAttachedProperty, value);
        }

        public static TInterval? GetIntervalOffset(ScaleElementDefinition element)
        {
            return (TInterval?)element.GetValue(Scale<TPosition, TInterval, TScaleUnit>.IntervalOffsetAttachedProperty);
        }

        private static void OnIntervalOffsetAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleElementDefinition element = d as ScaleElementDefinition;
            Scale<TPosition, TInterval, TScaleUnit> scale = element.Scale as Scale<TPosition, TInterval, TScaleUnit>;
            TInterval? newValue = (TInterval?)e.NewValue;
            TInterval? oldValue = (TInterval?)e.OldValue;
            scale.OnIntervalOffsetAttachedPropertyChanged(element, newValue, oldValue);
        }

        protected virtual void OnIntervalOffsetAttachedPropertyChanged(ScaleElementDefinition element, TInterval? newValue, TInterval? oldValue)
        {
            this.OnElementChanged(element);
        }

        public static void SetIntervalUnit(ScaleElementDefinition element, TPosition? value)
        {
            element.SetValue(Scale<TPosition, TInterval, TScaleUnit>.IntervalUnitAttachedProperty, value);
        }

        public static TScaleUnit? GetIntervalUnit(ScaleElementDefinition element)
        {
            return (TScaleUnit?)element.GetValue(Scale<TPosition, TInterval, TScaleUnit>.IntervalUnitAttachedProperty);
        }

        private static void OnIntervalUnitAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScaleElementDefinition element = d as ScaleElementDefinition;
            Scale<TPosition, TInterval, TScaleUnit> scale = element.Scale as Scale<TPosition, TInterval, TScaleUnit>;
            TScaleUnit? newValue = (TScaleUnit?)e.NewValue;
            TScaleUnit? oldValue = (TScaleUnit?)e.OldValue;
            scale.OnIntervalUnitAttachedPropertyChanged(element, newValue, oldValue);
        }

        protected virtual void OnIntervalUnitAttachedPropertyChanged(ScaleElementDefinition element, TScaleUnit? newValue, TScaleUnit? oldValue)
        {
            this.OnElementChanged(element);
        }

        public static void SetMaxCount(ScaleElementDefinition element, int? value)
        {
            element.SetValue(Scale<TPosition, TInterval, TScaleUnit>.MaxCountAttachedProperty, value);
        }

        public static int? GetMaxCount(ScaleElementDefinition element)
        {
            return (int?)element.GetValue(Scale<TPosition, TInterval, TScaleUnit>.MaxCountAttachedProperty);
        }

        private static void OnMaxCountAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((d as ScaleElementDefinition).Scale as Scale<TPosition, int, TScaleUnit>).OnMaxCountAttachedPropertyChanged((int?)e.NewValue, (int?)e.OldValue);
        }

        protected virtual void OnMaxCountAttachedPropertyChanged(int? newValue, int? oldValue)
        {
            this.Invalidate();
        }

        private static void OnActualMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnActualMinimumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnActualMinimumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.Invalidate();
        }

        private static void OnActualMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnActualMaximumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnActualMaximumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.Invalidate();
        }

        private static void OnActualViewMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnActualViewMinimumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnActualViewMinimumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.InvalidateView();
        }

        private static void OnActualViewMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnActualViewMaximumPropertyChanged((TPosition?)e.OldValue, (TPosition?)e.NewValue);
        }

        protected virtual void OnActualViewMaximumPropertyChanged(TPosition? oldValue, TPosition? newValue)
        {
            this.InvalidateView();
        }

        private static void OnActualDataRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Range<TPosition>? oldValue = (Range<TPosition>?)e.OldValue;
            Range<TPosition>? newValue = (Range<TPosition>?)e.NewValue;
            ((Scale<TPosition, TInterval, TScaleUnit>)d).OnActualDataRangeChanged(oldValue, newValue);
        }

        protected virtual void OnActualDataRangeChanged(Range<TPosition>? oldValue, Range<TPosition>? newValue)
        {
            if (this.DataRange.HasValue)
            {
                Range<TPosition>? nullable = newValue;
                Range<TPosition>? dataRange = this.DataRange;
                if ((nullable.HasValue != dataRange.HasValue ? 1 : (!nullable.HasValue ? 0 : (nullable.GetValueOrDefault() != dataRange.GetValueOrDefault() ? 1 : 0))) != 0)
                {
                    this.ActualDataRange = this.DataRange;
                    return;
                }
            }
            this.ResetView();
            this.Invalidate();
        }

        private void CalculateActualCrossingPosition()
        {
            IComparable comparable = (IComparable)this.GetValue(Scale<TPosition, TInterval, TScaleUnit>.CrossingPositionProperty);
            this.HasCustomCrossingPosition = false;
            if (comparable != null)
            {
                if (comparable.CompareTo(this.ActualMinimum) < 0)
                    this.ActualCrossingPosition = this.ActualMinimum;
                else if (comparable.CompareTo(this.ActualMaximum) > 0)
                {
                    this.ActualCrossingPosition = this.ActualMaximum;
                }
                else
                {
                    this.ActualCrossingPosition = comparable;
                    this.HasCustomCrossingPosition = true;
                }
            }
            else
            {
                switch (this.CrossingPositionMode)
                {
                    case AxisCrossingPositionMode.Minimum:
                        this.ActualCrossingPosition = this.ActualMinimum;
                        break;
                    case AxisCrossingPositionMode.Maximum:
                        this.ActualCrossingPosition = this.ActualMaximum;
                        break;
                    default:
                        this.ActualCrossingPosition = this.GetAutomaticCrossing();
                        break;
                }
            }
        }

        internal virtual object GetAutomaticCrossing()
        {
            return this.ActualMinimum;
        }

        public override void Recalculate()
        {
            this.CalculateActualCrossingPosition();
            this.CalculateActualZoomRange();
        }

        protected abstract TPosition ConvertToPositionType(object value);

        public abstract double Project(TPosition value);

        public override double ProjectDataValue(object value)
        {
            if (value == null)
                return double.NaN;
            return this.Project(this.ConvertToPositionType(value));
        }

        public override void UpdateRange(IEnumerable<Range<IComparable>> ranges)
        {
            if (ranges != null)
            {
                Range<TPosition> range = this.GetRange(ranges);
                this.DataRange = new Range<TPosition>?(range.HasData ? range : this.DefaultRange);
            }
            else
                this.DataRange = new Range<TPosition>?();
        }

        internal override void UpdateRangeIfUndefined(IEnumerable<Range<IComparable>> ranges)
        {
            if (this.DataRange.HasValue)
                return;
            Range<TPosition> range = this.GetRange(ranges);
            this.ActualDataRange = range.HasData ? new Range<TPosition>?(range) : new Range<TPosition>?();
        }

        private Range<TPosition> GetRange(IEnumerable<Range<IComparable>> ranges)
        {
            Range<TPosition> range1 = new Range<TPosition>();
            foreach (Range<IComparable> range2 in ranges)
            {
                if (range2.HasData)
                {
                    Range<TPosition> range3 = new Range<TPosition>(this.ConvertToPositionType(range2.Minimum), this.ConvertToPositionType(range2.Maximum));
                    range1 = range1.Add(range3);
                }
            }
            return range1;
        }

        internal virtual double ConvertProjectedValueToPercent(double value)
        {
            return new Range<double>(this.Project(this.ActualMinimum), this.Project(this.ActualMaximum)).Project(value, Scale.PercentRange);
        }

        public abstract void ScrollToValue(TPosition position);

        public override void ScrollBy(double offset)
        {
            this.ScrollToPercent(this.ConvertProjectedValueToPercent(offset));
        }

        public virtual void ZoomToValue(TPosition viewMinimum, TPosition viewMaximum)
        {
            if (viewMinimum.CompareTo(viewMaximum) == 0)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, Properties.Resources.Scale_ViewRangeIsEmpty, new object[2] { (object)viewMinimum, (object)viewMaximum }));
            if (viewMinimum.CompareTo(viewMaximum) > 0)
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, Properties.Resources.Scale_ViewRangeIsReverse, new object[2] { (object)viewMinimum, (object)viewMaximum }));
            this.BoxViewRange(ref viewMinimum, ref viewMaximum);
            this.IsZooming = true;
            this.BeginInit();
            this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ViewMinimumProperty, viewMinimum);
            this.SetValue(Scale<TPosition, TInterval, TScaleUnit>.ViewMaximumProperty, viewMaximum);
            this.EndInit();
        }

        public override void ZoomBy(double centerValue, double ratio)
        {
            double percent1 = this.ConvertProjectedValueToPercent(centerValue);
            Range<double> percent2 = this.ConvertActualViewToPercent();
            this.BoxZoomRatio(ref ratio, percent2);
            if (ratio.EqualsWithPrecision(1.0, 0.001))
                return;
            this.ZoomToPercent(percent1 - (percent1 - percent2.Minimum) / ratio, percent1 + (percent2.Maximum - percent1) / ratio);
        }

        internal void BoxZoomRatio(ref double ratio, Range<double> view)
        {
            double num1 = 1.0 / view.Size();
            double num2 = num1 * ratio;
            if (num2 < this.ActualZoomRange.Minimum)
            {
                double minimum = this.ActualZoomRange.Minimum;
                ratio = Math.Min(minimum / num1, 1.0);
            }
            else
            {
                if (num2 <= this.ActualZoomRange.Maximum)
                    return;
                double maximum = this.ActualZoomRange.Maximum;
                ratio = Math.Max(maximum / num1, 1.0);
            }
        }

        internal abstract void BoxViewRange(ref TPosition viewMinimum, ref TPosition viewMaximum);

        public override Range<double> ConvertActualViewToPercent()
        {
            return new Range<double>(this.ConvertToPercent(this.ActualViewMinimum), this.ConvertToPercent(this.ActualViewMaximum));
        }

        internal Range<double> ProjectActualRange()
        {
            return new Range<double>(this.Project(this.ActualMinimum), this.Project(this.ActualMaximum));
        }

        protected override void OnViewChanged()
        {
            Range<IComparable> newRange = new Range<IComparable>(this.ActualViewMinimum, this.ActualViewMaximum);
            Range<IComparable> oldRange = !this._previousViewMaximum.HasValue ? new Range<IComparable>(this.DefaultRange.Minimum, this.DefaultRange.Maximum) : new Range<IComparable>(this._previousViewMinimum, this._previousViewMaximum);
            this._previousViewMinimum = new TPosition?(this.ActualViewMinimum);
            this._previousViewMaximum = new TPosition?(this.ActualViewMaximum);
            this.OnViewChanged(oldRange, newRange);
        }
    }
}
