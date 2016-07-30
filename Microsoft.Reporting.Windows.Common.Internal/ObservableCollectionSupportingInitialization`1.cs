using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class ObservableCollectionSupportingInitialization<T> : ObservableCollection<T>, ISupportInitialize
    {
        private int _suspendCount;
        private bool _invalidated;

        public ObservableCollectionSupportingInitialization()
        {
        }

        public ObservableCollectionSupportingInitialization(IEnumerable<T> values)
          : base(values)
        {
        }

        public ObservableCollectionSupportingInitialization(List<T> list)
          : base(list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this._suspendCount > 0)
                this._invalidated = true;
            else
                base.OnCollectionChanged(e);
        }

        public void BeginInit()
        {
            ++this._suspendCount;
        }

        public void EndInit()
        {
            if (this._suspendCount <= 0)
                return;
            --this._suspendCount;
            if (this._suspendCount != 0 || !this._invalidated)
                return;
            this._invalidated = false;
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ResetIfNecessary(IList<T> newItems)
        {
            if (this.IsSameAs<T>(newItems))
                return;
            this.BeginInit();
            this.Clear();
            foreach (T newItem in newItems)
                this.Add(newItem);
            this.EndInit();
        }
    }
}
