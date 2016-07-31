using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    internal static class CollectionHelper
    {
        public static bool IsReadOnly(this IEnumerable collection)
        {
            if (!collection.GetType().IsArray)
                return EnumerableExtensions.Iterate<Type>(collection.GetType(), type => type.BaseType).TakeWhile<Type>(type => type != null).Any<Type>(type => type.FullName.StartsWith("System.Collections.ObjectModel.ReadOnlyCollection`1", StringComparison.Ordinal));
            return true;
        }

        public static bool CanInsert(this IEnumerable collection, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
                return collectionView.SourceCollection.CanInsert(item);
            if (collection.IsReadOnly())
                return false;
            Type type = ((IEnumerable<Type>)collection.GetType().GetInterfaces()).Where<Type>(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault<Type>();
            if (type != null)
                return type.GetGenericArguments()[0] == item.GetType();
            return collection is IList;
        }

        public static void Insert(this IEnumerable collection, int index, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                collectionView.SourceCollection.Insert(index, item);
            }
            else
            {
                Type type = ((IEnumerable<Type>)collection.GetType().GetInterfaces()).Where<Type>(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault<Type>();
                if (type != null)
                    type.GetMethod("Insert").Invoke(collection, new object[2]
                    {
             index,
            item
                    });
                else
                    (collection as IList).Insert(index, item);
            }
        }

        public static int Count(this IEnumerable collection)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
                return collectionView.SourceCollection.Count();
            Type type = ((IEnumerable<Type>)collection.GetType().GetInterfaces()).Where<Type>(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.ICollection`1", StringComparison.Ordinal)).FirstOrDefault<Type>();
            if (type != null)
                return (int)type.GetProperty("Count").GetValue(collection, new object[0]);
            IList list = collection as IList;
            if (list != null)
                return list.Count;
            return collection.OfType<object>().Count<object>();
        }

        public static void Add(this IEnumerable collection, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                collectionView.SourceCollection.Add(item);
            }
            else
            {
                int index = (int)collection.GetType().GetProperty("Count").GetValue(collection, new object[0]);
                collection.Insert(index, item);
            }
        }

        public static void Remove(this IEnumerable collection, object item)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                collectionView.SourceCollection.Remove(item);
            }
            else
            {
                Type type = ((IEnumerable<Type>)collection.GetType().GetInterfaces()).Where<Type>(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault<Type>();
                if (type != null)
                {
                    int num = (int)type.GetMethod("IndexOf").Invoke(collection, new object[1] { item });
                    if (num == -1)
                        return;
                    type.GetMethod("RemoveAt").Invoke(collection, new object[1] { num });
                }
                else
                    (collection as IList).Remove(item);
            }
        }

        public static void RemoveAt(this IEnumerable collection, int index)
        {
            ICollectionView collectionView = collection as ICollectionView;
            if (collectionView != null)
            {
                collectionView.SourceCollection.RemoveAt(index);
            }
            else
            {
                Type type = ((IEnumerable<Type>)collection.GetType().GetInterfaces()).Where<Type>(interfaceType => interfaceType.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal)).FirstOrDefault<Type>();
                if (type != null)
                    type.GetMethod("RemoveAt").Invoke(collection, new object[1]
                    {
             index
                    });
                else
                    (collection as IList).RemoveAt(index);
            }
        }
    }
}
