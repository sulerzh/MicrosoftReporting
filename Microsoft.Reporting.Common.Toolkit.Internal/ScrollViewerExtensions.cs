using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public static class ScrollViewerExtensions
    {
        public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty = DependencyProperty.RegisterAttached("IsMouseWheelScrollingEnabled", typeof(bool), typeof(ScrollViewerExtensions), new PropertyMetadata(false, new PropertyChangedCallback(ScrollViewerExtensions.OnIsMouseWheelScrollingEnabledPropertyChanged)));
        private static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerExtensions), new PropertyMetadata(new PropertyChangedCallback(ScrollViewerExtensions.OnVerticalOffsetPropertyChanged)));
        private static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double), typeof(ScrollViewerExtensions), new PropertyMetadata(new PropertyChangedCallback(ScrollViewerExtensions.OnHorizontalOffsetPropertyChanged)));
        private const double LineChange = 16.0;

        public static bool GetIsMouseWheelScrollingEnabled(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            return (bool)viewer.GetValue(ScrollViewerExtensions.IsMouseWheelScrollingEnabledProperty);
        }

        public static void SetIsMouseWheelScrollingEnabled(this ScrollViewer viewer, bool value)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            viewer.SetValue(ScrollViewerExtensions.IsMouseWheelScrollingEnabledProperty, value);
        }

        private static void OnIsMouseWheelScrollingEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            if ((bool)e.NewValue)
                scrollViewer.MouseWheel += new MouseWheelEventHandler(ScrollViewerExtensions.OnMouseWheel);
            else
                scrollViewer.MouseWheel -= new MouseWheelEventHandler(ScrollViewerExtensions.OnMouseWheel);
        }

        private static void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer viewer = sender as ScrollViewer;
            if (e.Handled)
                return;
            double offset = ScrollViewerExtensions.CoerceVerticalOffset(viewer, viewer.VerticalOffset - e.Delta);
            viewer.ScrollToVerticalOffset(offset);
            e.Handled = true;
        }

        private static double GetVerticalOffset(ScrollViewer element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (double)element.GetValue(ScrollViewerExtensions.VerticalOffsetProperty);
        }

        private static void SetVerticalOffset(ScrollViewer element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(ScrollViewerExtensions.VerticalOffsetProperty, value);
        }

        private static void OnVerticalOffsetPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ScrollViewer scrollViewer = dependencyObject as ScrollViewer;
            if (scrollViewer == null)
                throw new ArgumentNullException("dependencyObject");
            scrollViewer.ScrollToVerticalOffset((double)eventArgs.NewValue);
        }

        private static double GetHorizontalOffset(ScrollViewer element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (double)element.GetValue(ScrollViewerExtensions.HorizontalOffsetProperty);
        }

        private static void SetHorizontalOffset(ScrollViewer element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(ScrollViewerExtensions.HorizontalOffsetProperty, value);
        }

        private static void OnHorizontalOffsetPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ScrollViewer scrollViewer = dependencyObject as ScrollViewer;
            if (scrollViewer == null)
                throw new ArgumentNullException("dependencyObject");
            scrollViewer.ScrollToHorizontalOffset((double)eventArgs.NewValue);
        }

        private static double CoerceVerticalOffset(ScrollViewer viewer, double offset)
        {
            return Math.Max(Math.Min(offset, viewer.ExtentHeight), 0.0);
        }

        private static double CoerceHorizontalOffset(ScrollViewer viewer, double offset)
        {
            return Math.Max(Math.Min(offset, viewer.ExtentWidth), 0.0);
        }

        private static void ScrollByVerticalOffset(ScrollViewer viewer, double offset)
        {
            offset += viewer.VerticalOffset;
            offset = ScrollViewerExtensions.CoerceVerticalOffset(viewer, offset);
            viewer.ScrollToVerticalOffset(offset);
        }

        private static void ScrollByHorizontalOffset(ScrollViewer viewer, double offset)
        {
            offset += viewer.HorizontalOffset;
            offset = ScrollViewerExtensions.CoerceHorizontalOffset(viewer, offset);
            viewer.ScrollToHorizontalOffset(offset);
        }

        public static void LineUp(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByVerticalOffset(viewer, -16.0);
        }

        public static void LineDown(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByVerticalOffset(viewer, 16.0);
        }

        public static void LineLeft(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByHorizontalOffset(viewer, -16.0);
        }

        public static void LineRight(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByHorizontalOffset(viewer, 16.0);
        }

        public static void PageUp(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByVerticalOffset(viewer, -viewer.ViewportHeight);
        }

        public static void PageDown(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByVerticalOffset(viewer, viewer.ViewportHeight);
        }

        public static void PageLeft(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByHorizontalOffset(viewer, -viewer.ViewportWidth);
        }

        public static void PageRight(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            ScrollViewerExtensions.ScrollByHorizontalOffset(viewer, viewer.ViewportWidth);
        }

        public static void ScrollToTop(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            viewer.ScrollToVerticalOffset(0.0);
        }

        public static void ScrollToBottom(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            viewer.ScrollToVerticalOffset(viewer.ExtentHeight);
        }

        public static void ScrollToLeft(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            viewer.ScrollToHorizontalOffset(0.0);
        }

        public static void ScrollToRight(this ScrollViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            viewer.ScrollToHorizontalOffset(viewer.ExtentWidth);
        }

        public static void ScrollIntoView(this ScrollViewer viewer, FrameworkElement element)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            if (element == null)
                throw new ArgumentNullException("element");
            viewer.ScrollIntoView(element, 0.0, 0.0, TimeSpan.Zero);
        }

        public static void ScrollIntoView(this ScrollViewer viewer, FrameworkElement element, double horizontalMargin, double verticalMargin, Duration duration)
        {
            if (viewer == null)
                throw new ArgumentNullException("viewer");
            if (element == null)
                throw new ArgumentNullException("element");
            Rect? boundsRelativeTo = element.GetBoundsRelativeTo(viewer);
            if (!boundsRelativeTo.HasValue)
                return;
            double verticalOffset = viewer.VerticalOffset;
            double num1 = 0.0;
            double viewportHeight = viewer.ViewportHeight;
            double num2 = boundsRelativeTo.Value.Bottom + verticalMargin;
            if (viewportHeight < num2)
            {
                num1 = num2 - viewportHeight;
                verticalOffset += num1;
            }
            double num3 = boundsRelativeTo.Value.Top - verticalMargin;
            if (num3 - num1 < 0.0)
                verticalOffset -= num1 - num3;
            double horizontalOffset = viewer.HorizontalOffset;
            double num4 = 0.0;
            double viewportWidth = viewer.ViewportWidth;
            double num5 = boundsRelativeTo.Value.Right + horizontalMargin;
            if (viewportWidth < num5)
            {
                num4 = num5 - viewportWidth;
                horizontalOffset += num4;
            }
            double num6 = boundsRelativeTo.Value.Left - horizontalMargin;
            if (num6 - num4 < 0.0)
                horizontalOffset -= num4 - num6;
            if (duration == TimeSpan.Zero)
            {
                viewer.ScrollToVerticalOffset(verticalOffset);
                viewer.ScrollToHorizontalOffset(horizontalOffset);
            }
            else
            {
                Storyboard storyboard = new Storyboard();
                ScrollViewerExtensions.SetVerticalOffset(viewer, viewer.VerticalOffset);
                ScrollViewerExtensions.SetHorizontalOffset(viewer, viewer.HorizontalOffset);
                DoubleAnimation doubleAnimation1 = new DoubleAnimation();
                doubleAnimation1.To = new double?(verticalOffset);
                doubleAnimation1.Duration = duration;
                DoubleAnimation doubleAnimation2 = doubleAnimation1;
                DoubleAnimation doubleAnimation3 = new DoubleAnimation();
                doubleAnimation3.To = new double?(verticalOffset);
                doubleAnimation3.Duration = duration;
                DoubleAnimation doubleAnimation4 = doubleAnimation3;
                Storyboard.SetTarget(doubleAnimation2, viewer);
                Storyboard.SetTarget(doubleAnimation4, viewer);
                Storyboard.SetTargetProperty(doubleAnimation4, new PropertyPath(ScrollViewerExtensions.HorizontalOffsetProperty));
                Storyboard.SetTargetProperty(doubleAnimation2, new PropertyPath(ScrollViewerExtensions.VerticalOffsetProperty));
                storyboard.Children.Add(doubleAnimation2);
                storyboard.Children.Add(doubleAnimation4);
                storyboard.Begin();
            }
        }
    }
}
