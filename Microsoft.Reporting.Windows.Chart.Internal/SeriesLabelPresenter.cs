using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class SeriesLabelPresenter : SeriesAttachedPresenter
    {
        private Panel _labelPanel;
        private PanelElementPool<LabelControl, DataPoint> _labelsPool;

        private ChartArea ChartArea
        {
            get
            {
                return this.SeriesPresenter.ChartArea;
            }
        }

        protected virtual Panel LabelsPanel
        {
            get
            {
                if (this._labelPanel == null)
                    this._labelPanel = this.ChartArea.ChartAreaLayerProvider.GetLayer(LayerType.SmartLabels, 1200, () => (Panel)new AnchorPanel()
                    {
                        CollisionDetectionEnabled = true
                    });
                return this._labelPanel;
            }
        }

        internal PanelElementPool<LabelControl, DataPoint> LabelsPool
        {
            get
            {
                if (this._labelsPool == null)
                    this._labelsPool = (PanelElementPool<LabelControl, DataPoint>)this.ChartArea.SingletonRegistry.GetSingleton(this.LabelsPanel, new Func<object>(this.CreateLabelsPool), new Action<object>(this.DisposeLabelsPool));
                return this._labelsPool;
            }
        }

        internal virtual bool IsDataPointVisibilityUsesXAxisOnly
        {
            get
            {
                return false;
            }
        }

        internal virtual double YScaleLabelDensity
        {
            get
            {
                return double.NaN;
            }
        }

        internal virtual double XScaleLabelDensity
        {
            get
            {
                return double.NaN;
            }
        }

        public SeriesLabelPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        private object CreateLabelsPool()
        {
            return new PanelElementPool<LabelControl, DataPoint>(this.LabelsPanel, new Func<LabelControl>(this.CreateLabel), new Action<LabelControl, DataPoint>(this.InitializeLabel), new Action<LabelControl>(this.ResetLabel));
        }

        private void DisposeLabelsPool(object poolObject)
        {
            (poolObject as PanelElementPool<LabelControl, DataPoint>).Clear();
        }

        private LabelControl CreateLabel()
        {
            return new LabelControl();
        }

        private void InitializeLabel(LabelControl label, DataPoint dataPoint)
        {
            label.Content = dataPoint.ActualLabelContent;
        }

        private void ResetLabel(LabelControl label)
        {
            label.Content = null;
        }

        private bool IsDataPointLabelVisible(DataPoint point)
        {
            if (this.SeriesPresenter.IsDataPointVisible(point) && this.SeriesPresenter.Series.LabelVisibility == Visibility.Visible && point.ActualLabelContent != null)
                return point.ActualLabelVisibility == Visibility.Visible;
            return false;
        }

        internal override void OnCreateView(DataPoint dataPoint)
        {
            if (!this.IsDataPointLabelVisible(dataPoint) || dataPoint.View == null)
                return;
            ContentControl viewElement = this.CreateViewElement(dataPoint);
            dataPoint.View.LabelView = viewElement;
            dataPoint.View.IsLabelVisible = true;
            this.BindViewToDataPoint(dataPoint, viewElement, null);
        }

        internal override void OnRemoveView(DataPoint dataPoint)
        {
            if (dataPoint.View == null)
                return;
            LabelControl element = dataPoint.View.LabelView as LabelControl;
            if (element == null)
                return;
            this.LabelsPool.Release(element);
            dataPoint.View.LabelView = null;
        }

        internal override void OnUpdateView(DataPoint dataPoint)
        {
            if (dataPoint.View == null)
                return;
            ContentControl contentControl = dataPoint.View.LabelView;
            if (this.IsDataPointLabelVisible(dataPoint))
            {
                if (contentControl == null)
                {
                    this.OnCreateView(dataPoint);
                    contentControl = dataPoint.View.LabelView;
                }
            }
            else if (contentControl != null)
            {
                this.OnRemoveView(dataPoint);
                contentControl = null;
            }
            if (contentControl == null)
                return;
            AnchorPanel.SetAnchorPoint(contentControl, dataPoint.View.AnchorPoint);
            AnchorPanel.SetValidContentPositions(contentControl, ContentPositions.All);
            AnchorPanel.SetAnchorRectOrientation(contentControl, dataPoint.View.AnchorRectOrientation);
            AnchorPanel.SetAnchorRect(contentControl, dataPoint.View.AnchorRect);
            AnchorPanel.SetOutsidePlacement(contentControl, OutsidePlacement.Disallowed);
            AnchorPanel.SetMaximumMovingDistance(contentControl, 0.0);
            Size desiredSize = contentControl.DesiredSize;
            if (desiredSize.Width > 0.0 && desiredSize.Height > 0.0)
                return;
            ((AnchorPanel)this.LabelsPanel).Invalidate();
        }

        internal override void OnSeriesRemoved()
        {
        }

        protected virtual ContentControl CreateViewElement(DataPoint dataPoint)
        {
            return this.LabelsPool.Get(dataPoint);
        }

        internal virtual void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            LabelControl labelControl = view as LabelControl;
            if (labelControl == null)
                return;
            IAppearanceProvider appearanceProvider = dataPoint;
            bool flag = false;
            if (appearanceProvider == null)
                return;
            if (valueName == "ActualLabelContent" || valueName == null)
            {
                flag = flag || labelControl.Content != dataPoint.ActualLabelContent;
                labelControl.Content = dataPoint.ActualLabelContent;
            }
            if (valueName == "LabelRotation" || valueName == null)
            {
                flag = flag || labelControl.RotationAngle != dataPoint.LabelRotation;
                labelControl.RotationAngle = dataPoint.LabelRotation;
            }
            if (valueName == "LabelStyle" || valueName == null)
            {
                flag = flag || labelControl.Style != dataPoint.LabelStyle;
                labelControl.Style = dataPoint.LabelStyle;
            }
            if (valueName == "ActualLabelMargin" || valueName == null)
                AnchorPanel.SetAnchorMargin(labelControl, dataPoint.ActualLabelMargin);
            if (valueName == "ActualEffect" || valueName == null)
            {
                if (this.SeriesPresenter.Series.IsDataPointAppearsUnselected(dataPoint))
                    labelControl.UpdateActuialEffect(dataPoint.ActualEffect);
                else
                    labelControl.UpdateActuialEffect(null);
            }
            if (valueName == "ActualOpacity" || valueName == null)
            {
                if (this.SeriesPresenter.Series.IsDataPointAppearsUnselected(dataPoint))
                    labelControl.UpdateActuialOpacity(new double?(dataPoint.ActualOpacity));
                else
                    labelControl.UpdateActuialOpacity(new double?());
            }
            if (!flag)
                return;
            ((AnchorPanel)this.LabelsPanel).Invalidate();
        }

        internal virtual void AdjustDataPointLabelVisibilityRating(LabelVisibilityManager.DataPointRange range, Dictionary<XYDataPoint, double> dataPointRanks)
        {
        }
    }
}
