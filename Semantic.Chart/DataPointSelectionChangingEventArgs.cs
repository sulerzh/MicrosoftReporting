using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class DataPointSelectionChangingEventArgs : DataPointSelectionChangedEventArgs
    {
        public bool Cancel { get; set; }

        public DataPointSelectionChangingEventArgs(IList<DataPoint> removedItems, IList<DataPoint> addedItems)
          : base(removedItems, addedItems)
        {
        }
    }
}
