using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class UIElementExtension
    {
        public static IEnumerable<FrameworkElement> GetElementsByType(this FrameworkElement root, Type type)
        {
            if (root != null)
            {
                FrameworkElement element = root;
                if (element.GetType() == type)
                    yield return element;
                int childrenCount = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < childrenCount; ++i)
                {
                    foreach (FrameworkElement frameworkElement in ((FrameworkElement)VisualTreeHelper.GetChild(root, i)).GetElementsByType(type))
                        yield return frameworkElement;
                }
            }
        }
    }
}
