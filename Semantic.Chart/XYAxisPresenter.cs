using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class XYAxisPresenter : AxisPresenter
    {
        public static readonly DependencyProperty ActualLocationProperty = DependencyProperty.Register("ActualLocation", typeof(Edge), typeof(XYAxisPresenter), new PropertyMetadata(Edge.Center, new PropertyChangedCallback(XYAxisPresenter.OnActualLocationPropertyChanged)));
        internal const string ActualLocationPropertyName = "ActualLocation";
        private Orientation? _actualOrientation;
        private bool? _actualIsScaleReversed;
        private double? _actualLength;

        private XYChartArea ChartArea
        {
            get
            {
                return this.Axis.ChartArea as XYChartArea;
            }
        }

        internal XYAxisGridlinesPanel MajorGrid { get; private set; }

        internal XYAxisGridlinesPanel MinorGrid { get; private set; }

        public Axis OppositeAxis
        {
            get
            {
                if (this.ChartArea == null || this.Axis.Orientation == AxisOrientation.None)
                    return null;
                AxisOrientation oppositeOrientation = this.Axis.Orientation == AxisOrientation.X ? AxisOrientation.Y : AxisOrientation.X;
                return this.ChartArea.Axes.FirstOrDefault<Axis>(axis => axis.Orientation == oppositeOrientation);
            }
        }

        internal AxisPosition ActualAxisPosition
        {
            get
            {
                AxisPosition axisPosition = this.Axis.Position;
                if (this.Axis.Position == AxisPosition.Auto)
                {
                    if (this.ChartArea != null)
                    {
                        int num = 0;
                        foreach (Axis ax in this.ChartArea.Axes)
                        {
                            if (ax.Orientation == this.Axis.Orientation && ax.Position == AxisPosition.Auto)
                                ++num;
                            if (ax == this.Axis)
                                break;
                        }
                        axisPosition = num % 2 > 0 ? AxisPosition.Near : AxisPosition.Far;
                    }
                    else
                        axisPosition = AxisPosition.Near;
                }
                if (this.OppositeAxis != null && this.OppositeAxis.IsReversed)
                    axisPosition = axisPosition == AxisPosition.Near ? AxisPosition.Far : AxisPosition.Near;
                return axisPosition;
            }
        }

        internal Orientation ActualOrientation
        {
            get
            {
                if (!this._actualOrientation.HasValue)
                    this._actualOrientation = this.ChartArea.Orientation != Orientation.Horizontal ? new Orientation?(this.Axis.Orientation == AxisOrientation.X ? Orientation.Vertical : Orientation.Horizontal) : new Orientation?(this.Axis.Orientation == AxisOrientation.X ? Orientation.Horizontal : Orientation.Vertical);
                return this._actualOrientation.Value;
            }
        }

        internal override bool ActualIsScaleReversed
        {
            get
            {
                if (!this._actualIsScaleReversed.HasValue)
                    this._actualIsScaleReversed = this.ActualOrientation != Orientation.Vertical ? new bool?(this.Axis.IsReversed) : new bool?(this.Axis.Scale is ICategoryScale ? !this.Axis.IsReversed : this.Axis.IsReversed);
                return this._actualIsScaleReversed.Value;
            }
        }

        public Edge ActualLocation
        {
            get
            {
                return (Edge)this.GetValue(XYAxisPresenter.ActualLocationProperty);
            }
            set
            {
                this.SetValue(XYAxisPresenter.ActualLocationProperty, value);
            }
        }

        public double ActualLength
        {
            get
            {
                if (!this._actualLength.HasValue)
                    this._actualLength = new double?(this.GetLength(new Size(this.RootPanel.ActualWidth, this.RootPanel.ActualHeight)));
                return this._actualLength.Value;
            }
        }

        public XYAxisPresenter(Axis axis)
          : base(axis)
        {
        }

        private static void OnActualLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((XYAxisPresenter)d).OnActualLocationPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualLocationPropertyChanged(object oldValue, object newValue)
        {
            this.OnLocationChanged();
        }

        private void OnLocationChanged()
        {
            EdgePanel.SetEdge(this.GetAxisPanel(AxisPresenter.AxisPanelType.AxisAndTickMarks), this.ActualLocation);
            EdgePanel.SetEdge(this.GetAxisPanel(AxisPresenter.AxisPanelType.Labels), this.ActualLocation);
            EdgePanel.SetEdge(this.GetAxisPanel(AxisPresenter.AxisPanelType.Title), this.ActualLocation);
            EdgePanel.SetEdge(this.GetAxisPanel(AxisPresenter.AxisPanelType.DisplayUnit), this.ActualLocation);
            this.OnPropertyChanged("ActualLocation");
        }

        public double GetLength(Size availableSize)
        {
            if (this.ActualOrientation == Orientation.Horizontal)
                return availableSize.Width;
            if (this.ActualOrientation == Orientation.Vertical)
                return availableSize.Height;
            return 0.0;
        }

        private void UpdateLocation()
        {
            if (this.ActualOrientation == Orientation.Vertical)
            {
                if (this.ActualAxisPosition == AxisPosition.Near)
                {
                    this.ActualLocation = Edge.Left;
                }
                else
                {
                    if (this.ActualAxisPosition != AxisPosition.Far)
                        return;
                    this.ActualLocation = Edge.Right;
                }
            }
            else
            {
                if (this.ActualOrientation != Orientation.Horizontal)
                    return;
                if (this.ActualAxisPosition == AxisPosition.Near)
                {
                    this.ActualLocation = Edge.Bottom;
                }
                else
                {
                    if (this.ActualAxisPosition != AxisPosition.Far)
                        return;
                    this.ActualLocation = Edge.Top;
                }
            }
        }

        protected override Panel CreateAxisTickMarksView()
        {
            return new XYAxisTickMarksPanel(this);
        }

        protected override Panel CreateGridView()
        {
            return new XYAxisGridlinesPanel(this);
        }

        protected override Panel CreateTitleView()
        {
            UpdatableGrid updatableGrid = new UpdatableGrid();
            updatableGrid.Children.Add(new XYAxisTitle(this));
            return updatableGrid;
        }

        protected override Panel CreateDisplayUnitView()
        {
            UpdatableGrid updatableGrid = new UpdatableGrid();
            XYAxisDisplayUnitTitle displayUnitTitle = new XYAxisDisplayUnitTitle(this);
            updatableGrid.Children.Add(displayUnitTitle);
            return updatableGrid;
        }

        protected override Panel CreateLabelsView()
        {
            return new XYLabelsPanel(this);
        }

        protected override void UpdateView()
        {
            if (this.ChartArea.IsTemplateApplied)
                this.UpdateLocation();
            base.UpdateView();
        }

        internal override void OnMeasureIterationStarts()
        {
            base.OnMeasureIterationStarts();
            this._actualLength = new double?();
            this._actualIsScaleReversed = new bool?();
            this._actualOrientation = new Orientation?();
        }

        public override double? ConvertDataToAxisUnits(object value)
        {
            if (this.Axis.Scale != null && this.ChartArea != null)
                return new double?(this.ConvertScaleToAxisUnits(this.Axis.Scale.ProjectDataValue(value), this.ActualLength));
            return new double?();
        }

        public override double? ConvertScaleToAxisUnits(double value)
        {
            if (this.ChartArea != null)
                return new double?(this.ConvertScaleToAxisUnits(value, this.ActualLength));
            return new double?();
        }

        public override double ConvertScaleToAxisUnits(double value, double axisLength)
        {
            if (this.ActualOrientation == Orientation.Vertical)
                return axisLength - base.ConvertScaleToAxisUnits(value, axisLength);
            return base.ConvertScaleToAxisUnits(value, axisLength);
        }

        public override double ConvertAxisToScaleUnits(double value, double axisLength)
        {
            if (this.ActualOrientation == Orientation.Vertical)
                return 1.0 - base.ConvertAxisToScaleUnits(value, axisLength);
            return base.ConvertAxisToScaleUnits(value, axisLength);
        }

        public override double GetClusterSizeInScaleUnitsFromValues(IEnumerable values)
        {
            if (this.Axis.Scale != null)
                return this.Axis.Scale.ProjectClusterSize(values);
            return 0.0;
        }

        public override double GetClusterSize(XYSeriesPresenter presenter)
        {
            this.EnsureAxisMargins();
            double num = 0.0;
            if (this.SeriesMarginInfos.ContainsKey(presenter.Series.ClusterKey))
                num = this.SeriesMarginInfos[presenter.Series.ClusterKey].Start;
            if (num == 0.0 && this.Axis.IsMarginVisible != AutoBool.True)
                num = presenter.GetSeriesMarginInfo(AutoBool.True).Start;
            if (num > 0.0)
                return Math.Abs(this.ConvertScaleToAxisUnits(num, this.ActualLength) - this.ConvertScaleToAxisUnits(2.0 * num, this.ActualLength));
            return 0.0;
        }

        internal override void ResetAxisLabels()
        {
            if (this.ChartArea == null || !this.ChartArea.IsTemplateApplied)
                return;
            XYLabelsPanel xyLabelsPanel = this.GetAxisPanel(AxisPresenter.AxisPanelType.Labels) as XYLabelsPanel;
            if (xyLabelsPanel == null)
                return;
            xyLabelsPanel.ResetAxisLabels();
        }

        protected override void OnChartAreaPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnChartAreaPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "Orientation":
                    this._actualOrientation = new Orientation?();
                    this.ResetAxisLabels();
                    if (this.Axis.Scale == null)
                        break;
                    this.Axis.Scale.InvalidateView();
                    this.Axis.Scale.Invalidate();
                    break;
            }
        }

        protected override void OnAxisPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnAxisPropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "IsReversed":
                case "Orientation":
                    this._actualOrientation = new Orientation?();
                    this._actualIsScaleReversed = new bool?();
                    if (this.Axis.Scale == null)
                        break;
                    this.Axis.Scale.InvalidateView();
                    this.Axis.Scale.Invalidate();
                    break;
            }
        }
    }
}
