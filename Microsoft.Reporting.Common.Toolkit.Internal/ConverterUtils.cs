using System;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public static class ConverterUtils
    {
        public static Color GetColorFromString(string color)
        {
            if (string.Equals(color, "Black", StringComparison.OrdinalIgnoreCase))
                return Colors.Black;
            if (string.Equals(color, "Blue", StringComparison.OrdinalIgnoreCase))
                return Colors.Blue;
            if (string.Equals(color, "Brown", StringComparison.OrdinalIgnoreCase))
                return Colors.Brown;
            if (string.Equals(color, "DarkGray", StringComparison.OrdinalIgnoreCase))
                return Colors.DarkGray;
            if (string.Equals(color, "Gray", StringComparison.OrdinalIgnoreCase))
                return Colors.Gray;
            if (string.Equals(color, "Green", StringComparison.OrdinalIgnoreCase))
                return Colors.Green;
            if (string.Equals(color, "LightGray", StringComparison.OrdinalIgnoreCase))
                return Colors.LightGray;
            if (string.Equals(color, "Magenta", StringComparison.OrdinalIgnoreCase))
                return Colors.Magenta;
            if (string.Equals(color, "Orange", StringComparison.OrdinalIgnoreCase))
                return Colors.Orange;
            if (string.Equals(color, "Purple", StringComparison.OrdinalIgnoreCase))
                return Colors.Purple;
            if (string.Equals(color, "Red", StringComparison.OrdinalIgnoreCase))
                return Colors.Red;
            if (string.Equals(color, "Transparent", StringComparison.OrdinalIgnoreCase))
                return Colors.Transparent;
            if (string.Equals(color, "White", StringComparison.OrdinalIgnoreCase))
                return Colors.White;
            if (string.Equals(color, "Yellow", StringComparison.OrdinalIgnoreCase))
                return Colors.Yellow;
            if (string.Equals(color, "Silver", StringComparison.OrdinalIgnoreCase))
                return Color.FromArgb(byte.MaxValue, 192, 192, 192);
            if (color[0] != 35)
                throw new ArgumentException("The string does not contain a named color, or the named color is not supported.", color);
            if (color.Length == 9)
                return Color.FromArgb(byte.Parse(color.Substring(1, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture), byte.Parse(color.Substring(3, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture), byte.Parse(color.Substring(5, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture), byte.Parse(color.Substring(7, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
            return Color.FromArgb(byte.MaxValue, byte.Parse(color.Substring(1, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture), byte.Parse(color.Substring(3, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture), byte.Parse(color.Substring(5, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
        }
    }
}
