using System;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public sealed class HighContrastBrushConverter : HighContrastColorConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (HighContrastHelper.CurrentTheme == HighContrastTheme.None && value is Brush)
                return value;
            return new SolidColorBrush((Color)base.Convert(value, targetType, parameter, culture));
        }
    }
}
