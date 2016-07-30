using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal abstract class AxisPresenter : FrameworkElement, INotifyPropertyChanged
    {
        public static readonly Range<double> DefaultScaleRange = new Range<double>(0.0, 1.0);
        public static readonly DependencyProperty VisibilityListenerProperty = DependencyProperty.Register("VisibilityListener", typeof(Visibility), typeof(AxisPresenter), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(AxisPresenter.OnVisibilityListenerPropertyChanged)));
        internal const string VisibilityListenerPropertyName = "VisibilityListener";
        private Panel _rootPanel;
        private Range<double>? _actualScaleRange;
        private bool _axisViewChanging;

        private ChartArea ChartArea
        {
            get
            {
                return this.Axis.ChartArea;
            }
        }

        protected Panel RootPanel
        {
            get
            {
                if (this._rootPanel == null)
                    this._rootPanel = this.GetAxisPanel(AxisPresenter.AxisPanelType.AxisAlignment);
                return this._rootPanel;
            }
        }

        internal Dictionary<AxisPresenter.AxisPanelType, Func<Panel>> PanelsDictionary { get; private set; }

        public Axis Axis { get; private set; }

        public Range<double> ActualScaleRange
        {
            get
            {
                if (!this._actualScaleRange.HasValue)
                {
                    this.EnsureAxisMargins();
                    Range<double> overrideInternal = this.Axis.GetScaleRangeOverrideInternal();
                    this._actualScaleRange = !overrideInternal.HasData ? new Range<double>?(new Range<double>(AxisPresenter.DefaultScaleRange.Minimum + this.AggregatedSeriesMargins.Start, AxisPresenter.DefaultScaleRange.Maximum - this.AggregatedSeriesMargins.End)) : new Range<double>?(overrideInternal);
                }
                return this._actualScaleRange.Value;
            }
        }

        internal abstract bool ActualIsScaleReversed { get; }

        internal TickMark TickMarkForStyling { get; set; }

        public bool IsDirty { get; protected set; }

        public bool IsMinorTickMarksVisible
        {
            get
            {
                if (this.Axis.ShowMinorTickMarks == AutoBool.Auto && this.Axis.Scale != null)
                    return this.Axis.Scale.MinorTickmarkDefinition.Visibility == Visibility.Visible;
                return this.Axis.ShowMinorTickMarks == AutoBool.True;
            }
        }

        public bool IsMinorGridlinesVisible
        {
            get
            {
                if (this.Axis.ShowMinorGridlines == AutoBool.Auto && this.Axis.Scale != null)
                    return this.Axis.Scale is DateTimeScale;
                return this.Axis.ShowMinorGridlines == AutoBool.True;
            }
        }

        internal AxisMargin AggregatedSeriesMargins { get; private set; }

        protected internal Dictionary<int, AxisMargin> SeriesMarginInfos { get; set; }

        internal double ScaleViewPositionInPercent
        {
            get
            {
                if (this.Axis.Scale != null)
                {
                    Range<double> percent = this.Axis.Scale.ConvertActualViewToPercent();
                    if (percent.HasData && !double.IsNaN(percent.Minimum))
                        return percent.Minimum;
                }
                return 0.0;
            }
        }

        internal bool IsScaleZoomed
        {
            get
            {
                if (this.Axis.Scale != null)
                    return this.ScaleViewSizeInPercent.LessWithPrecision(1.0);
                return false;
            }
        }

        internal bool ShowScrollZoomBar
        {
            get
            {
                if (!this.Axis.IsAllowsAutoZoom)
                    return this.Axis.ActualShowScrollZoomBar;
                if (this.Axis.ActualShowScrollZoomBar)
                    return this.IsScaleZoomed;
                return false;
            }
        }

        public bool IsScrollZoomBarAllowsZooming
        {
            get
            {
                if (!this.Axis.IsAllowsAutoZoom)
                    return this.Axis.IsScrollZoomBarAllowsZooming;
                return !(this.Axis.Scale is CategoryScale);
            }
        }

        internal double ScaleViewSizeInPercent
        {
            get
            {
                double d = 1.0;
                if (this.Axis.Scale != null)
                {
                    Range<double> percent = this.Axis.Scale.ConvertActualViewToPercent();
                    if (percent.HasData)
                        d = percent.Size();
                }
                return Math.Min(1.0, double.IsNaN(d) ? 1.0 : d);
            }
        }

        public Visibility VisibilityListener
        {
            get
            {
                return (Visibility)this.GetValue(AxisPresenter.VisibilityListenerProperty);
            }
            set
            {
                this.SetValue(AxisPresenter.VisibilityListenerProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected AxisPresenter(Axis axis)
        {
            this.Axis = axis;
            this.PanelsDictionary = new Dictionary<AxisPresenter.AxisPanelType, Func<Panel>>();
            this.PanelsDictionary.Add(AxisPresenter.AxisPanelType.Gridlines, () => this.CreateGridView());
            this.PanelsDictionary.Add(AxisPresenter.AxisPanelType.AxisAndTickMarks, () => this.CreateAxisTickMarksView());
            this.PanelsDictionary.Add(AxisPresenter.AxisPanelType.AxisAlignment, () => (Panel)new Grid());
            this.PanelsDictionary.Add(AxisPresenter.AxisPanelType.Labels, () => this.CreateLabelsView());
            this.PanelsDictionary.Add(AxisPresenter.AxisPanelType.DisplayUnit, () => this.CreateDisplayUnitView());
            this.PanelsDictionary.Add(AxisPresenter.AxisPanelType.Title, () => this.CreateTitleView());
            this.AggregatedSeriesMargins = AxisMargin.Empty;
            this.SeriesMarginInfos = new Dictionary<int, AxisMargin>();
            TickMark tickMark = new TickMark();
            tickMark.Visibility = Visibility.Collapsed;
            this.TickMarkForStyling = tickMark;
            this.TickMarkForStyling.SetBinding(FrameworkElement.StyleProperty, new Binding("MajorTickMarkStyle")
            {
                Source = (object)this.Axis
            });
        }

        internal Panel GetAxisPanel(AxisPresenter.AxisPanelType axisPanel)
        {
            LayerType layerType = axisPanel == AxisPresenter.AxisPanelType.Gridlines ? LayerType.Gridlines : LayerType.Axis;
            if (this.Axis.ActualShowScrollZoomBar && axisPanel == AxisPresenter.AxisPanelType.AxisAndTickMarks)
                layerType = LayerType.Foreground;
            return this.ChartArea.ChartAreaLayerProvider.GetLayer(new Tuple<Axis, AxisPresenter.AxisPanelType>(this.Axis, axisPanel), (int)layerType, this.PanelsDictionary[axisPanel]);
        }

        internal void UpdateAxisZIndex()
        {
            if (this.Axis.ActualShowScrollZoomBar)
                Panel.SetZIndex(this.GetAxisPanel(AxisPresenter.AxisPanelType.AxisAndTickMarks), 1300);
            else
                Panel.SetZIndex(this.GetAxisPanel(AxisPresenter.AxisPanelType.AxisAndTickMarks), 300);
            this.UpdateView();
        }

        protected internal void EnsureAxisMargins()
        {
            if (!this.IsDirty)
                return;
            this.SeriesMarginInfos.Clear();
            AxisMargin marginResult = AxisMargin.Empty;
            if (this.Axis.Scale != null && this.Axis.Orientation == AxisOrientation.X)
            {
                this.ChartArea.FindSeries(this.Axis).GroupBy<Series, int>(series => series.ClusterKey).Select<IGrouping<int, Series>, KeyValuePair<int, AxisMargin>>(seriesGroup => new KeyValuePair<int, AxisMargin>(seriesGroup.Key, ((XYSeriesPresenter)seriesGroup.FirstOrDefault<Series>().SeriesPresenter).GetSeriesMarginInfo(this.Axis.IsMarginVisible))).ForEach<KeyValuePair<int, AxisMargin>>(info => this.SeriesMarginInfos.Add(info.Key, info.Value));
                if (!(this.Axis.Scale is CategoryScale))
                {
                    this.SeriesMarginInfos.ForEach<KeyValuePair<int, AxisMargin>>(info => marginResult = marginResult.Extend(info.Value));
                    AxisMargin axisMargin = new AxisMargin(this.Axis.Scale.ProjectedStartMargin, this.Axis.Scale.ProjectedEndMargin);
                    marginResult = new AxisMargin(Math.Max(0.0, marginResult.Start - axisMargin.Start), Math.Max(0.0, marginResult.End - axisMargin.End));
                }
            }
            this.AggregatedSeriesMargins = new AxisMargin(marginResult.Start * (1.0 - marginResult.Start), marginResult.End * (1.0 - marginResult.End));
            this.IsDirty = false;
        }

        public abstract double? ConvertDataToAxisUnits(object value);

        public abstract double? ConvertScaleToAxisUnits(double value);

        public virtual double ConvertScaleToAxisUnits(double value, double axisLength)
        {
            value = AxisPresenter.DefaultScaleRange.Project(value, this.ActualScaleRange);
            if (this.ActualIsScaleReversed)
                return axisLength - axisLength * value;
            return axisLength * value;
        }

        public virtual double ConvertAxisToScaleUnits(double value, double axisLength)
        {
            return this.ActualScaleRange.Project(value, AxisPresenter.DefaultScaleRange);
        }

        public abstract double GetClusterSize(XYSeriesPresenter presetner);

        public abstract double GetClusterSizeInScaleUnitsFromValues(IEnumerable values);

        protected virtual void OnAxisPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ShowMinorGridlines":
                case "ShowMajorGridlines":
                case "ShowLabels":
                case "ShowMajorTickMarks":
                case "ShowMinorTickMarks":
                case "CanRotateLabels":
                case "CanStaggerLabels":
                case "CanWordWrapLabels":
                case "IsScrollZoomBarAllwaysMaximized":
                    this.UpdateView();
                    break;
                case "ShowScrollZoomBar":
                    this.UpdateAxisZIndex();
                    break;
                case "ViewPositionInPercent":
                    if (this._axisViewChanging || this.Axis.Scale == null || this.Axis.Scale.IsEmpty)
                        break;
                    this.Axis.Scale.ZoomToPercent(this.Axis.ViewPositionInPercent, this.Axis.ViewPositionInPercent + this.Axis.ViewSizeInPercent);
                    break;
                case "ViewSizeInPercent":
                    if (this._axisViewChanging || this.Axis.Scale == null || this.Axis.Scale.IsEmpty)
                        break;
                    this.Axis.Scale.ZoomToPercent(this.Axis.ViewSizeInPercent);
                    break;
                case "IsReversed":
                case "IsMarginVisible":
                    if (this.ChartArea == null || !this.ChartArea.IsTemplateApplied)
                        break;
                    this.ChartArea.Update();
                    break;
            }
        }

        protected virtual void OnChartAreaPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        protected virtual void Axis_ScaleViewChanged(object sender, ScaleViewChangedArgs e)
        {
            if (this._axisViewChanging)
                return;
            try
            {
                this._axisViewChanging = true;
                this.Axis.ViewPositionInPercent = this.ScaleViewPositionInPercent;
                this.Axis.ViewSizeInPercent = this.ScaleViewSizeInPercent;
            }
            finally
            {
                this._axisViewChanging = false;
            }
        }

        protected virtual void CreateView()
        {
            this.PanelsDictionary.Keys.ForEachWithIndex<AxisPresenter.AxisPanelType>((item, index) => this.GetAxisPanel(item));
            this.SetBinding(AxisPresenter.VisibilityListenerProperty, new Binding("Visibility")
            {
                Source = (object)this.Axis
            });
        }

        protected virtual void RemoveView()
        {
            this.PanelsDictionary.Keys.ForEachWithIndex<AxisPresenter.AxisPanelType>((item, index) => this.ChartArea.ChartAreaLayerProvider.RemoveLayer((object)new Tuple<Axis, AxisPresenter.AxisPanelType>(this.Axis, item)));
        }

        protected virtual void UpdateView()
        {
            this.PanelsDictionary.Keys.Select<AxisPresenter.AxisPanelType, Panel>(panelType => this.GetAxisPanel(panelType)).OfType<IUpdatable>().ForEach<IUpdatable>(item => item.Update());
        }

        protected abstract Panel CreateAxisTickMarksView();

        protected abstract Panel CreateGridView();

        protected abstract Panel CreateTitleView();

        protected abstract Panel CreateDisplayUnitView();

        protected abstract Panel CreateLabelsView();

        public virtual void InvalidateAxis()
        {
            this.IsDirty = true;
            if (!this.ChartArea.IsTemplateApplied || this.Axis.Scale == null)
                return;
            this.UpdateView();
            this.ChartArea.Invalidate();
        }

        public virtual void OnAxisAdded()
        {
            this.Axis.PropertyChanged += new PropertyChangedEventHandler(this.OnAxisPropertyChanged);
            this.Axis.ScaleViewChanged += new EventHandler<ScaleViewChangedArgs>(this.Axis_ScaleViewChanged);
            this.ChartArea.ActivateChildModel(this.Axis);
            this.ChartArea.ActivateChildModel(this.TickMarkForStyling);
            this.ChartArea.PropertyChanged += new PropertyChangedEventHandler(this.OnChartAreaPropertyChanged);
            if (!this.ChartArea.IsTemplateApplied)
                return;
            this.CreateView();
            this.InvalidateAxis();
        }

        public virtual void OnAxisRemoved()
        {
            this.Axis.PropertyChanged -= new PropertyChangedEventHandler(this.OnAxisPropertyChanged);
            this.Axis.ScaleViewChanged -= new EventHandler<ScaleViewChangedArgs>(this.Axis_ScaleViewChanged);
            this.ChartArea.PropertyChanged -= new PropertyChangedEventHandler(this.OnChartAreaPropertyChanged);
            this.ChartArea.DeactivateChildModel(this.Axis);
            this.ChartArea.DeactivateChildModel(this.TickMarkForStyling);
            this.RemoveView();
        }

        protected virtual void AdjustScaleMaxElements()
        {
        }

        internal IEnumerable<ScaleElementDefinition> GetScaleElements()
        {
            this.EnsureAxisMargins();
            if (this.Axis.Scale != null)
                return this.Axis.Scale.ProjectElements();
            return Enumerable.Empty<ScaleElementDefinition>();
        }

        private static void OnVisibilityListenerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisPresenter)d).OnVisibilityListenerPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnVisibilityListenerPropertyChanged(object oldValue, object newValue)
        {
            if ((Visibility)newValue == Visibility.Collapsed)
                this.PanelsDictionary.Keys.Select<AxisPresenter.AxisPanelType, Panel>(panelType => this.GetAxisPanel(panelType)).OfType<UIElement>().ForEach<UIElement>(item => item.Visibility = Visibility.Collapsed);
            else
                this.PanelsDictionary.Keys.Select<AxisPresenter.AxisPanelType, Panel>(panelType => this.GetAxisPanel(panelType)).OfType<UIElement>().ForEach<UIElement>(item => item.ClearValue(UIElement.VisibilityProperty));
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        internal abstract void ResetAxisLabels();

        internal virtual void OnMeasureIterationStarts()
        {
            this._actualScaleRange = new Range<double>?();
        }

        internal enum AxisPanelType
        {
            Gridlines,
            AxisAndTickMarks,
            AxisAlignment,
            Labels,
            Title,
            DisplayUnit,
        }
    }
}
