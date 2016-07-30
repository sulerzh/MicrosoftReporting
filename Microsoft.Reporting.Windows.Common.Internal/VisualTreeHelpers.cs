using Microsoft.Reporting.Common.Toolkit.Internal;
using Microsoft.Reporting.Windows.Common.PivotViewer.Internal;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    internal static class VisualTreeHelpers
    {
        static VisualTreeHelpers()
        {
            CompositionTarget.Rendering += new EventHandler(VisualTreeHelpers.CompositionTarget_Rendering);
        }

        internal static IEnumerable<UIElement> GetChildren(this UIElement element)
        {
            Panel panel = element as Panel;
            if (panel != null)
            {
                foreach (UIElement child in panel.Children)
                    yield return child;
            }
            else
            {
                Border border = element as Border;
                if (border != null)
                {
                    yield return border.Child;
                }
                else
                {
                    int count = VisualTreeHelper.GetChildrenCount(element);
                    for (int i = 0; i < count; ++i)
                        yield return VisualTreeHelper.GetChild(element, i) as UIElement;
                }
            }
        }

        internal static void InvalidateMeasureInSubtree(this UIElement element)
        {
            if (element == null || element.Visibility == Visibility.Collapsed)
                return;
            element.InvalidateMeasure();
            foreach (UIElement child in element.GetChildren())
                child.InvalidateMeasureInSubtree();
        }

        internal static void ForEachChildAndNodeBreadth<TNodeType>(DependencyObject node, Func<TNodeType, bool> callback) where TNodeType : class
        {
            foreach (DependencyObject dependencyObject in node.GetVisualDescendantsAndSelf())
            {
                TNodeType nodeType = dependencyObject as TNodeType;
                if (nodeType != null && !callback(nodeType))
                    break;
            }
        }

        internal static void ForEachChildAndNodeDepth<TNodeType>(DependencyObject node, Func<TNodeType, bool> callback) where TNodeType : class
        {
            Stack<DependencyObject> dependencyObjectStack = new Stack<DependencyObject>();
            dependencyObjectStack.Push(node);
            while (dependencyObjectStack.Count > 0)
            {
                node = dependencyObjectStack.Pop();
                TNodeType nodeType = node as TNodeType;
                if (nodeType != null && !callback(nodeType))
                    break;
                for (int childrenCount = VisualTreeHelper.GetChildrenCount(node); childrenCount > 0; --childrenCount)
                    dependencyObjectStack.Push(VisualTreeHelper.GetChild(node, childrenCount - 1));
            }
        }

        internal static void ForEachParentAndNode<TNodeType>(DependencyObject node, Func<TNodeType, bool> callback) where TNodeType : class
        {
            foreach (DependencyObject dependencyObject in node.GetVisualAncestorsAndSelf())
            {
                TNodeType nodeType = dependencyObject as TNodeType;
                if (nodeType != null && !callback(nodeType))
                    break;
            }
        }

        internal static void ForEachParent<TNodeType>(DependencyObject node, Func<TNodeType, bool> callback) where TNodeType : class
        {
            VisualTreeHelpers.ForEachParentAndNode<TNodeType>(VisualTreeHelper.GetParent(node), callback);
        }

        internal static DependencyObject GetVisualTreeRoot(DependencyObject element)
        {
            DependencyObject reference = element;
            for (DependencyObject dependencyObject = reference; dependencyObject != null; dependencyObject = VisualTreeHelper.GetParent(reference))
                reference = dependencyObject;
            return reference;
        }

        internal static IUnparentedPopupProvider GetUnparentedPopupProvider(DependencyObject element)
        {
            IUnparentedPopupProvider unparentedPopupProvider = null;
            foreach (DependencyObject dependencyObject in element.GetVisualAncestorsAndSelf())
            {
                unparentedPopupProvider = dependencyObject as IUnparentedPopupProvider;
                if (unparentedPopupProvider != null)
                    break;
            }
            return unparentedPopupProvider;
        }

        internal static MatrixTransform GetAnimationTransform(FrameworkElement element)
        {
            Matrix matrix1 = new Matrix();
            Transform thisObject1 = element.TransformToVisual(Window.GetWindow(element)) as Transform;
            if (thisObject1 != null)
                matrix1 = MatrixHelper.Multiply(matrix1, thisObject1.GetMatrix());
            Transform thisObject2 = element.RenderTransformToAncestor(null).Inverse as Transform;
            if (thisObject2 != null)
                matrix1 = MatrixHelper.Multiply(matrix1, thisObject2.GetMatrix());
            return new MatrixTransform() { Matrix = matrix1 };
        }

        private static void CompositionTarget_Rendering(object sender, EventArgs e)
        {
        }
    }
}
