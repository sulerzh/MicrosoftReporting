using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [TemplatePart(Name = "RotatableControl", Type = typeof(RotatableControl))]
    public class LabelControl : ContentControl, ILabelControl
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LabelControl), new PropertyMetadata(new PropertyChangedCallback(LabelControl.OnCornerRadiusPropertyChanged)));
        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof(double), typeof(LabelControl), new PropertyMetadata(0.0, new PropertyChangedCallback(LabelControl.OnRotationAngleChanged)));
        public static readonly DependencyProperty CalloutGeometryProperty = DependencyProperty.Register("CalloutGeometry", typeof(Geometry), typeof(LabelControl), new PropertyMetadata(new PropertyChangedCallback(LabelControl.OnCalloutGeometryPropertyChanged)));
        public static readonly DependencyProperty CalloutStrokeProperty = DependencyProperty.Register("CalloutStroke", typeof(Brush), typeof(LabelControl), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(LabelControl.OnCalloutStrokeChanged)));
        public static readonly DependencyProperty CalloutStrokeThicknessProperty = DependencyProperty.Register("CalloutStrokeThickness", typeof(double), typeof(LabelControl), new PropertyMetadata(1.0, new PropertyChangedCallback(LabelControl.OnCalloutStrokeThicknessChanged)));
        public static readonly DependencyProperty ActualOpacityProperty = DependencyProperty.Register("ActualOpacity", typeof(double), typeof(LabelControl), new PropertyMetadata(1.0, new PropertyChangedCallback(LabelControl.OnActualOpacityPropertyChanged)));
        public static readonly DependencyProperty ActualEffectProperty = DependencyProperty.Register("ActualEffect", typeof(Effect), typeof(LabelControl), new PropertyMetadata(null, new PropertyChangedCallback(LabelControl.OnActualEffectPropertyChanged)));
        public static readonly DependencyProperty TextEffectProperty = DependencyProperty.Register("TextEffect", typeof(Effect), typeof(LabelControl), new PropertyMetadata(null, new PropertyChangedCallback(LabelControl.OnTextEffectPropertyChanged)));
        public static readonly DependencyProperty TextOpacityProperty = DependencyProperty.Register("TextOpacity", typeof(double), typeof(LabelControl), new PropertyMetadata(1.0, new PropertyChangedCallback(LabelControl.OnTextOpacityPropertyChanged)));
        public static readonly DependencyProperty ActualTextEffectProperty = DependencyProperty.Register("ActualTextEffect", typeof(Effect), typeof(LabelControl), new PropertyMetadata(null, new PropertyChangedCallback(LabelControl.OnActualTextEffectPropertyChanged)));
        public static readonly DependencyProperty ActualTextOpacityProperty = DependencyProperty.Register("ActualTextOpacity", typeof(double), typeof(LabelControl), new PropertyMetadata(1.0, new PropertyChangedCallback(LabelControl.OnActualTextOpacityPropertyChanged)));
        protected const string RotatableControltName = "RotatableControl";
        internal const string ActualOpacityPropertyName = "ActualOpacity";
        internal const string ActualEffectPropertyName = "ActualEffect";
        internal const string TextEffectPropertyName = "TextEffect";
        internal const string TextOpacityPropertyName = "TextOpacity";
        internal const string ActualTextEffectPropertyName = "ActualTextEffect";
        internal const string ActualTextOpacityPropertyName = "ActualTextOpacity";
        private object _lastMeasuredContent;
        private Size _lastMeasuredSize;
        private RotatableControl _rotatableControl;

        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)this.GetValue(LabelControl.CornerRadiusProperty);
            }
            set
            {
                this.SetValue(LabelControl.CornerRadiusProperty, value);
            }
        }

        public double RotationAngle
        {
            get
            {
                return (double)this.GetValue(LabelControl.RotationAngleProperty);
            }
            set
            {
                this.SetValue(LabelControl.RotationAngleProperty, value);
            }
        }

        public Geometry CalloutGeometry
        {
            get
            {
                return this.GetValue(LabelControl.CalloutGeometryProperty) as Geometry;
            }
            set
            {
                this.SetValue(LabelControl.CalloutGeometryProperty, value);
            }
        }

        public Brush CalloutStroke
        {
            get
            {
                return this.GetValue(LabelControl.CalloutStrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(LabelControl.CalloutStrokeProperty, value);
            }
        }

        public double CalloutStrokeThickness
        {
            get
            {
                return (double)this.GetValue(LabelControl.CalloutStrokeThicknessProperty);
            }
            set
            {
                this.SetValue(LabelControl.CalloutStrokeThicknessProperty, value);
            }
        }

        public double ActualOpacity
        {
            get
            {
                return (double)this.GetValue(LabelControl.ActualOpacityProperty);
            }
            set
            {
                this.SetValue(LabelControl.ActualOpacityProperty, value);
            }
        }

        public Effect ActualEffect
        {
            get
            {
                return (Effect)this.GetValue(LabelControl.ActualEffectProperty);
            }
            set
            {
                this.SetValue(LabelControl.ActualEffectProperty, value);
            }
        }

        public Effect TextEffect
        {
            get
            {
                return (Effect)this.GetValue(LabelControl.TextEffectProperty);
            }
            set
            {
                this.SetValue(LabelControl.TextEffectProperty, value);
            }
        }

        public double TextOpacity
        {
            get
            {
                return (double)this.GetValue(LabelControl.TextOpacityProperty);
            }
            set
            {
                this.SetValue(LabelControl.TextOpacityProperty, value);
            }
        }

        public Effect ActualTextEffect
        {
            get
            {
                return (Effect)this.GetValue(LabelControl.ActualTextEffectProperty);
            }
            set
            {
                this.SetValue(LabelControl.ActualTextEffectProperty, value);
            }
        }

        public double ActualTextOpacity
        {
            get
            {
                return (double)this.GetValue(LabelControl.ActualTextOpacityProperty);
            }
            set
            {
                this.SetValue(LabelControl.ActualTextOpacityProperty, value);
            }
        }

        public LabelControl()
        {
            this.DefaultStyleKey = typeof(LabelControl);
        }

        private static void OnCornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnRotationAngleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((LabelControl)o).OnRotationAngleChanged(e.OldValue, e.NewValue);
        }

        private void OnRotationAngleChanged(object oldValue, object newValue)
        {
            this.InvalidateLastMeasured();
        }

        private static void OnCalloutGeometryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnCalloutStrokeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnCalloutStrokeThicknessChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._rotatableControl = this.GetTemplateChild("RotatableControl") as RotatableControl;
            this.UpdateActuialEffect(this.Effect);
            this.UpdateActuialOpacity(new double?(this.Opacity));
        }

        public Size GetLabelContentDesiredSize()
        {
            if (this._rotatableControl != null)
            {
                UIElement uiElement = this._rotatableControl.Child;
                if (uiElement != null)
                    return uiElement.DesiredSize;
            }
            return Size.Empty;
        }

        private static void OnActualOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelControl)d).OnActualOpacityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualOpacityPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnActualEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelControl)d).OnActualEffectPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualEffectPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnTextEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelControl)d).OnTextEffectPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTextEffectPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnTextOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelControl)d).OnTextOpacityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnTextOpacityPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnActualTextEffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelControl)d).OnActualTextEffectPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualTextEffectPropertyChanged(object oldValue, object newValue)
        {
        }

        private static void OnActualTextOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelControl)d).OnActualTextOpacityPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnActualTextOpacityPropertyChanged(object oldValue, object newValue)
        {
        }

        public void UpdateCalloutGeometry(Point start, Point end)
        {
            this.CalloutGeometry = new LineGeometry()
            {
                StartPoint = start,
                EndPoint = end
            };
        }

        public void UpdateActuialEffect(Effect effect)
        {
            this.ActualEffect = effect ?? this.Effect;
            this.ActualTextEffect = effect ?? this.TextEffect;
        }

        public void UpdateActuialOpacity(double? opacity)
        {
            this.ActualOpacity = opacity ?? this.Opacity;
            this.ActualTextOpacity = opacity ?? this.TextOpacity;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            object content = this.Content;
            if (content != this._lastMeasuredContent)
            {
                this._lastMeasuredContent = this.Content;
                this._lastMeasuredSize = content != null ? base.MeasureOverride(availableSize) : Size.Empty;
            }
            return this._lastMeasuredSize;
        }

        private void InvalidateLastMeasured()
        {
            this._lastMeasuredContent = null;
        }

        public Size GetDesiredSize()
        {
            if (this.DesiredSize.Width > 0.0 && this.DesiredSize.Height > 0.0 || this._lastMeasuredSize.IsEmpty)
                return this.DesiredSize;
            return this._lastMeasuredSize;
        }
    }
}
