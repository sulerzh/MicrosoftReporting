using System;
using System.ComponentModel;
using System.Windows;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class DisplayUnit : DependencyObject, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DisplayUnit), new PropertyMetadata(1.0, new PropertyChangedCallback(DisplayUnit.OnValueChanged)));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DisplayUnit), new PropertyMetadata(null, new PropertyChangedCallback(DisplayUnit.OnTitleChanged)));
        public static readonly DependencyProperty LabelFormatProperty = DependencyProperty.Register("LabelFormat", typeof(string), typeof(DisplayUnit), new PropertyMetadata(null, new PropertyChangedCallback(DisplayUnit.OnLabelFormatChanged)));
        public static readonly DependencyProperty ApplicableRangeMinimumProperty = DependencyProperty.Register("ApplicableRangeMinimum", typeof(double), typeof(DisplayUnit), new PropertyMetadata(double.MinValue, new PropertyChangedCallback(DisplayUnit.OnApplicableRangeMinimumChanged)));
        public static readonly DependencyProperty ApplicableRangeMaximumProperty = DependencyProperty.Register("ApplicableRangeMaximum", typeof(double), typeof(DisplayUnit), new PropertyMetadata(double.MaxValue, new PropertyChangedCallback(DisplayUnit.OnApplicableRangeMaximumChanged)));
        private const string ValuePropertyName = "Value";
        private const string TitlePropertyName = "Title";
        private const string LabelFormatPropertyName = "LabelFormat";
        private const string ApplicableRangeMinimumPropertyName = "ApplicableRangeMinimum";
        private const string ApplicableRangeMaximumPropertyName = "ApplicableRangeMaximum";

        public double Value
        {
            get
            {
                return (double)this.GetValue(DisplayUnit.ValueProperty);
            }
            set
            {
                this.SetValue(DisplayUnit.ValueProperty, value);
            }
        }

        public string Title
        {
            get
            {
                return (string)this.GetValue(DisplayUnit.TitleProperty);
            }
            set
            {
                this.SetValue(DisplayUnit.TitleProperty, value);
            }
        }

        public string LabelFormat
        {
            get
            {
                return (string)this.GetValue(DisplayUnit.LabelFormatProperty);
            }
            set
            {
                this.SetValue(DisplayUnit.LabelFormatProperty, value);
            }
        }

        public double ApplicableRangeMinimum
        {
            get
            {
                return (double)this.GetValue(DisplayUnit.ApplicableRangeMinimumProperty);
            }
            set
            {
                this.SetValue(DisplayUnit.ApplicableRangeMinimumProperty, value);
            }
        }

        public double ApplicableRangeMaximum
        {
            get
            {
                return (double)this.GetValue(DisplayUnit.ApplicableRangeMaximumProperty);
            }
            set
            {
                this.SetValue(DisplayUnit.ApplicableRangeMaximumProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnit)d).OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("Value"));
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnit)d).OnTitleChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnTitleChanged(string oldValue, string newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("Title"));
        }

        private static void OnLabelFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnit)d).OnLabelFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnLabelFormatChanged(string oldValue, string newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("LabelFormat"));
        }

        private static void OnApplicableRangeMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnit)d).OnApplicableRangeMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnApplicableRangeMinimumChanged(double oldValue, double newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("ApplicableRangeMinimum"));
        }

        private static void OnApplicableRangeMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnit)d).OnApplicableRangeMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnApplicableRangeMaximumChanged(double oldValue, double newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("ApplicableRangeMaximum"));
        }

        public virtual double Project(double value)
        {
            if (this.Value != 0.0)
                return value / this.Value;
            return value;
        }

        public virtual double ReverseProject(double value)
        {
            if (this.Value != 0.0)
                return value * this.Value;
            return value;
        }

        public virtual bool IsApplicableTo(double value)
        {
            value = Math.Abs(value);
            double precision = DoubleHelper.GetPrecision(3, value);
            if (value.GreaterOrEqualWithPrecision(this.ApplicableRangeMinimum, precision))
                return value.LessWithPrecision(this.ApplicableRangeMaximum, precision);
            return false;
        }

        private void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler changedEventHandler = this.PropertyChanged;
            if (changedEventHandler == null)
                return;
            changedEventHandler(this, args);
        }
    }
}
