using Microsoft.Reporting.Windows.Common.Internal.Properties;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class DefaultDisplayUnitSystem : DisplayUnitSystem
    {
        private double _scientificBigNumbersBoundary = 1E+15;
        private double _scientificSmallNumbersBoundary = 1E-15;

        public DefaultDisplayUnitSystem()
        {
            this.AddUnitIfNonEmpty(1000.0, Resources.DisplayUnitSystem_E03_Title, Resources.DisplayUnitSystem_E03_LabelFormat);
            this.AddUnitIfNonEmpty(10000.0, Resources.DisplayUnitSystem_E04_Title, Resources.DisplayUnitSystem_E04_LabelFormat);
            this.AddUnitIfNonEmpty(100000.0, Resources.DisplayUnitSystem_E05_Title, Resources.DisplayUnitSystem_E05_LabelFormat);
            this.AddUnitIfNonEmpty(1000000.0, Resources.DisplayUnitSystem_E06_Title, Resources.DisplayUnitSystem_E06_LabelFormat);
            this.AddUnitIfNonEmpty(10000000.0, Resources.DisplayUnitSystem_E07_Title, Resources.DisplayUnitSystem_E07_LabelFormat);
            this.AddUnitIfNonEmpty(100000000.0, Resources.DisplayUnitSystem_E08_Title, Resources.DisplayUnitSystem_E08_LabelFormat);
            this.AddUnitIfNonEmpty(1000000000.0, Resources.DisplayUnitSystem_E09_Title, Resources.DisplayUnitSystem_E09_LabelFormat);
            this.AddUnitIfNonEmpty(10000000000.0, Resources.DisplayUnitSystem_E10_Title, Resources.DisplayUnitSystem_E10_LabelFormat);
            this.AddUnitIfNonEmpty(100000000000.0, Resources.DisplayUnitSystem_E11_Title, Resources.DisplayUnitSystem_E11_LabelFormat);
            this.AddUnitIfNonEmpty(1000000000000.0, Resources.DisplayUnitSystem_E12_Title, Resources.DisplayUnitSystem_E12_LabelFormat);
            this.AddUnitIfNonEmpty(10000000000000.0, Resources.DisplayUnitSystem_E13_Title, Resources.DisplayUnitSystem_E13_LabelFormat);
            this.AddUnitIfNonEmpty(100000000000000.0, Resources.DisplayUnitSystem_E14_Title, Resources.DisplayUnitSystem_E14_LabelFormat);
            this.AddUnitIfNonEmpty(1E+15, Resources.DisplayUnitSystem_E15_Title, Resources.DisplayUnitSystem_E15_LabelFormat);
            this.AddUnitIfNonEmpty(1E+16, Resources.DisplayUnitSystem_E16_Title, Resources.DisplayUnitSystem_E16_LabelFormat);
            this.AddUnitIfNonEmpty(1E+17, Resources.DisplayUnitSystem_E17_Title, Resources.DisplayUnitSystem_E17_LabelFormat);
            this.AddUnitIfNonEmpty(1E+18, Resources.DisplayUnitSystem_E18_Title, Resources.DisplayUnitSystem_E18_LabelFormat);
            this.AddUnitIfNonEmpty(1E+19, Resources.DisplayUnitSystem_E19_Title, Resources.DisplayUnitSystem_E19_LabelFormat);
            this.AddUnitIfNonEmpty(1E+20, Resources.DisplayUnitSystem_E20_Title, Resources.DisplayUnitSystem_E20_LabelFormat);
            this.AddUnitIfNonEmpty(1E+21, Resources.DisplayUnitSystem_E21_Title, Resources.DisplayUnitSystem_E21_LabelFormat);
            this.AddUnitIfNonEmpty(1E+22, Resources.DisplayUnitSystem_E22_Title, Resources.DisplayUnitSystem_E22_LabelFormat);
            this.AddUnitIfNonEmpty(1E+23, Resources.DisplayUnitSystem_E23_Title, Resources.DisplayUnitSystem_E23_LabelFormat);
            this.AddUnitIfNonEmpty(1E+24, Resources.DisplayUnitSystem_E24_Title, Resources.DisplayUnitSystem_E24_LabelFormat);
        }

        public override object FormatLabel(string format, object data, int? decimals = null)
        {
            double doubleValue;
            if (this.DisplayUnit == null && ValueHelper.TryConvert(data, out doubleValue) && this.IsScientific(doubleValue))
            {
                format = format.ToUpperInvariant();
                if (!format.Contains("E"))
                    format = "{0}";
            }
            return base.FormatLabel(format, data, decimals);
        }

        private bool IsScientific(double val)
        {
            if (val < -this._scientificBigNumbersBoundary || val > this._scientificBigNumbersBoundary)
                return true;
            if (-this._scientificSmallNumbersBoundary < val && val < this._scientificSmallNumbersBoundary)
                return val != 0.0;
            return false;
        }

        private void AddUnitIfNonEmpty(double value, string title, string format)
        {
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(format))
                return;
            double num = value;
            if (this.Items.Count > 0)
            {
                DisplayUnit displayUnit = this.Items[this.Items.Count - 1];
                if (value - displayUnit.Value >= 1000.0)
                    num = value / 10.0;
                displayUnit.ApplicableRangeMaximum = num;
            }
            DisplayUnit displayUnit1 = new DisplayUnit() { Title = title, LabelFormat = format, Value = value, ApplicableRangeMinimum = num, ApplicableRangeMaximum = num * 1000.0 };
            if (this.Items.Count > 0)
                this.Items[this.Items.Count - 1].ApplicableRangeMaximum = displayUnit1.ApplicableRangeMinimum;
            this.Items.Add(displayUnit1);
            if (displayUnit1.ApplicableRangeMaximum <= this._scientificBigNumbersBoundary)
                return;
            this._scientificBigNumbersBoundary = displayUnit1.ApplicableRangeMaximum;
        }
    }
}
