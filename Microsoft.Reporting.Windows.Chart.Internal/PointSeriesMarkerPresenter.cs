using Microsoft.Reporting.Windows.Common.Internal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class PointSeriesMarkerPresenter : SeriesMarkerPresenter
    {
        protected override Panel MarkersPanel
        {
            get
            {
                AnchorPanel anchorPanel = base.MarkersPanel as AnchorPanel;
                if (anchorPanel == null)
                    return base.MarkersPanel;
                anchorPanel.CollisionDetectionEnabled = false;
                return anchorPanel;
            }
        }

        public PointSeriesMarkerPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        protected override FrameworkElement CreateViewElement(DataPoint dataPoint)
        {
            if (this.PointMarkerElementPool.InUseElementCount == 0)
            {
                bool flag = this.SeriesPresenter.ShouldSimplifiedRenderingModeBeEnabled();
                if (flag != this.SeriesPresenter.IsSimplifiedRenderingModeEnabled)
                {
                    this.SeriesPresenter.IsSimplifiedRenderingModeEnabled = flag;
                    this.PointMarkerElementPool.Clear();
                }
            }
            return this.PointMarkerElementPool.Get(dataPoint);
        }

        internal override FrameworkElement CreateViewElement()
        {
            if (!this.SeriesPresenter.IsSimplifiedRenderingModeEnabled)
                return new MarkerControl();
            Path path = new Path();
            path.Stretch = Stretch.Fill;
            return path;
        }

        internal override void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            MarkerControl markerControl = view as MarkerControl;
            if (markerControl != null)
            {
                IAppearanceProvider appearanceProvider = dataPoint;
                if (appearanceProvider != null)
                {
                    if (valueName == "Fill" || valueName == null)
                        markerControl.Background = appearanceProvider.Fill;
                    if (valueName == "Stroke" || valueName == null)
                        markerControl.Stroke = appearanceProvider.Stroke;
                    if (valueName == "StrokeThickness" || valueName == null)
                        markerControl.StrokeThickness = appearanceProvider.StrokeThickness;
                    if (valueName == "MarkerType" || valueName == null)
                        markerControl.MarkerType = dataPoint.MarkerType;
                    if (valueName == "MarkerStyle" || valueName == null)
                        markerControl.Style = dataPoint.MarkerStyle;
                    if (valueName == "MarkerSize" || valueName == null)
                    {
                        double markerSize = this.GetMarkerSize(dataPoint);
                        markerControl.Width = markerSize;
                        markerControl.Height = markerSize;
                    }
                    if (valueName == "Opacity" || valueName == "ActualOpacity" || valueName == null)
                        markerControl.Opacity = dataPoint.ActualOpacity;
                    if (valueName == "Effect" || valueName == "ActualEffect" || valueName == null)
                        markerControl.Effect = dataPoint.ActualEffect;
                    ((AnchorPanel)this.MarkersPanel).Invalidate();
                }
            }
            Path path = view as Path;
            if (path == null)
                return;
            IAppearanceProvider appearanceProvider1 = dataPoint;
            if (appearanceProvider1 == null)
                return;
            if (valueName == "Fill" || valueName == null)
                path.Fill = appearanceProvider1.Fill;
            if (valueName == "Stroke" || valueName == null)
                path.Stroke = appearanceProvider1.Stroke;
            if (valueName == "StrokeThickness" || valueName == null)
                path.StrokeThickness = appearanceProvider1.StrokeThickness;
            if (valueName == "MarkerType" || valueName == "MarkerSize" || valueName == null)
            {
                double markerSize = this.GetMarkerSize(dataPoint);
                path.Width = markerSize;
                path.Height = markerSize;
                path.Data = VisualUtilities.GetMarkerGeometry(dataPoint.MarkerType, new Size(markerSize, markerSize));
            }
            if (valueName == "Opacity" || valueName == "ActualOpacity" || valueName == null)
                path.Opacity = dataPoint.ActualOpacity;
            if (!(valueName == "Effect") && !(valueName == "ActualEffect") && valueName != null)
                return;
            path.Effect = dataPoint.ActualEffect;
        }

        internal override bool IsMarkerVisible(DataPoint dataPoint)
        {
            return this.SeriesPresenter.IsDataPointVisible(dataPoint) && dataPoint.MarkerType != MarkerType.None;
        }
    }
}
