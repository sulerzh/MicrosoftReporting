using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.Reporting.Windows.Common.Internal
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            return string.Format(CultureInfo.CurrentCulture, parameter as string ?? "{0}", new object[1] { value });
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
