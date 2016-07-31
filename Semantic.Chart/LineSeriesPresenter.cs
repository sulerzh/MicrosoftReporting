using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class LineSeriesPresenter : XYSeriesPresenter
    {
        private HashSet<DataPoint> _dataPointsToForceVisibility = new HashSet<DataPoint>();
        internal const string UpdateLinePointsActionKey = "__UpdateLinePoints__";
        private const double MaximumScaleUnitValue = 100.0;
        private Point _mousePosition;
        private ToolTip _tooltip;
        private PolylineControl _polylineControl;

        internal PolylineControl PolylineControl
        {
            get
            {
                if (this._polylineControl == null && this.ChartArea != null)
                {
                    this._polylineControl = new PolylineControl();
                    this._tooltip = new ToolTip();
                    this._tooltip.Opened += new RoutedEventHandler(this.Tooltip_Opened);
                    ToolTipService.SetToolTip(this._polylineControl, this._tooltip);
                    this.UpdateToolTipStyle();
                    this.RootPanel.MouseMove += new MouseEventHandler(this.RootPanel_MouseMove);
                    this.RootPanel.Children.Add(this._polylineControl);
                }
                return this._polylineControl;
            }
        }

        public LineSeriesPresenter(XYSeries series)
          : base(series)
        {
            this.IsRootPanelClipped = true;
        }

        internal override SeriesMarkerPresenter CreateMarkerPresenter()
        {
            return new SeriesMarkerPresenter(this);
        }

        internal override SeriesLabelPresenter CreateLabelPresenter()
        {
            return new LineSeriesLabelPresenter((SeriesPresenter)this);
        }

        protected override void OnRemoved()
        {
            if (this._tooltip != null)
            {
                this._tooltip.Opened -= new RoutedEventHandler(this.Tooltip_Opened);
                this.RootPanel.MouseMove -= new MouseEventHandler(this.RootPanel_MouseMove);
            }
            base.OnRemoved();
        }

        protected override FrameworkElement CreateViewElement(DataPoint dataPoint)
        {
            this.CheckSimplifiedRenderingMode();
            return null;
        }

        protected override void RemoveView(DataPoint dataPoint)
        {
            if (this.ChartArea != null)
                this.ChartArea.UpdateSession.ExecuteOnceAfterUpdating(() => this.UpdateLinePoints(), new Tuple<Series, string>((Series)this.Series, "__UpdateLinePoints__"), null);
            base.RemoveView(dataPoint);
        }

        protected override void UpdateView(DataPoint dataPoint)
        {
            if (this.IsDataPointVisible(dataPoint))
            {
                XYDataPoint xyDataPoint = dataPoint as XYDataPoint;
                if (xyDataPoint.View != null)
                {
                    dataPoint.View.AnchorPoint = this.ChartArea.ConvertScaleToPlotCoordinate(this.Series.XAxis, this.Series.YAxis, xyDataPoint.XValueInScaleUnits, xyDataPoint.YValueInScaleUnits);
                    this.ChartArea.UpdateSession.ExecuteOnceAfterUpdating(() => this.UpdateLinePoints(), new Tuple<Series, string>((Series)this.Series, "__UpdateLinePoints__"), null);
                }
            }
            base.UpdateView(dataPoint);
        }

        protected override void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            if (valueName == null || valueName == "Stroke" || (valueName == "StrokeThickness" || valueName == "StrokeDashType") || (valueName == "ActualEffect" || valueName == "Effect" || (valueName == "ActualOpacity" || valueName == "Opacity")))
                this.ChartArea.UpdateSession.ExecuteOnceAfterUpdating(() => this.UpdateLinePoints(), new Tuple<Series, string>((Series)this.Series, "__UpdateLinePoints__"), null);
            DataPointView dataPointView = dataPoint != null ? dataPoint.View : null;
            if (dataPointView == null)
                return;
            this.LabelPresenter.BindViewToDataPoint(dataPoint, dataPointView.LabelView, valueName);
            this.MarkerPresenter.BindViewToDataPoint(dataPoint, dataPointView.MarkerView, valueName);
        }

        private void UpdateLinePoints()
        {
            if (this.ChartArea == null || !this.IsRootPanelVisible)
                return;
            DateTime now = DateTime.Now;
            PointCollection pointCollection = new PointCollection();
            Collection<IAppearanceProvider> collection = new Collection<IAppearanceProvider>();
            XYDataPoint xyDataPoint1 = null;
            if (this.Series.Visibility == Visibility.Visible)
            {
                for (int index = 0; index < this.Series.DataPoints.Count; ++index)
                {
                    XYDataPoint xyDataPoint2 = this.Series.DataPoints[index] as XYDataPoint;
                    if (xyDataPoint2.View != null && this.IsDataPointVisible(xyDataPoint2) && (this.IsValidScaleUnitValue(xyDataPoint2.XValueInScaleUnits) && this.IsValidScaleUnitValue(xyDataPoint2.YValueInScaleUnits)))
                    {
                        if (xyDataPoint1 != null && xyDataPoint1.ActualIsEmpty)
                        {
                            collection.Add(xyDataPoint1);
                            pointCollection.Add(xyDataPoint2.View.AnchorPoint);
                            if (index < this.Series.DataPoints.Count - 1)
                            {
                                XYDataPoint xyDataPoint3 = this.Series.DataPoints[index + 1] as XYDataPoint;
                                if (xyDataPoint3.ActualIsEmpty)
                                {
                                    collection.Add(xyDataPoint3);
                                    pointCollection.Add(xyDataPoint2.View.AnchorPoint);
                                }
                            }
                        }
                        collection.Add(xyDataPoint2);
                        pointCollection.Add(xyDataPoint2.View.AnchorPoint);
                    }
                    xyDataPoint1 = xyDataPoint2;
                }
            }
            if (this.PolylineControl != null)
            {
                this.PolylineControl.Points = pointCollection;
                this.PolylineControl.Appearances = collection;
                this.PolylineControl.Update();
            }
            this.ChartArea.UpdateSession.AddCounter("LineSeriesPresenter.UpdateLinePoints", DateTime.Now - now);
        }

        private bool IsValidScaleUnitValue(double scaleUnitValue)
        {
            if (ValueHelper.CanGraph(scaleUnitValue) && scaleUnitValue < 100.0)
                return scaleUnitValue > -100.0;
            return false;
        }

        internal override void UpdateDataPointVisibility()
        {
            int index1 = 0;
            bool flag1 = true;
            bool flag2 = true;
            DataPointViewState[] that = new DataPointViewState[this.Series.DataPoints.Count];
            foreach (DataPoint dataPoint1 in this.Series.DataPoints)
            {
                DataPointViewState dataPointViewState = DataPointViewState.Hidden;
                dataPoint1.IsVisible = false;
                XYDataPoint xyDataPoint = dataPoint1 as XYDataPoint;
                if (xyDataPoint != null && this.ChartArea != null && (this.ChartArea.IsTemplateApplied && ValueHelper.CanGraph(xyDataPoint.XValueInScaleUnits)) && ValueHelper.CanGraph(xyDataPoint.YValueInScaleUnits))
                {
                    if (xyDataPoint.XValueInScaleUnits.GreaterOrEqualWithPrecision(0.0) && xyDataPoint.XValueInScaleUnits.LessOrEqualWithPrecision(1.0) && (xyDataPoint.YValueInScaleUnits.GreaterOrEqualWithPrecision(0.0) && xyDataPoint.YValueInScaleUnits.LessOrEqualWithPrecision(1.0)))
                    {
                        if (this.Series.Visibility == Visibility.Visible)
                        {
                            flag1 = false;
                            dataPoint1.IsVisible = true;
                            if (!dataPoint1.IsNewlyAdded)
                                flag2 = false;
                            dataPointViewState = dataPoint1.IsNewlyAdded ? DataPointViewState.Showing : DataPointViewState.Normal;
                            if (index1 > 0)
                            {
                                DataPoint dataPoint2 = this.Series.DataPoints[index1 - 1];
                                if (that[index1 - 1] == DataPointViewState.Hidden || that[index1 - 1] == DataPointViewState.Hiding)
                                {
                                    dataPoint2.IsVisible = true;
                                    that[index1 - 1] = dataPointViewState;
                                    this.ChartArea.UpdateSession.Update(dataPoint2);
                                }
                            }
                            if (index1 < this.Series.DataPoints.Count - 1)
                            {
                                DataPoint dataPoint2 = this.Series.DataPoints[index1 + 1];
                                if (!this._dataPointsToForceVisibility.Contains(dataPoint2))
                                    this._dataPointsToForceVisibility.Add(dataPoint2);
                            }
                            this.ChartArea.UpdateSession.Update(dataPoint1);
                        }
                    }
                    else if (this._dataPointsToForceVisibility.Contains(dataPoint1))
                    {
                        dataPoint1.IsVisible = true;
                        dataPointViewState = dataPoint1.IsNewlyAdded ? DataPointViewState.Showing : DataPointViewState.Normal;
                    }
                }
                if (this._dataPointsToForceVisibility.Contains(dataPoint1))
                    this._dataPointsToForceVisibility.Remove(dataPoint1);
                that[index1] = dataPointViewState;
                ++index1;
            }
            this.IsSimplifiedRenderingModeCheckRequired = true;
            this.CheckSimplifiedRenderingMode();
            if (!flag2)
            {
                for (int index2 = 0; index2 < that.FastCount(); ++index2)
                {
                    if (that[index2] == DataPointViewState.Showing)
                        that[index2] = DataPointViewState.Normal;
                }
            }
            if (flag1 && this.Series.DataPoints.Count > 0 && (this.ChartArea != null && this.ChartArea.IsTemplateApplied) && (((XYDataPoint)this.Series.DataPoints[0]).XValueInScaleUnits.LessWithPrecision(0.0) && ((XYDataPoint)this.Series.DataPoints[this.Series.DataPoints.Count - 1]).XValueInScaleUnits.GreaterWithPrecision(1.0)))
            {
                int index2;
                for (index2 = 1; index2 < this.Series.DataPoints.Count - 2; ++index2)
                {
                    if ((this.Series.DataPoints[index2] as XYDataPoint).XValueInScaleUnits.GreaterOrEqualWithPrecision(0.0))
                    {
                        --index2;
                        break;
                    }
                }
                XYDataPoint xyDataPoint1 = this.Series.DataPoints[index2] as XYDataPoint;
                xyDataPoint1.IsVisible = true;
                that[index2] = xyDataPoint1.IsNewlyAdded ? DataPointViewState.Showing : DataPointViewState.Normal;
                this.ChartArea.UpdateSession.Update(xyDataPoint1);
                XYDataPoint xyDataPoint2 = this.Series.DataPoints[index2 + 1] as XYDataPoint;
                xyDataPoint2.IsVisible = true;
                that[index2 + 1] = xyDataPoint2.IsNewlyAdded ? DataPointViewState.Showing : DataPointViewState.Normal;
                this.ChartArea.UpdateSession.Update(xyDataPoint2);
            }
            int index3 = 0;
            foreach (DataPoint dataPoint in this.Series.DataPoints)
            {
                this.SetDataPointViewState(dataPoint, that[index3]);
                dataPoint.IsNewlyAdded = false;
                ++index3;
            }
        }

        internal override bool CheckSimplifiedRenderingMode()
        {
            bool flag = base.CheckSimplifiedRenderingMode();
            if (flag)
                this.VisibleDataPoints.ForEach<DataPoint>((Action<DataPoint>)(item => this.UpdateView(item)));
            return flag;
        }

        internal override FrameworkElement GetLegendSymbol()
        {
            Grid grid = new Grid();
            grid.Width = 20.0;
            grid.Height = 12.0;
            PointCollection pointCollection = new PointCollection();
            pointCollection.Add(new Point(0.0, 6.0));
            pointCollection.Add(new Point(20.0, 6.0));
            Collection<IAppearanceProvider> collection = new Collection<IAppearanceProvider>();
            LineDataPoint lineDataPoint1 = this.Series.DataPoints.OfType<LineDataPoint>().Where<LineDataPoint>(p =>
           {
               if (!p.ActualIsEmpty)
                   return p.IsVisible;
               return false;
           }).FirstOrDefault<LineDataPoint>();
            LineDataPoint lineDataPoint2 = this.Series.CreateDataPoint() as LineDataPoint;
            if (lineDataPoint1 != null)
            {
                lineDataPoint2.Stroke = lineDataPoint1.Stroke;
                lineDataPoint2.StrokeThickness = lineDataPoint1.StrokeThickness;
                lineDataPoint2.StrokeDashType = lineDataPoint1.StrokeDashType;
            }
            else if (this.Series.ItemsBinder != null)
                this.Series.ItemsBinder.Bind(lineDataPoint2, this.Series.DataContext);
            lineDataPoint2.Series = this.Series;
            collection.Add(lineDataPoint2);
            collection.Add(lineDataPoint2);
            lineDataPoint2.ActualEffect = lineDataPoint2.Effect;
            lineDataPoint2.ActualOpacity = lineDataPoint2.Opacity;
            PolylineControl polylineControl = new PolylineControl();
            polylineControl.Points = pointCollection;
            polylineControl.Appearances = collection;
            polylineControl.Update();
            polylineControl.Width = 20.0;
            polylineControl.Height = 12.0;
            grid.Children.Add(polylineControl);
            DataPoint dataPoint = lineDataPoint1 ?? lineDataPoint2;
            if (!this.IsSimplifiedRenderingModeEnabled && dataPoint.MarkerType != MarkerType.None)
            {
                FrameworkElement view = new MarkerControl();
                this.MarkerPresenter.BindViewToDataPoint(dataPoint, view, null);
                view.Opacity = lineDataPoint2.Opacity;
                view.Effect = lineDataPoint2.Effect;
                grid.Children.Add(view);
            }
            lineDataPoint2.Series = null;
            return grid;
        }

        private void RootPanel_MouseMove(object sender, MouseEventArgs e)
        {
            this._mousePosition = e.GetPosition(this.RootPanel);
        }

        private void Tooltip_Opened(object sender, RoutedEventArgs e)
        {
            this._tooltip.Content = LineSeriesPresenter.FindDataPoint(this.PolylineControl, this._mousePosition).ToolTipContent;
        }

        internal static XYDataPoint FindDataPoint(PolylineControl polyline, Point position)
        {
            XYDataPoint xyDataPoint1 = null;
            foreach (IAppearanceProvider appearance in polyline.Appearances)
            {
                XYDataPoint xyDataPoint2 = appearance as XYDataPoint;
                if (xyDataPoint2 != null && xyDataPoint2.View != null && !xyDataPoint2.ActualIsEmpty)
                {
                    if (xyDataPoint1 == null)
                        xyDataPoint1 = xyDataPoint2;
                    else if (LineSeriesPresenter.GetDistance(position, xyDataPoint2.View.AnchorPoint) < LineSeriesPresenter.GetDistance(position, xyDataPoint1.View.AnchorPoint))
                        xyDataPoint1 = xyDataPoint2;
                }
            }
            return xyDataPoint1;
        }

        private void UpdateToolTipStyle()
        {
            DataPoint dataPoint = this.VisibleDataPoints.FirstOrDefault<DataPoint>();
            if (this._tooltip == null || dataPoint == null)
                return;
            this._tooltip.Style = dataPoint.ToolTipStyle;
        }

        private static double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2.0) + Math.Pow(p2.Y - p1.Y, 2.0));
        }

        internal override Geometry GetSelectionOutline(DataPoint dataPoint)
        {
            if (dataPoint.View == null)
                return null;
            Rect rect1 = new Rect(0.0, 0.0, 6.0, 6.0);
            Point anchorPoint = dataPoint.View.AnchorPoint;
            if (double.IsNaN(anchorPoint.X) || double.IsInfinity(anchorPoint.X) || (double.IsNaN(anchorPoint.Y) || double.IsInfinity(anchorPoint.Y)))
                return null;
            Rect rect2 = new Rect(anchorPoint.X - rect1.Width / 2.0, anchorPoint.Y - rect1.Height / 2.0, rect1.Width, rect1.Height);
            FrameworkElement child = this.RootPanel.Children.OfType<FrameworkElement>().FirstOrDefault<FrameworkElement>();
            return new RectangleGeometry() { Rect = rect2.TranslateToParent(child, (FrameworkElement)this.ChartArea).Expand(1.0, 1.0) };
        }
    }
}
