using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class EdgePanel : Panel
    {
        public static readonly DependencyProperty EdgeProperty = DependencyProperty.RegisterAttached("Edge", typeof(Edge), typeof(EdgePanel), new PropertyMetadata(Edge.Center, new PropertyChangedCallback(EdgePanel.OnEdgePropertyChanged)));
        public static readonly DependencyProperty CenterMarginProperty = DependencyProperty.Register("CenterMargin", typeof(Thickness), typeof(EdgePanel), new PropertyMetadata(new Thickness(0.0), new PropertyChangedCallback(EdgePanel.OnCenterMarginPropertyChanged)));
        public static readonly DependencyProperty ActualCenterMarginProperty = DependencyProperty.Register("ActualCenterMargin", typeof(Thickness), typeof(EdgePanel), new PropertyMetadata(new Thickness(0.0), new PropertyChangedCallback(EdgePanel.OnActualCenterMarginPropertyChanged)));
        private Thickness _centerMargins = new Thickness(0.0);
        private Size _previousArrangeSize = Size.Empty;
        private Size _previousMeasureConstraint = Size.Empty;
        private Size _lastMeasuredResult = Size.Empty;
        internal const int MinimumPanelSize = 16;
        internal const double MinimumRelativeCenterSpace = 0.35;
        internal const string EdgePropertyName = "Edge";
        internal const string CenterMarginPropertyName = "CenterMargin";
        public const string ActualCenterMarginPropertyName = "ActualCenterMargin";
        private const int MaximumMeasureIterations = 3;
        private bool _autoMeasure;
        private int _arrangeWitnNoMeasureCount;
        private int _measureWitnNoArrangeCount;

        internal ChartArea ChartArea { get; set; }

        private int SeriesCount
        {
            get
            {
                if (this.ChartArea != null)
                    return this.ChartArea.GetSeries().FastCount();
                return 0;
            }
        }

        public Thickness CenterMargin
        {
            get
            {
                return (Thickness)this.GetValue(EdgePanel.CenterMarginProperty);
            }
            set
            {
                this.SetValue(EdgePanel.CenterMarginProperty, value);
            }
        }

        public Thickness ActualCenterMargin
        {
            get
            {
                return (Thickness)this.GetValue(EdgePanel.ActualCenterMarginProperty);
            }
            private set
            {
                this.SetValue(EdgePanel.ActualCenterMarginProperty, value);
            }
        }

        internal bool IsDirty { get; private set; }

        public event EventHandler ArrangeComplete;

        public static Edge GetEdge(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (Edge)element.GetValue(EdgePanel.EdgeProperty);
        }

        public static void SetEdge(UIElement element, Edge value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(EdgePanel.EdgeProperty, value);
        }

        private static void OnEdgePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EdgePanel.InvalidateParentArrange(d);
        }

        private static void OnCenterMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EdgePanel)d).OnCenterMarginPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnCenterMarginPropertyChanged(object oldValue, object newValue)
        {
            this.Invalidate();
        }

        private static void OnActualCenterMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EdgePanel)d).OnActualCenterMarginPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualCenterMarginPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void InvalidateParentArrange(DependencyObject d)
        {
            UIElement uiElement = d as UIElement;
            if (uiElement == null)
                return;
            EdgePanel edgePanel = VisualTreeHelper.GetParent(uiElement) as EdgePanel;
            if (edgePanel == null)
                return;
            edgePanel.InvalidateArrange();
        }

        private bool IsCenterMarginSet()
        {
            if (this.CenterMargin.Height() <= 0.0)
                return this.CenterMargin.Width() > 0.0;
            return true;
        }

        private bool IsCenterMarginApplies(UIElement child)
        {
            switch (EdgePanel.GetEdge(child))
            {
                case Edge.Left:
                case Edge.Right:
                    return this.CenterMargin.Width() > 0.0;
                case Edge.Bottom:
                case Edge.Top:
                    return this.CenterMargin.Height() > 0.0;
                default:
                    return false;
            }
        }

        private bool IsTakeSpace(UIElement child)
        {
            XYAxisElementsPanel axisElementsPanel = child as XYAxisElementsPanel;
            if (axisElementsPanel != null)
            {
                if (child is XYLabelsPanel)
                    return axisElementsPanel.Axis.ShowLabels;
                if (child is XYAxisTickMarksPanel)
                {
                    if (!axisElementsPanel.Axis.ShowMajorTickMarks && !axisElementsPanel.Presenter.IsMinorTickMarksVisible)
                        return axisElementsPanel.Axis.ActualShowScrollZoomBar;
                    return true;
                }
            }
            if (child is XYAxisTitle)
                return ((Title)child).ActualContent != null;
            return true;
        }

        private double GetMinimumRelativeCenterSpace()
        {
            return this.SeriesCount <= 0 ? 1.0 : 0.35;
        }

        private Size GetChildConstraintSize(UIElement child, Size availableSize, Thickness edgeMargins)
        {
            Size size1 = availableSize;
            Size size2 = new Size(Math.Max(0.0, availableSize.Width - edgeMargins.Width()), Math.Max(0.0, availableSize.Height - edgeMargins.Height()));
            Thickness thickness = new Thickness(double.PositiveInfinity);
            if (!double.IsPositiveInfinity(availableSize.Width))
                thickness.Left = thickness.Right = availableSize.Width * this.GetMinimumRelativeCenterSpace();
            if (!double.IsPositiveInfinity(availableSize.Height))
                thickness.Top = thickness.Bottom = availableSize.Height * this.GetMinimumRelativeCenterSpace();
            switch (EdgePanel.GetEdge(child))
            {
                case Edge.Left:
                    size1 = new Size(thickness.Left, size2.Height);
                    break;
                case Edge.Right:
                    size1 = new Size(thickness.Right, size2.Height);
                    break;
                case Edge.Bottom:
                    size1 = new Size(size2.Width, thickness.Bottom);
                    break;
                case Edge.Top:
                    size1 = new Size(size2.Width, thickness.Top);
                    break;
                case Edge.Center:
                    size1 = size2;
                    break;
            }
            return size1;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.IsDirty = true;
            Size availableSize1 = constraint;
            if (double.IsNaN(this.Height) && availableSize1.Height == 0.0)
                availableSize1.Height = double.PositiveInfinity;
            if (double.IsNaN(this.Width) && availableSize1.Width == 0.0)
                availableSize1.Width = double.PositiveInfinity;
            ++this._measureWitnNoArrangeCount;
            if (this._measureWitnNoArrangeCount > 6)
                return this._lastMeasuredResult;
            if (this._autoMeasure)
            {
                if (!this._previousArrangeSize.IsEmpty)
                {
                    availableSize1.Height = !double.IsInfinity(availableSize1.Height) ? Math.Max(availableSize1.Height, this._previousArrangeSize.Height) : this._previousArrangeSize.Height;
                    availableSize1.Width = !double.IsInfinity(availableSize1.Width) ? Math.Max(availableSize1.Width, this._previousArrangeSize.Width) : this._previousArrangeSize.Width;
                }
                this._autoMeasure = false;
            }
            foreach (Series series in this.ChartArea.GetSeries())
                series.SeriesPresenter.EnsureRootPanelsCreated();
            IEnumerable<IGrouping<Edge, UIElement>> groupings = this.Children.OfType<UIElement>().Where<UIElement>(child => child != null).OrderBy<UIElement, Edge>(child => EdgePanel.GetEdge(child)).GroupBy<UIElement, Edge>(child => EdgePanel.GetEdge(child)).Select<IGrouping<Edge, UIElement>, IGrouping<Edge, UIElement>>(edgeGroups => edgeGroups);
            this._centerMargins = new Thickness(0.0);
            int num1 = 0;
            Thickness edgeMargins = new Thickness(availableSize1.Width, availableSize1.Height, availableSize1.Width, availableSize1.Height);
            bool resetScrollingInfo = this._previousMeasureConstraint != constraint;
            int num2 = this.Children.OfType<IRequireMarginOnEdges>().Count<IRequireMarginOnEdges>();
            foreach (IRequireMarginOnEdges requireMarginOnEdges in this.Children.OfType<IRequireMarginOnEdges>())
                requireMarginOnEdges.ResetRequiredMargin(resetScrollingInfo);
            this._previousMeasureConstraint = constraint;
            double width;
            double height;
            do
            {
                this.ChartArea.OnMeasureIterationStarts();
                Thickness margin1 = new Thickness(0.0);
                Size size = new Size(0.0, 0.0);
                Thickness margin2 = new Thickness(0.0);
                foreach (IEnumerable<UIElement> source in groupings)
                {
                    foreach (UIElement uiElement in source.OrderBy<UIElement, int>(element => !(element is XYAxisBasePanel) ? 1 : 0).ToList<UIElement>())
                    {
                        Size availableSize2 = num1 != 0 ? this.GetChildConstraintSize(uiElement, availableSize1, edgeMargins) : new Size(Math.Max(0.0, availableSize1.Width - margin1.Width()), Math.Max(0.0, availableSize1.Height - margin1.Height()));
                        IRequireMarginOnEdges requireMarginOnEdges1 = uiElement as IRequireMarginOnEdges;
                        if (requireMarginOnEdges1 != null && num1 > 0 && resetScrollingInfo)
                            requireMarginOnEdges1.ScheduleUpdate();
                        uiElement.Measure(availableSize2);
                        Size desiredSize = uiElement.DesiredSize;
                        if (EdgePanel.GetEdge(uiElement) != Edge.Center && !this.IsCenterMarginApplies(uiElement) && this.IsTakeSpace(uiElement))
                        {
                            IRequireMarginOnEdges requireMarginOnEdges2 = uiElement as IRequireMarginOnEdges;
                            if (requireMarginOnEdges2 != null)
                                margin2 = margin2.Inflate(requireMarginOnEdges2.RequiredMargin);
                            switch (EdgePanel.GetEdge(uiElement))
                            {
                                case Edge.Left:
                                    margin1.Left += desiredSize.Width;
                                    size.Height = Math.Max(size.Height, margin1.Height() + desiredSize.Height);
                                    continue;
                                case Edge.Right:
                                    margin1.Right += desiredSize.Width;
                                    size.Height = Math.Max(size.Height, margin1.Height() + desiredSize.Height);
                                    continue;
                                case Edge.Bottom:
                                    margin1.Bottom += desiredSize.Height;
                                    size.Width = Math.Max(size.Width, margin1.Width() + desiredSize.Width);
                                    continue;
                                case Edge.Top:
                                    margin1.Top += desiredSize.Height;
                                    size.Width = Math.Max(size.Width, margin1.Width() + desiredSize.Width);
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
                margin1 = this.ApplyCenterMargins(margin1.Union(margin2));
                size.Width = Math.Max(size.Width, margin1.Width());
                size.Height = Math.Max(size.Height, margin1.Height());
                width = size.Width;
                height = size.Height;
                this._centerMargins = margin1;
                if (!(edgeMargins == this._centerMargins) && !this.IsCenterMarginSet() && num2 > 1)
                    edgeMargins = this._centerMargins;
                else
                    break;
            }
            while (++num1 <= 3);
            this._lastMeasuredResult = new Size(Math.Max(16.0, width), Math.Max(16.0, height));
            return this._lastMeasuredResult;
        }

        private Thickness ApplyCenterMargins(Thickness calculatingMargins)
        {
            if (this.IsCenterMarginSet())
            {
                Thickness centerMargin = this.CenterMargin;
                if (centerMargin.Width() > 0.0 && centerMargin.Height() == 0.0)
                {
                    centerMargin.Bottom = calculatingMargins.Bottom;
                    centerMargin.Top = calculatingMargins.Top;
                }
                if (centerMargin.Width() == 0.0 && centerMargin.Height() > 0.0)
                {
                    centerMargin.Left = calculatingMargins.Left;
                    centerMargin.Right = calculatingMargins.Right;
                }
                calculatingMargins = centerMargin;
            }
            return calculatingMargins;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (!this.IsDirty && !this._previousArrangeSize.IsEmpty && (arrangeSize != this._previousArrangeSize && this._arrangeWitnNoMeasureCount < 2))
            {
                ++this._arrangeWitnNoMeasureCount;
                this._autoMeasure = true;
                this._previousArrangeSize = arrangeSize;
                this.InvalidateMeasure();
                return arrangeSize;
            }
            this._measureWitnNoArrangeCount = 0;
            this._arrangeWitnNoMeasureCount = 0;
            this._previousArrangeSize = arrangeSize;
            if (this.ChartArea.UpdateSession.IsUpdating)
            {
                this.Dispatcher.BeginInvoke(new Action(((UIElement)this).InvalidateArrange));
                return arrangeSize;
            }
            double num1 = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            Thickness thickness = this._centerMargins;
            Rect finalRect1 = new Rect(thickness.Left, thickness.Top, Math.Max(0.0, arrangeSize.Width - (thickness.Left + thickness.Right)), Math.Max(0.0, arrangeSize.Height - (thickness.Top + thickness.Bottom)));
            foreach (UIElement element in this.Children.OfType<UIElement>())
            {
                if (element != null && !(element is AnchorPanel))
                {
                    Size desiredSize = element.DesiredSize;
                    Rect finalRect2 = Rect.Empty;
                    switch (EdgePanel.GetEdge(element))
                    {
                        case Edge.Left:
                            num1 += desiredSize.Width;
                            finalRect2 = new Rect(finalRect1.X - num1, finalRect1.Y, desiredSize.Width, finalRect1.Height);
                            break;
                        case Edge.Right:
                            finalRect2 = new Rect(finalRect1.Right + num3, finalRect1.Y, desiredSize.Width, finalRect1.Height);
                            num3 += desiredSize.Width;
                            break;
                        case Edge.Bottom:
                            finalRect2 = new Rect(finalRect1.X, finalRect1.Bottom + num4, finalRect1.Width, desiredSize.Height);
                            num4 += desiredSize.Height;
                            break;
                        case Edge.Top:
                            num2 += desiredSize.Height;
                            finalRect2 = new Rect(finalRect1.X, finalRect1.Top - num2, finalRect1.Width, desiredSize.Height);
                            break;
                        case Edge.Center:
                            finalRect2 = finalRect1;
                            break;
                    }
                    element.Arrange(finalRect2);
                }
            }
            this.ActualCenterMargin = thickness;
            if (this.IsDirty)
            {
                this.IsDirty = false;
                this.OnArrangeComplete();
            }
            foreach (UIElement uiElement in this.Children.OfType<UIElement>())
            {
                if (uiElement is AnchorPanel)
                    uiElement.Arrange(finalRect1);
            }
            return arrangeSize;
        }

        internal void Invalidate()
        {
            this.IsDirty = true;
            this.InvalidateMeasure();
        }

        protected virtual void OnArrangeComplete()
        {
            if (this.ArrangeComplete == null)
                return;
            this.ArrangeComplete(this, EventArgs.Empty);
        }
    }
}
