using Microsoft.Reporting.Windows.Common.Internal;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal class DataPointView
    {
        public DataPoint DataPoint { get; internal set; }

        public bool IsLabelVisible { get; set; }

        public Point AnchorPoint { get; set; }

        public Rect AnchorRect { get; set; }

        public RectOrientation AnchorRectOrientation { get; set; }

        public FrameworkElement MainView { get; set; }

        public FrameworkElement HighlightView { get; set; }

        public FrameworkElement MarkerView { get; set; }

        public ContentControl LabelView { get; set; }

        internal DataPointView(DataPoint dataPoint)
        {
            this.DataPoint = dataPoint;
        }
    }
}
