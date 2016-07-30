using System;
using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class DataPointSelectionChangedEventArgs : EventArgs
    {
        private DataPoint[] _addedItems = new DataPoint[0];
        private DataPoint[] _removedItems = new DataPoint[0];

        public IList<DataPoint> AddedItems
        {
            get
            {
                return this._addedItems;
            }
        }

        public IList<DataPoint> RemovedItems
        {
            get
            {
                return this._removedItems;
            }
        }

        public DataPointSelectionChangedEventArgs(IList<DataPoint> removedItems, IList<DataPoint> addedItems)
        {
            if (addedItems != null)
            {
                this._addedItems = new DataPoint[addedItems.Count];
                addedItems.CopyTo(this._addedItems, 0);
            }
            if (removedItems == null)
                return;
            this._removedItems = new DataPoint[removedItems.Count];
            removedItems.CopyTo(this._removedItems, 0);
        }
    }
}
