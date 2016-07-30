using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public static class ItemsSourceHelper
    {
        public static IList<T> CreateOrUpdate<T>(IEnumerable destinationItemsSource, IEnumerable<T> source)
        {
            ObservableCollection<T> observableCollection = destinationItemsSource as ObservableCollection<T>;
            if (observableCollection == null && destinationItemsSource != null)
                throw new ArgumentException("destinationItemsSource must be ObservableCollection<T> or null");
            if (observableCollection == null)
            {
                if (source == null)
                    return null;
                return new ObservableCollectionSupportingInitialization<T>(source);
            }
            if (source == null)
            {
                observableCollection.Clear();
                return observableCollection;
            }
            ISupportInitialize supportInitialize = observableCollection as ISupportInitialize;
            if (supportInitialize != null)
                supportInitialize.BeginInit();
            int index = 0;
            foreach (T obj in source)
            {
                if (index < observableCollection.Count)
                    observableCollection[index] = obj;
                else
                    observableCollection.Add(obj);
                ++index;
            }
            while (index < observableCollection.Count)
                observableCollection.RemoveAt(observableCollection.Count - 1);
            if (supportInitialize != null)
                supportInitialize.EndInit();
            return observableCollection;
        }
    }
}
