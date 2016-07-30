using Microsoft.Reporting.Windows.Chart.Internal.Properties;
using Microsoft.Reporting.Windows.Common.Internal;
using System;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class AxesCollection : UniqueObservableCollection<Axis>
    {
        private ChartArea _seriesHost;

        internal AxesCollection(ChartArea seriesHost)
        {
            this._seriesHost = seriesHost;
        }

        protected override void RemoveItem(int index)
        {
            if (!this._seriesHost.CanRemoveAxis(this[index]))
                throw new InvalidOperationException(Resources.ChartAreaAxesCollection_RemoveItem_AxisCannotBeRemovedWhenOneOrMoreSeriesAreListeningToIt);
            base.RemoveItem(index);
        }
    }
}
