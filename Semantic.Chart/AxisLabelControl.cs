using Microsoft.Reporting.Windows.Common.Internal;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [TemplatePart(Name = "RotatableControl", Type = typeof(RotatableControl))]
    public class AxisLabelControl : ContentControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(AxisLabelControl), new PropertyMetadata(null, new PropertyChangedCallback(AxisLabelControl.OnTextPropertyChanged)));
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(AxisLabelControl), new PropertyMetadata(TextWrapping.NoWrap, new PropertyChangedCallback(AxisLabelControl.OnTextWrappingPropertyChanged)));
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(AxisLabelControl), new PropertyMetadata(TextAlignment.Left, new PropertyChangedCallback(AxisLabelControl.OnTextAlignmentPropertyChanged)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(AxisLabelControl), new PropertyMetadata(new PropertyChangedCallback(AxisLabelControl.OnCornerRadiusPropertyChanged)));
        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof(double), typeof(AxisLabelControl), new PropertyMetadata(0.0, new PropertyChangedCallback(AxisLabelControl.OnRotationAnglePropertyChanged)));
        public static readonly DependencyProperty ActualContentProperty = DependencyProperty.Register("ActualContent", typeof(object), typeof(AxisLabelControl), new PropertyMetadata(null, new PropertyChangedCallback(AxisLabelControl.OnActualContentPropertyChanged)));
        internal const string TextPropertyName = "Text";
        internal const string TextWrappingPropertyName = "TextWrapping";
        internal const string TextAlignmentPropertyName = "TextAlignment";
        internal const string CornerRadiusPropertyName = "CornerRadius";
        internal const string RotationAnglePropertyName = "RotationAngle";
        protected const string RotatableControltName = "RotatableControl";
        internal const string ActualContentPropertyName = "ActualContent";
        private RotatableControl _rotatableControl;

        public string Text
        {
            get
            {
                return (string)this.GetValue(AxisLabelControl.TextProperty);
            }
            set
            {
                this.SetValue(AxisLabelControl.TextProperty, value);
            }
        }

        public TextWrapping TextWrapping
        {
            get
            {
                return (TextWrapping)this.GetValue(AxisLabelControl.TextWrappingProperty);
            }
            set
            {
                this.SetValue(AxisLabelControl.TextWrappingProperty, value);
            }
        }

        public TextAlignment TextAlignment
        {
            get
            {
                return (TextAlignment)this.GetValue(AxisLabelControl.TextAlignmentProperty);
            }
            set
            {
                this.SetValue(AxisLabelControl.TextAlignmentProperty, value);
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)this.GetValue(AxisLabelControl.CornerRadiusProperty);
            }
            set
            {
                this.SetValue(AxisLabelControl.CornerRadiusProperty, value);
            }
        }

        public double RotationAngle
        {
            get
            {
                return (double)this.GetValue(AxisLabelControl.RotationAngleProperty);
            }
            set
            {
                this.SetValue(AxisLabelControl.RotationAngleProperty, value);
            }
        }

        public object ActualContent
        {
            get
            {
                return this.GetValue(AxisLabelControl.ActualContentProperty);
            }
            set
            {
                this.SetValue(AxisLabelControl.ActualContentProperty, value);
            }
        }

        private TextBlock TextBlockAsContent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AxisLabelControl()
        {
            this.DefaultStyleKey = typeof(AxisLabelControl);
            this.TextBlockAsContent = new TextBlock()
            {
                TextTrimming = TextTrimming.WordEllipsis
            };
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisLabelControl)d).OnTextPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTextPropertyChanged(object oldValue, object newValue)
        {
            this.TextBlockAsContent.Text = newValue as string;
            this.OnPropertyChanged("Text");
        }

        private static void OnTextWrappingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisLabelControl)d).OnTextWrappingPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTextWrappingPropertyChanged(object oldValue, object newValue)
        {
            this.TextBlockAsContent.TextWrapping = (TextWrapping)newValue;
            this.OnPropertyChanged("TextWrapping");
        }

        private static void OnTextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisLabelControl)d).OnTextAlignmentPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTextAlignmentPropertyChanged(object oldValue, object newValue)
        {
            this.TextBlockAsContent.TextAlignment = (TextAlignment)newValue;
            this.OnPropertyChanged("TextAlignment");
        }

        private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisLabelControl)d).OnCornerRadiusPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnCornerRadiusPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("CornerRadius");
        }

        private static void OnRotationAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisLabelControl)d).OnRotationAnglePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnRotationAnglePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("RotationAngle");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._rotatableControl = this.GetTemplateChild("RotatableControl") as RotatableControl;
            this.UpdateActualContent();
        }

        public Size GetLabelContentDesiredSize()
        {
            if (this._rotatableControl != null)
            {
                UIElement uiElement = this._rotatableControl.Child;
                if (uiElement != null)
                    return uiElement.DesiredSize;
            }
            return new Size(0.0, 0.0);
        }

        public void SetLabelContentBounds(double width, double height)
        {
            if (this._rotatableControl == null)
                return;
            FrameworkElement child = this._rotatableControl.Child;
            if (child == null)
                return;
            child.MaxWidth = width;
            child.MaxHeight = height;
        }

        private static void OnActualContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxisLabelControl)d).OnActualContentPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualContentPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualContent");
        }

        public void SetContent(object content)
        {
            this.Text = content as string;
            if (string.IsNullOrEmpty(this.Text))
                this.Content = content;
            else
                this.Content = null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this.UpdateActualContent();
        }

        private void UpdateActualContent()
        {
            this.ActualContent = this.Content != null ? this.Content : this.TextBlockAsContent;
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
