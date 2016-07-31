using Microsoft.Reporting.Windows.Common.Internal.Properties;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    [ContentProperty("Items")]
    public class DisplayUnitSystem : DependencyObject, INotifyPropertyChanged
    {
        public static readonly Range<double> SupportedRange = new Range<double>(-1E+300, 1E+300);
        public static readonly DependencyProperty EnableTitleProperty = DependencyProperty.Register("EnableTitle", typeof(bool), typeof(DisplayUnitSystem), new PropertyMetadata(true, new PropertyChangedCallback(DisplayUnitSystem.OnEnableTitleChanged)));
        public static readonly DependencyProperty EnableLabelFormatProperty = DependencyProperty.Register("EnableLabelFormat", typeof(bool), typeof(DisplayUnitSystem), new PropertyMetadata(true, new PropertyChangedCallback(DisplayUnitSystem.OnEnableLabelFormatChanged)));
        public static readonly DependencyProperty TitleFormatProperty = DependencyProperty.Register("TitleFormat", typeof(string), typeof(DisplayUnitSystem), new PropertyMetadata(Resources.DisplayUnitSystem_TitleFormat, new PropertyChangedCallback(DisplayUnitSystem.OnTitleFormatChanged)));
        private static readonly Regex UnsupportedFormats = new Regex("^(p\\d*)|(.*\\%)|(e\\d*)$", RegexOptions.IgnoreCase);
        private static readonly Regex CustomFractionRegexHash = new Regex("0\\.#+");
        private static readonly Regex CustomFractionRegexZero = new Regex("0\\.0*");
        private static readonly Regex NFractionRegex = new Regex("^(n\\d*)$", RegexOptions.IgnoreCase);
        private static readonly Regex FFractionRegex = new Regex("^(f\\d*)$", RegexOptions.IgnoreCase);
        private static readonly Regex CFractionRegex = new Regex("^(c\\d*)$", RegexOptions.IgnoreCase);
        private static readonly Regex PFractionRegex = new Regex("^(p\\d*)$", RegexOptions.IgnoreCase);
        private static readonly Regex DRegex = new Regex("(d\\d*)", RegexOptions.IgnoreCase);
        private const string EnableTitlePropertyName = "EnableTitle";
        private const string EnableLabelFormatPropertyName = "EnableLabelFormat";
        private const string TitleFormatPropertyName = "TitleFormat";
        private double m_value;
        private DisplayUnit _dataUnit;
        private DisplayUnit _displayUnit;
        private DisplayUnit _actualDisplayUnit;
        private string _actualTitle;

        public bool EnableTitle
        {
            get
            {
                return (bool)this.GetValue(DisplayUnitSystem.EnableTitleProperty);
            }
            set
            {
                this.SetValue(DisplayUnitSystem.EnableTitleProperty, value);
            }
        }

        public bool EnableLabelFormat
        {
            get
            {
                return (bool)this.GetValue(DisplayUnitSystem.EnableLabelFormatProperty);
            }
            set
            {
                this.SetValue(DisplayUnitSystem.EnableLabelFormatProperty, value);
            }
        }

        public string TitleFormat
        {
            get
            {
                return (string)this.GetValue(DisplayUnitSystem.TitleFormatProperty);
            }
            set
            {
                this.SetValue(DisplayUnitSystem.TitleFormatProperty, value);
            }
        }

        public ObservableCollection<DisplayUnit> Items { get; private set; }

        public DisplayUnit DataUnit
        {
            get
            {
                return this._dataUnit;
            }
            set
            {
                if (this._dataUnit == value)
                    return;
                this._dataUnit = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("DataUnit"));
            }
        }

        public DisplayUnit DisplayUnit
        {
            get
            {
                return this._displayUnit;
            }
            set
            {
                if (this._displayUnit == value)
                    return;
                this._displayUnit = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("DisplayUnit"));
                if (this._displayUnit == null)
                    return;
                this.ActualDisplayUnit = this._displayUnit;
                this.CalculateActualTitle();
            }
        }

        public DisplayUnit ActualDisplayUnit
        {
            get
            {
                return this._actualDisplayUnit;
            }
            private set
            {
                if (this._actualDisplayUnit == value)
                    return;
                this._actualDisplayUnit = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ActualDisplayUnit"));
            }
        }

        public string ActualTitle
        {
            get
            {
                return this._actualTitle;
            }
            private set
            {
                if (!(this._actualTitle != value))
                    return;
                this._actualTitle = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ActualTitle"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DisplayUnitSystem()
        {
            this.Items = new ObservableCollection<DisplayUnit>();
            this.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
        }

        private static void OnEnableTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnitSystem)d).OnEnableTitleChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnEnableTitleChanged(bool oldValue, bool newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("EnableTitle"));
            this.CalculateActualTitle();
        }

        private static void OnEnableLabelFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnitSystem)d).OnEnableLabelFormatChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnEnableLabelFormatChanged(bool oldValue, bool newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("EnableLabelFormat"));
        }

        private static void OnTitleFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DisplayUnitSystem)d).OnTitleFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual void OnTitleFormatChanged(string oldValue, string newValue)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("TitleFormat"));
            this.CalculateActualTitle();
        }

        public void CalculateActualDisplayUnit(double value, string formatString = null)
        {
            this.m_value = value;
            this.ActualDisplayUnit = !this.IsSupportedFormat(formatString) ? null : (this._displayUnit != null ? this._displayUnit : this.FindApplicableDisplayUnit(value));
            this.CalculateActualTitle();
        }

        private void CalculateActualTitle()
        {
            string str;
            if (!this.EnableTitle || this.ActualDisplayUnit == null || this.ActualDisplayUnit.Title == null)
                str = null;
            else
                str = string.Format(CultureInfo.CurrentCulture, this.TitleFormat, new object[1]
                {
           this.ActualDisplayUnit.Title
                });
            this.ActualTitle = str;
        }

        public DisplayUnit FindApplicableDisplayUnit(double value)
        {
            if (this.DataUnit != null)
                value = this.DataUnit.ReverseProject(value);
            foreach (DisplayUnit displayUnit in this.Items)
            {
                if (displayUnit.IsApplicableTo(value))
                    return displayUnit;
            }
            return null;
        }

        public virtual object FormatLabel(string format, object data, int? decimals = null)
        {
            if ((this.EnableTitle || this.EnableLabelFormat) && this.ActualDisplayUnit != null)
            {
                double num1 = this.Project(Convert.ToDouble(data, CultureInfo.CurrentCulture));
                double num2;
                if (decimals.HasValue)
                {
                    num2 = Math.Round(num1, decimals.Value);
                }
                else
                {
                    double precision = Math.Max(DoubleHelper.GetPrecision(num1), 1E-308);
                    num2 = num1.RoundWithPrecision(precision);
                }
                data = num2;
                format = !this.EnableLabelFormat || string.IsNullOrEmpty(this.ActualDisplayUnit.LabelFormat) ? "{0}" : this.ActualDisplayUnit.LabelFormat;
            }
            else
            {
                format = this.RemoveFractionIfNecessary(format);
                format = ValueHelper.PrepareFormatString(format);
            }
            format = this.ReplaceDFormat(format);
            return string.Format(CultureInfo.CurrentCulture, format, new object[1] { data });
        }

        private bool IsSupportedFormat(string formatString)
        {
            if (!string.IsNullOrEmpty(formatString))
                return !DisplayUnitSystem.UnsupportedFormats.IsMatch(formatString);
            return true;
        }

        private string RemoveFractionIfNecessary(string formatString)
        {
            if (Math.Abs(this.m_value) >= 1.0)
            {
                formatString = DisplayUnitSystem.CustomFractionRegexHash.Replace(formatString, "0");
                formatString = DisplayUnitSystem.CustomFractionRegexZero.Replace(formatString, "0");
                formatString = DisplayUnitSystem.NFractionRegex.Replace(formatString, "n0");
                formatString = DisplayUnitSystem.FFractionRegex.Replace(formatString, "f0");
                formatString = DisplayUnitSystem.CFractionRegex.Replace(formatString, "c0");
                formatString = DisplayUnitSystem.PFractionRegex.Replace(formatString, "p0");
            }
            return formatString;
        }

        private string ReplaceDFormat(string formatString)
        {
            return DisplayUnitSystem.DRegex.Replace(formatString, "f0");
        }

        internal double Project(double value)
        {
            if (this.DataUnit != null)
                value = this.DataUnit.ReverseProject(value);
            if (this.ActualDisplayUnit != null)
                return this.ActualDisplayUnit.Project(value);
            return value;
        }

        internal double ReverseProject(double value)
        {
            if (this.ActualDisplayUnit != null)
                value = this.ActualDisplayUnit.ReverseProject(value);
            if (this.DataUnit != null)
                return this.DataUnit.Project(value);
            return value;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnRemoveItems(e.OldItems);
            this.OnAddItems(e.NewItems);
            this.OnPropertyChanged(new PropertyChangedEventArgs("Items"));
        }

        private void OnAddItems(IEnumerable items)
        {
            if (items == null)
                return;
            foreach (DisplayUnit displayUnit in items)
                displayUnit.PropertyChanged += new PropertyChangedEventHandler(this.DisplayUnit_PropertyChanged);
        }

        private void OnRemoveItems(IEnumerable items)
        {
            if (items == null)
                return;
            foreach (DisplayUnit displayUnit in items)
                displayUnit.PropertyChanged -= new PropertyChangedEventHandler(this.DisplayUnit_PropertyChanged);
        }

        private void DisplayUnit_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("Items"));
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
