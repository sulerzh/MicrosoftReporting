using Microsoft.Reporting.Windows.Common.Internal.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class UniqueObservableCollection<T> : ObservableCollection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            if (this.Contains(item))
                throw new InvalidOperationException(Resources.UniqueObservableCollection_InvalidAttemptToInsertADuplicateItem);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            int num = this.IndexOf(item);
            if (num != -1 && num != index)
                throw new InvalidOperationException(Resources.UniqueObservableCollection_InvalidAttemptToInsertADuplicateItem);
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            foreach (T obj in new List<T>(this))
                this.Remove(obj);
        }
    }
}
