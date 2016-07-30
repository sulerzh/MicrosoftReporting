using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public static class ItemsControlExtensions
    {
        public static Panel GetItemsHost(this ItemsControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            DependencyObject reference = control.ItemContainerGenerator.ContainerFromIndex(0);
            if (reference != null)
                return VisualTreeHelper.GetParent(reference) as Panel;
            FrameworkElement parent = control.GetVisualChildren().FirstOrDefault<DependencyObject>() as FrameworkElement;
            if (parent != null)
            {
                ItemsPresenter itemsPresenter = parent.GetLogicalDescendents().OfType<ItemsPresenter>().FirstOrDefault<ItemsPresenter>();
                if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
                    return VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
            }
            return null;
        }

        public static ScrollViewer GetScrollHost(this ItemsControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            Panel itemsHost = control.GetItemsHost();
            if (itemsHost == null)
                return null;
            return itemsHost.GetVisualAncestors().Where<DependencyObject>(c => c != control).OfType<ScrollViewer>().FirstOrDefault<ScrollViewer>();
        }

        public static IEnumerable<DependencyObject> GetContainers(this ItemsControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            return ItemsControlExtensions.GetContainersIterator<DependencyObject>(control);
        }

        public static IEnumerable<TContainer> GetContainers<TContainer>(this ItemsControl control) where TContainer : DependencyObject
        {
            if (control == null)
                throw new ArgumentNullException("control");
            return ItemsControlExtensions.GetContainersIterator<TContainer>(control);
        }

        private static IEnumerable<TContainer> GetContainersIterator<TContainer>(ItemsControl control) where TContainer : DependencyObject
        {
            return control.GetItemsAndContainers<TContainer>().Select<KeyValuePair<object, TContainer>, TContainer>(p => p.Value);
        }

        public static IEnumerable<KeyValuePair<object, DependencyObject>> GetItemsAndContainers(this ItemsControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            return ItemsControlExtensions.GetItemsAndContainersIterator<DependencyObject>(control);
        }

        public static IEnumerable<KeyValuePair<object, TContainer>> GetItemsAndContainers<TContainer>(this ItemsControl control) where TContainer : DependencyObject
        {
            if (control == null)
                throw new ArgumentNullException("control");
            return ItemsControlExtensions.GetItemsAndContainersIterator<TContainer>(control);
        }

        private static IEnumerable<KeyValuePair<object, TContainer>> GetItemsAndContainersIterator<TContainer>(ItemsControl control) where TContainer : DependencyObject
        {
            int count = control.Items.Count;
            for (int i = 0; i < count; ++i)
            {
                DependencyObject container = control.ItemContainerGenerator.ContainerFromIndex(i);
                if (container != null)
                    yield return new KeyValuePair<object, TContainer>(control.Items[i], container as TContainer);
            }
        }

        internal static bool CanAddItem(this ItemsControl that, object item)
        {
            if (that.ItemsSource == null)
                return true;
            return that.ItemsSource.CanInsert(item);
        }

        internal static bool CanRemoveItem(this ItemsControl that)
        {
            if (that.ItemsSource == null)
                return true;
            if (!that.ItemsSource.IsReadOnly())
                return that.ItemsSource is INotifyCollectionChanged;
            return false;
        }

        internal static void InsertItem(this ItemsControl that, int index, object item)
        {
            if (that.ItemsSource == null)
                that.Items.Insert(index, item);
            else
                that.ItemsSource.Insert(index, item);
        }

        internal static void AddItem(this ItemsControl that, object item)
        {
            if (that.ItemsSource == null)
                that.InsertItem(that.Items.Count, item);
            else
                that.ItemsSource.Add(item);
        }

        internal static void RemoveItem(this ItemsControl that, object item)
        {
            if (that.ItemsSource == null)
                that.Items.Remove(item);
            else
                that.ItemsSource.Remove(item);
        }

        internal static void RemoveItemAtIndex(this ItemsControl that, int index)
        {
            if (that.ItemsSource == null)
                that.Items.RemoveAt(index);
            else
                that.ItemsSource.RemoveAt(index);
        }

        internal static int GetItemCount(this ItemsControl that)
        {
            if (that.ItemsSource == null)
                return that.Items.Count;
            return that.ItemsSource.Count();
        }
    }
}
