using System.Collections.Generic;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class BubbleSeriesLabelPresenter : PointSeriesLabelPresenter
    {
        public BubbleSeriesLabelPresenter(SeriesPresenter seriesPresenter)
          : base(seriesPresenter)
        {
        }

        internal override void AdjustDataPointLabelVisibilityRating(LabelVisibilityManager.DataPointRange range, Dictionary<XYDataPoint, double> dataPointRanks)
        {
            BubbleDataPoint bubbleDataPoint1 = null;
            double num = double.MinValue;
            foreach (XYDataPoint dataPoint in range.DataPoints)
            {
                BubbleDataPoint bubbleDataPoint2 = dataPoint as BubbleDataPoint;
                if (bubbleDataPoint2 != null && bubbleDataPoint2.SizeValueInScaleUnitsWithoutAnimation > num)
                {
                    num = bubbleDataPoint2.SizeValueInScaleUnitsWithoutAnimation;
                    bubbleDataPoint1 = bubbleDataPoint2;
                }
            }
            if (bubbleDataPoint1 == null)
                return;
            if (dataPointRanks.ContainsKey(bubbleDataPoint1))
            {
                Dictionary<XYDataPoint, double> dictionary;
                XYDataPoint index;
                (dictionary = dataPointRanks)[index = bubbleDataPoint1] = dictionary[index] + 150.0;
            }
            else
                dataPointRanks.Add(bubbleDataPoint1, 150.0);
        }
    }
}
