using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public sealed class ChartAreaAutomationPeer : FrameworkElementAutomationPeer, ISelectionProvider, IScrollProvider, ITransformProvider
    {
        private ChartArea ChartArea
        {
            get
            {
                return (ChartArea)this.Owner;
            }
        }

        private XYChartArea XYChartArea
        {
            get
            {
                return this.Owner as XYChartArea;
            }
        }

        bool ISelectionProvider.CanSelectMultiple
        {
            get
            {
                return true;
            }
        }

        bool ISelectionProvider.IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        private Axis HorizontalAxis
        {
            get
            {
                if (this.XYChartArea != null)
                    return this.XYChartArea.Axes.FirstOrDefault<Axis>(axis =>
                   {
                       if (axis.Orientation == AxisOrientation.X && this.XYChartArea.Orientation == Orientation.Horizontal)
                           return true;
                       if (axis.Orientation == AxisOrientation.Y)
                           return this.XYChartArea.Orientation == Orientation.Vertical;
                       return false;
                   });
                return null;
            }
        }

        private Axis VerticalAxis
        {
            get
            {
                if (this.XYChartArea != null)
                    return this.XYChartArea.Axes.FirstOrDefault<Axis>(axis =>
                   {
                       if (axis.Orientation == AxisOrientation.X && this.XYChartArea.Orientation == Orientation.Vertical)
                           return true;
                       if (axis.Orientation == AxisOrientation.Y)
                           return this.XYChartArea.Orientation == Orientation.Horizontal;
                       return false;
                   });
                return null;
            }
        }

        double IScrollProvider.HorizontalScrollPercent
        {
            get
            {
                if (this.HorizontalAxis != null)
                    return this.HorizontalAxis.AxisPresenter.ScaleViewPositionInPercent;
                return 0.0;
            }
        }

        double IScrollProvider.HorizontalViewSize
        {
            get
            {
                if (this.HorizontalAxis != null)
                    return this.HorizontalAxis.AxisPresenter.ScaleViewSizeInPercent;
                return 1.0;
            }
        }

        bool IScrollProvider.HorizontallyScrollable
        {
            get
            {
                if (this.HorizontalAxis != null)
                    return this.HorizontalAxis.Scale.ActualZoom > 1.0;
                return false;
            }
        }

        double IScrollProvider.VerticalScrollPercent
        {
            get
            {
                if (this.VerticalAxis != null)
                    return this.VerticalAxis.AxisPresenter.ScaleViewPositionInPercent;
                return 0.0;
            }
        }

        double IScrollProvider.VerticalViewSize
        {
            get
            {
                if (this.VerticalAxis != null)
                    return this.VerticalAxis.AxisPresenter.ScaleViewSizeInPercent;
                return 1.0;
            }
        }

        bool IScrollProvider.VerticallyScrollable
        {
            get
            {
                if (this.VerticalAxis != null)
                    return this.VerticalAxis.Scale.ActualZoom > 1.0;
                return false;
            }
        }

        bool ITransformProvider.CanMove
        {
            get
            {
                return false;
            }
        }

        bool ITransformProvider.CanResize
        {
            get
            {
                return true;
            }
        }

        bool ITransformProvider.CanRotate
        {
            get
            {
                return false;
            }
        }

        public ChartAreaAutomationPeer(ChartArea owner)
          : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        protected override string GetClassNameCore()
        {
            return Resources.Automation_ChartAreaClassName;
        }

        protected override string GetAutomationIdCore()
        {
            string str = base.GetAutomationIdCore();
            if (string.IsNullOrEmpty(str))
                str = this.GetName();
            return str;
        }

        protected override string GetNameCore()
        {
            if (this.ChartArea.SelectionPanel != null && this.ChartArea.SelectionPanel.SelectedElement != null && this.ChartArea.SelectionPanel.SelectedElement != this.ChartArea)
            {
                AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(this.ChartArea.SelectionPanel.SelectedElement);
                if (peerForElement != null)
                    return peerForElement.GetName();
            }
            string str = base.GetNameCore();
            if (string.IsNullOrEmpty(str))
                str = this.GetClassName();
            if (string.IsNullOrEmpty(str))
                str = this.ChartArea.Name;
            if (string.IsNullOrEmpty(str))
                str = this.ChartArea.GetType().Name;
            return str;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
                return this;
            if (patternInterface == PatternInterface.Scroll)
                return this;
            if (patternInterface == PatternInterface.Transform)
                return this;
            return null;
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> automationPeerList = new List<AutomationPeer>();
            IEnumerable<AutomationPeer> collection = this.ChartArea.GetSeries().Select<Series, AutomationPeer>(series => UIElementAutomationPeer.CreatePeerForElement((UIElement)series));
            automationPeerList.AddRange(collection);
            return automationPeerList;
        }

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            return this.ChartArea.GetSelectedDataPoints().Select<DataPoint, IRawElementProviderSimple>(dataPoint => this.ProviderFromPeer(UIElementAutomationPeer.CreatePeerForElement((UIElement)dataPoint))).ToArray<IRawElementProviderSimple>();
        }

        internal ScrollZoomBar GetScrollBar(Axis axis)
        {
            if (axis != null && axis.AxisPresenter != null)
            {
                Panel axisPanel = axis.AxisPresenter.GetAxisPanel(AxisPresenter.AxisPanelType.AxisAndTickMarks);
                if (axisPanel != null)
                    return axisPanel.GetElementsByType(typeof(ScrollZoomBar)).FirstOrDefault<FrameworkElement>() as ScrollZoomBar;
            }
            return null;
        }

        void IScrollProvider.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
        {
            if (horizontalAmount != ScrollAmount.NoAmount && ((IScrollProvider)this).HorizontallyScrollable)
            {
                ScrollZoomBar scrollBar = this.GetScrollBar(this.HorizontalAxis);
                if (scrollBar != null)
                    scrollBar.ScrollByAmount(horizontalAmount);
            }
            if (verticalAmount == ScrollAmount.NoAmount || !((IScrollProvider)this).VerticallyScrollable)
                return;
            ScrollZoomBar scrollBar1 = this.GetScrollBar(this.VerticalAxis);
            if (scrollBar1 == null)
                return;
            scrollBar1.ScrollByAmount(verticalAmount);
        }

        void IScrollProvider.SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            if (this.HorizontalAxis != null)
                this.HorizontalAxis.Scale.ScrollToPercent(horizontalPercent);
            if (this.VerticalAxis == null)
                return;
            this.VerticalAxis.Scale.ScrollToPercent(verticalPercent);
        }

        void ITransformProvider.Move(double x, double y)
        {
            throw new InvalidOperationException();
        }

        void ITransformProvider.Resize(double width, double height)
        {
            if (this.HorizontalAxis != null && (this.HorizontalAxis.IsScrollZoomBarAllowsZooming || this.HorizontalAxis.IsAllowsAutoZoom))
                this.HorizontalAxis.ViewSizeInPercent = this.HorizontalAxis.ViewSizeInPercent / Math.Max(double.Epsilon, width);
            if (this.VerticalAxis == null || !this.VerticalAxis.IsScrollZoomBarAllowsZooming && !this.VerticalAxis.IsAllowsAutoZoom)
                return;
            this.VerticalAxis.ViewSizeInPercent = this.VerticalAxis.ViewSizeInPercent / Math.Max(double.Epsilon, height);
        }

        void ITransformProvider.Rotate(double degrees)
        {
            throw new InvalidOperationException();
        }
    }
}
