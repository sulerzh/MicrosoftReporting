using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [TemplatePart(Name = "TitleContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "TitleLayout", Type = typeof(RotatableControl))]
    public class Title : ContentControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TextOrientationProperty = DependencyProperty.Register("TextOrientation", typeof(TextOrientation), typeof(Title), new PropertyMetadata(TextOrientation.Auto, new PropertyChangedCallback(Title.OnTextOrientationPropertyChanged)));
        public static readonly DependencyProperty ActualTextOrientationProperty = DependencyProperty.Register("ActualTextOrientation", typeof(TextOrientation), typeof(Title), new PropertyMetadata(TextOrientation.Horizontal, new PropertyChangedCallback(Title.OnActualTextOrientationPropertyChanged)));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Title), new PropertyMetadata(new CornerRadius(0.0), new PropertyChangedCallback(Title.OnCornerRadiusPropertyChanged)));
        public static readonly DependencyProperty ActualRotationAngleProperty = DependencyProperty.Register("ActualRotationAngle", typeof(double), typeof(Title), new PropertyMetadata(0.0, new PropertyChangedCallback(Title.OnActualRotationAnglePropertyChanged)));
        public static readonly DependencyProperty ActualContentProperty = DependencyProperty.Register("ActualContent", typeof(object), typeof(Title), new PropertyMetadata(null));
        public static readonly DependencyProperty ActualTextContentProperty = DependencyProperty.Register("ActualTextContent", typeof(string), typeof(Title), new PropertyMetadata(null));
        protected const string TitleLayoutName = "TitleLayout";
        protected const string TitleContentName = "TitleContent";
        internal const string TextOrientationPropertyName = "TextOrientation";
        internal const string ActualTextOrientationPropertyName = "ActualTextOrientation";
        internal const string CornerRadiusPropertyName = "CornerRadius";
        internal const string ActualRotationAnglePropertyName = "ActualRotationAngle";
        internal const string ActualContentPropertyName = "ActualContent";
        internal const string ActualTextContentPropertyName = "ActualTextContent";

        public TextOrientation TextOrientation
        {
            get
            {
                return (TextOrientation)this.GetValue(Title.TextOrientationProperty);
            }
            set
            {
                this.SetValue(Title.TextOrientationProperty, value);
            }
        }

        public TextOrientation ActualTextOrientation
        {
            get
            {
                return (TextOrientation)this.GetValue(Title.ActualTextOrientationProperty);
            }
            protected set
            {
                this.SetValue(Title.ActualTextOrientationProperty, value);
            }
        }

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)this.GetValue(Title.CornerRadiusProperty);
            }
            set
            {
                this.SetValue(Title.CornerRadiusProperty, value);
            }
        }

        public double ActualRotationAngle
        {
            get
            {
                return (double)this.GetValue(Title.ActualRotationAngleProperty);
            }
            protected set
            {
                this.SetValue(Title.ActualRotationAngleProperty, value);
            }
        }

        public object ActualContent
        {
            get
            {
                return this.GetValue(Title.ActualContentProperty);
            }
            set
            {
                this.SetValue(Title.ActualContentProperty, value);
            }
        }

        public string ActualTextContent
        {
            get
            {
                return (string)this.GetValue(Title.ActualTextContentProperty);
            }
            set
            {
                this.SetValue(Title.ActualTextContentProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Title()
        {
            this.DefaultStyleKey = typeof(Title);
        }

        private static void OnTextOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Title)d).OnTextOrientationPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTextOrientationPropertyChanged(object oldValue, object newValue)
        {
            this.UpdateActualTextOrientation();
            this.OnPropertyChanged("TextOrientation");
        }

        private static void OnActualTextOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Title)d).OnActualTextOrientationPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualTextOrientationPropertyChanged(object oldValue, object newValue)
        {
            switch (this.ActualTextOrientation)
            {
                case TextOrientation.Auto:
                case TextOrientation.Horizontal:
                    this.ActualRotationAngle = 0.0;
                    break;
                case TextOrientation.Rotated90:
                    this.ActualRotationAngle = 90.0;
                    break;
                case TextOrientation.Rotated270:
                case TextOrientation.Stacked:
                    this.ActualRotationAngle = 270.0;
                    break;
                default:
                    this.ActualRotationAngle = 0.0;
                    break;
            }
            this.OnPropertyChanged("ActualTextOrientation");
        }

        private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Title)d).OnCornerRadiusPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnCornerRadiusPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("CornerRadius");
        }

        private static void OnActualRotationAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Title)d).OnActualRotationAnglePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualRotationAnglePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("ActualRotationAngle");
        }

        protected virtual void UpdateActualTextOrientation()
        {
            this.ActualTextOrientation = this.TextOrientation == TextOrientation.Auto ? TextOrientation.Horizontal : this.TextOrientation;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = base.MeasureOverride(availableSize);
            if (size.Width == 0.0 || size.Height == 0.0)
                return Size.Empty;
            return size;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            string str = newContent as string;
            if (str != null)
            {
                this.ActualTextContent = str;
                this.ActualContent = null;
            }
            else
            {
                this.ActualTextContent = null;
                this.ActualContent = newContent;
            }
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged == null)
                return;
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
