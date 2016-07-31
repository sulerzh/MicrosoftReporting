using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [TemplatePart(Name = "ChartAreaCanvas", Type = typeof(EdgePanel))]
    [TemplatePart(Name = "LayoutRoot", Type = typeof(Grid))]
    public abstract class ChartArea : Control, IUpdatable, ISupportInitialize, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PlotAreaMarginProperty = DependencyProperty.Register("PlotAreaMargin", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0.0), new PropertyChangedCallback(ChartArea.OnPlotAreaMarginPropertyChanged)));
        public static readonly DependencyProperty ActualPlotAreaMarginProperty = DependencyProperty.Register("ActualPlotAreaMargin", typeof(Thickness), typeof(ChartArea), new PropertyMetadata(new Thickness(0.0), new PropertyChangedCallback(ChartArea.OnActualPlotAreaMarginPropertyChanged)));
        public static readonly DependencyProperty PlotAreaStyleProperty = DependencyProperty.Register("PlotAreaStyle", typeof(Style), typeof(ChartArea), null);
        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register("Palette", typeof(Microsoft.Reporting.Windows.Common.Internal.ResourceDictionaryCollection), typeof(ChartArea), new PropertyMetadata(new PropertyChangedCallback(ChartArea.OnPalettePropertyChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(XYChartArea), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(ChartArea.OnOrientationPropertyChanged)));
        public static readonly DependencyProperty IsMouseZoomEnabledProperty = DependencyProperty.Register("IsMouseZoomEnabled", typeof(bool), typeof(ChartArea), new PropertyMetadata(false, new PropertyChangedCallback(ChartArea.OnIsMouseZoomEnabledPropertyChanged)));
        public static readonly DependencyProperty IsMouseDragEnabledProperty = DependencyProperty.Register("IsMouseDragEnabled", typeof(bool), typeof(ChartArea), new PropertyMetadata(true, new PropertyChangedCallback(ChartArea.OnIsMouseDragEnabledPropertyChanged)));
        public static readonly DependencyProperty IsMouseSelectionEnabledProperty = DependencyProperty.Register("IsMouseSelectionEnabled", typeof(bool), typeof(ChartArea), new PropertyMetadata(true, new PropertyChangedCallback(ChartArea.OnIsMouseSelectionEnabledPropertyChanged)));
        public static readonly DependencyProperty IsShowingAnimationEnabledProperty = DependencyProperty.Register("IsShowingAnimationEnabled", typeof(bool), typeof(ChartArea), new PropertyMetadata(true, new PropertyChangedCallback(ChartArea.OnIsShowingAnimationEnabledPropertyChanged)));
        public static readonly DependencyProperty WatermarkContentProperty = DependencyProperty.Register("WatermarkContent", typeof(object), typeof(ChartArea), new PropertyMetadata(null));
        public static readonly TimeSpan DefaultTransitionDuration = TimeSpan.FromSeconds(0.5);
        public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register("TransitionDuration", typeof(TimeSpan?), typeof(ChartArea), new PropertyMetadata(ChartArea.DefaultTransitionDuration));
        public static readonly DependencyProperty TransitionEasingFunctionProperty = DependencyProperty.Register("TransitionEasingFunction", typeof(IEasingFunction), typeof(ChartArea), new PropertyMetadata(null));
        public new static readonly DependencyProperty IsFocusedProperty = DependencyProperty.Register("IsFocused", typeof(bool), typeof(ChartArea), new PropertyMetadata(false, new PropertyChangedCallback(ChartArea.OnIsFocusedPropertyChanged)));
        public static readonly DependencyProperty IsKeyboardNavigationEnabledProperty = DependencyProperty.Register("IsKeyboardNavigationEnabled", typeof(bool), typeof(ChartArea), new PropertyMetadata(true, new PropertyChangedCallback(ChartArea.OnIsKeyboardNavigationEnabledPropertyChanged)));
        private LayerProvider _plotAreaLayersProvider = new LayerProvider();
        private LayerProvider _chartAreaLayersProvider = new LayerProvider();
        private PaletteDispenser _paletteDispenser = new PaletteDispenser();
        private List<DataPoint> _selectedDataPointsState = new List<DataPoint>();
        internal const string ChartAreaCanvasName = "ChartAreaCanvas";
        internal const string LayoutRootName = "LayoutRoot";
        public const string PlotAreaMarginPropertyName = "PlotAreaMargin";
        public const string ActualPlotAreaMarginPropertyName = "ActualPlotAreaMargin";
        internal const string PlotAreaStylePropertyName = "PlotAreaStyle";
        internal const string OrientationPropertyName = "Orientation";
        internal const string IsMouseZoomEnabledPropertyName = "IsMouseZoomEnabled";
        internal const string IsMouseDragEnabledPropertyName = "IsMouseDragEnabled";
        internal const string IsMouseSelectionEnabledPropertyName = "IsMouseSelectionEnabled";
        internal const string IsShowingAnimationEnabledPropertyName = "IsShowingAnimationEnabled";
        internal const string WatermarkContentPropertyName = "WatermarkContent";
        internal const string IsFocusedPropertyName = "IsFocused";
        internal const string IsKeyboardNavigationEnabledPropertyName = "IsKeyboardNavigationEnabled";
        private bool _isTemplateApplied;
        private int _initCount;
        private bool _isDragging;
        private MouseEventArgs _oldMouseArgs;

        internal UpdateSession UpdateSession { get; private set; }

        internal SingletonRegistry SingletonRegistry { get; private set; }

        public EdgePanel ChartAreaPanel { get; private set; }

        public Grid PlotAreaPanel { get; private set; }

        public Thickness PlotAreaMargin
        {
            get
            {
                return (Thickness)this.GetValue(ChartArea.PlotAreaMarginProperty);
            }
            set
            {
                this.SetValue(ChartArea.PlotAreaMarginProperty, value);
            }
        }

        public Thickness ActualPlotAreaMargin
        {
            get
            {
                return (Thickness)this.GetValue(ChartArea.ActualPlotAreaMarginProperty);
            }
            set
            {
                this.SetValue(ChartArea.ActualPlotAreaMarginProperty, value);
            }
        }

        public Style PlotAreaStyle
        {
            get
            {
                return (Style)this.GetValue(ChartArea.PlotAreaStyleProperty);
            }
            set
            {
                this.SetValue(ChartArea.PlotAreaStyleProperty, value);
            }
        }

        public bool IsTemplateApplied
        {
            get
            {
                return this._isTemplateApplied;
            }
        }

        internal virtual LayerProvider PlotAreaLayerProvider
        {
            get
            {
                return this._plotAreaLayersProvider;
            }
        }

        internal virtual LayerProvider ChartAreaLayerProvider
        {
            get
            {
                return this._chartAreaLayersProvider;
            }
        }

        public Microsoft.Reporting.Windows.Common.Internal.ResourceDictionaryCollection Palette
        {
            get
            {
                return this.GetValue(ChartArea.PaletteProperty) as Microsoft.Reporting.Windows.Common.Internal.ResourceDictionaryCollection;
            }
            set
            {
                this.SetValue(ChartArea.PaletteProperty, value);
            }
        }

        public PaletteDispenser PaletteDispenser
        {
            get
            {
                return this._paletteDispenser;
            }
            set
            {
                if (this._paletteDispenser == value)
                    return;
                this._paletteDispenser = value;
                if (this._paletteDispenser == null)
                    return;
                this._paletteDispenser.ResourceDictionaries = this.Palette;
                this._paletteDispenser.Reset();
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(ChartArea.OrientationProperty);
            }
            set
            {
                this.SetValue(ChartArea.OrientationProperty, value);
            }
        }

        public bool IsMouseZoomEnabled
        {
            get
            {
                return (bool)this.GetValue(ChartArea.IsMouseZoomEnabledProperty);
            }
            set
            {
                this.SetValue(ChartArea.IsMouseZoomEnabledProperty, value);
            }
        }

        public bool IsMouseDragEnabled
        {
            get
            {
                return (bool)this.GetValue(ChartArea.IsMouseDragEnabledProperty);
            }
            set
            {
                this.SetValue(ChartArea.IsMouseDragEnabledProperty, value);
            }
        }

        public abstract bool IsZoomed { get; }

        public bool IsMouseSelectionEnabled
        {
            get
            {
                return (bool)this.GetValue(ChartArea.IsMouseSelectionEnabledProperty);
            }
            set
            {
                this.SetValue(ChartArea.IsMouseSelectionEnabledProperty, value);
            }
        }

        public bool IsShowingAnimationEnabled
        {
            get
            {
                return (bool)this.GetValue(ChartArea.IsShowingAnimationEnabledProperty);
            }
            set
            {
                this.SetValue(ChartArea.IsShowingAnimationEnabledProperty, value);
            }
        }

        public object WatermarkContent
        {
            get
            {
                return this.GetValue(ChartArea.WatermarkContentProperty);
            }
            set
            {
                this.SetValue(ChartArea.WatermarkContentProperty, value);
            }
        }

        [TypeConverter(typeof(NullableConverter<TimeSpan>))]
        public TimeSpan? TransitionDuration
        {
            get
            {
                return (TimeSpan?)this.GetValue(ChartArea.TransitionDurationProperty);
            }
            set
            {
                this.SetValue(ChartArea.TransitionDurationProperty, value);
            }
        }

        public IEasingFunction TransitionEasingFunction
        {
            get
            {
                return (IEasingFunction)this.GetValue(ChartArea.TransitionEasingFunctionProperty);
            }
            set
            {
                this.SetValue(ChartArea.TransitionEasingFunctionProperty, value);
            }
        }

        internal bool IsInitializing
        {
            get
            {
                return this._initCount > 0;
            }
        }

        IUpdatable IUpdatable.Parent
        {
            get
            {
                return this.Parent as IUpdatable;
            }
        }

        internal virtual bool IsDirty
        {
            get
            {
                if (this.ChartAreaPanel != null)
                    return this.ChartAreaPanel.IsDirty;
                return false;
            }
        }

        internal int SuspendSelectionChangedEventsCount { get; private set; }

        internal bool IsAnySeriesHaveSelection { get; private set; }

        internal MouseClickHelper MouseClickHelper { get; private set; }

        public new bool IsFocused
        {
            get
            {
                return (bool)this.GetValue(ChartArea.IsFocusedProperty);
            }
            private set
            {
                this.SetValue(ChartArea.IsFocusedProperty, value);
            }
        }

        public bool IsKeyboardNavigationEnabled
        {
            get
            {
                return (bool)this.GetValue(ChartArea.IsKeyboardNavigationEnabledProperty);
            }
            set
            {
                this.SetValue(ChartArea.IsKeyboardNavigationEnabledProperty, value);
            }
        }

        public SelectionPanel SelectionPanel { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event DataPointSelectionChangingEventHandler DataPointSelectionChanging;

        public event DataPointSelectionChangedEventHandler DataPointSelectionChanged;

        public event MouseButtonEventHandler MouseSingleClick;

        public new event MouseButtonEventHandler MouseDoubleClick;

        protected ChartArea()
        {
            this.SingletonRegistry = new SingletonRegistry();
            this.UpdateSession = new UpdateSession(SynchronizationContext.Current);
            this.UpdateSession.DisableUpdates = true;
            this.DefaultStyleKey = typeof(ChartArea);
        }

        private static void OnPlotAreaMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnPlotAreaMarginPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnPlotAreaMarginPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("PlotAreaMargin");
        }

        private static void OnActualPlotAreaMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnActualPlotAreaMarginPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualPlotAreaMarginPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualPlotAreaMargin");
        }

        private static void OnPalettePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnPalettePropertyChanged((Microsoft.Reporting.Windows.Common.Internal.ResourceDictionaryCollection)e.NewValue);
        }

        private void OnPalettePropertyChanged(Microsoft.Reporting.Windows.Common.Internal.ResourceDictionaryCollection newValue)
        {
            if (this.PaletteDispenser == null)
                return;
            this.PaletteDispenser.ResourceDictionaries = newValue;
        }

        internal void ReapplyPalette()
        {
            if (this.PaletteDispenser == null)
                return;
            this.PaletteDispenser.Reset();
        }

        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XYChartArea xyChartArea = (XYChartArea)d;
            Orientation oldValue = (Orientation)e.OldValue;
            Orientation newValue = (Orientation)e.NewValue;
            if (oldValue == newValue)
                return;
            xyChartArea.OnOrientationPropertyChanged(oldValue, newValue);
        }

        protected virtual void OnOrientationPropertyChanged(Orientation oldValue, Orientation newValue)
        {
            this.OnPropertyChanged("Orientation");
            this.Update();
        }

        private static void OnIsMouseZoomEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnIsMouseZoomEnabledPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsMouseZoomEnabledPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsMouseZoomEnabled");
        }

        private static void OnIsMouseDragEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnIsMouseDragEnabledPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsMouseDragEnabledPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsMouseDragEnabled");
        }

        private static void OnIsMouseSelectionEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnIsMouseSelectionEnabledPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsMouseSelectionEnabledPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsMouseSelectionEnabled");
        }

        private static void OnIsShowingAnimationEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnPropertyChanged("IsShowingAnimationEnabled");
        }

        public new void BeginInit()
        {
            if (!this.IsInitializing)
                this.UpdateSession.BeginUpdates();
            this.BeginInitCore();
        }

        protected void BeginInitCore()
        {
            ++this._initCount;
        }

        public new void EndInit()
        {
            this.EndInitCore();
            if (this.IsInitializing)
                return;
            if (!this.IsDirty)
                this.Invalidate();
            this.SyncSeriesAndAxes();
            this.UpdateSession.EndUpdates();
        }

        protected void EndInitCore()
        {
            --this._initCount;
        }

        public abstract void Update();

        public abstract void UpdatePlotArea();

        public virtual void Invalidate()
        {
            if (this.ChartAreaPanel == null)
                return;
            this.ChartAreaPanel.Invalidate();
        }

        internal abstract bool CanRemoveAxis(Axis axis);

        public abstract void SyncSeriesAndAxes();

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public override void OnApplyTemplate()
        {
            if (this.ChartAreaPanel != null)
                this.ChartAreaPanel.ArrangeComplete -= new EventHandler(this.ChartArea_ArrangeComplete);
            base.OnApplyTemplate();
            this.ChartAreaPanel = this.GetTemplateChild("ChartAreaCanvas") as EdgePanel;
            if (this.ChartAreaPanel != null)
            {
                this._chartAreaLayersProvider.ParentPanel = this.ChartAreaPanel;
                this.ChartAreaPanel.ChartArea = this;
                this.ChartAreaPanel.SetBinding(EdgePanel.CenterMarginProperty, new Binding("PlotAreaMargin")
                {
                    Source = (object)this
                });
                this.SetBinding(ChartArea.ActualPlotAreaMarginProperty, new Binding("ActualCenterMargin")
                {
                    Source = (object)this.ChartAreaPanel
                });
                this.PlotAreaPanel = (Grid)this._chartAreaLayersProvider.GetLayer(LayerType.PlotArea, 100, () => (Panel)new Grid());
                this.PlotAreaPanel.SetBinding(FrameworkElement.StyleProperty, new Binding("PlotAreaStyle")
                {
                    Source = (object)this
                });
                this.PlotAreaPanel.Background = new SolidColorBrush(Color.FromArgb((byte)1, byte.MaxValue, byte.MaxValue, byte.MaxValue));
                EdgePanel.SetEdge(this.PlotAreaPanel, Edge.Center);
                this._plotAreaLayersProvider.ParentPanel = this.PlotAreaPanel;
                this.InitializeWatermark();
                this.ChartAreaPanel.ArrangeComplete += new EventHandler(this.ChartArea_ArrangeComplete);
                this.ChartAreaPanel.MouseWheel += new MouseWheelEventHandler(this.ChartAreaPanel_MouseWheel);
                this.ChartAreaPanel.MouseLeftButtonDown += new MouseButtonEventHandler(this.ChartAreaPanel_MouseLeftButtonDown);
                this.ChartAreaPanel.MouseLeftButtonUp += new MouseButtonEventHandler(this.ChartAreaPanel_MouseLeftButtonUp);
                this.ChartAreaPanel.MouseMove += new MouseEventHandler(this.ChartAreaPanel_MouseMove);
                this.MouseClickHelper = new MouseClickHelper(new Action<object, MouseButtonEventArgs>(this.OnMouseSingleClick), new Action<object, MouseButtonEventArgs>(this.OnMouseDoubleClick), new TimeSpan?());
                this.MouseClickHelper.Attach(this.ChartAreaPanel);
                Grid grid = this.GetTemplateChild("LayoutRoot") as Grid;
                if (grid != null)
                {
                    this.SelectionPanel = new SelectionPanel(this);
                    grid.Children.Add(this.SelectionPanel);
                }
            }
            if (!this._isTemplateApplied)
                this._isTemplateApplied = true;
            this.UpdateSession.DisableUpdates = false;
            this.ResetView();
        }

        private void InitializeWatermark()
        {
            Panel layer = this.ChartAreaLayerProvider.GetLayer(LayerType.Foreground, 0, () => (Panel)new Grid());
            ContentControl contentControl1 = new ContentControl();
            contentControl1.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            contentControl1.VerticalContentAlignment = VerticalAlignment.Stretch;
            contentControl1.IsTabStop = false;
            ContentControl contentControl2 = contentControl1;
            contentControl2.SetBinding(ContentControl.ContentProperty, new Binding("WatermarkContent")
            {
                Source = (object)this
            });
            layer.Children.Add(contentControl2);
        }

        internal void SuspendSelectionChangedEvents()
        {
            if (this.SuspendSelectionChangedEventsCount == 0)
                this._selectedDataPointsState = this.GetSelectedDataPoints().ToList<DataPoint>();
            ++this.SuspendSelectionChangedEventsCount;
        }

        internal void ReleaseSelectionChangedEvents()
        {
            --this.SuspendSelectionChangedEventsCount;
            if (this.SuspendSelectionChangedEventsCount != 0)
                return;
            List<DataPoint> list = this.GetSelectedDataPoints().ToList<DataPoint>();
            List<DataPoint> dataPointList1 = new List<DataPoint>();
            List<DataPoint> dataPointList2 = new List<DataPoint>();
            foreach (DataPoint dataPoint in list)
            {
                if (!this._selectedDataPointsState.Contains(dataPoint))
                    dataPointList1.Add(dataPoint);
            }
            foreach (DataPoint dataPoint in this._selectedDataPointsState)
            {
                if (!list.Contains(dataPoint))
                    dataPointList2.Add(dataPoint);
            }
            this.FireDataPointSelectionChanged(new DataPointSelectionChangedEventArgs(dataPointList2, dataPointList1));
            this.UpdateIsAnySeriesHaveSelectionFlag();
        }

        public bool CanGetDataPointFromMouseEvent(MouseButtonEventArgs e)
        {
            if (this.IsMouseSelectionEnabled)
                return this.GetDataPointFromMouseEvent(e) != null;
            return false;
        }

        public DataPoint GetDataPointFromMouseEvent(MouseButtonEventArgs e)
        {
            Polyline polyline1 = e.OriginalSource as Polyline;
            if (polyline1 != null)
            {
                PolylineControl polyline2 = polyline1.Parent as PolylineControl;
                if (polyline2 != null && polyline2.Parent is UIElement)
                    return LineSeriesPresenter.FindDataPoint(polyline2, e.GetPosition((IInputElement)(polyline2.Parent as UIElement)));
            }
            return this.GetDataPointFromVisualElement(e.OriginalSource as DependencyObject);
        }

        public DataPoint GetDataPointFromVisualElement(DependencyObject element)
        {
            return SeriesVisualStatePresenter.GetDataPointFromSelectedElement(element, this);
        }

        private void ChartAreaPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this.ChartAreaPanel, CaptureMode.SubTree);
            if (this.CanGetDataPointFromMouseEvent(e) || !this.IsMouseDragEnabled || !this.IsZoomed)
                return;
            this._oldMouseArgs = e;
            this._isDragging = true;
            this.ChartAreaPanel.Cursor = Cursors.Hand;
        }

        private void ChartAreaPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this._isDragging || this._oldMouseArgs == null)
                return;
            this.DragPlotArea(this._oldMouseArgs, e);
            this._oldMouseArgs = e;
        }

        internal abstract void ScrollPlotArea(bool isForward, bool isHorizontalScroll);

        internal abstract void ScrollPlotArea(int numOfView, bool isHorizontalScroll);

        internal abstract void ZoomPlotArea(bool isZoomIn);

        private void ChartAreaPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ChartAreaPanel.ReleaseMouseCapture();
            if (!this._isDragging)
                return;
            this._oldMouseArgs = null;
            this._isDragging = false;
            this.ChartAreaPanel.Cursor = Cursors.Arrow;
        }

        internal void FireDataPointSelectionChanging(DataPoint dataPoint, bool addToSelection)
        {
            if (!this.IsMouseSelectionEnabled || dataPoint != null && !this.GetSeries().Contains<Series>(dataPoint.Series))
                return;
            this.SuspendSelectionChangedEvents();
            try
            {
                if (dataPoint == null)
                {
                    DataPointSelectionChangingEventArgs e = new DataPointSelectionChangingEventArgs(this.GetSelectedDataPoints().ToList<DataPoint>(), null);
                    this.OnDataPointDataPointSelectionChanging(e);
                    if (e.Cancel)
                        return;
                    this.ClearSelectedDataPoints();
                }
                else
                {
                    List<DataPoint> dataPointList1 = new List<DataPoint>();
                    List<DataPoint> dataPointList2 = new List<DataPoint>();
                    if (!dataPoint.IsSelected)
                        dataPointList2.Add(dataPoint);
                    else if (addToSelection)
                        dataPointList1.Add(dataPoint);
                    if (!addToSelection)
                        dataPointList1 = this.GetSelectedDataPoints().Where<DataPoint>(p =>
                       {
                           if (p == dataPoint)
                               return dataPoint.IsSelected;
                           return true;
                       }).ToList<DataPoint>();
                    DataPointSelectionChangingEventArgs e = new DataPointSelectionChangingEventArgs(dataPointList1, dataPointList2);
                    this.OnDataPointDataPointSelectionChanging(e);
                    if (e.Cancel)
                        return;
                    dataPointList1.ForEach(p => p.IsSelected = false);
                    dataPointList2.ForEach(p => p.IsSelected = true);
                }
            }
            finally
            {
                this.ReleaseSelectionChangedEvents();
            }
        }

        private void ChartAreaPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.IsMouseZoomEnabled)
                return;
            this.ZoomPlotArea(e);
        }

        protected abstract void DragPlotArea(MouseEventArgs oldArgs, MouseEventArgs newArgs);

        public void ClearSelectedDataPoints()
        {
            try
            {
                this.SuspendSelectionChangedEvents();
                foreach (Series series in this.GetSeries())
                    series.ClearSelectedDataPoints();
            }
            finally
            {
                this.ReleaseSelectionChangedEvents();
            }
        }

        public IEnumerable<DataPoint> GetSelectedDataPoints()
        {
            foreach (Series series in this.GetSeries())
            {
                foreach (DataPoint selectedDataPoint in series.GetSelectedDataPoints())
                    yield return selectedDataPoint;
            }
        }

        internal void UpdateIsAnySeriesHaveSelectionFlag()
        {
            if (this.SuspendSelectionChangedEventsCount != 0)
                return;
            bool seriesHaveSelection = this.IsAnySeriesHaveSelection;
            this.IsAnySeriesHaveSelection = this.GetSeries().FirstOrDefault<Series>(s => s.HaveSelectedDataPoints) != null;
            if (seriesHaveSelection == this.IsAnySeriesHaveSelection)
                return;
            this.GetSeries().ForEach<Series>(s => s.UpdateSeriesSelectionStateFromChart());
        }

        internal abstract IEnumerable<Series> GetSeries();

        protected abstract void ZoomPlotArea(MouseWheelEventArgs e);

        private void ChartArea_ArrangeComplete(object sender, EventArgs e)
        {
            this.UpdatePlotArea();
        }

        internal abstract void ResetView();

        internal void ResetSingletonRegistry()
        {
            this.SingletonRegistry = new SingletonRegistry();
        }

        internal virtual void ActivateChildModel(FrameworkElement childModel)
        {
            if (this.ChartAreaPanel == null || this.ChartAreaPanel.Children.Contains(childModel))
                return;
            this.ChartAreaPanel.Children.Add(childModel);
        }

        internal virtual void DeactivateChildModel(FrameworkElement childModel)
        {
            if (this.ChartAreaPanel == null)
                return;
            this.ChartAreaPanel.Children.Remove(childModel);
        }

        internal virtual void OnMeasureIterationStarts()
        {
        }

        internal abstract IEnumerable<Series> FindSeries(Axis axis);

        public abstract Point ConvertDataToPlotCoordinate(Axis xAxis, Axis yAxis, object x, object y);

        public abstract Point ConvertScaleToPlotCoordinate(Axis xAxis, Axis yAxis, double scaleX, double scaleY);

        protected virtual void OnDataPointDataPointSelectionChanging(DataPointSelectionChangingEventArgs e)
        {
            if (this.DataPointSelectionChanging == null)
                return;
            this.DataPointSelectionChanging(this, e);
        }

        internal void FireDataPointSelectionChanged(DataPointSelectionChangedEventArgs e)
        {
            this.OnDataPointSelectionChanged(e);
        }

        protected virtual void OnDataPointSelectionChanged(DataPointSelectionChangedEventArgs e)
        {
            if (this.SuspendSelectionChangedEventsCount > 0 || this.DataPointSelectionChanged == null)
                return;
            this.DataPointSelectionChanged(this, e);
        }

        private void OnMouseSingleClick(object sender, MouseButtonEventArgs e)
        {
            this.BeginInit();
            bool addToSelection = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control || (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            this.FireDataPointSelectionChanging(this.GetDataPointFromMouseEvent(e), addToSelection);
            this.EndInit();
            MouseButtonEventHandler buttonEventHandler = this.MouseSingleClick;
            if (buttonEventHandler == null)
                return;
            buttonEventHandler(sender, e);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MouseButtonEventHandler buttonEventHandler = this.MouseDoubleClick;
            if (buttonEventHandler == null)
                return;
            buttonEventHandler(sender, e);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartAreaAutomationPeer(this);
        }

        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnIsFocusedPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsFocusedPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsFocused");
        }

        private static void OnIsKeyboardNavigationEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChartArea)d).OnIsKeyboardNavigationEnabledPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnIsKeyboardNavigationEnabledPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("IsKeyboardNavigationEnabled");
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.Focus();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.IsFocused = true;
            this.ChangeVisualState(true);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.IsFocused = false;
            this.ChangeVisualState(true);
        }

        private new void ChangeVisualState(bool useTransitions)
        {
            if (this.IsFocused)
                VisualStateManager.GoToState(this, "Focused", useTransitions);
            else
                VisualStateManager.GoToState(this, "Unfocused", useTransitions);
        }
    }
}
