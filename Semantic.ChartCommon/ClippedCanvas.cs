using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class ClippedCanvas : Canvas
    {
        public ClippedCanvas()
        {
            this.Loaded += new RoutedEventHandler(this.ClippedCanvas_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(this.ClippedCanvas_SizeChanged);
        }

        private void ClippedCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ClipCanvasToBounds();
        }

        private void ClippedCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            this.ClipCanvasToBounds();
        }

        private void ClipCanvasToBounds()
        {
            this.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight)
            };
        }
    }
}
