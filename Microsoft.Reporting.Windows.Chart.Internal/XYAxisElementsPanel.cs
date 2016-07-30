using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class XYAxisElementsPanel : XYAxisBasePanel
    {
        public static readonly DependencyProperty CoordinateProperty = DependencyProperty.RegisterAttached("Coordinate", typeof(double), typeof(XYAxisElementsPanel), new PropertyMetadata(0.0, new PropertyChangedCallback(XYAxisElementsPanel.OnCoordinatePropertyChanged)));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(AxisElementPosition), typeof(XYAxisElementsPanel), new PropertyMetadata(AxisElementPosition.Outside, new PropertyChangedCallback(XYAxisElementsPanel.OnPositionPropertyChanged)));

        internal XYAxisElementsPanel(XYAxisPresenter presenter)
          : base(presenter)
        {
        }

        public static double GetCoordinate(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (double)element.GetValue(XYAxisElementsPanel.CoordinateProperty);
        }

        public static void SetCoordinate(UIElement element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(XYAxisElementsPanel.CoordinateProperty, value);
        }

        public static void OnCoordinatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (uiElement == null)
                throw new ArgumentNullException("dependencyObject");
            XYAxisElementsPanel axisElementsPanel = VisualTreeHelper.GetParent(uiElement) as XYAxisElementsPanel;
            if (axisElementsPanel == null)
                return;
            axisElementsPanel.InvalidateMeasure();
        }

        public static AxisElementPosition GetPosition(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (AxisElementPosition)element.GetValue(XYAxisElementsPanel.PositionProperty);
        }

        public static void SetPosition(UIElement element, AxisElementPosition value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(XYAxisElementsPanel.PositionProperty, value);
        }

        public static void OnPositionPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            UIElement uiElement = dependencyObject as UIElement;
            if (uiElement == null)
                throw new ArgumentNullException("dependencyObject");
            XYAxisElementsPanel axisElementsPanel = VisualTreeHelper.GetParent(uiElement) as XYAxisElementsPanel;
            if (axisElementsPanel == null)
                return;
            axisElementsPanel.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.Populate(this.ElementWidth(availableSize));
            double val2 = 0.0;
            if (this.Children.Count > 0)
            {
                Size availableSize1 = new Size(double.PositiveInfinity, double.PositiveInfinity);
                foreach (UIElement child in this.Children)
                    child.Measure(availableSize1);
                val2 = this.Children.Cast<UIElement>().Select<UIElement, double>(child => this.ElementHeight(XYAxisBasePanel.GetDesiredSize(child)) + this.ElementOffset(child)).MaxOrNullable<double>() ?? 0.0;
            }
            if (this.Orientation == Orientation.Horizontal)
                return new Size(0.0, Math.Max(0.0, val2));
            return new Size(Math.Max(0.0, val2), 0.0);
        }

        protected override double GetCenterCoordinate(UIElement element)
        {
            return XYAxisElementsPanel.GetCoordinate(element);
        }

        protected override double ElementOffset(UIElement element)
        {
            Size desiredSize = XYAxisBasePanel.GetDesiredSize(element);
            double num = 0.0;
            switch (XYAxisElementsPanel.GetPosition(element))
            {
                case AxisElementPosition.Inside:
                case AxisElementPosition.None:
                    num = this.ElementHeight(desiredSize);
                    break;
                case AxisElementPosition.Cross:
                    num = this.ElementHeight(desiredSize) / 2.0;
                    break;
            }
            return -num;
        }
    }
}
