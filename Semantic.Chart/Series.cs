using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "EmptyDataPointStyle", StyleTargetType = typeof(DataPoint))]
    [StyleTypedProperty(Property = "DataPointStyle", StyleTargetType = typeof(DataPoint))]
    public abstract class Series : FrameworkElement, INotifyPropertyChanged, IUpdatable
    {
        public static readonly DependencyProperty DataPointStyleProperty = DependencyProperty.Register("DataPointStyle", typeof(Style), typeof(Series), new PropertyMetadata(null, new PropertyChangedCallback(Series.OnDataPointStyleChanged)));
        public static readonly DependencyProperty EmptyDataPointStyleProperty = DependencyProperty.Register("EmptyDataPointStyle", typeof(Style), typeof(Series), new PropertyMetadata(null, new PropertyChangedCallback(Series.OnDataPointStyleChanged)));
        public static readonly DependencyProperty IsDimmedProperty = DependencyProperty.Register("IsDimmed", typeof(bool), typeof(Series), new PropertyMetadata(false, new PropertyChangedCallback(Series.OnIsDimmedPropertyChanged)));
        public static readonly DependencyProperty IsLegendDimmedProperty = DependencyProperty.Register("IsLegendDimmed", typeof(bool), typeof(Series), new PropertyMetadata(false, new PropertyChangedCallback(Series.OnIsLegendDimmedPropertyChanged)));
        public static readonly DependencyProperty UnselectedDataPointOpacityProperty = DependencyProperty.Register("UnselectedDataPointOpacity", typeof(double), typeof(Series), new PropertyMetadata(0.4, new PropertyChangedCallback(Series.OnUnselectedDataPointOpacityPropertyChanged)));
        public static readonly DependencyProperty UnselectedDataPointEffectProperty = DependencyProperty.Register("UnselectedDataPointEffect", typeof(Effect), typeof(Series), new PropertyMetadata(new GloomEffect() { BaseSaturation = 0.3 }, new PropertyChangedCallback(Series.OnUnselectedDataPointEffectPropertyChanged)));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(Series), new PropertyMetadata(null, new PropertyChangedCallback(Series.OnTitleChanged)));
        public static readonly DependencyProperty PaletteKeyProperty = DependencyProperty.Register("PaletteKey", typeof(object), typeof(Series), new PropertyMetadata(new PropertyChangedCallback(Series.OnPaletteKeyChanged)));
        public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register("TransitionDuration", typeof(TimeSpan?), typeof(Series), new PropertyMetadata(null));
        public static readonly DependencyProperty TransitionEasingFunctionProperty = DependencyProperty.Register("TransitionEasingFunction", typeof(NullableObject<IEasingFunction>), typeof(Series), new PropertyMetadata(new NullableObject<IEasingFunction>()));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(Series), new PropertyMetadata(null, new PropertyChangedCallback(Series.OnItemsSourceChanged)));
        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(Series), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(Series.OnLabelVisibilityChanged)));
        public static readonly DependencyProperty LegendTextProperty = DependencyProperty.Register("LegendText", typeof(string), typeof(Series), new PropertyMetadata(string.Empty, new PropertyChangedCallback(Series.OnLegendTextChanged)));
        public static readonly DependencyProperty OpacityListenerProperty = DependencyProperty.RegisterAttached("OpacityListener", typeof(object), typeof(FrameworkElement), new PropertyMetadata(new PropertyChangedCallback(Series.OnOpacityChanged)));
        public static readonly DependencyProperty EffectListenerProperty = DependencyProperty.RegisterAttached("EffectListener", typeof(object), typeof(FrameworkElement), new PropertyMetadata(new PropertyChangedCallback(Series.OnEffectChanged)));
        public static readonly DependencyProperty VisibilityListenerProperty = DependencyProperty.RegisterAttached("VisibilityListener", typeof(object), typeof(FrameworkElement), new PropertyMetadata(new PropertyChangedCallback(Series.OnVisibilityChanged)));
        public static readonly DependencyProperty HaveSelectedDataPointsProperty = DependencyProperty.Register("HaveSelectedDataPoints", typeof(bool), typeof(Series), new PropertyMetadata(false, new PropertyChangedCallback(Series.OnHaveSelectedDataPointsPropertyChanged)));
        internal static readonly DependencyProperty IsLegendSelectedProperty = DependencyProperty.Register("IsLegendSelected", typeof(bool), typeof(Series), null);
        private ObservableCollectionListSynchronizer<DataPoint> _itemsSourceCollectionAdapter = new ObservableCollectionListSynchronizer<DataPoint>();
        private DisplayUnitSystem _labelDisplayUnitSystem = new DefaultDisplayUnitSystem();
        private List<DataPoint> _dataPointsToRemove = new List<DataPoint>();
        private List<DataPoint> _dataPointsToAdd = new List<DataPoint>();
        internal const string DataPointCollectionChangeActionID = "__OnDataPointsCollectionChanged__";
        internal const string IsTraceSeriesPropertyName = "IsTraceSeries";
        internal const string DataPointStylePropertyName = "DataPointStyle";
        internal const string EmptyDataPointStylePropertyName = "EmptyDataPointStyle";
        internal const string IsDimmedPropertyName = "IsDimmed";
        internal const string IsLegendDimmedPropertyName = "IsLegendDimmed";
        internal const string UnselectedDataPointOpacityPropertyName = "UnselectedDataPointOpacity";
        internal const string UnselectedDataPointEffectPropertyName = "UnselectedDataPointEffect";
        internal const string TitlePropertyName = "Title";
        internal const string LabelVisibilityPropertyName = "LabelVisibility";
        internal const string LegendTextPropertyName = "LegendText";
        internal const string LabelDisplayUnitSystemPropertyName = "LabelDisplayUnitSystem";
        internal const string SeriesStylePropertyName = "SeriesStyle";
        public const string VisibilityPropertyName = "Visibility";
        internal const string HaveSelectedDataPointsPropertyName = "HaveSelectedDataPoints";
        internal const string IsLegendSelectedPropertyName = "IsLegendSelected";
        private SeriesPresenter _seriesPresenter;
        private ChartArea _chartArea;
        private bool _isTraceSeries;
        private Binding _labelContentBinding;
        private Binding _toolTipContentBinding;
        private IItemsBinder<DataPoint> _itemsBinder;
        private int? _simplifiedRenderingThreshold;
        private ObservableCollection<DataPoint> _dataPoints;
        private bool _dataPointsReset;
        private Style _paletteStyle;
        private bool _clearSelectedDataPointsPerformingFlag;

        protected int CallNestLevelFromRoot { get; set; }

        internal SeriesPresenter SeriesPresenter
        {
            get
            {
                if (this._seriesPresenter == null)
                    this._seriesPresenter = this.CreateSeriesPresenter();
                return this._seriesPresenter;
            }
        }

        public ChartArea ChartArea
        {
            get
            {
                return this._chartArea;
            }
            set
            {
                if (this._chartArea == value)
                    return;
                ChartArea oldValue = this._chartArea;
                this._chartArea = value;
                this.OnChartAreaPropertyChanged(oldValue, this._chartArea);
            }
        }

        public bool IsTraceSeries
        {
            get
            {
                return this._isTraceSeries;
            }
            set
            {
                this._isTraceSeries = value;
            }
        }

        public Style DataPointStyle
        {
            get
            {
                return this.GetValue(Series.DataPointStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Series.DataPointStyleProperty, value);
            }
        }

        public Style EmptyDataPointStyle
        {
            get
            {
                return this.GetValue(Series.EmptyDataPointStyleProperty) as Style;
            }
            set
            {
                this.SetValue(Series.EmptyDataPointStyleProperty, value);
            }
        }

        public bool IsDimmed
        {
            get
            {
                return (bool)this.GetValue(Series.IsDimmedProperty);
            }
            set
            {
                this.SetValue(Series.IsDimmedProperty, value);
            }
        }

        public bool IsLegendDimmed
        {
            get
            {
                return (bool)this.GetValue(Series.IsLegendDimmedProperty);
            }
            set
            {
                this.SetValue(Series.IsLegendDimmedProperty, value);
            }
        }

        public double UnselectedDataPointOpacity
        {
            get
            {
                return (double)this.GetValue(Series.UnselectedDataPointOpacityProperty);
            }
            set
            {
                this.SetValue(Series.UnselectedDataPointOpacityProperty, value);
            }
        }

        public Effect UnselectedDataPointEffect
        {
            get
            {
                return (Effect)this.GetValue(Series.UnselectedDataPointEffectProperty);
            }
            set
            {
                this.SetValue(Series.UnselectedDataPointEffectProperty, value);
            }
        }

        public object Title
        {
            get
            {
                return this.GetValue(Series.TitleProperty);
            }
            set
            {
                this.SetValue(Series.TitleProperty, value);
            }
        }

        public Binding LabelContentBinding
        {
            get
            {
                return this._labelContentBinding;
            }
            set
            {
                if (value == this._labelContentBinding)
                    return;
                this._labelContentBinding = value;
                this.DataPoints.ForEachWithIndex<DataPoint>((item, index) => item.UpdateBinding());
            }
        }

        public Binding ToolTipContentBinding
        {
            get
            {
                return this._toolTipContentBinding;
            }
            set
            {
                if (value == this._toolTipContentBinding)
                    return;
                this._toolTipContentBinding = value;
                this.DataPoints.ForEachWithIndex<DataPoint>((item, index) => item.UpdateBinding());
            }
        }

        public virtual IItemsBinder<DataPoint> ItemsBinder
        {
            get
            {
                return this._itemsBinder;
            }
            set
            {
                if (value == this._itemsBinder)
                    return;
                if (this._itemsBinder != null)
                    this.DataPoints.ForEachWithIndex<DataPoint>((item, index) => this._itemsBinder.Unbind(item, item.DataContext));
                this._itemsBinder = value;
                if (this._itemsBinder == null)
                    return;
                this.DataPoints.ForEachWithIndex<DataPoint>((item, index) => this._itemsBinder.Bind(item, item.DataContext));
            }
        }

        public object PaletteKey
        {
            get
            {
                return this.GetValue(Series.PaletteKeyProperty);
            }
            set
            {
                this.SetValue(Series.PaletteKeyProperty, value);
            }
        }

        public TimeSpan? TransitionDuration
        {
            get
            {
                return (TimeSpan?)this.GetValue(Series.TransitionDurationProperty);
            }
            set
            {
                this.SetValue(Series.TransitionDurationProperty, value);
            }
        }

        internal TimeSpan ActualTransitionDuration
        {
            get
            {
                TimeSpan timeSpan = new TimeSpan();
                if (this.TransitionDuration.HasValue)
                    timeSpan = this.TransitionDuration.Value;
                else if (this.ChartArea != null && this.ChartArea.TransitionDuration.HasValue)
                    timeSpan = this.ChartArea.TransitionDuration.Value;
                return timeSpan;
            }
        }

        public NullableObject<IEasingFunction> TransitionEasingFunction
        {
            get
            {
                return (NullableObject<IEasingFunction>)this.GetValue(Series.TransitionEasingFunctionProperty);
            }
            set
            {
                this.SetValue(Series.TransitionEasingFunctionProperty, value);
            }
        }

        internal IEasingFunction ActualTransitionEasingFunction
        {
            get
            {
                IEasingFunction easingFunction = null;
                if (this.TransitionEasingFunction.HasValue)
                    easingFunction = this.TransitionEasingFunction.Value;
                else if (this.ChartArea != null)
                    easingFunction = this.ChartArea.TransitionEasingFunction;
                return easingFunction;
            }
        }

        [TypeConverter(typeof(NullableConverter<int>))]
        public int? SimplifiedRenderingThreshold
        {
            get
            {
                return this._simplifiedRenderingThreshold;
            }
            set
            {
                this._simplifiedRenderingThreshold = value;
            }
        }

        internal virtual int ClusterKey
        {
            get
            {
                return new Tuple<string, Series>("__DefaultAxisMargin__", this).GetHashCode();
            }
        }

        IUpdatable IUpdatable.Parent
        {
            get
            {
                return null;
            }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(Series.ItemsSourceProperty);
            }
            set
            {
                this.SetValue(Series.ItemsSourceProperty, value);
            }
        }

        public Visibility LabelVisibility
        {
            get
            {
                return (Visibility)this.GetValue(Series.LabelVisibilityProperty);
            }
            set
            {
                this.SetValue(Series.LabelVisibilityProperty, value);
            }
        }

        public string LegendText
        {
            get
            {
                return this.GetValue(Series.LegendTextProperty) as string;
            }
            set
            {
                this.SetValue(Series.LegendTextProperty, value);
            }
        }

        public ObservableCollection<DataPoint> DataPoints
        {
            get
            {
                return this._dataPoints;
            }
        }

        public DisplayUnitSystem LabelDisplayUnitSystem
        {
            get
            {
                return this._labelDisplayUnitSystem;
            }
            set
            {
                if (this._labelDisplayUnitSystem == value)
                    return;
                if (this._labelDisplayUnitSystem != null)
                    this._labelDisplayUnitSystem.PropertyChanged -= new PropertyChangedEventHandler(this.DisplayUnitSystem_PropertyChanged);
                this._labelDisplayUnitSystem = value;
                if (this._labelDisplayUnitSystem != null)
                    this._labelDisplayUnitSystem.PropertyChanged += new PropertyChangedEventHandler(this.DisplayUnitSystem_PropertyChanged);
                this.OnPropertyChanged("LabelDisplayUnitSystem");
            }
        }

        protected ResourceDictionary PaletteResources { get; private set; }

        public bool HaveSelectedDataPoints
        {
            get
            {
                return (bool)this.GetValue(Series.HaveSelectedDataPointsProperty);
            }
            set
            {
                this.SetValue(Series.HaveSelectedDataPointsProperty, value);
            }
        }

        internal event EventHandler<ValueChangedEventArgs> DataPointValueChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected Series()
        {
            this._dataPoints = new ObservableCollection<DataPoint>();
            this._dataPoints.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnDataPointsCollectionChanged);
            this._itemsSourceCollectionAdapter.StartUpdating = () =>
           {
               if (this.ChartArea == null)
                   return;
               this.ChartArea.UpdateSession.BeginUpdates();
           };
            this._itemsSourceCollectionAdapter.EndUpdating = () =>
           {
               if (this.ChartArea == null)
                   return;
               this.ChartArea.UpdateSession.EndUpdates();
           };
            this._itemsSourceCollectionAdapter.CreateItem = item =>
           {
               DataPoint dataPoint = item as DataPoint;
               if (dataPoint == null || !this.IsDataPointCompatible(dataPoint))
               {
                   dataPoint = this.CreateDataPoint();
                   dataPoint.DataContext = item;
                   if (this.ItemsBinder != null && dataPoint.DataContext != null)
                       this.ItemsBinder.Bind(dataPoint, dataPoint.DataContext);
               }
               return dataPoint;
           };
            this._itemsSourceCollectionAdapter.RemoveItem = point =>
           {
               if (this.ItemsBinder == null || point.DataContext == null)
                   return;
               this.ItemsBinder.Unbind(point, point.DataContext);
           };
            this._itemsSourceCollectionAdapter.ReplaceItem = (point, item) =>
           {
               if (point == null)
                   return;
               point.DataContext = item;
               if (this.ItemsBinder == null || point.DataContext == null)
                   return;
               this.ItemsBinder.Bind(point, point.DataContext);
           };
            this._itemsSourceCollectionAdapter.TargetList = this._dataPoints;
            this._itemsSourceCollectionAdapter.SourceCollection = this.ItemsSource;
            Binding binding1 = new Binding("Opacity") { Source = (this) };
            this.SetBinding(Series.OpacityListenerProperty, binding1);
            Binding binding2 = new Binding("Effect") { Source = (this) };
            this.SetBinding(Series.EffectListenerProperty, binding2);
            Binding binding3 = new Binding("Visibility") { Source = (this) };
            this.SetBinding(Series.VisibilityListenerProperty, binding3);
        }

        internal abstract SeriesPresenter CreateSeriesPresenter();

        protected virtual void OnChartAreaPropertyChanged(ChartArea oldValue, ChartArea newValue)
        {
            if (oldValue != null && oldValue.PaletteDispenser != null)
                oldValue.PaletteDispenser.PaletteChanged -= new EventHandler(this.PaletteChanged);
            if (newValue == null || newValue.PaletteDispenser == null)
                return;
            newValue.PaletteDispenser.PaletteChanged += new EventHandler(this.PaletteChanged);
        }

        private void PaletteChanged(object sender, EventArgs e)
        {
            this.OnApplyPaletteStyle();
        }

        private static void OnDataPointStyleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Series)o).OnDataPointStyleChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnDataPointStyleChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("DataPointStyle");
        }

        protected virtual void OnEmptyDataPointStyleChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("EmptyDataPointStyle");
        }

        private static void OnIsDimmedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Series)d).OnIsDimmedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsDimmedPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsDimmed");
        }

        private static void OnIsLegendDimmedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Series)d).OnIsLegendDimmedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsLegendDimmedPropertyChanged(object oldValue, object newValue)
        {
            this.UpdateIsLegendSelectedFlag();
            this.OnPropertyChanged("IsLegendDimmed");
        }

        private static void OnUnselectedDataPointOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Series)d).OnUnselectedDataPointOpacityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnUnselectedDataPointOpacityPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("UnselectedDataPointOpacity");
        }

        private static void OnUnselectedDataPointEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Series)d).OnUnselectedDataPointEffectPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnUnselectedDataPointEffectPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("UnselectedDataPointEffect");
        }

        private static void OnTitleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Series)o).OnTitleChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTitleChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("Title");
        }

        private static void OnPaletteKeyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Series)o).OnPaletteKeyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnPaletteKeyChanged(object oldValue, object newValue)
        {
            if (this.ChartArea == null || this.PaletteResources == null || oldValue == newValue)
                return;
            this.ChartArea.ReapplyPalette();
        }

        public void Update()
        {
            if (this.ChartArea == null)
                return;
            this.SeriesPresenter.UpdateSeries();
        }

        private static void OnItemsSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Series)o)._itemsSourceCollectionAdapter.SourceCollection = (IEnumerable)e.NewValue;
        }

        private static void OnLabelVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Series)d).OnPropertyChanged("LabelVisibility");
        }

        private static void OnLegendTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((Series)o).OnPropertyChanged("LegendText");
        }

        private void DisplayUnitSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("LabelDisplayUnitSystem");
        }

        private void OnDataPointsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.ChartArea == null || this.ChartArea.UpdateSession.DisableUpdates)
                return;
            if (e.Action == NotifyCollectionChangedAction.Reset)
                this._dataPointsReset = true;
            if (e.NewItems != null && !this._dataPointsReset)
                this._dataPointsToAdd.AddRange(e.NewItems.Cast<DataPoint>());
            if (e.OldItems != null && !this._dataPointsReset)
                this._dataPointsToRemove.AddRange(e.OldItems.Cast<DataPoint>());
            Action action = () =>
           {
               if (this._dataPointsReset)
                   this.SyncDataPoints();
               else if (this._dataPointsToRemove.Count > 0 || this._dataPointsToAdd.Count > 0)
                   this.OnDataPointsCollectionChanged((IEnumerable<DataPoint>)this._dataPointsToRemove, (IEnumerable<DataPoint>)this._dataPointsToAdd, this._dataPointsReset);
               this._dataPointsReset = false;
               this._dataPointsToAdd.Clear();
               this._dataPointsToRemove.Clear();
           };
            if (this.ChartArea != null)
            {
                Tuple<Series, string> tuple = new Tuple<Series, string>(this, "__OnDataPointsCollectionChanged__");
                this.ChartArea.UpdateSession.ExecuteOnceBeforeUpdating(action, tuple);
            }
            else
                action();
        }

        internal abstract void OnDataPointsCollectionChanged(IEnumerable<DataPoint> removedDataPoints, IEnumerable<DataPoint> addedDataPoints, bool isReset);

        internal void SyncDataPoints()
        {
            this.OnDataPointsCollectionChanged(null, null, true);
        }

        internal abstract void RemoveAllDataPoints();

        internal abstract void UpdateActualDataRange();

        internal virtual void UpdateVisibility()
        {
            this.SeriesPresenter.UpdateDataPointVisibility();
        }

        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
            if (changedEventHandler != null)
                changedEventHandler(this, new PropertyChangedEventArgs(name));
            switch (name)
            {
                case "DataPointStyle":
                case "EmptyDataPointStyle":
                case "UnselectedDataPointEffect":
                case "HaveSelectedDataPoints":
                case "IsDimmed":
                case "LabelDisplayUnitSystem":
                case "Effect":
                case "Opacity":
                case null:
                    using (IEnumerator<DataPoint> enumerator = this.DataPoints.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                            enumerator.Current.UpdateActualPropertiesFromSeries(name);
                        break;
                    }
                case "Visibility":
                    this.UpdateVisibility();
                    break;
            }
        }

        internal virtual void InitializeDataPoint(DataPoint dataPoint)
        {
            if (dataPoint == null || dataPoint.Series != null)
                return;
            dataPoint.ViewState = DataPointViewState.Hidden;
            dataPoint.IsNewlyAdded = true;
            dataPoint.Series = this;
            if (dataPoint.IsSelected && !this.HaveSelectedDataPoints)
                this.UpdateSelectedDataPointFlag(dataPoint);
            dataPoint.ValueChanged += new ValueChangedEventHandler(this.OnDataPointValueChanged_EventHandler);
        }

        internal virtual void UninitializeDataPoint(DataPoint dataPoint)
        {
            if (dataPoint == null)
                return;
            dataPoint.ValueChanged -= new ValueChangedEventHandler(this.OnDataPointValueChanged_EventHandler);
            dataPoint.Series = null;
        }

        private void OnDataPointValueChanged_EventHandler(object sender, ValueChangedEventArgs e)
        {
            this.OnDataPointValueChanged(sender as DataPoint, e.ValueName, e.OldValue, e.NewValue);
            if (this.DataPointValueChanged == null)
                return;
            this.DataPointValueChanged(sender, e);
        }

        internal virtual void OnDataPointValueChanged(DataPoint dataPoint, string propertyName, object oldValue, object newValue)
        {
        }

        internal abstract DataPoint CreateDataPoint();

        protected abstract bool IsDataPointCompatible(DataPoint dataPoint);

        internal virtual void UpdateRelatedSeries()
        {
        }

        protected virtual void OnApplyPaletteStyle()
        {
            if (this.PaletteResources != null)
            {
                this.Resources.MergedDictionaries.Remove(this.PaletteResources);
                this.PaletteResources = null;
            }
            this.PaletteResources = this.GetNextPaletteResourceDictionary(this.PaletteKey);
            if (this.PaletteResources != null)
                this.Resources.MergedDictionaries.Add(this.PaletteResources);
            if (this.Style != null && this.Style != this._paletteStyle)
                return;
            this._paletteStyle = this.Resources[this.GetType().Name + "Style"] as Style ?? this.Resources["SeriesStyle"] as Style;
            this.Style = this._paletteStyle;
        }

        protected virtual ResourceDictionary GetNextPaletteResourceDictionary(object key)
        {
            return Series.GetNextPaletteResourceDictionaryWithTargetType(this.ChartArea.PaletteDispenser, key, this.GetType(), true);
        }

        internal static ResourceDictionary GetNextPaletteResourceDictionaryWithTargetType(PaletteDispenser dispenser, object key, Type targetType, bool takeAncestors)
        {
            return dispenser.Next(dictionary =>
           {
               bool flag = false;
               string str = targetType.Name + "Style";
               Style style1 = dictionary[(object)str] as Style;
               if (style1 != null)
                   flag = (Type)null != style1.TargetType && (targetType == style1.TargetType || takeAncestors && style1.TargetType.IsAssignableFrom(targetType));
               if (!flag)
               {
                   Style style2 = dictionary[(object)"SeriesStyle"] as Style;
                   if (style2 != null)
                       flag = (Type)null != style2.TargetType && (targetType == style2.TargetType || takeAncestors && style2.TargetType.IsAssignableFrom(targetType));
               }
               return flag;
           }, key);
        }

        private static void OnOpacityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is Series)
            {
                ((Series)o).OnPropertyChanged("Opacity");
            }
            else
            {
                if (!(o is DataPoint))
                    return;
                ((DataPoint)o).OnPropertyChanged("Opacity");
            }
        }

        private static void OnEffectChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is Series)
            {
                ((Series)o).OnPropertyChanged("Effect");
            }
            else
            {
                if (!(o is DataPoint))
                    return;
                ((DataPoint)o).OnPropertyChanged("Effect");
            }
        }

        private static void OnVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is Series)
            {
                ((Series)o).OnPropertyChanged("Visibility");
            }
            else
            {
                if (!(o is DataPoint))
                    return;
                ((DataPoint)o).OnPropertyChanged("Visibility");
            }
        }

        public void ClearSelectedDataPoints()
        {
            if (!this.HaveSelectedDataPoints)
                return;
            DataPointSelectionChangedEventArgs e = new DataPointSelectionChangedEventArgs(this.DataPoints.Where<DataPoint>((Func<DataPoint, bool>)(p => p.IsSelected)).ToList<DataPoint>(), null);
            if (e.RemovedItems.Count <= 0)
                return;
            try
            {
                this._clearSelectedDataPointsPerformingFlag = true;
                e.RemovedItems.ForEach<DataPoint>(p => p.IsSelected = false);
                this.ChartArea.FireDataPointSelectionChanged(e);
            }
            finally
            {
                this._clearSelectedDataPointsPerformingFlag = false;
            }
        }

        private static void OnHaveSelectedDataPointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;
            ((Series)d).OnHaveSelectedDataPointsPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnHaveSelectedDataPointsPropertyChanged(object oldValue, object newValue)
        {
            if (this.ChartArea != null)
                this.ChartArea.UpdateIsAnySeriesHaveSelectionFlag();
            this.OnPropertyChanged("HaveSelectedDataPoints");
        }

        internal void UpdateSelectedDataPointFlag(DataPoint dataPoint)
        {
            this.HaveSelectedDataPoints = dataPoint != null && dataPoint.IsSelected || this.DataPoints.FirstOrDefault<DataPoint>(p => p.IsSelected) != null;
            if (this.ChartArea != null && dataPoint != null && !this._clearSelectedDataPointsPerformingFlag)
            {
                DataPointSelectionChangedEventArgs e;
                if (dataPoint.IsSelected)
                    e = new DataPointSelectionChangedEventArgs(new DataPoint[0], new DataPoint[1] { dataPoint });
                else
                    e = new DataPointSelectionChangedEventArgs(new DataPoint[1] { dataPoint }, new DataPoint[0]);
                this.ChartArea.FireDataPointSelectionChanged(e);
            }
            this.UpdateIsLegendSelectedFlag();
        }

        internal virtual void UpdateIsLegendSelectedFlag()
        {
            this.SetValue(Series.IsLegendSelectedProperty, !this.IsLegendDimmed);
        }

        internal virtual bool IsDataPointAppearsUnselected(DataPoint dataPoint)
        {
            if (dataPoint.IsSelected)
                return false;
            if (!this.HaveSelectedDataPoints && ((this.ChartArea != null ? (this.ChartArea.IsAnySeriesHaveSelection ? 1 : 0) : 0) == 0 && !this.IsDimmed))
                return dataPoint.IsDimmed;
            return true;
        }

        internal void UpdateSeriesSelectionStateFromChart()
        {
            this.UpdateIsLegendSelectedFlag();
            this.OnPropertyChanged("HaveSelectedDataPoints");
        }

        public IEnumerable<DataPoint> GetSelectedDataPoints()
        {
            return this.DataPoints.Where<DataPoint>(p => p.IsSelected);
        }

        internal void Unbind()
        {
            if (this.ItemsBinder == null)
                return;
            foreach (DataPoint dataPoint in this.DataPoints)
            {
                object dataContext = dataPoint.DataContext;
                if (dataContext != null)
                    this.ItemsBinder.Unbind(dataPoint, dataContext);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SeriesAutomationPeer(this);
        }
    }
}
