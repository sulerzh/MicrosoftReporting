using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal abstract class SeriesPresenter
    {
        internal const int MaximumCanvasZIndex = 32766;
        internal const double LegendSymbolWidth = 20.0;
        internal const double LegendSymbolHeight = 12.0;
        internal const int MaxElementsInPool = 100;
        private Panel _rootPanel;
        private SeriesLabelPresenter _labelPresenter;
        private SeriesMarkerPresenter _markerPresenter;
        private bool _isMeasureStarted;
        private bool _isSeriesRemoved;

        public ChartArea ChartArea
        {
            get
            {
                if (this.Series != null)
                    return this.Series.ChartArea;
                return null;
            }
        }

        internal Series Series { get; private set; }

        internal virtual Panel RootPanel
        {
            get
            {
                if (this._rootPanel == null)
                    this._rootPanel = this.ChartArea.ChartAreaLayerProvider.GetLayer(this.Series, (this.Series.IsTraceSeries ? 400 : 401) + this.GetZIndex(), new Func<Panel>(this.CreateCanvas));
                return this._rootPanel;
            }
        }

        internal bool IsRootPanelClipped { get; set; }

        internal bool IsRootPanelVisible
        {
            get
            {
                Panel rootPanel = this.RootPanel;
                if (rootPanel != null && rootPanel.ActualWidth > 0.0)
                    return rootPanel.ActualHeight > 0.0;
                return false;
            }
        }

        internal bool IsSeriesAnimationEnabled
        {
            get
            {
                return this.ChartArea != null && this.Series != null && (this.Series.ActualTransitionDuration != TimeSpan.Zero && this.Series.SeriesPresenter != null);
            }
        }

        internal SeriesVisualStatePresenter VisualStatePresenter { get; set; }

        internal SeriesLabelPresenter LabelPresenter
        {
            get
            {
                if (this._labelPresenter == null)
                    this._labelPresenter = this.CreateLabelPresenter();
                return this._labelPresenter;
            }
        }

        internal SeriesMarkerPresenter MarkerPresenter
        {
            get
            {
                if (this._markerPresenter == null)
                    this._markerPresenter = this.CreateMarkerPresenter();
                return this._markerPresenter;
            }
        }

        internal SeriesTooltipPresenter TooltipPresenter { get; private set; }

        internal bool SeriesCollectionChanging { get; set; }

        internal IEnumerable<DataPoint> VisibleDataPoints
        {
            get
            {
                if (this.Series != null)
                {
                    foreach (DataPoint dataPoint in this.Series.DataPoints)
                    {
                        if (this.IsDataPointVisible(dataPoint))
                            yield return dataPoint;
                    }
                }
            }
        }

        internal IEnumerable<DataPoint> DataPointsWithView
        {
            get
            {
                if (this.Series != null)
                {
                    foreach (DataPoint dataPoint in this.Series.DataPoints)
                    {
                        if (this.IsDataPointViewVisible(dataPoint))
                            yield return dataPoint;
                    }
                }
            }
        }

        internal int DefaultSimplifiedRenderingThreshold { get; set; }

        internal bool IsSimplifiedRenderingModeEnabled { get; set; }

        public event EventHandler<SeriesPresenterEventArgs> ViewCreated;

        public event EventHandler<SeriesPresenterEventArgs> ViewRemoved;

        public event EventHandler<SeriesPresenterEventArgs> ViewUpdated;

        public event EventHandler<SeriesPresenterEventArgs> Removed;

        protected SeriesPresenter(Series series)
        {
            this.Series = series;
            this.DefaultSimplifiedRenderingThreshold = 500;
            this.TooltipPresenter = new SeriesTooltipPresenter(this);
            this.Series.PropertyChanged += (sender, e) => this.OnSeriesModelPropertyChanged(e.PropertyName);
            this.Series.DataPointValueChanged += (sender, e) => this.OnSeriesDataPointValueChanged(sender as DataPoint, e.ValueName, e.OldValue, e.NewValue);
            this.VisualStatePresenter = new SeriesVisualStatePresenter(this);
        }

        private Panel CreateCanvas()
        {
            if (!this.IsRootPanelClipped)
                return new Canvas();
            return new ClippedCanvas();
        }

        internal abstract SeriesLabelPresenter CreateLabelPresenter();

        internal abstract SeriesMarkerPresenter CreateMarkerPresenter();

        public virtual void InvalidateDataPointLabel(DataPoint dataPoint)
        {
            this.LabelPresenter.OnUpdateView(dataPoint);
        }

        public virtual void InvalidateSeries()
        {
            if (this.ChartArea == null)
                return;
            this.ChartArea.UpdateSession.Update(this.Series);
        }

        public virtual void UpdateDataPoint(DataPoint dataPoint)
        {
            if (this.ChartArea == null || this.ChartArea.IsDirty)
                return;
            this.UpdateView(dataPoint);
        }

        public virtual void UpdateSeries()
        {
            if (!this._isMeasureStarted)
                return;
            this.UpdateView();
        }

        public virtual void OnSeriesAdded()
        {
            this.SeriesCollectionChanging = true;
            this.ChartArea.ActivateChildModel(this.Series);
            if (this.ChartArea.IsTemplateApplied)
            {
                this.ChartArea.UpdateSession.BeginUpdates();
                this.Series.SyncDataPoints();
                this.UpdateRelatedSeriesPresenters();
                this.Series.UpdateSelectedDataPointFlag(null);
                this.ChartArea.UpdateSession.ExecuteOnceBeforeUpdating(() => this.UpdateDataPointVisibility(), new Tuple<Series, string>(this.Series, "__UpdateDataPointVisibility__"));
                this.ChartArea.UpdateSession.Update(this.Series);
                bool enableAnimation = this.ChartArea.IsShowingAnimationEnabled;
                this.Series.DataPoints.ForEach<DataPoint>(item => this.OnDataPointAdded(item, enableAnimation));
                this.ChartArea.UpdateSession.EndUpdates();
            }
            this.SeriesCollectionChanging = false;
        }

        public virtual void OnSeriesRemoved()
        {
            this.ChartArea.UpdateSession.ExecuteIfPending(new Tuple<Series, string>(this.Series, "__OnDataPointsCollectionChanged__"));
            this.SeriesCollectionChanging = true;
            ChartArea chartArea = this.ChartArea;
            chartArea.UpdateSession.BeginUpdates();
            this._isSeriesRemoved = true;
            if (this.Series.DataPoints.Count > 0)
                this.Series.RemoveAllDataPoints();
            else
                this.OnRemoved();
            chartArea.UpdateSession.EndUpdates();
            this.SeriesCollectionChanging = false;
        }

        internal virtual void OnDataPointRemoved(DataPoint dataPoint, bool useHidingAnimation)
        {
            if (!this.IsDataPointViewVisible(dataPoint))
                return;
            dataPoint.ViewState = useHidingAnimation ? DataPointViewState.Hiding : DataPointViewState.Hidden;
        }

        internal abstract void OnDataPointAdded(DataPoint dataPoint, bool useShowingAnimation);

        internal virtual void OnDataPointViewStateChanged(DataPoint dataPoint, DataPointViewState oldValue, DataPointViewState newValue)
        {
            if (newValue == DataPointViewState.Showing)
            {
                this.CreateView(dataPoint);
                if (this.StartDataPointShowingAnimation(dataPoint))
                    return;
                dataPoint.ViewState = DataPointViewState.Normal;
            }
            else if (newValue == DataPointViewState.Normal)
            {
                if (oldValue != DataPointViewState.Hidden)
                    return;
                this.CreateView(dataPoint);
            }
            else if (newValue == DataPointViewState.Hiding)
            {
                if (this.StartDataPointHidingAnimation(dataPoint))
                    return;
                dataPoint.ViewState = DataPointViewState.Hidden;
            }
            else
            {
                if (newValue != DataPointViewState.Hidden)
                    return;
                this.RemoveView(dataPoint);
                if (this.Series.DataPoints.Contains(dataPoint))
                    return;
                this.Series.UninitializeDataPoint(dataPoint);
            }
        }

        protected virtual void UpdateView()
        {
            if (this.Series.ChartArea == null)
                return;
            this.Series.ChartArea.UpdateSession.BeginUpdates();
            this.UpdateView(this.VisibleDataPoints);
            this.Series.ChartArea.UpdateSession.EndUpdates();
        }

        protected virtual void UpdateRelatedSeriesPresenters()
        {
        }

        protected void RemoveView(IEnumerable<DataPoint> dataPoints)
        {
            dataPoints.ForEachWithIndex<DataPoint>((Action<DataPoint, int>)((item, index) => this.RemoveView(item)));
        }

        protected void UpdateView(IEnumerable<DataPoint> dataPoints)
        {
            dataPoints.ForEachWithIndex<DataPoint>((Action<DataPoint, int>)((item, index) => this.UpdateView(item)));
        }

        protected void CreateView(IEnumerable<DataPoint> dataPoints)
        {
            dataPoints.ForEachWithIndex<DataPoint>((Action<DataPoint, int>)((item, index) => this.CreateView(item)));
        }

        protected abstract FrameworkElement CreateViewElement(DataPoint dataPoint);

        protected virtual void CreateView(DataPoint dataPoint)
        {
            if (this.ChartArea == null)
                return;
            DateTime now = DateTime.Now;
            DataPointView dataPointView = new DataPointView(dataPoint);
            dataPoint.View = dataPointView;
            dataPointView.MainView = this.CreateViewElement(dataPoint);
            dataPoint.CreateNotificationBindings();
            this.BindViewToDataPoint(dataPoint, dataPointView.MainView, null);
            this.OnViewCreated(dataPoint);
            this.ChartArea.UpdateSession.Update(dataPoint);
            this.ChartArea.UpdateSession.AddCounter("SeriesPresenter.CreateView", DateTime.Now - now);
        }

        protected virtual void UpdateView(DataPoint dataPoint)
        {
            if (this.ChartArea != null)
                this.ChartArea.UpdateSession.AddCounter("SeriesPresenter.UpdateView");
            this.OnViewUpdated(dataPoint);
        }

        protected virtual void RemoveView(DataPoint dataPoint)
        {
            DateTime now = DateTime.Now;
            this.OnViewRemoved(dataPoint);
            dataPoint.View = null;
            dataPoint.ViewState = DataPointViewState.Hidden;
            if (this.ChartArea != null && this._isSeriesRemoved && !this.DataPointsWithView.Any<DataPoint>())
                this.OnRemoved();
            if (this.ChartArea == null)
                return;
            this.ChartArea.UpdateSession.AddCounter("SeriesPresenter.RemoveView", DateTime.Now - now);
        }

        internal void OnViewRemoved(DataPoint dataPoint)
        {
            if (this.ViewRemoved == null)
                return;
            this.ViewRemoved(this, new SeriesPresenterEventArgs(this.Series, dataPoint));
        }

        internal void OnViewCreated(DataPoint dataPoint)
        {
            if (this.ViewCreated == null)
                return;
            this.ViewCreated(this, new SeriesPresenterEventArgs(this.Series, dataPoint));
        }

        internal void OnViewUpdated(DataPoint dataPoint)
        {
            if (this.ViewUpdated == null)
                return;
            this.ViewUpdated(this, new SeriesPresenterEventArgs(this.Series, dataPoint));
        }

        protected abstract void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName);

        protected abstract bool StartDataPointShowingAnimation(DataPoint dataPoint);

        protected abstract bool StartDataPointHidingAnimation(DataPoint dataPoint);

        public virtual void AxisInvalidated(Axis axis)
        {
            if (this.ChartArea == null)
                return;
            this.ChartArea.UpdateSession.BeginUpdates();
            this.ChartArea.UpdateSession.Update(this.Series);
            this.ChartArea.UpdateSession.EndUpdates();
        }

        protected virtual void OnSeriesModelPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "LabelVisibility":
                    if (this.ChartArea == null)
                        break;
                    this.ChartArea.UpdateSession.Update(this.Series);
                    break;
                case "LegendText":
                    if (this.ChartArea == null)
                        break;
                    this.ChartArea.UpdateSession.ExecuteOnceAfterUpdating(() => ((XYChartArea)this.ChartArea).UpdateLegendItems(), "__UpdateLegendItems__", null);
                    break;
            }
        }

        protected virtual void OnSeriesDataPointValueChanged(DataPoint dataPoint, string valueName, object oldValue, object newValue)
        {
            if (valueName == "ViewState")
            {
                this.OnDataPointViewStateChanged(dataPoint, (DataPointViewState)oldValue, (DataPointViewState)newValue);
            }
            else
            {
                if (!this.IsDataPointViewVisible(dataPoint) || dataPoint.View == null)
                    return;
                this.BindViewToDataPoint(dataPoint, dataPoint.View.MainView, valueName);
            }
        }

        protected virtual void OnRemoved()
        {
            if (this.Removed != null)
                this.Removed(this, new SeriesPresenterEventArgs(this.Series, null));
            this.UpdateRelatedSeriesPresenters();
            this.Series.ClearSelectedDataPoints();
            ChartArea chartArea = this.Series.ChartArea;
            this.Series.ChartArea = null;
            chartArea.ChartAreaLayerProvider.RemoveLayer(this.Series);
            chartArea.ReapplyPalette();
            chartArea.DeactivateChildModel(this.Series);
        }

        internal static IEnumerable GetXValuesFromSeries(IEnumerable<XYSeries> series)
        {
            foreach (XYSeries xySeries in series)
            {
                foreach (XYDataPoint dataPoint in xySeries.DataPoints)
                    yield return dataPoint.XValue;
            }
        }

        internal virtual bool IsDataPointVisible(DataPoint dataPoint)
        {
            if (this.ChartArea != null && this.ChartArea.IsTemplateApplied)
                return dataPoint.IsVisible;
            return false;
        }

        internal virtual bool IsDataPointViewVisible(DataPoint dataPoint)
        {
            if (this.ChartArea != null && this.ChartArea.IsTemplateApplied)
                return dataPoint.ViewState != DataPointViewState.Hidden;
            return false;
        }

        internal abstract void UpdateDataPointVisibility();

        internal bool ShouldSimplifiedRenderingModeBeEnabled()
        {
            int num = this.Series.SimplifiedRenderingThreshold ?? this.DefaultSimplifiedRenderingThreshold;
            if (num == 0)
                return true;
            if (this.Series.DataPoints.Count >= num)
                return this.VisibleDataPoints.FastCount() >= num;
            return false;
        }

        internal virtual LegendItem GetLegendItem()
        {
            if (string.IsNullOrWhiteSpace(this.Series.LegendText))
                return null;
            LegendItem legendItem1 = new LegendItem();
            legendItem1.Label = this.Series.LegendText;
            legendItem1.SymbolContent = this.GetLegendSymbol();
            legendItem1.DataContext = this.Series;
            LegendItem legendItem2 = legendItem1;
            legendItem2.SetBinding(LegendItem.IsSelectedProperty, new Binding("IsLegendSelected")
            {
                Source = (object)this.Series
            });
            legendItem2.SetBinding(LegendItem.UnselectedOpacityProperty, new Binding("UnselectedDataPointOpacity")
            {
                Source = (object)this.Series
            });
            legendItem2.SetBinding(LegendItem.UnselectedEffectProperty, new Binding("UnselectedDataPointEffect")
            {
                Source = (object)this.Series
            });
            return legendItem2;
        }

        internal virtual FrameworkElement GetLegendSymbol()
        {
            return null;
        }

        internal virtual void UpdateDataPointZIndex(DataPoint dataPoint)
        {
            FrameworkElement dataPointView = SeriesVisualStatePresenter.GetDataPointView(dataPoint);
            if (dataPointView == null)
                return;
            Panel.SetZIndex(dataPointView, dataPoint.IsSelected ? Math.Max(dataPoint.ZIndex, 32766) : Math.Max(dataPoint.ZIndex, 0));
        }

        internal virtual void OnMeasureIterationStarts()
        {
            this._isMeasureStarted = true;
        }

        protected virtual int GetZIndex()
        {
            return !this.Series.IsDimmed ? 1 : 0;
        }

        internal virtual void EnsureRootPanelsCreated()
        {
            if (this.ChartArea == null)
                return;
            Panel rootPanel = this.RootPanel;
            PanelElementPool<LabelControl, DataPoint> labelsPool = this.LabelPresenter.LabelsPool;
        }

        internal virtual IEnumerable<Geometry> GetSelectionOutlines()
        {
            foreach (DataPoint visibleDataPoint in this.VisibleDataPoints)
            {
                Geometry dataPointOutline = this.GetSelectionOutline(visibleDataPoint);
                if (dataPointOutline != null)
                    yield return dataPointOutline;
            }
        }

        internal virtual Geometry GetSelectionOutline(DataPoint dataPoint)
        {
            if (dataPoint.View != null)
            {
                FrameworkElement frameworkElement = dataPoint.View.MainView ?? dataPoint.View.MarkerView;
                if (frameworkElement != null)
                {
                    Rect rect;
                    if (dataPoint.View.AnchorRectOrientation == RectOrientation.None)
                    {
                        Rect layoutSlot = LayoutInformation.GetLayoutSlot(frameworkElement);
                        Point anchorPoint = dataPoint.View.AnchorPoint;
                        rect = new Rect(anchorPoint.X - layoutSlot.Width / 2.0, anchorPoint.Y - layoutSlot.Height / 2.0, layoutSlot.Width, layoutSlot.Height);
                    }
                    else
                        rect = dataPoint.View.AnchorRect;
                    return new RectangleGeometry() { Rect = rect.TranslateToParent(frameworkElement, (FrameworkElement)this.ChartArea) };
                }
            }
            return null;
        }
    }
}
