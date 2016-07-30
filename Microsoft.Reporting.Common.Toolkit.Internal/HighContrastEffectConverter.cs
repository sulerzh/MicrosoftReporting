using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public class HighContrastEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] strArray = parameter.ToString().Split(',');
            HighContrastTheme theme = HighContrastHelper.GetTheme(value);
            if (theme != HighContrastTheme.None)
            {
                if (strArray[0] == "HighContrastBlackAndWhiteEffect")
                {
                    HighContrastBlackAndWhiteEffect blackAndWhiteEffect = new HighContrastBlackAndWhiteEffect();
                    blackAndWhiteEffect.Invert = theme == HighContrastTheme.White ? 1.0 : 0.0;
                    if (strArray.Length == 2)
                        blackAndWhiteEffect.Amount = double.Parse(strArray[1], CultureInfo.InvariantCulture);
                    return blackAndWhiteEffect;
                }
                if (strArray[0] == "HighContrastInvertColorsEffect")
                {
                    HighContrastInvertColorsEffect invertColorsEffect = new HighContrastInvertColorsEffect();
                    invertColorsEffect.Invert = theme == HighContrastTheme.White ? 1.0 : 0.0;
                    if (strArray.Length == 3)
                    {
                        invertColorsEffect.Brightness = double.Parse(strArray[1], CultureInfo.InvariantCulture);
                        invertColorsEffect.Contrast = double.Parse(strArray[2], CultureInfo.InvariantCulture);
                    }
                    return invertColorsEffect;
                }
                if (strArray[0] == "MonochromeEffect")
                    return new MonochromeEffect() { Invert = (theme == HighContrastTheme.White ? 1.0 : 0.0) };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
