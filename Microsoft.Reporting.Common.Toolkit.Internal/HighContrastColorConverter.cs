using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public class HighContrastColorConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] strArray = parameter.ToString().Split(',');
            int length = strArray.Length;
            if (HighContrastHelper.CurrentTheme == HighContrastTheme.None && value is Color)
                return value;
            return ConverterUtils.GetColorFromString(HighContrastHelper.GetTheme(value) == HighContrastTheme.None && !false ? strArray[0].Trim() : (length != 3 || !HighContrastHelper.IsHighContrastWhiteOn() ? strArray[1].Trim() : strArray[2].Trim()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
