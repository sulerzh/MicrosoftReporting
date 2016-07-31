using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public static class VisualTreeExtensions
    {
        public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return VisualTreeExtensions.GetVisualAncestorsAndSelfIterator(element).Skip<DependencyObject>(1);
        }

        public static IEnumerable<DependencyObject> GetVisualAncestorsAndSelf(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return VisualTreeExtensions.GetVisualAncestorsAndSelfIterator(element);
        }

        private static IEnumerable<DependencyObject> GetVisualAncestorsAndSelfIterator(DependencyObject element)
        {
            for (DependencyObject obj = element; obj != null; obj = VisualTreeHelper.GetParent(obj))
                yield return obj;
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; ++i)
                yield return VisualTreeHelper.GetChild(element, i);
        }

        public static IEnumerable<DependencyObject> GetVisualChildrenAndSelf(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return element.GetVisualChildrenAndSelfIterator();
        }

        private static IEnumerable<DependencyObject> GetVisualChildrenAndSelfIterator(this DependencyObject element)
        {
            yield return element;
            int count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; ++i)
                yield return VisualTreeHelper.GetChild(element, i);
        }

        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return VisualTreeExtensions.GetVisualDescendantsAndSelfIterator(element).Skip<DependencyObject>(1);
        }

        public static IEnumerable<DependencyObject> GetVisualDescendantsAndSelf(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return VisualTreeExtensions.GetVisualDescendantsAndSelfIterator(element);
        }

        private static IEnumerable<DependencyObject> GetVisualDescendantsAndSelfIterator(DependencyObject element)
        {
            Queue<DependencyObject> remaining = new Queue<DependencyObject>();
            remaining.Enqueue(element);
            while (remaining.Count > 0)
            {
                DependencyObject obj = remaining.Dequeue();
                yield return obj;
                foreach (DependencyObject visualChild in obj.GetVisualChildren())
                    remaining.Enqueue(visualChild);
            }
        }

        public static IEnumerable<DependencyObject> GetVisualSiblings(this DependencyObject element)
        {
            return element.GetVisualSiblingsAndSelf().Where<DependencyObject>(p => p != element);
        }

        public static IEnumerable<DependencyObject> GetVisualSiblingsAndSelf(this DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent != null)
                return parent.GetVisualChildren();
            return Enumerable.Empty<DependencyObject>();
        }

        public static Rect? GetBoundsRelativeTo(this FrameworkElement element, UIElement otherElement)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (otherElement == null)
                throw new ArgumentNullException("otherElement");
            try
            {
                GeneralTransform visual = element.TransformToVisual(otherElement);
                if (visual != null)
                {
                    Point result1;
                    if (visual.TryTransform(new Point(), out result1))
                    {
                        Point result2;
                        if (visual.TryTransform(new Point(element.ActualWidth, element.ActualHeight), out result2))
                            return new Rect?(new Rect(result1, result2));
                    }
                }
            }
            catch (ArgumentException ex)
            {
            }
            return new Rect?();
        }

        public static void InvokeOnLayoutUpdated(this FrameworkElement element, Action action)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            if (action == null)
                throw new ArgumentNullException("action");
            EventHandler handler = null;
            handler = (s, e) =>
           {
               element.LayoutUpdated -= handler;
               action();
           };
            element.LayoutUpdated += handler;
        }

        internal static IEnumerable<FrameworkElement> GetLogicalChildren(this FrameworkElement parent)
        {
            Popup popup = parent as Popup;
            if (popup != null)
            {
                FrameworkElement popupChild = popup.Child as FrameworkElement;
                if (popupChild != null)
                    yield return popupChild;
            }
            ItemsControl itemsControl = parent as ItemsControl;
            if (itemsControl != null)
            {
                foreach (FrameworkElement frameworkElement in Enumerable.Range(0, itemsControl.Items.Count).Select<int, DependencyObject>(index => itemsControl.ItemContainerGenerator.ContainerFromIndex(index)).OfType<FrameworkElement>())
                    yield return frameworkElement;
            }
            Queue<FrameworkElement> queue = new Queue<FrameworkElement>(parent.GetVisualChildren().OfType<FrameworkElement>());
            while (queue.Count > 0)
            {
                FrameworkElement element = queue.Dequeue();
                if (element.Parent == parent || element is UserControl)
                {
                    yield return element;
                }
                else
                {
                    foreach (FrameworkElement frameworkElement in element.GetVisualChildren().OfType<FrameworkElement>())
                        queue.Enqueue(frameworkElement);
                }
            }
        }

        internal static IEnumerable<FrameworkElement> GetLogicalDescendents(this FrameworkElement parent)
        {
            return FunctionalProgramming.TraverseBreadthFirst<FrameworkElement>(parent, node => node.GetLogicalChildren(), node => true);
        }
    }
}
