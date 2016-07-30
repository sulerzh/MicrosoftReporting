using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class XYAxisTickMarksPanel : XYAxisElementsPanel
    {
        private PanelElementPool<RotatableControl, Axis> _majorTickMarkPool;
        private PanelElementPool<RotatableControl, Axis> _minorTickMarkPool;
        private Line _axisLine;
        private ScrollZoomBar _scrollBar;
        private bool _scrollBarScaleChanging;

        private bool IsScrollBarReversed
        {
            get
            {
                if (this.Presenter.ActualOrientation != Orientation.Vertical)
                    return this.Presenter.ActualIsScaleReversed;
                return !this.Presenter.ActualIsScaleReversed;
            }
        }

        internal Line AxisLine
        {
            get
            {
                return this._axisLine;
            }
        }

        private bool LabelsUseCategoryTickmarks
        {
            get
            {
                XYLabelsPanel xyLabelsPanel = this.Presenter.GetAxisPanel(AxisPresenter.AxisPanelType.Labels) as XYLabelsPanel;
                if (xyLabelsPanel != null)
                    return xyLabelsPanel.IsGrouped;
                return false;
            }
        }

        internal XYAxisTickMarksPanel(XYAxisPresenter presenter)
          : base(presenter)
        {
            this._majorTickMarkPool = new PanelElementPool<RotatableControl, Axis>(this, () => this.CreateTickMark(true), (item, axis) => this.PrepareTickMark(item), null)
            {
                ReservedTopElements = 1
            };
            this._minorTickMarkPool = new PanelElementPool<RotatableControl, Axis>(this, () => this.CreateTickMark(false), (item, axis) => this.PrepareTickMark(item), null)
            {
                ReservedTopElements = 1
            };
            this._axisLine = this.CreateAxisLine();
            this._scrollBar = this.CreateScrollBar();
            this._scrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.ScrollBar_ValueChanged);
            this._scrollBar.ViewportSizeChangeCompleted += new RoutedPropertyChangedEventHandler<double>(this.ScrollBar_ViewportSizeDragCompleted);
            this.Axis.ScaleChanged += new EventHandler(this.Axis_ScaleChanged);
        }

        private void Axis_ScaleChanged(object sender, EventArgs e)
        {
            if (this._scrollBarScaleChanging)
                return;
            this._scrollBar.Minimum = 0.0;
            this._scrollBar.Maximum = 1.0;
            this._scrollBar.ViewportSize = this.Presenter.ScaleViewSizeInPercent;
            this._scrollBar.Value = Math.Max(0.0, Math.Min(this.Presenter.ScaleViewPositionInPercent, this._scrollBar.Maximum));
            this._scrollBar.SmallChange = this._scrollBar.ViewportSize / 50.0;
            this._scrollBar.LargeChange = this._scrollBar.ViewportSize;
            if (this.Axis.Scale != null && this.Axis.Scale.ActualZoomRange.HasData)
                this._scrollBar.MinimumViewportSize = 1.0 / this.Axis.Scale.ActualZoomRange.Maximum;
            else
                this._scrollBar.ClearValue(ScrollZoomBar.MinimumViewportSizeProperty);
            this._scrollBar.InvalidateArrange();
            this.Presenter.ResetAxisLabels();
        }

        private void ScrollBar_ViewportSizeDragCompleted(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._scrollBar.IsResizing)
                return;
            this._scrollBarScaleChanging = true;
            this.Presenter.Axis.ChartArea.BeginInit();
            try
            {
                this.Axis.Scale.ZoomToPercent(this.GetScrollBarValue(), this.GetScrollBarValue() + this._scrollBar.ViewportSize);
                this._scrollBar.SmallChange = this._scrollBar.ViewportSize / 50.0;
                this._scrollBar.LargeChange = this._scrollBar.ViewportSize;
                this.Presenter.ResetAxisLabels();
            }
            finally
            {
                this.Presenter.Axis.ChartArea.EndInit();
                this._scrollBarScaleChanging = false;
            }
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this._scrollBar.IsResizing)
                return;
            this._scrollBarScaleChanging = true;
            this.Axis.Scale.ScrollToPercent(this.GetScrollBarValue());
            this._scrollBarScaleChanging = false;
        }

        private double GetScrollBarValue()
        {
            return this.GetScrollBarValue(this._scrollBar.Value);
        }

        private double GetScrollBarValue(double value)
        {
            if (this.IsScrollBarReversed)
                return Math.Max(0.0, Math.Min(1.0 - value - this._scrollBar.ViewportSize, 1.0));
            return value;
        }

        private ScrollZoomBar CreateScrollBar()
        {
            ScrollZoomBar scrollZoomBar = new ScrollZoomBar();
            scrollZoomBar.SetBinding(FrameworkElement.StyleProperty, new Binding("ScrollZoomBarStyle")
            {
                Source = (object)this.Axis
            });
            scrollZoomBar.SetBinding(ScrollZoomBar.IsAllwaysMaximizedProperty, new Binding("IsScrollZoomBarAllwaysMaximized")
            {
                Source = (object)this.Axis
            });
            scrollZoomBar.SetBinding(ScrollZoomBar.ResizeViewHandlesVisibilityProperty, new Binding("ActualIsZoomEnabled")
            {
                Source = (object)this.Axis,
                Converter = (IValueConverter)new BooleanToVisibilityConverter()
            });
            this.Children.Add(scrollZoomBar);
            return scrollZoomBar;
        }

        private Line CreateAxisLine()
        {
            Line line1 = new Line();
            line1.X2 = 1.0;
            line1.Y2 = 1.0;
            line1.Stretch = Stretch.Fill;
            Line line2 = line1;
            line2.UseLayoutRounding = true;
            line2.SetBinding(FrameworkElement.StyleProperty, new Binding("LineStyle")
            {
                Source = (object)this.Axis
            });
            this.UpdateAxisLineVisibility();
            this.Children.Add(line2);
            return line2;
        }

        private void UpdateAxisLineVisibility()
        {
            if (this.AxisLine == null)
                return;
            if (this.Presenter != null && this.Presenter.OppositeAxis != null && (this.Presenter.OppositeAxis.Scale != null && this.Presenter.OppositeAxis.Scale.HasCustomCrossingPosition))
                this.AxisLine.Visibility = Visibility.Collapsed;
            else
                this.AxisLine.SetBinding(UIElement.VisibilityProperty, new Binding("ShowAxisLine")
                {
                    Source = (object)this.Axis,
                    Converter = (IValueConverter)new BooleanToVisibilityConverter()
                });
        }

        private RotatableControl CreateTickMark(bool major)
        {
            RotatableControl result = new RotatableControl();
            TickMark tickMark = new TickMark();
            tickMark.SetBinding(FrameworkElement.StyleProperty, new Binding(major ? "MajorTickMarkStyle" : "MinorTickMarkStyle")
            {
                Source = (object)this.Axis
            });
            result.Child = tickMark;
            tickMark.PositionChanged += (s, e) =>
           {
               this.PrepareTickMark(result);
               XYAxisElementsPanel.SetPosition((UIElement)result, ((TickMark)s).Position);
           };
            return result;
        }

        private void PrepareTickMark(RotatableControl item)
        {
            double num = 0.0;
            switch (this.Orientation)
            {
                case Orientation.Horizontal:
                    num = this.IsInverted ? 180.0 : 0.0;
                    break;
                case Orientation.Vertical:
                    num = this.IsInverted ? 90.0 : -90.0;
                    break;
            }
            if (((TickMark)item.Child).Position == AxisElementPosition.Inside)
                num += 180.0;
            if (item.RotationAngle == num)
                return;
            item.RotationAngle = num;
        }

        private void PrepareAxisLine()
        {
            Line line = this._axisLine;
            line.X1 = line.X2 = line.Y1 = line.Y2 = 0.0;
            if (this.Orientation == Orientation.Horizontal)
                line.X2 = 1.0;
            else
                line.Y2 = 1.0;
            this.UpdateAxisLineVisibility();
            XYAxisElementsPanel.SetPosition(this._axisLine, AxisElementPosition.Cross);
        }

        private void PrepareScrollBar()
        {
            XYAxisElementsPanel.SetPosition(this._scrollBar, AxisElementPosition.Outside);
            this._scrollBar.Orientation = this.Orientation;
            if (this.Orientation == Orientation.Horizontal)
            {
                this._scrollBar.VerticalAlignment = this.Presenter.ActualLocation == Edge.Top ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                this._scrollBar.HorizontalAlignment = HorizontalAlignment.Stretch;
            }
            else
            {
                this._scrollBar.HorizontalAlignment = this.Presenter.ActualLocation == Edge.Left ? HorizontalAlignment.Left : HorizontalAlignment.Right;
                this._scrollBar.VerticalAlignment = VerticalAlignment.Stretch;
            }
            if (this.Presenter.Axis.ActualShowScrollZoomBar && this._scrollBar.Visibility == Visibility.Collapsed)
            {
                this._scrollBar.Visibility = Visibility.Visible;
            }
            else
            {
                if (this.Presenter.Axis.ActualShowScrollZoomBar || this._scrollBar.Visibility != Visibility.Visible)
                    return;
                this._scrollBar.Visibility = Visibility.Collapsed;
            }
        }

        protected override double ElementOffset(UIElement element)
        {
            double num = base.ElementOffset(element);
            if (XYAxisElementsPanel.GetPosition(element) == AxisElementPosition.Outside && element != this._scrollBar)
                num += this.ElementHeight(this._scrollBar.DesiredSize);
            return num;
        }

        protected override void ArrangeChild(UIElement child, Rect rect, Size finalSize)
        {
            if (child == this._axisLine)
            {
                double num = this.IsInverted ? this.ElementHeight(finalSize) : 0.0;
                rect = this.Orientation != Orientation.Horizontal ? new Rect(num - rect.Width / 2.0, 0.0, rect.Width, finalSize.Height) : new Rect(0.0, num - rect.Height / 2.0, finalSize.Width, rect.Height);
            }
            if (child == this._scrollBar)
            {
                double num = this.IsInverted ? 1.0 : 0.0;
                rect = this.Orientation != Orientation.Horizontal ? new Rect(rect.X + num, 0.0, rect.Width, finalSize.Height) : new Rect(0.0, rect.Y + num, finalSize.Width, rect.Height);
            }
            base.ArrangeChild(child, rect, finalSize);
        }

        protected override void Populate(double availableLength)
        {
            this._majorTickMarkPool.ReleaseAll();
            this._minorTickMarkPool.ReleaseAll();
            if (this.Presenter.IsMinorTickMarksVisible)
                this._majorTickMarkPool.AdjustPoolSize();
            try
            {
                this.PrepareAxisLine();
                this.PrepareScrollBar();
                if (!this.Axis.ShowMajorTickMarks && !this.Presenter.IsMinorTickMarksVisible)
                    return;
                foreach (ScaleElementDefinition elementDefinition in new List<ScaleElementDefinition>((IEnumerable<ScaleElementDefinition>)this.Presenter.GetScaleElements().Where<ScaleElementDefinition>((Func<ScaleElementDefinition, bool>)(p => p.Kind == ScaleElementKind.Tickmark)).OrderBy<ScaleElementDefinition, int>((Func<ScaleElementDefinition, int>)(p => p.Group != ScaleElementGroup.Major ? 0 : 1))))
                {
                    if (elementDefinition.Group == ScaleElementGroup.Major && this.Axis.ShowMajorTickMarks && !this.LabelsUseCategoryTickmarks)
                        elementDefinition.Positions.Where<ScalePosition>(p =>
                       {
                           if (p.Position >= 0.0)
                               return p.Position <= 1.0;
                           return false;
                       }).ForEachWithIndex<ScalePosition>((position, index) =>
                       {
                           RotatableControl rotatableControl = this._majorTickMarkPool.Get(this.Axis);
                           XYAxisElementsPanel.SetCoordinate((UIElement)rotatableControl, position.Position);
                           XYAxisElementsPanel.SetPosition((UIElement)rotatableControl, ((TickMark)rotatableControl.Child).Position);
                       });
                    if (elementDefinition.Group == ScaleElementGroup.Minor && this.Presenter.IsMinorTickMarksVisible)
                        elementDefinition.Positions.Where<ScalePosition>(p =>
                       {
                           if (p.Position.GreaterOrEqualWithPrecision(0.0))
                               return p.Position.LessOrEqualWithPrecision(1.0);
                           return false;
                       }).ForEachWithIndex<ScalePosition>((position, index) =>
                       {
                           RotatableControl rotatableControl = this._minorTickMarkPool.Get(this.Axis);
                           XYAxisElementsPanel.SetCoordinate((UIElement)rotatableControl, position.Position);
                           XYAxisElementsPanel.SetPosition((UIElement)rotatableControl, ((TickMark)rotatableControl.Child).Position);
                       });
                }
            }
            finally
            {
                if (!this.Presenter.IsMinorTickMarksVisible)
                    this._majorTickMarkPool.AdjustPoolSize();
                this._minorTickMarkPool.AdjustPoolSize();
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }
    }
}
