using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [System.Windows.Markup.ContentProperty("Child")]
    public class RotatableControl : Control
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Child", typeof(FrameworkElement), typeof(RotatableControl), new PropertyMetadata(new PropertyChangedCallback(RotatableControl.ChildChanged)));
        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof(double), typeof(RotatableControl), new PropertyMetadata(0.0, new PropertyChangedCallback(RotatableControl.OnRotationAnglePropertyChanged)));
        internal const string RotationAnglePropertyName = "RotationAngle";
        private RotateTransform _rotateTransform;
        private Panel _layoutRoot;

        public FrameworkElement Child
        {
            get
            {
                return (FrameworkElement)this.GetValue(RotatableControl.ContentProperty);
            }
            set
            {
                this.SetValue(RotatableControl.ContentProperty, value);
            }
        }

        public double RotationAngle
        {
            get
            {
                return (double)this.GetValue(RotatableControl.RotationAngleProperty);
            }
            set
            {
                this.SetValue(RotatableControl.RotationAngleProperty, value);
            }
        }

        public RotatableControl()
        {
            this.IsTabStop = false;
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'><Grid x:Name='LayoutRoot' Background='{TemplateBinding Background}'><Grid.RenderTransform><RotateTransform x:Name='MatrixTransform'/></Grid.RenderTransform></Grid></ControlTemplate>")))
                this.Template = (ControlTemplate)XamlReader.Load(memoryStream);
        }

        private static void ChildChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((RotatableControl)o).OnChildChanged((FrameworkElement)e.NewValue);
        }

        private void OnChildChanged(FrameworkElement newContent)
        {
            if (this._layoutRoot == null)
                return;
            this._layoutRoot.Children.Clear();
            if (newContent != null)
                this._layoutRoot.Children.Add(newContent);
            this.InvalidateMeasure();
        }

        private static void OnRotationAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RotatableControl)d).OnRotationAnglePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnRotationAnglePropertyChanged(object oldValue, object newValue)
        {
            this.ApplyTransform((double)newValue);
            this.InvalidateMeasure();
        }

        public override void OnApplyTemplate()
        {
            FrameworkElement child = this.Child;
            this.Child = null;
            base.OnApplyTemplate();
            this._layoutRoot = this.GetTemplateChild("LayoutRoot") as Panel;
            this._rotateTransform = this.GetTemplateChild("MatrixTransform") as RotateTransform;
            this.Child = child;
            this.ApplyTransform(this.RotationAngle);
        }

        private void ApplyTransform(double angle)
        {
            if (this._rotateTransform == null)
                return;
            this._rotateTransform.Angle = angle;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this._layoutRoot == null || this.Child == null)
                return Size.Empty;
            Size size = availableSize;
            if (!double.IsInfinity(size.Width) && !double.IsInfinity(size.Height))
                size = this.TransformSize(size);
            this._layoutRoot.Measure(size);
            return this.TransformSize(this._layoutRoot.DesiredSize);
        }

        private Size TransformSize(Size size)
        {
            Rect rect = this.TransformRect(size);
            return new Size(rect.Width, rect.Height);
        }

        private Rect TransformRect(Size size)
        {
            return new Rect(0.0, 0.0, size.Width, size.Height).Transform(this._rotateTransform);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this._layoutRoot == null || this.Child == null)
                return finalSize;
            Size desiredSize = this._layoutRoot.DesiredSize;
            Rect rect = this.TransformRect(desiredSize);
            this._layoutRoot.Arrange(new Rect(-rect.Left + (finalSize.Width - rect.Width) / 2.0, -rect.Top + (finalSize.Height - rect.Height) / 2.0, desiredSize.Width, desiredSize.Height));
            return finalSize;
        }
    }
}
