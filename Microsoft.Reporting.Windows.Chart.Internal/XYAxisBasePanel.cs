using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal abstract class XYAxisBasePanel : Panel, IUpdatable
    {
        internal XYAxisPresenter Presenter { get; private set; }

        public Axis Axis { get; private set; }

        public bool IsInverted
        {
            get
            {
                if (this.Presenter.ActualLocation != Edge.Left)
                    return this.Presenter.ActualLocation == Edge.Top;
                return true;
            }
        }

        public Orientation Orientation
        {
            get
            {
                return this.Presenter.ActualOrientation;
            }
        }

        IUpdatable IUpdatable.Parent
        {
            get
            {
                return this.Axis;
            }
        }

        internal XYAxisBasePanel(XYAxisPresenter presenter)
        {
            this.UseLayoutRounding = true;
            this.Presenter = presenter;
            this.Axis = this.Presenter.Axis;
        }

        public virtual void Invalidate()
        {
            this.InvalidateMeasure();
        }

        public virtual void Update()
        {
            this.Invalidate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.Populate(this.ElementWidth(availableSize));
            return base.MeasureOverride(availableSize);
        }

        protected virtual void Populate(double availableLength)
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in this.Children)
            {
                Size desiredSize = XYAxisBasePanel.GetDesiredSize(child);
                double num1 = this.Presenter.ConvertScaleToAxisUnits(this.GetCenterCoordinate(child), this.ElementWidth(finalSize)) - this.ElementWidth(desiredSize) / 2.0;
                double num2 = this.ElementOffset(child);
                double num3 = this.ElementWidth(desiredSize);
                double num4 = this.ElementHeight(desiredSize);
                if (this.IsInverted)
                    num2 = this.ElementHeight(finalSize) - (num2 + num4);
                if (this.Orientation == Orientation.Horizontal)
                    this.ArrangeChild(child, new Rect(num1, num2, num3, num4), finalSize);
                else
                    this.ArrangeChild(child, new Rect(num2, num1, num4, num3), finalSize);
            }
            return finalSize;
        }

        protected virtual void ArrangeChild(UIElement child, Rect rect, Size finalSize)
        {
            child.Arrange(rect);
        }

        protected abstract double GetCenterCoordinate(UIElement element);

        protected static Size GetDesiredSize(UIElement element)
        {
            Line line = element as Line;
            if (line != null)
                return new Size(Math.Max(line.StrokeThickness, line.X2 - line.X1), Math.Max(line.StrokeThickness, line.Y2 - line.Y1));
            return element.DesiredSize;
        }

        protected double ElementHeight(Size size)
        {
            if (this.Orientation != Orientation.Horizontal)
                return size.Width;
            return size.Height;
        }

        protected double ElementWidth(Size size)
        {
            if (this.Orientation != Orientation.Horizontal)
                return size.Height;
            return size.Width;
        }

        protected virtual double ElementOffset(UIElement element)
        {
            return 0.0;
        }
    }
}
