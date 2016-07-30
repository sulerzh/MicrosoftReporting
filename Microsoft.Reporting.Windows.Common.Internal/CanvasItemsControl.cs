using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class CanvasItemsControl : ItemsControl
    {
        public static readonly DependencyProperty XBindingPathProperty = DependencyProperty.Register("XBindingPath", typeof(string), typeof(CanvasItemsControl), new PropertyMetadata("X"));
        public static readonly DependencyProperty YBindingPathProperty = DependencyProperty.Register("YBindingPath", typeof(string), typeof(CanvasItemsControl), new PropertyMetadata("Y"));

        public string XBindingPath
        {
            get
            {
                return (string)this.GetValue(CanvasItemsControl.XBindingPathProperty);
            }
            set
            {
                this.SetValue(CanvasItemsControl.XBindingPathProperty, value);
            }
        }

        public string YBindingPath
        {
            get
            {
                return (string)this.GetValue(CanvasItemsControl.YBindingPathProperty);
            }
            set
            {
                this.SetValue(CanvasItemsControl.YBindingPathProperty, value);
            }
        }

        public CanvasItemsControl()
        {
            this.DefaultStyleKey = typeof(CanvasItemsControl);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            FrameworkElement frameworkElement = element as FrameworkElement;
            Binding binding1 = new Binding(this.XBindingPath);
            Binding binding2 = new Binding(this.YBindingPath);
            frameworkElement.SetBinding(Canvas.LeftProperty, binding1);
            frameworkElement.SetBinding(Canvas.TopProperty, binding2);
            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
