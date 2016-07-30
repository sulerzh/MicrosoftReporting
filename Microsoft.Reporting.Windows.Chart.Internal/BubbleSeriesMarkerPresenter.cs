using Microsoft.Reporting.Windows.Common.Internal;
using System.Windows;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class BubbleSeriesMarkerPresenter : PointSeriesMarkerPresenter
    {
        public BubbleSeriesMarkerPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        internal override void OnUpdateView(DataPoint dataPoint)
        {
            base.OnUpdateView(dataPoint);
            DataPointView view = dataPoint.View;
            if (view == null || view.MarkerView == null)
                return;
            double markerSize = this.GetMarkerSize(dataPoint);
            MarkerControl markerControl = view.MarkerView as MarkerControl;
            if (markerControl != null)
            {
                markerControl.Width = markerSize;
                markerControl.Height = markerSize;
            }
            Path path = view.MarkerView as Path;
            if (path != null)
            {
                path.Width = markerSize;
                path.Height = markerSize;
                path.Data = VisualUtilities.GetMarkerGeometry(dataPoint.MarkerType, new Size(markerSize, markerSize));
            }
            this.SeriesPresenter.UpdateDataPointZIndex(dataPoint);
            dataPoint.UpdateActualLabelMargin(markerSize);
            ((AnchorPanel)this.MarkersPanel).Invalidate();
        }

        internal override double GetMarkerSize(DataPoint dataPoint)
        {
            BubbleDataPoint bubbleDataPoint = dataPoint as BubbleDataPoint;
            if (bubbleDataPoint != null)
                return bubbleDataPoint.SizeValueInScaleUnits;
            return 0.0;
        }
    }
}
