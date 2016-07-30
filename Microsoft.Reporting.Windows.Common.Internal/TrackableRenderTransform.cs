using System.Windows;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    internal class TrackableRenderTransform
    {
        internal static readonly DependencyProperty TransformProperty = AttachedProperty.RegisterAttached("Transform", typeof(GeneralTransform), typeof(TrackableRenderTransform), new PropertyMetadata(new PropertyChangedCallback(TrackableRenderTransform.OnTransformChanged)));

        internal static event PropertyChangedCallback TransformChanged;

        internal static void SetTransform(DependencyObject o, GeneralTransform value)
        {
            o.SetValue(TrackableRenderTransform.TransformProperty, value);
        }

        internal static GeneralTransform GetTransform(DependencyObject o)
        {
            return (GeneralTransform)o.GetValue(TrackableRenderTransform.TransformProperty);
        }

        private static void OnTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(UIElement.RenderTransformProperty, e.NewValue);
            if (TrackableRenderTransform.TransformChanged == null)
                return;
            TrackableRenderTransform.TransformChanged(d, e);
        }
    }
}
