using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class PointSeriesLabelPresenter : SeriesLabelPresenter
    {
        public PointSeriesLabelPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        internal override void BindViewToDataPoint(DataPoint dataPoint, FrameworkElement view, string valueName)
        {
            base.BindViewToDataPoint(dataPoint, view, valueName);
            LabelControl labelControl = view as LabelControl;
            if (labelControl == null)
                return;
            PointDataPoint pointDataPoint = dataPoint as PointDataPoint;
            if (pointDataPoint == null || !(valueName == "LabelPosition") && valueName != null)
                return;
            ContentPositions alignment;
            switch (pointDataPoint.LabelPosition)
            {
                case PointLabelPosition.Auto:
                    alignment = this.GetAutomaticLabelPosition(dataPoint);
                    break;
                case PointLabelPosition.TopLeft:
                    alignment = ContentPositions.TopLeft;
                    break;
                case PointLabelPosition.TopCenter:
                    alignment = ContentPositions.TopCenter;
                    break;
                case PointLabelPosition.TopRight:
                    alignment = ContentPositions.TopRight;
                    break;
                case PointLabelPosition.MiddleLeft:
                    alignment = ContentPositions.MiddleLeft;
                    break;
                case PointLabelPosition.MiddleCenter:
                    alignment = ContentPositions.MiddleCenter;
                    break;
                case PointLabelPosition.MiddleRight:
                    alignment = ContentPositions.MiddleRight;
                    break;
                case PointLabelPosition.BottomLeft:
                    alignment = ContentPositions.BottomLeft;
                    break;
                case PointLabelPosition.BottomCenter:
                    alignment = ContentPositions.BottomCenter;
                    break;
                case PointLabelPosition.BottomRight:
                    alignment = ContentPositions.BottomRight;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
            AnchorPanel.SetContentPosition(labelControl, alignment);
        }

        internal virtual ContentPositions GetAutomaticLabelPosition(DataPoint dataPoint)
        {
            return ContentPositions.TopCenter;
        }

        internal override void AdjustDataPointLabelVisibilityRating(LabelVisibilityManager.DataPointRange range, Dictionary<XYDataPoint, double> dataPointRanks)
        {
            XYDataPoint key = null;
            if (range.DataPoints.Count > 0)
                key = range.DataPoints[range.DataPoints.Count / 2] as XYDataPoint;
            if (key == null)
                return;
            if (dataPointRanks.ContainsKey(key))
            {
                Dictionary<XYDataPoint, double> dictionary;
                XYDataPoint index;
                (dictionary = dataPointRanks)[index = key] = dictionary[index] + 150.0;
            }
            else
                dataPointRanks.Add(key, 150.0);
        }
    }
}
