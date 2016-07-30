using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    [StyleTypedProperty(Property = "LabelStyle", StyleTargetType = typeof(LabelControl))]
    public class LabelDefinition : ScaleElementDefinition
    {
        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle", typeof(Style), typeof(LabelDefinition), new PropertyMetadata(new PropertyChangedCallback(LabelDefinition.OnLabelStylePropertyChanged)));
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(string), typeof(LabelDefinition), new PropertyMetadata(null, new PropertyChangedCallback(LabelDefinition.OnFormatPropertyChanged)));
        internal const string LabelStylePropertyName = "LabelStyle";
        internal const string FormatPropertyName = "Format";

        public Style LabelStyle
        {
            get
            {
                return (Style)this.GetValue(LabelDefinition.LabelStyleProperty);
            }
            set
            {
                this.SetValue(LabelDefinition.LabelStyleProperty, value);
            }
        }

        public string Format
        {
            get
            {
                return (string)this.GetValue(LabelDefinition.FormatProperty);
            }
            set
            {
                this.SetValue(LabelDefinition.FormatProperty, value);
            }
        }

        internal object SampleContent { get; set; }

        internal DisplayUnitSystem DisplayUnitSystem { get; set; }

        public LabelDefinition()
        {
            this.Kind = ScaleElementKind.Label;
        }

        private static void OnLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelDefinition)d).OnLabelStylePropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnLabelStylePropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("LabelStyle");
        }

        private static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LabelDefinition)d).OnFormatPropertyChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnFormatPropertyChanged(object oldValue, object newValue)
        {
            this.OnPropertyChanged("Format");
        }

        public object GetContent(ScalePosition s)
        {
            return this.GetContent(s.Data);
        }

        public object GetContent(object data)
        {
            string format = ValueHelper.PrepareFormatString(this.Format);
            if (this.DisplayUnitSystem != null)
                return this.DisplayUnitSystem.FormatLabel(this.Format, data, new int?());
            if (string.IsNullOrEmpty(this.Format))
                return data;
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, format, new object[1] { data });
        }
    }
}
