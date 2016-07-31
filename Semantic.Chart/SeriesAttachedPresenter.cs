using System;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal abstract class SeriesAttachedPresenter : DependencyObject
    {
        protected SeriesPresenter SeriesPresenter { get; private set; }

        public SeriesAttachedPresenter(SeriesPresenter seriesPresenter)
        {
            this.SeriesPresenter = seriesPresenter;
            this.SeriesPresenter.ViewCreated += (sender, args) => this.OnCreateView(args.DataPoint);
            this.SeriesPresenter.ViewRemoved += (sender, args) => this.OnRemoveView(args.DataPoint);
            this.SeriesPresenter.ViewUpdated += (sender, args) => this.OnUpdateView(args.DataPoint);
            this.SeriesPresenter.Removed += (sender, args) => this.OnSeriesRemoved();
        }

        internal abstract void OnCreateView(DataPoint dataPoint);

        internal abstract void OnRemoveView(DataPoint dataPoint);

        internal abstract void OnUpdateView(DataPoint dataPoint);

        internal abstract void OnSeriesRemoved();
    }
}
