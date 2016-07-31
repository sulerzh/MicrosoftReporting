using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class ColumnSeriesPresenter : XYSeriesPresenter
    {
        private const double MaxScreeenCoordinate = 1E+20;
        private PanelElementPool<FrameworkElement, DataPoint> _dataPointElementPool;

        private PanelElementPool<FrameworkElement, DataPoint> DataPointElementPool
        {
            get
            {
                if (this._dataPointElementPool == null)
                {
                    this._dataPointElementPool = new PanelElementPool<FrameworkElement, DataPoint>(this.RootPanel, new Func<FrameworkElement>(this.CreateViewElement), new Action<FrameworkElement, DataPoint>(this.UpdateViewElement), new Action<FrameworkElement>(this.ResetViewElement));
                    this._dataPointElementPool.MaxElementCount = 100;
                }
                return this._dataPointElementPool;
            }
        }

        public double PointWidth { get; set; }

        public double PointClusterOffset { get; set; }

        public ColumnSeriesPresenter(XYSeries series)
          : base(series)
        {
            this.IsRootPanelClipped = true;
            this.DefaultSimplifiedRenderingThreshold = 200;
        }

        private FrameworkElement CreateViewElement()
        {
            if (this.IsSimplifiedRenderingModeEnabled)
                return new Rectangle();
            return new BarControl();
        }

        private void UpdateViewElement(FrameworkElement element, DataPoint dataPoint)
        {
            element.DataContext = dataPoint;
            this.BindViewToDataPoint(dataPoint, element, null);
        }

        private void ResetViewElement(FrameworkElement element)
        {
            SeriesTooltipPresenter.ClearToolTip(element);
            element.DataContext = null;
        }

        internal override SeriesMarkerPresenter CreateMarkerPresenter()
        {
            return new SeriesMarkerPresenter(this);
        }

        internal override SeriesLabelPresenter CreateLabelPresenter()
        {
            return new ColumnSeriesLabelPresenter((SeriesPresenter)this);
        }

        public override void InvalidateSeries()
        {
            if (this.ChartArea != null)
                this.ChartArea.UpdateSession.ExecuteOnceBeforeUpdating(() => this.CalculateRelatedSeriesPointWidth(), new Tuple<string, Axis>("__CalculatePointWidth__", this.Series.XAxis));
            base.InvalidateSeries();
        }

        protected override void UpdateView()
        {
            if (this.ChartArea != null)
                this.ChartArea.UpdateSession.ExecuteOnceDuringUpdating(() => this.CalculateRelatedSeriesPointWidth(), new Tuple<string, Axis>("__CalculatePointWidth__", this.Series.XAxis));
            base.UpdateView();
        }

        protected override void UpdateRelatedSeriesPresenters()
        {
            this.ChartArea.UpdateSession.BeginUpdates();
            if (this.XYChartArea != null)
                this.XYChartArea.Series.Where<XYSeries>(item =>
               {
                   if (item.GetType() == this.Series.GetType())
                       return item != this.Series;
                   return false;
               }).ForEachWithIndex<XYSeries>((Action<XYSeries, int>)((item, index) => this.ChartArea.UpdateSession.Update(item)));
            base.UpdateRelatedSeriesPresenters();
            this.ChartArea.UpdateSession.EndUpdates();
        }

        protected override FrameworkElement CreateViewElement(DataPoint dataPoint)
        {
            if (this.DataPointElementPool.InUseElementCount == 0)
            {
                this.IsSimplifiedRenderingModeCheckRequired = true;
                this.CheckSimplifiedRenderingMode();
            }
            return this.DataPointElementPool.Get(dataPoint);
        }

        protected override void RemoveView(DataPoint dataPoint)
        {
            if (dataPoint.View != null && dataPoint.View.MainView != null)
            {
                this.DataPointElementPool.Release(dataPoint.View.MainView);
                dataPoint.View.MainView = null;
                this.ChartArea.UpdateSession.ExecuteOnceAfterUpdating(() => this.DataPointElementPool.AdjustPoolSize(), new Tuple<Series, string>((Series)this.Series, "__AdjustDataPointElementPoolSize__"), null);
            }
            base.RemoveView(dataPoint);
        }

        protected override void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            IAppearanceProvider appearanceProvider = dataPoint;
            if (appearanceProvider != null)
            {
                BarControl barControl = view as BarControl;
                if (barControl != null)
                {
                    if (valueName == "Fill" || valueName == null)
                        barControl.Background = appearanceProvider.Fill;
                    if (valueName == "Stroke" || valueName == null)
                        barControl.BorderBrush = appearanceProvider.Stroke;
                    if (valueName == "StrokeThickness" || valueName == null)
                        barControl.BorderThickness = new Thickness(appearanceProvider.StrokeThickness);
                    if (valueName == "Opacity" || valueName == "ActualOpacity" || valueName == null)
                        barControl.Opacity = dataPoint.ActualOpacity;
                    if (valueName == "Effect" || valueName == "ActualEffect" || valueName == null)
                        barControl.Effect = dataPoint.ActualEffect;
                }
                else
                {
                    Shape shape = view as Shape;
                    if (shape != null)
                    {
                        if (valueName == "Fill" || valueName == null)
                            shape.Fill = dataPoint.Fill;
                        if (valueName == "Opacity" || valueName == "ActualOpacity" || valueName == null)
                            shape.Opacity = dataPoint.ActualOpacity;
                        if (valueName == "Effect" || valueName == "ActualEffect" || valueName == null)
                            shape.Effect = dataPoint.ActualEffect is ShaderEffect ? dataPoint.ActualEffect : null;
                    }
                }
            }
            DataPointView dataPointView = dataPoint != null ? dataPoint.View : null;
            if (dataPointView == null)
                return;
            this.LabelPresenter.BindViewToDataPoint(dataPoint, dataPointView.LabelView, valueName);
            this.MarkerPresenter.BindViewToDataPoint(dataPoint, dataPointView.MarkerView, valueName);
        }

        internal virtual double GetYOffsetInAxisUnits(XYDataPoint dataPoint, Point valuePoint, Point basePoint)
        {
            return 0.0;
        }

        internal virtual Point GetPositionInAxisUnits(XYDataPoint dataPointXY)
        {
            return new Point(this.Series.XAxis.AxisPresenter.ConvertScaleToAxisUnits(dataPointXY.XValueInScaleUnits) ?? 0.0, this.Series.YAxis.AxisPresenter.ConvertScaleToAxisUnits(dataPointXY.YValueInScaleUnits) ?? 0.0);
        }

        internal virtual bool CanAdjustHeight()
        {
            return true;
        }

        protected override void UpdateView(DataPoint dataPoint)
        {
            if (!this.IsDataPointViewVisible(dataPoint))
                return;
            DateTime now = DateTime.Now;
            XYDataPoint xyDataPoint = dataPoint as XYDataPoint;
            if (xyDataPoint != null && this.CanGraph(xyDataPoint))
            {
                DataPointView view = dataPoint.View;
                if (view != null)
                {
                    FrameworkElement mainView = view.MainView;
                    if (mainView != null)
                    {
                        bool flag = this.ChartArea.Orientation != Orientation.Horizontal;
                        RectOrientation rectOrientation = RectOrientation.BottomTop;
                        Point positionInAxisUnits = this.GetPositionInAxisUnits(xyDataPoint);
                        Point point1 = new Point(Math.Round(positionInAxisUnits.X), Math.Round(positionInAxisUnits.Y));
                        object crossingPosition = this.Series.YAxis.Scale.ActualCrossingPosition;
                        Point basePoint = new Point(positionInAxisUnits.X, this.Series.YAxis.AxisPresenter.ConvertDataToAxisUnits(crossingPosition) ?? 0.0);
                        Point point2 = new Point(Math.Round(basePoint.X), Math.Round(basePoint.Y));
                        double num1 = point1.X + Math.Round(this.PointClusterOffset);
                        double num2 = this.MinMaxScreenCoordinates(positionInAxisUnits.Y);
                        double num3 = Math.Round(this.PointWidth);
                        double height = this.MinMaxScreenCoordinates(basePoint.Y - positionInAxisUnits.Y);
                        if (ValueHelper.Compare(xyDataPoint.YValue as IComparable, crossingPosition as IComparable) != 0 && Math.Abs(height) < 2.0 && this.CanAdjustHeight())
                        {
                            height = basePoint.Y - positionInAxisUnits.Y >= 0.0 ? 2.0 : -2.0;
                            num2 = point2.Y - height;
                        }
                        if (height < 0.0)
                        {
                            rectOrientation = RectOrientation.TopBottom;
                            height = Math.Abs(height);
                            num2 -= height;
                        }
                        double num4 = this.MinMaxScreenCoordinates(this.GetYOffsetInAxisUnits(xyDataPoint, positionInAxisUnits, basePoint));
                        double num5 = Math.Round(num2 - num4);
                        double num6 = this.AdjustColumnHeight(height);
                        if (flag)
                        {
                            if (rectOrientation == RectOrientation.BottomTop)
                                rectOrientation = RectOrientation.RightLeft;
                            else if (rectOrientation == RectOrientation.TopBottom)
                                rectOrientation = RectOrientation.LeftRight;
                            Canvas.SetLeft(mainView, num5);
                            Canvas.SetTop(mainView, num1);
                            mainView.Width = num6;
                            mainView.Height = num3;
                            view.AnchorRect = new Rect(num5, num1, num6, num3);
                            view.AnchorPoint = rectOrientation != RectOrientation.RightLeft ? new Point(num5 + num6, num1 + this.PointWidth / 2.0) : new Point(num5, num1 + this.PointWidth / 2.0);
                        }
                        else
                        {
                            Canvas.SetLeft(mainView, num1);
                            Canvas.SetTop(mainView, num5);
                            mainView.Width = num3;
                            mainView.Height = num6;
                            view.AnchorRect = new Rect(num1, num5, num3, num6);
                            view.AnchorPoint = rectOrientation != RectOrientation.BottomTop ? new Point(num1 + this.PointWidth / 2.0, num5 + num6) : new Point(num1 + this.PointWidth / 2.0, num5);
                        }
                        BarControl barControl = mainView as BarControl;
                        if (barControl != null)
                            barControl.Orientation = rectOrientation;
                        view.AnchorRectOrientation = rectOrientation;
                    }
                }
            }
            base.UpdateView(dataPoint);
            if (this.ChartArea == null)
                return;
            this.ChartArea.UpdateSession.AddCounter("ColumnSeriesPresenter.UpdateView", DateTime.Now - now);
        }

        private double MinMaxScreenCoordinates(double value)
        {
            return Math.Max(Math.Min(value, 1E+20), -1E+20);
        }

        protected virtual double AdjustColumnHeight(double height)
        {
            return Math.Ceiling(height);
        }

        internal override bool CanGraph(XYDataPoint dataPointXY)
        {
            if (ValueHelper.CanGraph(dataPointXY.XValueInScaleUnits) && ValueHelper.CanGraph(dataPointXY.YValueInScaleUnits) && dataPointXY.XValueInScaleUnits.GreaterOrEqualWithPrecision(0.0))
                return dataPointXY.XValueInScaleUnits.LessOrEqualWithPrecision(1.0);
            return false;
        }

        protected override void OnRemoved()
        {
            if (this._dataPointElementPool != null)
                this._dataPointElementPool.Clear();
            base.OnRemoved();
        }

        internal virtual Dictionary<object, List<Series>> GroupSeriesByClusters(IList<XYSeries> clusterSeries)
        {
            Dictionary<object, List<Series>> clusterGroups = new Dictionary<object, List<Series>>();
            foreach (ColumnSeries columnSeries1 in clusterSeries)
            {
                List<Series> seriesList = new List<Series>();
                if (columnSeries1.ClusterGroupKey == null)
                {
                    seriesList.Add(columnSeries1);
                    clusterGroups.Add(new Tuple<ColumnSeries>(columnSeries1), seriesList);
                }
                else if (!clusterGroups.ContainsKey(columnSeries1.ClusterGroupKey))
                {
                    foreach (ColumnSeries columnSeries2 in clusterSeries)
                    {
                        if (ValueHelper.AreEqual(columnSeries2.ClusterGroupKey, columnSeries1.ClusterGroupKey))
                            seriesList.Add(columnSeries2);
                    }
                    clusterGroups.Add(columnSeries1.ClusterGroupKey, seriesList);
                }
            }
            IList<StackedColumnSeries> stackedColumnSeriesList = this.XYChartArea.Series.Where<XYSeries>((Func<XYSeries, bool>)(s => s.Visibility == Visibility.Visible)).OfType<StackedColumnSeries>().ToList<StackedColumnSeries>();
            foreach (List<Series> seriesList in clusterGroups.Values)
            {
                List<Series> groupSeries = seriesList;
                ((IEnumerable<Series>)groupSeries.ToArray()).Where<Series>(s => s is StackedColumnSeries).ForEach<Series>(s => groupSeries.Remove(s));
            }
          ((IEnumerable<KeyValuePair<object, List<Series>>>)clusterGroups.ToArray<KeyValuePair<object, List<Series>>>()).Where<KeyValuePair<object, List<Series>>>(item => item.Value.Count == 0).ForEach<KeyValuePair<object, List<Series>>>(item => clusterGroups.Remove(item.Key));
            foreach (StackedColumnSeries series1 in stackedColumnSeriesList)
            {
                List<Series> seriesList = new List<Series>();
                Tuple<DataValueType, DataValueType, bool, object> seriesKey = StackedColumnSeriesPresenter.GetSeriesKey(series1);
                if (!clusterGroups.ContainsKey(seriesKey) && seriesKey.Item1 != DataValueType.Auto)
                {
                    foreach (StackedColumnSeries series2 in stackedColumnSeriesList)
                    {
                        if (StackedColumnSeriesPresenter.GetSeriesKey(series2).Equals(seriesKey))
                            seriesList.Add(series2);
                    }
                    clusterGroups.Add(seriesKey, seriesList);
                }
            }
            return clusterGroups;
        }

        internal void CalculateRelatedSeriesPointWidth()
        {
            if (this.ChartArea == null)
                return;
            DateTime now = DateTime.Now;
            IList<XYSeries> clusterSeries = this.XYChartArea.FindClusterSeries(this.Series).ToList<XYSeries>();
            double clusterSize = this.Series.XAxis.AxisPresenter.GetClusterSize(this);
            Dictionary<object, List<Series>> dictionary = this.GroupSeriesByClusters(clusterSeries);
            int count = dictionary.Count;
            double val1_1 = double.MaxValue;
            foreach (ColumnSeries columnSeries in clusterSeries)
                val1_1 = Math.Min(val1_1, columnSeries.PointGapRelativeWidth);
            double val1_2 = double.MaxValue;
            foreach (ColumnSeries columnSeries in clusterSeries)
            {
                if (columnSeries.PointWidth.HasValue)
                    val1_2 = Math.Min(val1_2, columnSeries.PointWidth.Value);
            }
            if (val1_2 == double.MaxValue)
                val1_2 = clusterSize * (1.0 - val1_1) / count;
            foreach (ColumnSeries columnSeries in clusterSeries)
            {
                if (columnSeries.PointMaximumWidth.HasValue && val1_2 > columnSeries.PointMaximumWidth.Value)
                    val1_2 = columnSeries.PointMaximumWidth.Value;
            }
            foreach (ColumnSeries columnSeries in clusterSeries)
            {
                if (columnSeries.PointMinimumWidth.HasValue && val1_2 < columnSeries.PointMinimumWidth.Value)
                    val1_2 = columnSeries.PointMinimumWidth.Value;
            }
            if (val1_2 < 1.0)
                val1_2 = 1.0;
            int num1 = 0;
            double num2 = val1_2 * count;
            foreach (List<Series> seriesList in dictionary.Values)
            {
                foreach (ColumnSeries columnSeries in seriesList)
                {
                    ColumnSeriesPresenter columnSeriesPresenter = (ColumnSeriesPresenter)columnSeries.SeriesPresenter;
                    columnSeriesPresenter.PointWidth = val1_2;
                    columnSeriesPresenter.PointClusterOffset = -num2 / 2.0 + num1 * val1_2;
                    if (count > 1)
                        columnSeriesPresenter.PointClusterOffset -= num1 * (count * val1_2 - num2) / (count - 1);
                    if (columnSeries.PointGapRelativeWidth > val1_1)
                    {
                        double num3 = columnSeriesPresenter.PointWidth - clusterSize * (1.0 - columnSeries.PointGapRelativeWidth) / count;
                        if (columnSeriesPresenter.PointWidth - num3 < 1.0)
                            num3 = columnSeriesPresenter.PointWidth - 1.0;
                        columnSeriesPresenter.PointWidth -= num3;
                        columnSeriesPresenter.PointClusterOffset += num3 / 2.0;
                    }
                }
                ++num1;
            }
            this.ChartArea.UpdateSession.AddCounter("ColumnSeriesPresenter.CalculateRelatedSeriesPointWidth", DateTime.Now - now);
        }

        public override AxisMargin GetSeriesMarginInfo(AutoBool isAxisMarginVisible)
        {
            if (isAxisMarginVisible == AutoBool.False)
                return AxisMargin.Empty;
            return base.GetSeriesMarginInfo(AutoBool.True);
        }

        internal override bool CheckSimplifiedRenderingMode()
        {
            bool flag = base.CheckSimplifiedRenderingMode();
            if (flag)
            {
                List<DataPoint> list = this.VisibleDataPoints.Where<DataPoint>(dataPoint =>
               {
                   if (dataPoint.View != null)
                       return dataPoint.View.MainView != null;
                   return false;
               }).ToList<DataPoint>();
                list.ForEach(dataPoint =>
               {
                   this.DataPointElementPool.Release(dataPoint.View.MainView);
                   this.OnViewRemoved(dataPoint);
                   dataPoint.View.MainView = (FrameworkElement)null;
                   dataPoint.View = (DataPointView)null;
               });
                this.DataPointElementPool.Clear();
                list.ForEach((Action<DataPoint>)(dataPoint => this.CreateView(dataPoint)));
            }
            return flag;
        }

        internal override FrameworkElement GetLegendSymbol()
        {
            FrameworkElement viewElement = this.CreateViewElement();
            XYDataPoint xyDataPoint = this.Series.DataPoints.OfType<XYDataPoint>().Where<XYDataPoint>(p =>
           {
               if (!p.ActualIsEmpty)
                   return p.IsVisible;
               return false;
           }).FirstOrDefault<XYDataPoint>();
            if (xyDataPoint != null)
            {
                this.BindViewToDataPoint(xyDataPoint, viewElement, null);
            }
            else
            {
                xyDataPoint = this.Series.CreateDataPoint() as XYDataPoint;
                xyDataPoint.Series = this.Series;
                if (this.Series.ItemsBinder != null)
                    this.Series.ItemsBinder.Bind(xyDataPoint, this.Series.DataContext);
                this.BindViewToDataPoint(xyDataPoint, viewElement, null);
                xyDataPoint.Series = null;
            }
            viewElement.Opacity = xyDataPoint.Opacity;
            viewElement.Effect = xyDataPoint.Effect;
            viewElement.Width = 20.0;
            viewElement.Height = 12.0;
            return viewElement;
        }
    }
}
