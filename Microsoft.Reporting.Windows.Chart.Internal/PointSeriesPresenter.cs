using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class PointSeriesPresenter : XYSeriesPresenter
    {
        public PointSeriesPresenter(XYSeries series)
          : base(series)
        {
        }

        internal override SeriesMarkerPresenter CreateMarkerPresenter()
        {
            return new PointSeriesMarkerPresenter((SeriesPresenter)this);
        }

        internal override SeriesLabelPresenter CreateLabelPresenter()
        {
            return new PointSeriesLabelPresenter((SeriesPresenter)this);
        }

        protected override FrameworkElement CreateViewElement(DataPoint dataPoint)
        {
            return null;
        }

        protected override void UpdateView(DataPoint dataPoint)
        {
            if (this.IsDataPointViewVisible(dataPoint))
            {
                XYDataPoint xyDataPoint = dataPoint as XYDataPoint;
                if (xyDataPoint != null && dataPoint.View != null)
                {
                    Point plotCoordinate = this.ChartArea.ConvertScaleToPlotCoordinate(this.Series.XAxis, this.Series.YAxis, xyDataPoint.XValueInScaleUnits, xyDataPoint.YValueInScaleUnits);
                    plotCoordinate.X = Math.Round(plotCoordinate.X);
                    plotCoordinate.Y = Math.Round(plotCoordinate.Y);
                    dataPoint.View.AnchorPoint = plotCoordinate;
                }
            }
            base.UpdateView(dataPoint);
        }

        protected override void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            DataPointView dataPointView = dataPoint != null ? dataPoint.View : null;
            if (dataPointView == null)
                return;
            this.LabelPresenter.BindViewToDataPoint(dataPoint, dataPointView.LabelView, valueName);
            this.MarkerPresenter.BindViewToDataPoint(dataPoint, dataPointView.MarkerView, valueName);
        }

        internal override FrameworkElement GetLegendSymbol()
        {
            DataPoint dataPoint1 = this.Series.DataPoints.OfType<XYDataPoint>().Where<XYDataPoint>((Func<XYDataPoint, bool>)(p =>
          {
              if (!p.ActualIsEmpty)
                  return p.IsVisible;
              return false;
          })).FirstOrDefault<XYDataPoint>();
            FrameworkElement viewElement = this.MarkerPresenter.CreateViewElement();
            if (dataPoint1 != null)
            {
                this.MarkerPresenter.BindViewToDataPoint(dataPoint1, viewElement, null);
                viewElement.Opacity = dataPoint1.Opacity;
                foreach (UIElement uiElement in this.Series.DataPoints.OfType<XYDataPoint>().Where<XYDataPoint>(p => !p.ActualIsEmpty))
                {
                    if (uiElement.Effect != viewElement.Effect)
                    {
                        viewElement.ClearValue(UIElement.EffectProperty);
                        break;
                    }
                }
            }
            else
            {
                DataPoint dataPoint2 = this.Series.CreateDataPoint() as XYDataPoint;
                dataPoint2.Series = this.Series;
                if (dataPoint2 is BubbleDataPoint)
                    ((BubbleDataPoint)dataPoint2).SizeValueInScaleUnits = dataPoint2.MarkerSize;
                if (this.Series != null && this.Series.ItemsBinder != null && this.Series.DataContext != null)
                    this.Series.ItemsBinder.Bind(dataPoint2, this.Series.DataContext);
                this.MarkerPresenter.BindViewToDataPoint(dataPoint2, viewElement, null);
                dataPoint2.Series = null;
            }
            viewElement.Effect = null;
            double num = Math.Min(20.0, 12.0);
            viewElement.Width = num;
            viewElement.Height = num;
            return viewElement;
        }
    }
}
