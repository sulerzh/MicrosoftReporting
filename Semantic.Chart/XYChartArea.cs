using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [TemplatePart(Name = "ChartAreaCanvas", Type = typeof(EdgePanel))]
    public class XYChartArea : ChartArea
    {
        private Queue<Tuple<object, Action>> _updateActions = new Queue<Tuple<object, Action>>();
        private ObservableCollectionSupportingInitialization<LegendItem> _legendItems = new ObservableCollectionSupportingInitialization<LegendItem>();
        private XYChartArea.XYSeriesQueue _loadingQueue = new XYChartArea.XYSeriesQueue();
        internal const string UpdateLegendItemsTaskKey = "__UpdateLegendItems__";
        internal const string PercentFormat = "P0";
        public const string DataVirtualizerPropertyName = "DataVirtualizer";
        private const double ZoomRate = 1.2;
        private FlowDirection _currentFlowDirection;
        private Size _availableSize;
        private ObservableCollection<XYSeries> _series;
        private ObservableCollection<Axis> _axes;
        private IXYChartAreaDataVirtualizer _dataVirtualizer;
        private bool _viewExists;

        public Collection<XYSeries> Series
        {
            get
            {
                return this._series;
            }
            set
            {
                throw new NotSupportedException("Not supported!");
            }
        }

        public ObservableCollection<LegendItem> LegendItems
        {
            get
            {
                return this._legendItems;
            }
        }

        public ObservableCollection<Axis> Axes
        {
            get
            {
                return this._axes;
            }
            set
            {
                throw new NotSupportedException(Properties.Resources.Chart_Axes_SetterNotSupported);
            }
        }

        public IXYChartAreaDataVirtualizer DataVirtualizer
        {
            get
            {
                return this._dataVirtualizer;
            }
            set
            {
                if (this._dataVirtualizer == value)
                    return;
                IXYChartAreaDataVirtualizer oldValue = this._dataVirtualizer;
                this._dataVirtualizer = value;
                this.OnDataVirtualizerPropertyChanged(oldValue, value);
            }
        }

        public override bool IsZoomed
        {
            get
            {
                foreach (Axis ax in this.Axes)
                {
                    if (ax.Scale.ActualZoom > 1.0)
                        return true;
                }
                return false;
            }
        }

        internal IEnumerable<XYSeries> VisibleSeries
        {
            get
            {
                return this.Series.Where<XYSeries>(s => s.Visibility == Visibility.Visible);
            }
        }

        public XYChartArea()
        {
            this.DefaultStyleKey = typeof(XYChartArea);
            this._series = new UniqueObservableCollection<XYSeries>();
            this.SubscribeToSeriesCollectionChanged();
            this._axes = new AxesCollection((ChartArea)this);
            this.SubscribeToAxesCollectionChanged();
            this.LayoutUpdated += (s, e) => this.OnLayoutUpdated();
        }

        public override void Update()
        {
            this.UpdateSession.BeginUpdates();
            foreach (Axis ax in this.Axes)
                ax.Update();
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
                series.Update();
            this.UpdateSession.EndUpdates();
        }

        public override void UpdatePlotArea()
        {
            this.UpdateSession.BeginUpdates();
            while (this._updateActions.Count > 0)
                this._updateActions.Dequeue().Item2();
            foreach (IUpdatable element in this.Series)
                this.UpdateSession.Update(element);
            this.UpdateSession.ExecuteOnceAfterUpdating(() => this.SelectionPanel.Invalidate(), "SelectionPanel_Invalidate", null);
            this.UpdateSession.EndUpdates();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize != this._availableSize)
            {
                this.OnAvailableSizeChanging(this._availableSize, availableSize);
                this._availableSize = availableSize;
            }
            return base.MeasureOverride(availableSize);
        }

        protected virtual void OnAvailableSizeChanging(Size currentSize, Size newSize)
        {
            foreach (Axis ax in this.Axes)
                ax.OnAvailableSizeChanging();
        }

        private bool IsXAxisReverseRequired(XYSeries series)
        {
            if (this.FlowDirection == FlowDirection.RightToLeft && this.Orientation == Orientation.Horizontal)
                return series.XAxis.Scale is NumericScale;
            return false;
        }

        private void OnLayoutUpdated()
        {
            if (this._currentFlowDirection == this.FlowDirection)
                return;
            this._currentFlowDirection = this.FlowDirection;
            foreach (XYSeries series in this.Series)
                series.XAxis.IsReversed = this.IsXAxisReverseRequired(series);
        }

        private void OnDataVirtualizerPropertyChanged(IXYChartAreaDataVirtualizer oldValue, IXYChartAreaDataVirtualizer newValue)
        {
            if (oldValue != null)
            {
                foreach (XYSeries series in this.Series)
                    oldValue.UninitializeSeries(series);
                foreach (Axis ax in this.Axes)
                {
                    if (ax.Scale != null)
                        oldValue.UninitializeAxisScale(ax, ax.Scale);
                }
            }
            if (newValue != null)
            {
                foreach (XYSeries series in this.Series)
                    newValue.InitializeSeries(series);
                foreach (Axis ax in this.Axes)
                {
                    if (ax.Scale != null)
                        newValue.InitializeAxisScale(ax, ax.Scale);
                }
                if (!this.IsInitializing)
                    this.SyncSeriesAndAxes();
                this.Invalidate();
            }
            this.OnPropertyChanged("DataVirtualizer");
        }

        public override Point ConvertDataToPlotCoordinate(Axis xAxis, Axis yAxis, object x, object y)
        {
            Point point = new Point(0.0, 0.0);
            if (xAxis.AxisPresenter != null)
                point.X = xAxis.AxisPresenter.ConvertDataToAxisUnits(x) ?? 0.0;
            if (yAxis.AxisPresenter != null)
                point.Y = yAxis.AxisPresenter.ConvertDataToAxisUnits(y) ?? 0.0;
            if (this.Orientation == Orientation.Vertical)
                return new Point(point.Y, point.X);
            return point;
        }

        public override Point ConvertScaleToPlotCoordinate(Axis xAxis, Axis yAxis, double x, double y)
        {
            Point point = new Point(0.0, 0.0);
            if (xAxis.AxisPresenter != null)
                point.X = xAxis.AxisPresenter.ConvertScaleToAxisUnits(x) ?? 0.0;
            if (yAxis.AxisPresenter != null)
                point.Y = yAxis.AxisPresenter.ConvertScaleToAxisUnits(y) ?? 0.0;
            if (this.Orientation == Orientation.Vertical)
                return new Point(point.Y, point.X);
            return point;
        }

        internal override void ResetView()
        {
            this.RemoveView();
            this.CreateView();
            this.UpdateLegendItems();
        }

        private void CreateView()
        {
            if (this._viewExists)
                return;
            this.ReapplyPalette();
            foreach (Axis ax in this.Axes)
                ax.AxisPresenter.OnAxisAdded();
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
                series.SeriesPresenter.OnSeriesAdded();
            this._viewExists = true;
        }

        private void RemoveView()
        {
            if (!this._viewExists)
                return;
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
                series.SeriesPresenter.OnSeriesRemoved();
            foreach (Axis ax in this.Axes)
                ax.AxisPresenter.OnAxisRemoved();
            this._viewExists = false;
        }

        public void HideSeries()
        {
            foreach (object layerKey in this.Series)
                this.ChartAreaLayerProvider.SetLayerVisibility(layerKey, Visibility.Collapsed);
            this.ChartAreaLayerProvider.SetLayerVisibility(LayerType.SmartLabels, Visibility.Collapsed);
        }

        public void ResetSeries()
        {
            this.UnsubscribeToSeriesCollectionChanged();
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
            {
                this.ChartAreaLayerProvider.RemoveLayer(series);
                this.DeactivateChildModel(series);
                series.Unbind();
                series.ChartArea = null;
            }
            this.ChartAreaLayerProvider.RemoveLayer(LayerType.SmartLabels);
            this.ReapplyPalette();
            this._series = new ObservableCollection<XYSeries>();
            this.ResetSingletonRegistry();
            this._updateActions.Clear();
            this._legendItems.Clear();
            this.SubscribeToSeriesCollectionChanged();
        }

        internal override void OnMeasureIterationStarts()
        {
            foreach (Axis ax in this.Axes)
                ax.AxisPresenter.OnMeasureIterationStarts();
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
                series.SeriesPresenter.OnMeasureIterationStarts();
        }

        private void SubscribeToSeriesCollectionChanged()
        {
            this._series.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSeriesCollectionChanged);
        }

        private void UnsubscribeToSeriesCollectionChanged()
        {
            this._series.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnSeriesCollectionChanged);
        }

        private void OnSeriesPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (this.IsInitializing)
                return;
            XYSeries series = sender as XYSeries;
            switch (args.PropertyName)
            {
                case "ActualXDataRange":
                    this.UpdateScaleRangeIfUndefined(series.XAxis, series.ActualXValueType);
                    break;
                case "ActualYDataRange":
                    this.UpdateScaleRangeIfUndefined(series.YAxis, series.ActualYValueType);
                    break;
                case "XValues":
                    this.UpdateScaleValuesIfUndefined(series.XAxis, series.ActualXValueType);
                    break;
                case "YValues":
                    this.UpdateScaleValuesIfUndefined(series.YAxis, series.ActualYValueType);
                    break;
                case "ActualXValueType":
                    this.UpdateScaleValueType(series.XAxis);
                    series.XAxis.IsReversed = this.IsXAxisReverseRequired(series);
                    break;
                case "ActualYValueType":
                    this.UpdateScaleValueType(series.YAxis);
                    break;
                case "Visibility":
                    this.UpdateScaleRangeIfUndefined(series.XAxis, series.ActualXValueType);
                    this.UpdateScaleRangeIfUndefined(series.YAxis, series.ActualYValueType);
                    break;
            }
        }

        private void UpdateScaleRangeIfUndefined(Axis axis, DataValueType valueType)
        {
            if (valueType == DataValueType.Auto)
                return;
            if (axis.Scale.CanProject(valueType))
                axis.Scale.UpdateRangeIfUndefined(this.AggregateRange(axis));
            else
                this.SyncSeriesAndAxes();
        }

        private void UpdateScaleValuesIfUndefined(Axis axis, DataValueType valueType)
        {
            if (valueType == DataValueType.Auto)
                return;
            if (axis.Scale.CanProject(valueType))
                axis.Scale.UpdateValuesIfUndefined(this.AggregateValues(axis));
            else
                this.SyncSeriesAndAxes();
        }

        private void UpdateScaleValueType(Axis axis)
        {
            DataValueType valueType = this.AggregateValueType(axis);
            if (axis != null && axis.Scale != null && axis.Scale.CanProject(valueType))
                axis.Scale.UpdateValueType(valueType);
            else
                this.SyncSeriesAndAxes();
        }

        private void OnSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.ResetView();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (XYSeries oldItem in e.OldItems)
                    {
                        this.OnSeriesRemoved(oldItem);
                        this.UpdateSession.SkipUpdate(oldItem);
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (XYSeries newItem in e.NewItems)
                        this.OnSeriesAdded(newItem);
                    this.ReapplyPalette();
                }
            }
            this.UpdateSession.ExecuteOnceAfterUpdating(() => this.UpdateLegendItems(), "__UpdateLegendItems__", null);
        }

        public void UpdateLegendItems()
        {
            if (!this.IsTemplateApplied)
                return;
            List<LegendItem> legendItemList = new List<LegendItem>();
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
            {
                LegendItem legendItem = series.SeriesPresenter.GetLegendItem();
                if (legendItem != null)
                    legendItemList.Add(legendItem);
            }
            this._legendItems.ResetIfNecessary(legendItemList);
        }

        private void OnSeriesAdded(XYSeries series)
        {
            series.ChartArea = (this);
            series.PropertyChanged += new PropertyChangedEventHandler(this.OnSeriesPropertyChanged);
            if (!this.IsInitializing)
            {
                this.SyncSeriesAndAxes();
            }
            else
            {
                series.UpdateActualValueTypes();
                series.UpdateActualDataPoints();
                series.UpdateActualDataRange();
                this.GetScale(series, AxisOrientation.X);
                this.GetScale(series, AxisOrientation.Y);
            }
            if (this.IsTemplateApplied)
                series.SeriesPresenter.OnSeriesAdded();
            if (this.DataVirtualizer == null)
                return;
            this.DataVirtualizer.InitializeSeries(series);
            this.LoadVirtualizedData(new XYSeries[1] { series });
        }

        private void OnSeriesRemoved(XYSeries series)
        {
            series.PropertyChanged -= new PropertyChangedEventHandler(this.OnSeriesPropertyChanged);
            if (!this.IsInitializing)
                this.SyncSeriesAndAxes();
            if (!this.IsTemplateApplied)
                return;
            series.SeriesPresenter.OnSeriesRemoved();
        }

        private void SubscribeToAxesCollectionChanged()
        {
            this._axes.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnAxesCollectionChanged);
        }

        private void OnAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Axis oldItem in e.OldItems)
                    this.OnAxisRemoved(oldItem);
            }
            if (e.NewItems == null)
                return;
            foreach (Axis newItem in e.NewItems)
                this.OnAxisAdded(newItem);
        }

        private void OnAxisAdded(Axis axis)
        {
            axis.ChartArea = (this);
            axis.AxisPresenter.OnAxisAdded();
            axis.ScaleChanged += new EventHandler(this.OnAxisScaleChanged);
            axis.ScaleViewChanged += new EventHandler<ScaleViewChangedArgs>(this.OnAxisScaleViewChanged);
            this.UpdateSession.SkipUpdate(axis);
            if (axis.Scale == null || this.DataVirtualizer == null)
                return;
            this.DataVirtualizer.InitializeAxisScale(axis, axis.Scale);
        }

        private void OnAxisRemoved(Axis axis)
        {
            axis.AxisPresenter.OnAxisRemoved();
            axis.ScaleChanged -= new EventHandler(this.OnAxisScaleChanged);
            axis.ScaleViewChanged -= new EventHandler<ScaleViewChangedArgs>(this.OnAxisScaleViewChanged);
            this.UpdateSession.SkipUpdate(axis);
            axis.ChartArea = null;
        }

        internal virtual void OnAxisScaleChanged(object sender, EventArgs e)
        {
            Axis axis = sender as Axis;
            this.UpdateSession.BeginUpdates();
            foreach (XYSeries xySeries in this.Series)
            {
                XYSeriesPresenter presenter = (XYSeriesPresenter)xySeries.SeriesPresenter;
                Action action = null;
                if (axis == xySeries.XAxis)
                    action = () => presenter.OnXScaleChanged();
                else if (axis == xySeries.YAxis)
                    action = () => presenter.OnYScaleChanged();
                if (action != null)
                {
                    if (this.ChartAreaPanel != null && this.ChartAreaPanel.IsDirty)
                    {
                        XYChartArea.SeriesAxisKey key = new XYChartArea.SeriesAxisKey() { Series = xySeries, Axis = axis };
                        if (this._updateActions.FindIndexOf<Tuple<object, Action>>(t => key.Equals(t.Item1)) == -1)
                            this._updateActions.Enqueue(new Tuple<object, Action>(key, action));
                    }
                    else
                        action();
                }
            }
            this.UpdateSession.EndUpdates();
        }

        internal virtual void OnAxisScaleViewChanged(object sender, ScaleViewChangedArgs e)
        {
            Axis axis = sender as Axis;
            this.UpdateSession.BeginUpdates();
            foreach (XYSeries xySeries in this.Series)
            {
                if (axis == xySeries.XAxis)
                    ((XYSeriesPresenter)xySeries.SeriesPresenter).OnXScaleViewChanged(e.OldRange, e.NewRange);
                else if (axis == xySeries.YAxis)
                    ((XYSeriesPresenter)xySeries.SeriesPresenter).OnYScaleViewChanged(e.OldRange, e.NewRange);
            }
            if (!this.IsInitializing)
                this.LoadVirtualizedData(this.FindSeries(axis).OfType<XYSeries>());
            this.UpdateSession.EndUpdates();
        }

        internal override IEnumerable<Microsoft.Reporting.Windows.Chart.Internal.Series> FindSeries(Axis axis)
        {
            if (axis.Orientation == AxisOrientation.X)
            {
                foreach (XYSeries xySeries in this.Series)
                {
                    if (xySeries.XAxisName == axis.Name)
                        yield return xySeries;
                }
            }
            else
            {
                foreach (XYSeries xySeries in this.Series)
                {
                    if (xySeries.YAxisName == axis.Name)
                        yield return xySeries;
                }
            }
        }

        private IEnumerable<Microsoft.Reporting.Windows.Chart.Internal.Series> FindSeriesWithDefinedValueType(Axis axis)
        {
            if (axis.Orientation == AxisOrientation.X)
            {
                foreach (XYSeries xySeries in this.Series)
                {
                    if (xySeries.XAxisName == axis.Name && xySeries.ActualXValueType != DataValueType.Auto)
                        yield return xySeries;
                }
            }
            else
            {
                foreach (XYSeries xySeries in this.Series)
                {
                    if (xySeries.YAxisName == axis.Name && xySeries.ActualYValueType != DataValueType.Auto)
                        yield return xySeries;
                }
            }
        }

        internal IEnumerable<XYSeries> FindClusterSeries(XYSeries series)
        {
            return this.Series.Where<XYSeries>(item =>
           {
               if (item.ClusterKey == series.ClusterKey)
                   return item.Visibility == Visibility.Visible;
               return false;
           });
        }

        internal override IEnumerable<Microsoft.Reporting.Windows.Chart.Internal.Series> GetSeries()
        {
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
                yield return series;
        }

        public Scale GetScale(XYSeries series, AxisOrientation orientation)
        {
            Axis axis = this.GetAxis(series, orientation);
            DataValueType valueType = orientation == AxisOrientation.X ? series.ActualXValueType : series.ActualYValueType;
            if (valueType != DataValueType.Auto)
            {
                if (axis.Scale != null && !axis.Scale.CanProject(valueType))
                {
                    bool flag = false;
                    foreach (XYSeries xySeries in this.FindSeriesWithDefinedValueType(axis))
                    {
                        if (xySeries != series && xySeries.DataPoints.Count > 0)
                        {
                            DataValueType dataValueType = orientation == AxisOrientation.X ? xySeries.ActualXValueType : xySeries.ActualYValueType;
                            if (valueType != dataValueType && dataValueType != DataValueType.Auto)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        axis.Scale = null;
                    }
                    else
                    {
                        axis = this.FindAxis(valueType, orientation) ?? this.CreateAxis(null, orientation);
                        series.XAxisName = axis.Name;
                    }
                }
            }
            else
                valueType = DataValueType.Float;
            if (axis.Scale == null)
            {
                axis.Scale = Scale.CreateScaleByType(valueType);
                if (this.DataVirtualizer != null)
                    this.DataVirtualizer.InitializeAxisScale(axis, axis.Scale);
            }
            return axis.Scale;
        }

        internal Axis GetAxis(XYSeries series, AxisOrientation orientation)
        {
            Axis axis;
            if (orientation == AxisOrientation.X)
            {
                axis = this.GetAxis(series.XAxisName, orientation);
                series.XAxisName = axis.Name;
            }
            else
            {
                axis = this.GetAxis(series.YAxisName, orientation);
                series.YAxisName = axis.Name;
            }
            return axis;
        }

        internal Axis GetAxis(string axisName, AxisOrientation orientation)
        {
            return this.FindAxis(axisName, orientation) ?? this.CreateAxis(axisName, orientation);
        }

        private Axis FindAxis(DataValueType valueType, AxisOrientation orientation)
        {
            foreach (Axis ax in this.Axes)
            {
                if (ax.Orientation == orientation && ax.Scale.ValueType == valueType)
                    return ax;
            }
            foreach (Axis ax in this.Axes)
            {
                if (ax.Orientation == orientation && ax.Scale.CanProject(valueType))
                    return ax;
            }
            return null;
        }

        private Axis FindAxis(string axisName, AxisOrientation orientation)
        {
            if (string.IsNullOrEmpty(axisName))
            {
                foreach (Axis ax in this.Axes)
                {
                    if (ax.Orientation == orientation)
                        return ax;
                }
            }
            else
            {
                foreach (Axis ax in this.Axes)
                {
                    if (axisName == ax.Name)
                        return ax;
                }
            }
            return null;
        }

        private Axis CreateAxis(string axisName, AxisOrientation orientation)
        {
            if (string.IsNullOrEmpty(axisName))
                axisName = XamlShims.NewFrameworkElementName();
            Axis axis = Axis.CreateAxis(axisName, orientation);
            this.Axes.Add(axis);
            return axis;
        }

        internal override bool CanRemoveAxis(Axis axis)
        {
            foreach (XYSeries xySeries in this.Series)
            {
                if (xySeries.XAxis == axis || xySeries.YAxis == axis)
                    return false;
            }
            return true;
        }

        private void VerifyPercentScaleLabelFormat(Axis axis, bool hasHundredPercentStackSeries)
        {
            if (hasHundredPercentStackSeries)
            {
                if (!string.Equals(axis.Scale.LabelDefinition.Format, "{0}", StringComparison.Ordinal))
                    return;
                axis.Scale.LabelDefinition.Format = "P0";
            }
            else
            {
                if (!string.Equals(axis.Scale.LabelDefinition.Format, "P0", StringComparison.Ordinal))
                    return;
                axis.Scale.LabelDefinition.Format = "{0}";
            }
        }

        public override void SyncSeriesAndAxes()
        {
            this.BeginInitCore();
            foreach (Microsoft.Reporting.Windows.Chart.Internal.Series series in this.Series)
                series.UpdateRelatedSeries();
            foreach (XYSeries xySeries in this.Series)
            {
                xySeries.UpdateActualValueTypes();
                xySeries.UpdateActualDataPoints();
                xySeries.UpdateActualDataRange();
            }
            List<Axis> axisList = new List<Axis>();
            foreach (XYSeries series in this.Series)
            {
                this.GetScale(series, AxisOrientation.X);
                this.GetScale(series, AxisOrientation.Y);
                axisList.Add(series.XAxis);
                axisList.Add(series.YAxis);
            }
            int index = 0;
            while (index < this.Axes.Count)
            {
                Axis axis = this.Axes[index];
                if (axis.IsAutoCreated && !axisList.Contains(axis))
                    this.Axes.RemoveAt(index);
                else
                    ++index;
            }
            this.UpdateSession.BeginUpdates();
            foreach (Axis ax in this.Axes)
            {
                IEnumerable<Microsoft.Reporting.Windows.Chart.Internal.Series> series = this.FindSeries(ax);
                if (ax.Scale != null && series.Any<Microsoft.Reporting.Windows.Chart.Internal.Series>())
                {
                    if (ax.Orientation == AxisOrientation.Y)
                        this.VerifyPercentScaleLabelFormat(ax, series.OfType<StackedColumnSeries>().FirstOrDefault<StackedColumnSeries>(s => s.ActualIsHundredPercent) != null);
                    ax.Scale.BeginInit();
                    ax.Scale.UpdateValueType(this.AggregateValueType(ax));
                    ax.Scale.UpdateRangeIfUndefined(this.AggregateRange(ax));
                    ax.Scale.UpdateValuesIfUndefined(this.AggregateValues(ax));
                    ax.Scale.UpdateDefaults(this.AggregateScaleDefaults(ax));
                    ax.Scale.EndInit();
                }
                else if (ax.Scale == null)
                    ax.Scale = Scale.CreateScaleByType(DataValueType.Integer);
            }
            this.UpdateSession.EndUpdates();
            this.EndInitCore();
        }

        private void LoadVirtualizedData(IEnumerable<XYSeries> series)
        {
            IList<XYSeries> series1 = this._loadingQueue.Add(series);
            if (this.DataVirtualizer != null && series1.Count != 0)
                this.DataVirtualizer.UpdateSeriesForCurrentView(series1);
            this._loadingQueue.Remove(series1);
        }

        internal override void ZoomPlotArea(bool isZoomIn)
        {
            this.ZoomPlotArea(isZoomIn, 0.5, 0.5);
        }

        protected override void ZoomPlotArea(MouseWheelEventArgs e)
        {
            Point position = e.GetPosition(this.PlotAreaPanel);
            if (!this.IsMouseZoomEnabled)
                return;
            this.ZoomPlotArea(e.Delta > 0, position.X / this.PlotAreaPanel.ActualWidth, (this.PlotAreaPanel.ActualHeight - position.Y) / this.PlotAreaPanel.ActualHeight);
        }

        private void ZoomPlotArea(bool isZoomIn, double xCenterValue, double yCenterValue)
        {
            double delta = isZoomIn ? 1.2 : 5.0 / 6.0;
            foreach (Axis ax in this.Axes)
            {
                if (ax.ActualIsZoomEnabled)
                {
                    if (this.IsHorizontalAxis(ax))
                        ax.Scale.ZoomBy(xCenterValue, delta);
                    else
                        ax.Scale.ZoomBy(yCenterValue, delta);
                    ax.ShowScrollZoomBar = ax.Scale.ActualZoom.GreaterWithPrecision(1.0);
                }
            }
        }

        protected override void DragPlotArea(MouseEventArgs oldArgs, MouseEventArgs newArgs)
        {
            Point position1 = oldArgs.GetPosition(this.PlotAreaPanel);
            Point position2 = newArgs.GetPosition(this.PlotAreaPanel);
            foreach (Axis ax in this.Axes)
            {
                double offset = !this.IsHorizontalAxis(ax) ? (position2.Y - position1.Y) / this.PlotAreaPanel.ActualHeight : (position1.X - position2.X) / this.PlotAreaPanel.ActualWidth;
                if (ax.AxisPresenter.ActualIsScaleReversed)
                    offset *= -1.0;
                ax.Scale.ScrollBy(offset);
            }
        }

        internal override void ScrollPlotArea(bool isForward, bool isHorizontalScroll)
        {
            foreach (Axis ax in this.Axes)
            {
                bool flag = this.IsHorizontalAxis(ax);
                double position = isForward ? 1.0 : 0.0;
                if (flag && isHorizontalScroll)
                    ax.Scale.ScrollToPercent(position);
                else if (!flag && !isHorizontalScroll)
                {
                    if (this.Orientation == Orientation.Horizontal)
                        position = isForward ? 0.0 : 1.0;
                    ax.Scale.ScrollToPercent(position);
                }
            }
        }

        internal override void ScrollPlotArea(int numOfView, bool isHorizontalScroll)
        {
            foreach (Axis ax in this.Axes)
            {
                bool flag = this.IsHorizontalAxis(ax);
                if (flag && isHorizontalScroll)
                    ax.Scale.ScrollBy(numOfView);
                else if (!flag && !isHorizontalScroll)
                {
                    if (this.Orientation == Orientation.Horizontal)
                        numOfView *= -1;
                    ax.Scale.ScrollBy(numOfView);
                }
            }
        }

        private bool IsHorizontalAxis(Axis axis)
        {
            if (axis.Orientation == AxisOrientation.X && this.Orientation == Orientation.Horizontal)
                return true;
            if (axis.Orientation == AxisOrientation.Y)
                return this.Orientation == Orientation.Vertical;
            return false;
        }

        internal virtual IEnumerable<Range<IComparable>> AggregateRange(Axis axis)
        {
            if (axis.Orientation == AxisOrientation.X)
            {
                foreach (XYSeries xySeries in this.VisibleSeries)
                {
                    if (xySeries.XAxisName == axis.Name)
                        yield return xySeries.ActualXDataRange;
                }
            }
            else
            {
                foreach (XYSeries xySeries in this.VisibleSeries)
                {
                    if (xySeries.YAxisName == axis.Name)
                        yield return xySeries.ActualYDataRange;
                }
            }
        }

        internal virtual IEnumerable<object> AggregateValues(Axis axis)
        {
            if (axis.Orientation == AxisOrientation.X)
            {
                foreach (XYSeries xySeries in this.VisibleSeries)
                {
                    if (xySeries.XAxisName == axis.Name)
                    {
                        foreach (object xvalue in xySeries.XValues)
                            yield return xvalue;
                    }
                }
            }
            else
            {
                foreach (XYSeries xySeries in this.VisibleSeries)
                {
                    if (xySeries.YAxisName == axis.Name)
                    {
                        foreach (object yvalue in xySeries.YValues)
                            yield return yvalue;
                    }
                }
            }
        }

        internal virtual IEnumerable<object> AggregateXValues(IEnumerable<XYSeries> series)
        {
            foreach (XYSeries xySeries in series)
            {
                if (xySeries.Visibility == Visibility.Visible)
                {
                    foreach (object xvalue in xySeries.XValues)
                        yield return xvalue;
                }
            }
        }

        internal virtual IEnumerable<object> AggregateYValues(IEnumerable<XYSeries> series)
        {
            foreach (XYSeries xySeries in series)
            {
                if (xySeries.Visibility == Visibility.Visible)
                {
                    foreach (object yvalue in xySeries.YValues)
                        yield return yvalue;
                }
            }
        }

        internal virtual DataValueType AggregateValueType(Axis axis)
        {
            DataValueType a = DataValueType.Auto;
            if (axis.Orientation == AxisOrientation.X)
            {
                foreach (XYSeries xySeries in this.VisibleSeries)
                {
                    if (xySeries.XAxisName == axis.Name)
                        a = ValueHelper.CombineDataValueTypes(a, xySeries.ActualXValueType);
                }
                return a;
            }
            foreach (XYSeries xySeries in this.VisibleSeries)
            {
                if (xySeries.YAxisName == axis.Name)
                    a = ValueHelper.CombineDataValueTypes(a, xySeries.ActualYValueType);
            }
            return a;
        }

        internal virtual ScaleDefaults AggregateScaleDefaults(Axis axis)
        {
            ScaleDefaults scaleDefaults = new ScaleDefaults();
            if (axis.Orientation == AxisOrientation.X)
            {
                foreach (XYSeries xySeries in this.Series)
                {
                    if (xySeries.XAxisName == axis.Name)
                        scaleDefaults += xySeries.XScaleDefaults;
                }
                return scaleDefaults;
            }
            foreach (XYSeries xySeries in this.Series)
            {
                if (xySeries.YAxisName == axis.Name)
                    scaleDefaults += xySeries.YScaleDefaults;
            }
            return scaleDefaults;
        }

        private class XYSeriesQueue
        {
            private HashSet<XYSeries> _series = new HashSet<XYSeries>();

            public IList<XYSeries> Add(IEnumerable<XYSeries> series)
            {
                List<XYSeries> xySeriesList = new List<XYSeries>();
                foreach (XYSeries xySeries in series)
                {
                    if (!this._series.Contains(xySeries))
                    {
                        this._series.Add(xySeries);
                        xySeriesList.Add(xySeries);
                    }
                }
                return xySeriesList;
            }

            public void Remove(IList<XYSeries> series)
            {
                foreach (XYSeries xySeries in series)
                    this._series.Remove(xySeries);
            }
        }

        private class SeriesAxisKey
        {
            public Microsoft.Reporting.Windows.Chart.Internal.Series Series { get; set; }

            public Axis Axis { get; set; }

            public override bool Equals(object obj)
            {
                XYChartArea.SeriesAxisKey seriesAxisKey = obj as XYChartArea.SeriesAxisKey;
                if (seriesAxisKey == null)
                    return base.Equals(obj);
                if (seriesAxisKey.Axis == this.Axis)
                    return seriesAxisKey.Series == this.Series;
                return false;
            }

            public override int GetHashCode()
            {
                if (this.Series == null || this.Axis == null)
                    return this.GetType().GetHashCode();
                return this.Axis.GetHashCode() ^ this.Series.GetHashCode();
            }
        }
    }
}
